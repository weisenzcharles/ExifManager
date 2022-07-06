using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace MagicFile
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        StringTable ownLocalizer;

        public App()
        {
            ApplicationConfig.Initialize(Assembly.GetExecutingAssembly(), "Charles", "MagicFile");
            ownLocalizer = new StringTable();
        }
    }
}
