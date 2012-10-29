using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Project;

namespace Clojure.VisualStudio.Project.Launching
{
    public static class LaunchParametersExtensions
    {
        public static LaunchParameters CreateLaunchParameters(this ProjectNode project)
        {
            string clojureLoadPath = project.GetProjectProperty("ClojureLoadPath");
            string frameworkPath = project.GetProjectProperty("ClojureRuntimesDirectory") + "\\" + project.GetProjectProperty("ClojureVersion", true);
            string applicationPath = project.GetProjectProperty("ProjectDir", false) + project.GetProjectProperty("OutputPath", false);
            string targetFile = project.GetProjectProperty("StartupFile");
            string remoteMachineDebug = project.GetProjectProperty("RemoteDebugMachine", false);
            string unmanagedDebugging = project.GetProjectProperty("EnableUnmanagedDebugging", false);
            string startupArguments = project.GetProjectProperty("StartupArguments", false);

            frameworkPath = frameworkPath.TrimEnd(new[] { '\\' });
            applicationPath = applicationPath.TrimEnd(new[] { '\\' });

            Guid debugType = !string.IsNullOrEmpty(unmanagedDebugging) && unmanagedDebugging.ToLower() == "true"
                ? new Guid("{3B476D35-A401-11D2-AAD4-00C04F990171}")
                : VSConstants.CLSID_ComPlusOnlyDebugEngine;

            string runnerPath = frameworkPath + "\\Clojure.Main.exe";

            var startupFileType = StartupFileType.Unknown;
            if (targetFile.ToLower().EndsWith(".exe")) startupFileType = StartupFileType.Executable;
            if (targetFile.ToLower().EndsWith(".clj")) startupFileType = StartupFileType.Clojure;

            return new LaunchParameters(
                runnerPath,
                frameworkPath,
                applicationPath,
                targetFile,
                remoteMachineDebug,
                unmanagedDebugging,
                debugType,
                startupArguments,
                startupFileType,
                clojureLoadPath);
        }

        public static void Validate(this LaunchParameters launchParameters)
        {
            if (string.IsNullOrEmpty(launchParameters.FrameworkPath))
                throw new Exception("No clojure framework path defined in project properties.");

            if (string.IsNullOrEmpty(launchParameters.StartupFile))
                throw new Exception("No startup file defined in project properties.");

            if (launchParameters.StartupFileType == StartupFileType.Unknown)
                throw new Exception("Cannot start file " + launchParameters.StartupFile);
        }
    }
}
