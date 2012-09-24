using System.IO;
using Clojure.System.IO.Streams;

namespace Clojure.System.IO.FileSystem
{
	public class RelativePathFileSystemWriter : IFileSystemWriter
	{
		private readonly string _rootPath;

		public RelativePathFileSystemWriter(string rootPath)
		{
			_rootPath = rootPath;
		}

		public void CreateFile(Stream contentsStream, string relativePath)
		{
			using (var inflatedFileStream = File.Create(Path.Combine(_rootPath, relativePath)))
			{
				contentsStream.CopyContentsTo(inflatedFileStream);
			}
		}

		public void CreateDirectory(string relativePath)
		{
			Directory.CreateDirectory(Path.Combine(_rootPath, relativePath));
		}
	}
}
