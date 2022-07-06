﻿using System;
using System.IO;

namespace MagicFile.Commands.Filename
{
	[Serializable, LocalizationKey("Command_Name_DeleteFilename")]
	public class DeleteFilenameCommand : ICommand, IOrderBy
	{
		public int Order => int.MinValue + 7;

		public bool ParallelProcessable => true;
		public CommandCategory Category => CommandCategory.Filename;

		public bool DoCommand(FileInfo file)
		{
			file.ChangedFilename = Path.GetExtension(file.ChangedFilename);
			return true;
		}
	}
}
