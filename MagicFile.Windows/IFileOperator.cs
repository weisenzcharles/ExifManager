using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicFile
{
    public interface IFileOperator
    {
        void BeginBatch();
        void EndBatch();

        void Move(string destination, string source, bool overwrite);
        void Copy(string destination, string source, bool overwrite);

        IEnumerable<string> GetFiles(string directory, bool topDirectoryOnly);

        bool FileExists(string path);
        bool DirectoryExists(string path);
    }
}
