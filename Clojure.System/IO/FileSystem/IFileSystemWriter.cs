using System.IO;

namespace Clojure.System.IO.FileSystem
{
	public interface IFileSystemWriter
	{
		void CreateFile(Stream contentsStream, string relativePath);
		void CreateDirectory(string relativePath);
	}
}