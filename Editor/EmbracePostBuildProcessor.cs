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

        // Add embrace config
        FileInfo fileToCopy = new FileInfo(baseDirectory + "/Android/embrace-config.json");
#if UNITY_2019_3_OR_NEWER
        FileInfo fileInfo = new FileInfo(string.Format("{0}/launcher/src/main/{1}", projectPath, "embrace-config.json"));
        if (fileInfo.Directory.Exists == false)
        {
            projectPath = Directory.GetParent(projectPath).FullName;
            fileInfo = new FileInfo(string.Format("{0}/launcher/src/main/{1}", projectPath, "embrace-config.json"));
        }
#else
        FileInfo fileInfo = new FileInfo(string.Format("{0}/src/main/{1}", projectPath, "embrace-config.json"));
#endif
        Debug.Log("final path: " + fileInfo.FullName + " : " + projectPath);
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
// In Unity 2019.3 the iOS target was split into two targets, a launcher and the framework.
// We have to be able to integrate with both target setups.
#if UNITY_2019_3_OR_NEWER
    private static string GetProjectName(PBXProject project)
    {
        return project.GetUnityMainTargetGuid();
    }
#else
    private static string GetProjectName(PBXProject project)
    {
        return project.TargetGuidByName(PBXProject.GetUnityTargetName()); ;
    }
#endif

    // iOS Xcode project fixup
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        string baseDirectory = EmbracePostBuildProcessorUtils.BaseDirectory();
        FileInfo embracePlistFile = new FileInfo(baseDirectory + "/iOS/Embrace-Info.plist");
        FileInfo embraceRunSHFile = new FileInfo(baseDirectory + "/iOS/run.sh");
        string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        string frameworkPath = "Frameworks/Plugins/Embrace/iOS/Embrace.xcframework/ios-arm64_armv7/Embrace.framework";

        // Load pbxproj
        PBXProject project = new PBXProject();
        project.ReadFromFile(projectPath);
        string targetGuid = GetProjectName(project);

        // Enable dSYM
        string debugConfigGuid = project.BuildConfigByName(targetGuid, "Debug");
        string releaseConfigGuid = project.BuildConfigByName(targetGuid, "ReleaseForRunning");
        project.SetBuildPropertyForConfig(debugConfigGuid, "DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym");
        project.SetBuildPropertyForConfig(releaseConfigGuid, "DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym");

        // Load Embrace-Info.plist
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(embracePlistFile.FullName);
        PlistElement apiKey = plist.root["API_KEY"];
        PlistElement apiToken = plist.root["API_TOKEN"];

        if (apiKey != null && apiToken != null)
        {
            // Add phase for dSYM upload
            string runScriptName = "Embrace Symbol Upload";
            string runScriptPhase = "EMBRACE_ID=" + apiKey.AsString() + " EMBRACE_TOKEN=" + apiToken.AsString() + " '" + embraceRunSHFile.FullName + "'";
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

        // Copy Embrace-Info.plist
        string resourcesBuildPhase = project.GetResourcesBuildPhaseByTarget(targetGuid);
        string resourcesFilesGuid = project.AddFile(embracePlistFile.FullName, "/Embrace-Info.plist", PBXSourceTree.Source);
        project.AddFileToBuildSection(targetGuid, resourcesBuildPhase, resourcesFilesGuid);

        // Embed Embrace.framework
        string fileGuid = project.FindFileGuidByProjectPath(frameworkPath);
        // fallback for SDK naming
        if (fileGuid == null)
        {
            frameworkPath = "Frameworks/Plugins/EmbraceSDK/iOS/Embrace.xcframework/ios-arm64_armv7/Embrace.framework";
            fileGuid = project.FindFileGuidByProjectPath(frameworkPath);
        }
        // fallback for upm mode:
        if (fileGuid == null) {
            frameworkPath = "Frameworks/io.embrace.unity/iOS/Embrace.xcframework/ios-arm64_armv7/Embrace.framework";
            fileGuid = project.FindFileGuidByProjectPath(frameworkPath);
        }
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
        else if (new DirectoryInfo("Assets/Plugins/EmbraceSDK").Exists)
        {
            return "Assets/Plugins/EmbraceSDK";
        }
        else
        {
            Debug.Log("Embrace dictionary not found");
            return null;
        }
    }
}