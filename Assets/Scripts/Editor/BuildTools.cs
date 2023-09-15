using System;
using System.IO;
using Config;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// 用于通过命令行构建工程
/// <remarks>
/// Rider会提示
/// Namespace does not correspond to file location, should be: 'Editor'
/// 但如果添加 namespace Editor 将导致其它类的编译错误
/// </remarks>
/// </summary>
public static class BuildTools
{
    private static readonly string[] DefaultScenes =
    {
        "Assets/Scenes/Offline.unity",
        "Assets/Scenes/Lobby.unity",
        "Assets/Scenes/Arena.unity",
        "Assets/Scenes/UI.unity"
    };

    /// <summary>
    /// 构建 Windows 客户端软件
    /// </summary>
    [MenuItem("Build/Build Windows Client")]
    public static void BuildWindowsClient()
    {
        var options = new BuildPlayerOptions
        {
            scenes = DefaultScenes,
            locationPathName = Application.dataPath + "/../Build/WindowsClient/SimulatorX.exe",
            // Windows.x86_64
            target = BuildTarget.StandaloneWindows64,
            targetGroup = BuildTargetGroup.Standalone,
            // Client Build
            options = BuildOptions.None
        };
        Build(options);
    }

    /// <summary>
    /// 构建 Windows 服务端软件
    /// </summary>
    [MenuItem("Build/Build Windows Server")]
    public static void BuildWindowsServer()
    {
        var options = new BuildPlayerOptions
        {
            scenes = DefaultScenes,
            locationPathName = Application.dataPath + "/../Build/WindowsServer/SimulatorX-Server.exe",
            // Windows.x86_64
            target = BuildTarget.StandaloneWindows64,
            targetGroup = BuildTargetGroup.Standalone,
            // Server Build
            options = BuildOptions.EnableHeadlessMode
        };
        Build(options);
    }
    
    [MenuItem("Build/Build Linux Client")]
    public static void BuildLinuxClient()
    {
        var options = new BuildPlayerOptions
        {
            scenes = DefaultScenes,
            locationPathName = Application.dataPath + "/../Build/LinuxClient/simulatorx",
            // Linux.x86_64
            target = BuildTarget.StandaloneLinux64,
            targetGroup = BuildTargetGroup.Standalone,
            // Server Build
            options = BuildOptions.None
        };

        Build(options);
    }

    /// <summary>
    /// 构建 Linux 服务端软件
    /// </summary>
    [MenuItem("Build/Build Linux Server")]
    public static void BuildLinuxServer()
    {
        var options = new BuildPlayerOptions
        {
            scenes = DefaultScenes,
            locationPathName = Application.dataPath + "/../Build/LinuxServer/simulatorx-server",
            // Linux.x86_64
            target = BuildTarget.StandaloneLinux64,
            targetGroup = BuildTargetGroup.Standalone,
            // Server Build
            options = BuildOptions.EnableHeadlessMode
        };

        Build(options);
    }

    /// <summary>
    /// 构建前执行
    /// </summary>
    private static void PreBuild(BuildPlayerOptions options)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(options.targetGroup, options.target);
        Debug.LogWarning($"Starting Build: Platform={options.target}, Options={options.options.ToString()}");
        var binaryFile = new FileInfo(options.locationPathName);
        var dir = binaryFile.Directory;

        //删除
        if (dir != null && Directory.Exists(dir.FullName))
        {
            Directory.Delete(dir.FullName, true);
        }

        VersionHandle();
    }

    /// <summary>
    /// 构建后执行
    /// </summary>
    private static void PostBuild(BuildPlayerOptions options, BuildReport report)
    {
        var binaryFile = new FileInfo(options.locationPathName);
        var dir = binaryFile.Directory;
        Debug.Assert(dir != null);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.LogWarning($"Build Finished: " +
                             $"Platform: {options.target}, Options: {options.options.ToString()} \n" +
                             $"Generated in: {report.summary.outputPath} \n" +
                             $"Size: {report.summary.totalSize} \n" +
                             $"Total time: {report.summary.totalTime}");
        }
        else
        {
            Debug.LogError($"Build Failed! " +
                           $"Total time: {report.summary.totalTime}");
        }
    }

    /// <summary>
    /// 执行构建
    /// </summary>
    /// <param name="options">构建选项</param>
    private static void Build(BuildPlayerOptions options)
    {
        PreBuild(options);
        var report = BuildPipeline.BuildPlayer(options);
        PostBuild(options, report);
    }

    private static void VersionHandle()
    {
        Debug.LogWarning("Handling version control.");
        //var phaseVersion = Environment.GetEnvironmentVariable("SIMULATORX_PHASEVERSION") ?? "";
        //var channel = Environment.GetEnvironmentVariable("SIMULATORX_CHANNEL") ?? "insider";
        //var tag = Environment.GetEnvironmentVariable("SIMULATORX_TAG") ?? "";
        var phaseVersion = "1";
        var channel = "release";
        var tag = "";

        //Manually setting patch version
        if (!phaseVersion.Equals(""))
        {
            var versionCodes = PlayerSettings.bundleVersion.Split('.');
            versionCodes[3] = phaseVersion;
            PlayerSettings.bundleVersion = string.Join(".", versionCodes);
            AssetDatabase.SaveAssets();
        }

        GlobalConfig.Version.versionModifier = VersionModifierParser.Parse(channel);

        //Auto generating tag
        if (tag.Equals(""))
        {
            tag = channel.Equals("insider") ? "insider" : $"{channel}-{PlayerSettings.bundleVersion}";
        }

        Debug.LogWarning("Build Detail: " +
                         $"Version={PlayerSettings.bundleVersion}, " +
                         $"Channel={channel}, " +
                         $"Tag={tag}");

        if (Environment.GetEnvironmentVariable("TEAMCITY_VERSION") != null)
        {
            TeamCityCi.TeamCityInteract(tag, channel);
        }
    }

    private struct TeamCityCi
    {
        public static void TeamCityInteract(string tag, string channel)
        {
            TeamCitySet("publish.artifact.version", PlayerSettings.bundleVersion);
            TeamCitySet("publish.artifact.tag", tag);
            TeamCitySet("publish.artifact.channel", channel);

            TeamCityTag(channel);
            var versions = PlayerSettings.bundleVersion.Split('.');
            TeamCityTag(string.Join(".", versions[0], versions[1], versions[2]));
        }

        private static void TeamCitySet(string key, string value)
        {
            Debug.LogWarning($"##teamcity[setParameter name='{key}' value='{value}']");
        }

        private static void TeamCityTag(string tag)
        {
            Debug.LogWarning($"##teamcity[addBuildTag '{tag}']");
        }
    }
}