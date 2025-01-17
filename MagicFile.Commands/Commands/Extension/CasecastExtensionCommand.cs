﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MagicFile.Commands.Extension
{
	[Serializable, LocalizationKey("Command_Name_CasecastExtension")]
	public class CasecastExtensionCommand : ICommand
	{
		public int Order => int.MinValue + 4;

		public bool ParallelProcessable => true;
		public CommandCategory Category => CommandCategory.Extension;

		[LocalizationKey("Command_Argument_CasecastExtension_Casecast")]
		public Casecast1 Casecast { get; set; } = Casecast1.LowercaseAll;
		[LocalizationKey("Commamd_Argument_CastcastExtension_ApplyToDirectory")]
		public bool ApplyToDirectory { get; set; } = false;

		public bool DoCommand(FileInfo file)
		{
			if (!ApplyToDirectory && file.IsDirectory)
				return true;

			var filename = Path.GetFileNameWithoutExtension(file.ChangedFilename);
			var ext = Path.GetExtension(file.ChangedFilename);
			switch (Casecast)
			{
				case Casecast1.UppercaseAll:
					file.ChangedFilename = $"{filename}{ext?.ToUpper()}";
					break;
				case Casecast1.LowercaseAll:
					file.ChangedFilename = $"{filename}{ext?.ToLower()}";
					break;

				default: return false;
			}

			return true;
		}
	}
}
