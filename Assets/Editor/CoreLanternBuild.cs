using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

// ──────────────────────────────────────────────────────────────────────
// Build automation for Eggcore Protocol (CoreLanternUnity).
//
// The game is constructed entirely at runtime via [RuntimeInitializeOnLoadMethod],
// so the only scene we need to ship is an empty Bootstrap.unity to trigger the
// scene-load callback.
//
// USAGE (from Unity Editor menu):
//     Build > WebGL          → outputs Builds/WebGL/
//     Build > Windows64      → outputs Builds/Windows/EggcoreProtocol.exe
//
// USAGE (headless / CI from PowerShell):
//   & "C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe" `
//       -batchmode -nographics -quit `
//       -projectPath "<projectFolder>" `
//       -executeMethod CoreLanternBuild.BuildWebGL `
//       -logFile -
//
// IMPORTANT: Close the Unity Editor first — batch builds can't share the license
// with an interactive session.
// ──────────────────────────────────────────────────────────────────────
public static class CoreLanternBuild
{
    const string BootstrapScenePath = "Assets/Bootstrap.unity";
    const string ProductName = "Eggcore Protocol";
    const string CompanyName = "CoreLantern";

    [MenuItem("Build/WebGL")]
    public static void BuildWebGL()
    {
        EnsureBootstrapScene();
        var output = "Builds/WebGL";
        if (Directory.Exists(output))
            Directory.Delete(output, true);
        Directory.CreateDirectory(output);

        PlayerSettings.productName = ProductName;
        PlayerSettings.companyName = CompanyName;
        // Disable compression so itch.io / static hosts can serve raw .data/.wasm files
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;

        var options = new BuildPlayerOptions
        {
            scenes = new[] { BootstrapScenePath },
            locationPathName = output,
            target = BuildTarget.WebGL,
            targetGroup = BuildTargetGroup.WebGL,
            options = BuildOptions.None
        };

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        var report = BuildPipeline.BuildPlayer(options);
        Debug.Log($"[Build] WebGL result={report.summary.result} size={report.summary.totalSize / 1024 / 1024}MB output={output}");
        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError("[Build] FAILED");
            if (Application.isBatchMode) EditorApplication.Exit(1);
        }
    }

    [MenuItem("Build/Windows64")]
    public static void BuildWindows()
    {
        EnsureBootstrapScene();
        Directory.CreateDirectory("Builds/Windows");
        var output = "Builds/Windows/EggcoreProtocol.exe";

        PlayerSettings.productName = ProductName;
        PlayerSettings.companyName = CompanyName;

        var options = new BuildPlayerOptions
        {
            scenes = new[] { BootstrapScenePath },
            locationPathName = output,
            target = BuildTarget.StandaloneWindows64,
            targetGroup = BuildTargetGroup.Standalone,
            options = BuildOptions.None
        };

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        var report = BuildPipeline.BuildPlayer(options);
        Debug.Log($"[Build] Windows result={report.summary.result} size={report.summary.totalSize / 1024 / 1024}MB output={output}");
        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError("[Build] FAILED");
            if (Application.isBatchMode) EditorApplication.Exit(1);
        }
    }

    static void EnsureBootstrapScene()
    {
        if (File.Exists(BootstrapScenePath))
            return;
        // Game initialises via RuntimeInitializeOnLoadMethod, so an empty scene is enough
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, BootstrapScenePath);
        Debug.Log("[Build] Created Bootstrap.unity at " + BootstrapScenePath);
    }
}
