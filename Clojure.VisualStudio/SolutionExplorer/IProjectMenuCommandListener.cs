namespace Clojure.VisualStudio.SolutionExplorer
{
    public interface IProjectMenuCommandListener
    {
        void Selected(ProjectSnapshot projectSnapshot);
    }
}