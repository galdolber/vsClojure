namespace Clojure.Workspace.Explorer
{
    public interface IProjectMenuCommandListener
    {
        void Selected(ProjectSnapshot projectSnapshot);
    }
}