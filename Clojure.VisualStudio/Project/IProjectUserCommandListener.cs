namespace Clojure.VisualStudio.Project
{
    public interface IProjectUserCommandListener
    {
        void LaunchRepl(ProjectSnapshot projectSnapshot);
    }
}