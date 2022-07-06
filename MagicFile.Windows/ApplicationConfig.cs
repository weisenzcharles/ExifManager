using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MagicFile
{

    public static class ApplicationConfig
    {
        public static Assembly ApplicationAssembly { get; private set; }
        public static Version ApplicationVersion => ApplicationAssembly.GetName().Version;
        public static string ApplicationName => ApplicationAssembly.GetName().Name;
        public static string ApplicationNamespace => ApplicationAssembly.EntryPoint.DeclaringType.Namespace;
        public static Guid ApplicationGUID => ApplicationAssembly.GetType().GUID;
        public static string ApplicationPath => Process.GetCurrentProcess().MainModule.FileName;
        public static string ApplicationAuthor { get; private set; }
        public static string ApplicationWebSite { get; private set; }
        //public static string ApplicationCompany { get; private set; }
        public static string ApplicationCopyright { get; private set; }
        public static string ApplicationRepositoryName { get; private set; }

        public static void Initialize(Assembly assembly, string author, string repositoryName)
        {
            ApplicationAssembly = assembly;
            ApplicationAuthor = author;
            ApplicationRepositoryName = repositoryName;
        }
    }
}
