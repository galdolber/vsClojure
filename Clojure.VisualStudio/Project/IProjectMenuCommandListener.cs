namespace Clojure.VisualStudio.Project
{
    public interface IProjectMenuCommandListener
    {
        void Selected(ProjectSnapshot projectSnapshot);
    }
}