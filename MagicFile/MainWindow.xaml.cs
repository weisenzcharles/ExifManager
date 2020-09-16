using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MagicFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Activated(object sender, EventArgs e)
        {

        }

        private void BatchProcess_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_System_Redo(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_System_Undo(object sender, RoutedEventArgs e)
        {

        }

        private void ListViewFiles_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void ListViewFiles_Drop(object sender, DragEventArgs e)
        {

        }

        private void ListViewFiles_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void Item_DoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            FrameworkElement overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            FrameworkElement mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }

        #region 规则事件...

        private void AddRule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpRule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DownRule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveRule_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
