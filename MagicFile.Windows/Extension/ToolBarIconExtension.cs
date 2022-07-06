using MagicFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MagicFile.Extension
{
    public class ToolBarIconExtension
    {
        static ToolBarIconExtension sharedExtension;
        public static ToolBarIconExtension SharedExtension
        {
            get => sharedExtension ?? new ToolBarIconExtension();
            set => sharedExtension = value;
        }

        public object OpenIcon { get; private set; }
        public object DeleteIcon { get; private set; }
        public object ApplyIcon { get; private set; }

        public object UndoIcon { get; private set; }
        public object RedoIcon { get; private set; }

        #region 规则工具栏按钮...

        public object RuleAddIcon { get; private set; }
        public object RuleRemoveIcon { get; private set; }
        public object RuleUpIcon { get; private set; }
        public object RuleDownIcon { get; private set; }

        #endregion

        #region 文件工具栏按钮...

        public object FileFilterIcon { get; private set; }
        //public object RuleRemoveIcon { get; private set; }
        //public object RuleUpIcon { get; private set; }
        //public object RuleDownIcon { get; private set; }

        #endregion

        public object ItemUpIcon { get; private set; }
        public object ItemDownIcon { get; private set; }
        public object ItemSortIcon { get; private set; }

        public object ReplaceTextIcon { get; private set; }
        public object ConcatTextIcon { get; private set; }
        public object TrimTextIcon { get; private set; }
        public object DeleteBlockIcon { get; private set; }
        public object DeleteTextIcon { get; private set; }
        public object SubstringIcon { get; private set; }
        public object CasecastTextIcon { get; private set; }

        public object AddExtensionIcon { get; private set; }
        public object DeleteExtensionIcon { get; private set; }
        public object ReplaceExtensionIcon { get; private set; }
        public object CasecastExtensionIcon { get; private set; }

        public object DeleteWithoutNumbersIcon { get; private set; }
        public object MatchNumberCountIcon { get; private set; }
        public object AddIndexIcon { get; private set; }
        public object IncrementDecrementNumberIcon { get; private set; }

        public object AddDateIcon { get; private set; }

        public ToolBarIconExtension(string name = null)
        {
            if (name != null && File.Exists($"{name}.zip"))
            {
                using (Stream fs = new FileStream($"{name}.zip", FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read, true))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            string entryName = Path.GetFileNameWithoutExtension(entry.Name);

                            if (entryName == "README" || entryName == "VERSION" || entryName == "LICENSE")
                                continue;

                            object icon = DecodeEntry(entry);
                            switch (entryName)
                            {
                                case "open": OpenIcon = icon; break;
                                case "clear": DeleteIcon = icon; break;
                                case "apply": ApplyIcon = icon; break;

                                case "undo": UndoIcon = icon; break;
                                case "redo": RedoIcon = icon; break;

                                case "item_up": ItemUpIcon = icon; break;
                                case "item_down": ItemDownIcon = icon; break;
                                case "item_sort": ItemSortIcon = icon; break;

                                case "replace_text": ReplaceTextIcon = icon; break;
                                case "concat_text": ConcatTextIcon = icon; break;
                                case "trim_text": TrimTextIcon = icon; break;
                                case "delete_block": DeleteBlockIcon = icon; break;
                                case "delete_text": DeleteTextIcon = icon; break;
                                case "substring": SubstringIcon = icon; break;
                                case "casecast_text": CasecastTextIcon = icon; break;

                                case "add_ext": AddExtensionIcon = icon; break;
                                case "delete_ext": DeleteExtensionIcon = icon; break;
                                case "replace_ext": ReplaceExtensionIcon = icon; break;
                                case "casecast_ext": CasecastExtensionIcon = icon; break;

                                case "del_without_num": DeleteWithoutNumbersIcon = icon; break;
                                case "match_num_count": MatchNumberCountIcon = icon; break;
                                case "add_index": AddIndexIcon = icon; break;
                                case "inc_dec_num": IncrementDecrementNumberIcon = icon; break;

                                case "add_date": AddDateIcon = icon; break;
                            }
                        }
                    }
                }
            }
            else
            {
                OpenIcon = App.Current.Resources["iconOpenButton"];
                DeleteIcon = System.Windows.Application.Current.Resources["iconDeleteButton"];
                ApplyIcon = System.Windows.Application.Current.Resources["iconApplyButton"];

                UndoIcon = System.Windows.Application.Current.Resources["iconUndoButton"];
                RedoIcon = System.Windows.Application.Current.Resources["iconRedoButton"];
                // 规则工具栏按钮
                RuleAddIcon = System.Windows.Application.Current.Resources["RuleAddIcon"];
                RuleRemoveIcon = System.Windows.Application.Current.Resources["RuleRemoveIcon"];
                RuleUpIcon = System.Windows.Application.Current.Resources["RuleUpIcon"];
                RuleDownIcon = System.Windows.Application.Current.Resources["RuleDownIcon"];

                FileFilterIcon = System.Windows.Application.Current.Resources["FileFilterIcon"];

                ItemSortIcon = System.Windows.Application.Current.Resources["iconItemSort"];

                ReplaceTextIcon = System.Windows.Application.Current.Resources["iconReplaceText"];
                ConcatTextIcon = System.Windows.Application.Current.Resources["iconConcatText"];
                TrimTextIcon = System.Windows.Application.Current.Resources["iconTrimText"];
                DeleteBlockIcon = System.Windows.Application.Current.Resources["iconDeleteBlock"];
                DeleteTextIcon = System.Windows.Application.Current.Resources["iconDeleteText"];
                SubstringIcon = System.Windows.Application.Current.Resources["iconSubstringText"];
                CasecastTextIcon = System.Windows.Application.Current.Resources["iconCasecastText"];

                AddExtensionIcon = System.Windows.Application.Current.Resources["iconAddExtension"];
                DeleteExtensionIcon = System.Windows.Application.Current.Resources["iconDeleteExtension"];
                ReplaceExtensionIcon = System.Windows.Application.Current.Resources["iconReplaceExtension"];
                CasecastExtensionIcon = System.Windows.Application.Current.Resources["iconCasecastExtension"];

                DeleteWithoutNumbersIcon = System.Windows.Application.Current.Resources["iconDeleteWithoutNumber"];
                MatchNumberCountIcon = System.Windows.Application.Current.Resources["iconMatchNumberCount"];
                AddIndexIcon = System.Windows.Application.Current.Resources["iconAddIndex"];
                IncrementDecrementNumberIcon = System.Windows.Application.Current.Resources["iconIncreaseDecreaseNumber"];

                AddDateIcon = System.Windows.Application.Current.Resources["iconAddDate"];
            }

            SharedExtension = this;
        }

        private object DecodeEntry(ZipArchiveEntry entry)
        {
            var ext = Path.GetExtension(entry.FullName);
            if (ext == ".png")
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = entry.Open();
                image.EndInit();
                //image.Freeze ();

                var element = new Image
                {
                    Source = image,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    Stretch = System.Windows.Media.Stretch.None,
                };

                return element;
            }
            else if (ext == ".txt")
            {
                using (Stream stream = entry.Open())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.ASCII, false, 256, true))
                    {
                        return System.Windows.Media.Geometry.Parse(reader.ReadToEnd());
                    }
                }
            }
            return null;
        }
    }
}
