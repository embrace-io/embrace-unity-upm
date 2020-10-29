using System.IO;
using UnityEngine;
using UnityEditor;

#if UNITY_ANDROID
using UnityEditor.Android;

public class EmbracePostBuildProcessor : IPostGenerateGradleAndroidProject
{
    public int callbackOrder { get { return 0; } }

    // Android gradle fixup
    public void OnPostGenerateGradleAndroidProject(string projectPath)
    {
        string baseDirectory = EmbracePostBuildProcessorUtils.BaseDirectory();
        var rootDirPath = Path.GetDirectoryName(projectPath);
        var launcherParh = Path.Combine(rootDirPath, "launcher");

        // Add embrace config
        FileInfo fileToCopy = new FileInfo(baseDirectory + "/Android/embrace-config.json");
        var fileInfo = new FileInfo(string.Format("{0}/src/main/{1}", launcherParh, "embrace-config.json"));
        fileToCopy.CopyTo(fileInfo.FullName);
    }
}
#endif

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEditor.Callbacks;

public class EmbracePostBuildProcessor
{
    // iOS Xcode project fixup
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        string baseDirectory = EmbracePostBuildProcessorUtils.BaseDirectory();
        string plistPath = baseDirectory + "/iOS/Embrace-Info.plist";
        string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject project = new PBXProject();
        project.ReadFromFile(projectPath);
        string targetGuid = project.GetUnityMainTargetGuid();

        // Enable dSYM
        string debugConfigGuid = project.BuildConfigByName(targetGuid, "Debug");
        string releaseConfigGuid = project.BuildConfigByName(targetGuid, "ReleaseForRunning");
        project.SetBuildPropertyForConfig(debugConfigGuid, "DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym");
        project.SetBuildPropertyForConfig(releaseConfigGuid, "DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym");

        // Add phase for dSYM upload
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
        PlistElement apiKey = plist.root["API_KEY"];
        PlistElement apiToken = plist.root["API_TOKEN"];
        string runScriptName = "Embrace symbol upload";
        string fullRunSHPath = new FileInfo(baseDirectory + "/iOS/run.sh").FullName;

        if (apiKey != null && apiToken != null)
        {
            string runScriptPhase = "EMBRACE_ID=" + apiKey.AsString() + " EMBRACE_TOKEN=" + apiToken.AsString() + " '" + fullRunSHPath + "'";
            string[] phases = project.GetAllBuildPhasesForTarget(targetGuid);
            bool embracePhaseExists = false;
            foreach (var item in phases)
            {
                if (project.GetBuildPhaseName(item) == runScriptName)
                {
                    embracePhaseExists = true;
                    break;
                }
            }
            if (embracePhaseExists == false)
            {
                project.AddShellScriptBuildPhase(targetGuid, runScriptName, "/bin/sh", runScriptPhase);
            }
        }

        // Add Embrace-Info.plist
        string embraceInfoPlist = new FileInfo(plistPath).FullName;
        string resourcesBuildPhase = project.GetResourcesBuildPhaseByTarget(targetGuid);
        string resourcesFilesGuid = project.AddFile(embraceInfoPlist, "/Embrace-Info.plist", PBXSourceTree.Source);
        project.AddFileToBuildSection(targetGuid, resourcesBuildPhase, resourcesFilesGuid);

        // Embed Embrace.framework
        string framework = "Frameworks/io.embrace.unity/iOS/Embrace.framework";
        string fileGuid = project.FindFileGuidByProjectPath(framework);
        PBXProjectExtensions.AddFileToEmbedFrameworks(project, targetGuid, fileGuid);

        project.WriteToFile(projectPath);
    }
}
#endif

public class EmbracePostBuildProcessorUtils
{
    public static string BaseDirectory()
    {
        if (new DirectoryInfo("Packages/io.embrace.unity").Exists)
        {
            return "Packages/io.embrace.unity";
        }
        else if (new DirectoryInfo("Assets/Plugins/Embrace").Exists)
        {
            return "Assets/Plugins/Embrace";
        }
        else
        {
            Debug.Log("Embrace dictionary not found");
            return null;
        }
    }
}