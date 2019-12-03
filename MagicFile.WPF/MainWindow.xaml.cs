using FileManager.ViewModel;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;

using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace FileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private readonly ILog log =  log4net.LogManager.GetLogger("", "ExifManager");//获取一个日志记录器
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowVM(this);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < listBox1.Items.Count; i++)
            //{
            //    Font normalContentFont = new Font("宋体", 36, FontStyle.Bold);
            //    Color normalContentColor = Color.Red;
            //    PropertyItem[] pi = GetExifProperties(listBox1.Items[i].ToString());
            //    // 获取元数据中的拍照日期时间，以字符串形式保存
            //    var TakePicDateTime = GetTakePicDateTime(pi);
            //    // 分析字符串分别保存拍照日期和时间的标准格式
            //    var SpaceLocation = TakePicDateTime.IndexOf(" ");
            //    var dt = TakePicDateTime.Substring(0, SpaceLocation);
            //    dt = dt.Replace(":", "-");
            //    var tm = TakePicDateTime.Substring(SpaceLocation + 1, TakePicDateTime.Length - SpaceLocation - 2);
            //    TakePicDateTime = dt + " " + tm;
            //    // 由列表中的文件创建内存位图对象
            //    var pic = new Bitmap(listBox1.Items[i].ToString());
            //    // 由位图对象创建Graphics对象的实例
            //    var g = Graphics.FromImage(pic);
            //    // 在 Graphics 表面绘制数码照片的日期/时间戳
            //    //g.DrawString(TakePicDateTime, normalContentFont, new SolidBrush(normalContentColor), pic.Width - 700, pic.Height - 200);
            //    // 将添加日期/时间戳后的图像进行保存
            //    pic.Save(textBox1.Text + Path.GetFileName(listBox1.Items[i].ToString()));
            //    // 释放内存位图对象
            //    pic.Dispose();
            //}

        }

        /// <summary>
        /// 获取图像文件的所有元数据属性，以 PropertyItem 数组的格式保存。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static PropertyItem[] GetExifProperties(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            // 通过指定的数据流来创建 Image
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream, true, false);
            return image.PropertyItems;
        }

        /// <summary>
        /// 遍历所有元数据，获取拍照日期/时间。
        /// </summary>
        /// <param name="parr"></param>
        /// <returns></returns>
        private string GetTakePicDateTime(System.Drawing.Imaging.PropertyItem[] parr)
        {
            Encoding ascii = Encoding.ASCII;
            // 遍历图像文件元数据，检索所有属性
            foreach (System.Drawing.Imaging.PropertyItem p in parr)
            {
                // 如果是 PropertyTagDateTime，则返回该属性所对应的值
                if (p.Id == 0x0132)
                {
                    return ascii.GetString(p.Value);
                }
            }
            // 若没有相关的 EXIF 信息则返回 N/A
            return "N/A";
        }


        /// <summary>
        /// 设置图片的经纬高
        /// </summary>
        /// <param name="IN_File">文件路径</param>
        /// <param name="IN_Lat">纬度</param>
        /// <param name="IN_Lng">经度</param>
        /// <param name="IN_Alt">高程</param>
        /// <param name="IN_Save">保存路径</param>
        private void PRV_Operate(string IN_File, double IN_Lat, double IN_Lng, double IN_Alt, string IN_Save)
        {
            Image image = Image.FromFile(IN_File);
            //构建版本
            byte[] _version = { 2, 2, 0, 0 };
            PRV_SetProperty(image, _version, 0x0000, 1);
            //设置南北半球
            PRV_SetProperty(image, BitConverter.GetBytes('N'), 0x0001, 2);
            //设置纬度
            PRV_SetProperty(image, PRV_GetLatlngByte(IN_Lat), 0x0002, 5);
            //设置东西半球
            PRV_SetProperty(image, BitConverter.GetBytes('E'), 0x0003, 2);
            //设置经度
            PRV_SetProperty(image, PRV_GetLatlngByte(IN_Lng), 0x0004, 5);
            //设置高度在海平面上还是下
            byte[] _altref = { 0 };//海平面上
            PRV_SetProperty(image, _altref, 0x0005, 1);
            //设置高度
            byte[] _alt = new byte[8];
            //类型为5可以通过分子/分母的形式表示小数,先乘后除
            int v1 = (int)(IN_Alt * 10000);
            int v2 = 10000;
            Array.Copy(BitConverter.GetBytes(v1), 0, _alt, 0, 4);
            Array.Copy(BitConverter.GetBytes(v2), 0, _alt, 4, 4);
            PRV_SetProperty(image, _alt, 0x0006, 5);
            image.Save(IN_Save);
            image.Dispose();
        }




        /// <summary>
        /// 设置图片参数
        /// </summary>
        /// <param name="IN_Image">图片</param>
        /// <param name="IN_Content">byte[] 要写入的内容</param>
        /// <param name="IN_Id">字段ID</param>
        /// <param name="IN_Type">值类型</param>
        private void PRV_SetProperty(Image IN_Image, byte[] IN_Content, int IN_Id, short IN_Type)
        {
            PropertyItem pi = IN_Image.PropertyItems[0];
            pi.Id = IN_Id;
            pi.Type = IN_Type;
            pi.Value = IN_Content;
            pi.Len = pi.Value.Length;
            IN_Image.SetPropertyItem(pi);
        }

        /// <summary>
        /// 经纬度转byte[]
        /// </summary>
        /// <param name="IN_Latlng">待处理的经度或纬度</param>
        /// <returns></returns>
        private byte[] PRV_GetLatlngByte(double IN_Latlng)
        {
            double temp;
            temp = Math.Abs(IN_Latlng);
            int degrees = (int)Math.Truncate(temp);
            temp = (temp - degrees) * 60;
            int minutes = (int)Math.Truncate(temp);
            temp = (temp - minutes) * 60;
            //分母设大提高精度
            int secondsNominator = (int)Math.Truncate(10000000 * temp);
            int secondsDenoninator = 10000000;
            byte[] result = new byte[24];
            Array.Copy(BitConverter.GetBytes(degrees), 0, result, 0, 4);
            Array.Copy(BitConverter.GetBytes(1), 0, result, 4, 4);
            Array.Copy(BitConverter.GetBytes(minutes), 0, result, 8, 4);
            Array.Copy(BitConverter.GetBytes(1), 0, result, 12, 4);
            Array.Copy(BitConverter.GetBytes(secondsNominator), 0, result, 16, 4);
            Array.Copy(BitConverter.GetBytes(secondsDenoninator), 0, result, 20, 4);
            return result;
        }

        private string InputPath = string.Empty;
        private string OutputPath = string.Empty;
        private bool isFilterDevice = false;
        private string filterDeviceName = string.Empty;
        private bool isModifyTime = false;
        private bool modifyTimeType = false;
        private int hout = 0;
        private int minute = 0;
        private int second = 0;
        private void SelectPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                InputPath = folderBrowserDialog.SelectedPath;
                txtInputPath.Text = InputPath;
                rtxLogs.AppendText(string.Format("已选择输入路径：{0}\n", InputPath));
                Log4NetHelper.Info(string.Format("已选择输入路径：{0}\n", InputPath));
            }
            //System.Windows.MessageBox.Show(SelectedPath);
        }

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            isFilterDevice = chkFilterDevice.IsChecked.Value;
            filterDeviceName = txtDeviceName.Text;
            isModifyTime = chkModifyTime.IsChecked.Value;
            modifyTimeType = radAddTime.IsChecked.Value == true ? true : false;

            hout = Convert.ToInt32(txtHourValue.Text);
            minute = Convert.ToInt32(txtMinuteValue.Text);
            second = Convert.ToInt32(txtSecondValue.Text);

            DirectoryInfo directory = new DirectoryInfo(InputPath);
            btnExecute.IsEnabled = false;
            btnInputPath.IsEnabled = false;
            btnOutputPath.IsEnabled = false;
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (FileInfo fileInfo in directory.GetFiles("*.jpg"))
                {
                    file = fileInfo;
                    UpdateExif();
                    rtxLogs.AppendText(string.Format("图片 {0} 处理完毕！\n", fileInfo.Name));
                    Log4NetHelper.Info(string.Format("图片 {0} 处理完毕！\n", fileInfo.Name));
                    //Thread thread = new Thread(new ThreadStart(UpdateExif));
                    //thread.Start();
                }
            });
            btnExecute.IsEnabled = true;
            btnInputPath.IsEnabled = true;
            btnOutputPath.IsEnabled = true;
            System.Windows.MessageBox.Show("处理完成！", "处理结果");
        }

        private FileInfo file = null;
        /// <summary>
        /// 更新 Exif 信息。
        /// </summary>
        private void UpdateExif()
        {
            ExifUtils exif = new ExifUtils(file.FullName);

            if (isFilterDevice)
            {
                if (exif.EquipmentModel == filterDeviceName)
                {
                    if (isModifyTime)
                    {
                        DateTime dateTime = new DateTime();
                        if (modifyTimeType)
                        {
                            dateTime = exif.DateTimeOriginal.AddHours(hout).AddMinutes(minute).AddSeconds(second);
                        }
                        else
                        {
                            dateTime = exif.DateTimeOriginal.AddHours(0 - hout).AddMinutes(0 - minute).AddSeconds(0 - second);
                        }
                        exif.DateTimeOriginal = dateTime;
                        exif.DateTimeDigitized = dateTime;
                        if (exif.DateTimeLastModified < exif.DateTimeOriginal)
                        {
                            exif.DateTimeLastModified = exif.DateTimeOriginal;
                        }
                    }
                    string savePath = OutputPath + "\\" + file.Name;
                    exif.Save(savePath);
                    exif.Dispose();
                }
            }
            else
            {
                if (isModifyTime)
                {
                    DateTime dateTime = new DateTime();
                    if (modifyTimeType)
                    {
                        dateTime = exif.DateTimeOriginal.AddHours(hout).AddMinutes(minute).AddSeconds(second);
                    }
                    else
                    {
                        dateTime = exif.DateTimeOriginal.AddHours(0 - hout).AddMinutes(0 - minute).AddSeconds(0 - second);
                    }
                    exif.DateTimeOriginal = dateTime;
                    exif.DateTimeDigitized = dateTime;
                    if (exif.DateTimeLastModified < exif.DateTimeOriginal)
                    {
                        exif.DateTimeLastModified = exif.DateTimeOriginal;
                    }
                }
                string savePath = OutputPath + "\\" + file.Name;
                exif.Save(savePath);
                exif.Dispose();
            }
        }

        private void BtnInputPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                InputPath = folderBrowserDialog.SelectedPath;
                txtInputPath.Text = InputPath;
                rtxLogs.AppendText(string.Format("已选择输入路径：{0}\n", InputPath));
                Log4NetHelper.Info(string.Format("已选择输入路径：{0}\n", InputPath));
            }
        }

        private void BtnOutputPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OutputPath = folderBrowserDialog.SelectedPath;
                txtOutputPath.Text = OutputPath;
                rtxLogs.AppendText(string.Format("已选择输入路径：{0}\n", InputPath));
                Log4NetHelper.Info(string.Format("已选择输入路径：{0}\n", InputPath));
            }
        }

        //private void AddDate()
        //{
        //    Font normalContentFont = new Font("宋体", 36, FontStyle.Bold);
        //    Color normalContentColor = Color.Red;
        //    int kk = 1;
        //    toolStripProgressBar1.Maximum = listBox1.Items.Count;
        //    toolStripProgressBar1.Minimum = 1;
        //    toolStripStatusLabel1.Text = "开始添加数码相片拍摄日期";
        //    for (int i = 0; i < listBox1.Items.Count; i)
        //    {
        //        pi = GetExif(listBox1.Items[i].ToString());
        //        //获取元数据中的拍照日期时间，以字符串形式保存
        //        TakePicDateTime = GetDateTime(pi);
        //        //分析字符串分别保存拍照日期和时间的标准格式
        //        SpaceLocation = TakePicDateTime.IndexOf(" ");
        //        pdt = TakePicDateTime.Substring(0, SpaceLocation);
        //        pdt = pdt.Replace(":", "-");
        //        ptm = TakePicDateTime.Substring(SpaceLocation   1, TakePicDateTime.Length - SpaceLocation - 2);
        //        TakePicDateTime = pdt   " "   ptm;
        //        //由列表中的文件创建内存位图对象
        //        Pic = new Bitmap(listBox1.Items[i].ToString());
        //        //由位图对象创建Graphics对象的实例
        //        g = Graphics.FromImage(Pic);
        //        //绘制数码照片的日期/时间
        //        g.DrawString(TakePicDateTime, normalContentFont, new SolidBrush(normalContentColor),
        //    Pic.Width - 700, Pic.Height - 200);
        //        //将添加日期/时间戳后的图像进行保存
        //        if (txtSavePath.Text.Length == 3)
        //        {
        //            Pic.Save(txtSavePath.Text   Path.GetFileName(listBox1.Items[i].ToString()));
        //        }
        //        else
        //        {
        //            Pic.Save(txtSavePath.Text  "\\"  Path.GetFileName(listBox1.Items[i].ToString()));
        //        }
        //        //释放内存位图对象
        //        Pic.Dispose();
        //        toolStripProgressBar1.Value = kk;
        //        if (kk == listBox1.Items.Count)
        //        {
        //            toolStripStatusLabel1.Text = "全部数码相片拍摄日期添加成功";
        //            toolStripProgressBar1.Visible = false;
        //            flag = null;
        //            listBox1.Items.Clear();
        //        }
        //        kk;
        //    }
        //}

    }
}
