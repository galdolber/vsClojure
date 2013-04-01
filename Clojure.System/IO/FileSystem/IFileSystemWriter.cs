using System.IO;

namespace Clojure.Base.IO.FileSystem
{
	public interface IFileSystemWriter
	{
		void CreateFile(Stream contentsStream, string relativePath);
		void CreateDirectory(string relativePath);
	}
}