﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.DaramCommonLib
{
	public static class ProgramHelper
	{
		public static Assembly ApplicationAssembly { get; private set; }
		public static Version ApplicationVersion => ApplicationAssembly.GetName ().Version;
		public static string ApplicationName => ApplicationAssembly.GetName ().Name;
		public static string ApplicationNamespace => ApplicationAssembly.EntryPoint.DeclaringType.Namespace;
		public static Guid ApplicationGUID => ApplicationAssembly.GetType ().GUID;
		public static string ApplicationPath => Process.GetCurrentProcess ().MainModule.FileName;
		public static string GitHubAuthor { get; private set; }
		public static string GitHubRepositoryName { get; private set; }

		public static void Initialize ( Assembly assembly, string githubAuthor, string githubRepositoryName )
		{
			ApplicationAssembly = assembly;

			GitHubAuthor = githubAuthor;
			GitHubRepositoryName = githubRepositoryName;
		}
	}
}
