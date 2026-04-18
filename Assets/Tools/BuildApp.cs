// // 检测当前编辑器平台，不一致，则中断

// // 优化设置 - 通用
// // android app KeyStore设置
// // Debug与Release模式
// // -- 支持Unity profiler
// // -- log级别设置
// // -- 宏处理

// // 命名
// // Product Name: Sweech RUN + Debug/Release + Version + Date.apk

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildApp {
    public static GameSettings gameSettings;

    public static void GetGameSettingsScript () {
        // 签名的流程已经打开了 Luanch_Build场景
        GameSettings[] scripts = Object.FindObjectsOfType<GameSettings> ();
        gameSettings = scripts[0];
    }

    public static void Build_Android () {
        string[] scenes = {
            "Assets/Scenes_State/Launch_Build.unity"
        };

        string buildPath = "D:/Sweech-Client/Build/Build.apk";

        BuildPipeline.BuildPlayer (scenes, buildPath, BuildTarget.Android, BuildOptions.None);
    }

    public static void OpenBuildDirectory (string buildPath) {
        // 获取APK文件所在的目录
        string directoryPath = Path.GetDirectoryName (buildPath);

        if (Directory.Exists (directoryPath)) {
            // 在文件管理器中打开目录
            EditorUtility.RevealInFinder (directoryPath);
            Debug.Log ($"构建完成！已打开目录: {directoryPath}");
        } else {
            Debug.LogWarning ($"目录不存在: {directoryPath}");
        }
    }

    public static void HandleSceneForBuild_Pre () {
        string scenePath = "Assets/Scenes_State/Launch_Build.unity";

        EditorSceneManager.OpenScene (scenePath);
        AssetDatabase.SaveAssets ();
        AssetDatabase.Refresh ();

        // 3. 清空 BuildSettings 里的所有场景
        // ClearBuildScenes();
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];
        Debug.Log ("已清空 BuildSettings 中的场景列表");
        AssetDatabase.SaveAssets ();
        AssetDatabase.Refresh ();

        // 4. 把目标场景添加到 BuildSettings
        AddSceneToBuildSettings (scenePath);
        AssetDatabase.SaveAssets ();
        AssetDatabase.Refresh ();

        Debug.Log ($"=== 构建前 已处理场景: {scenePath}");
    }
    public static void HandleSceneForDevMode () {
        // 打开Luanch_Build场景
        string scenePath = "Assets/Scenes_State/Launch_Inner.unity";

        // 3. 清空 BuildSettings 里的所有场景
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];
        Debug.Log ("已清空 BuildSettings 中的场景列表");

        // 4. 把目标场景添加到 BuildSettings
        AddSceneToBuildSettings (scenePath);

        EditorSceneManager.OpenScene (scenePath, OpenSceneMode.Single);
        Debug.Log ("已切换到场景: " + scenePath);
    }

    // public static void HandleSceneForBuild_Post () {
    //     // 打开Luanch_Build场景
    //     string scenePath = "Assets/Scenes_State/Launch_Inner.unity";

    //     // 3. 清空 BuildSettings 里的所有场景
    //     // ClearBuildScenes();
    //     EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];
    //     Debug.Log ("已清空 BuildSettings 中的场景列表");
    //     AssetDatabase.SaveAssets ();
    //     AssetDatabase.Refresh ();

    //     // 4. 把目标场景添加到 BuildSettings
    //     AddSceneToBuildSettings (scenePath);
    //     AssetDatabase.SaveAssets ();
    //     AssetDatabase.Refresh ();

    //     EditorApplication.delayCall += () => {
    //         EditorSceneManager.OpenScene (scenePath, OpenSceneMode.Single);
    //         Debug.Log ("已切换到场景: " + scenePath);
    //         AssetDatabase.SaveAssets ();
    //         AssetDatabase.Refresh ();

    //         Debug.Log ($"=== 构建后 已处理场景: {scenePath}");
    //     };
    // }

    static void AddSceneToBuildSettings (string scenePath) {
        // 创建新的场景列表
        EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[1];
        // 添加目标场景
        newScenes[0] = new EditorBuildSettingsScene (scenePath, true); // true 表示启用该场景
        // 设置到 BuildSettings
        EditorBuildSettings.scenes = newScenes;
        Debug.Log ($"已添加场景到 BuildSettings: {scenePath}");
    }

    static bool isDevOrBuildMode;
    public static void Tool_ChangeBuildOrDevMode () {
        /// 获取当前活动的场景
        Scene activeScene = SceneManager.GetActiveScene ();
        if (!activeScene.IsValid ()) {
            throw new UnityEditor.Build.BuildFailedException ("构建验证失败：原因说明");
        }
        string scenePath = activeScene.path;
        Debug.Log ($"scenePath:{scenePath}");
        if (scenePath.Equals ("Assets/Scenes_State/Launch_Build.unity")) {
            Debug.Log ("当前是Build_构建模式");
            isDevOrBuildMode = false;
        } else {
            Debug.Log ("当前是Dev_开发模式");
            isDevOrBuildMode = true;
        }

        if (isDevOrBuildMode) {
            //切换为build模式
            BuildApp.HandleSceneForBuild_Pre ();
            isDevOrBuildMode = false;
            Debug.Log ("已切换为Build_构建模式");
        } else {
            //切换为Dev模式
            BuildApp.HandleSceneForDevMode ();
            isDevOrBuildMode = true;
            Debug.Log ("已切换为Dev_开发模式");
        }
    }

    public static void SetDevelopmentBuild () {
        EditorUserBuildSettings.development = BuildApp.gameSettings.isBuildPerfTestPackage;
        // AssetDatabase.SaveAssets ();
        // AssetDatabase.Refresh ();
    }

    public static void HandleDebugUtil () {
        DebugUtil DebugUtil = BuildApp.gameSettings.GetComponent<DebugUtil> ();
        if (BuildApp.gameSettings.isBuildPerfTestPackage || BuildApp.gameSettings.serverType is LoginServerType.Release or LoginServerType.Review) {
            if (DebugUtil != null)
                GameObject.DestroyImmediate (DebugUtil);
        } else {
            if (DebugUtil == null)
                DebugUtil = BuildApp.gameSettings.gameObject.AddComponent<DebugUtil> ();

            DebugUtil.enabled = true;
        }
    }
}
#endif