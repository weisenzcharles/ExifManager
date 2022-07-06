using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ExifTools.ViewModel
{
    public class MainWindowVM
    {
        //private readonly WrapPanel _mainContentPanel;

        private readonly Window _currentWindow;

        public MainWindowVM(Window window) {

            _currentWindow = window;
        }

    }
}
