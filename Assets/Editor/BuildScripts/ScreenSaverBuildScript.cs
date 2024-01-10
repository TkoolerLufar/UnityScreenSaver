using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEditor.Build.Content;

public class ScreenSaverBuildScript
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.StandaloneWindows ||
            target == BuildTarget.StandaloneWindows64)
        {
            /// Path to VC++ Solution for the launcher
            string LauncherSolutionDir = Path.Combine(
                Path.GetDirectoryName(Application.dataPath), "ExternalPrograms", "Windows", "UnityScreenSaverLauncher");
            string LauncherProjectPath = Path.Combine(LauncherSolutionDir, "UnityScreenSaverLauncher", "ScreenSaverByUnity.csproj");

            /// csproj project settings
            var csproj = XDocument.Load(LauncherProjectPath);

            // Modify project settings
            var propertyGroup = csproj.Elements("Project").Elements("PropertyGroup").First();
            propertyGroup.SetElementValue("Company", PlayerSettings.companyName);
            propertyGroup.SetElementValue("AssemblyName", PlayerSettings.productName);
            propertyGroup.SetElementValue("AssemblyVersion", $"{PlayerSettings.bundleVersion}");
            csproj.Save(LauncherProjectPath);
        }
    }
}
