namespace Clojure.Workspace.Explorer
{
	public class ProjectSnapshot
	{
		private readonly string _path;
		private readonly string _frameworkPath;

		public ProjectSnapshot(string path, string frameworkPath)
		{
			_path = path;
			_frameworkPath = frameworkPath;
		}

		public string FrameworkPath
		{
			get { return _frameworkPath; }
		}

		public string Path
		{
			get { return _path; }
		}
	}
}