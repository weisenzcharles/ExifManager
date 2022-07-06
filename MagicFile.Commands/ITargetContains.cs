using System.Collections.Generic;

namespace MagicFile
{
	public interface ITargetContains
	{
		void SetTargets(IEnumerable<FileInfo> files);
	}
}
