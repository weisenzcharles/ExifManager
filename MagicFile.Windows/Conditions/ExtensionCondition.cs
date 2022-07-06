using Daramee.Nargs;
using MagicFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicFile.Conditions
{
    public class ExtensionCondition : ICondition
    {
        public string Name => "condition_extension";

        [Argument(Name = "extension")]
        public string Extension { get; set; } = "";

        public bool IsSatisfyThisCondition(FileInfo file)
        {
            throw new NotImplementedException();
        }

        public bool IsValid(FileInfo file)
        {
            return Path.GetExtension(file.ChangedFilename) == Extension;
        }
    }
}
