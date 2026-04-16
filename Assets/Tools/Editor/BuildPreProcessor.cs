using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildPreProcessor : IPreprocessBuildWithReport {
    // 构建前执行的顺序，数字越小，越先执行
    public int callbackOrder => 100;

    public void OnPreprocessBuild (BuildReport report) {
        Debug.Log ("OnPreprocessBuild");

        // Debug.LogError("即将抛出异常...");  // 确认执行到此处
        // throw new BuildFailedException("构建验证失败：原因说明");

        // 获取当前活动的场景
        Scene activeScene = SceneManager.GetActiveScene ();
        if (!activeScene.IsValid ()) {
            throw new BuildFailedException ("构建验证失败：原因说明");
        }

        string scenePath = activeScene.path;
        Debug.Log ($"scenePath:{scenePath}");
        // throw new BuildFailedException("构建验证失败, 当前打开的场景不正确");
        if (!scenePath.Equals ("Assets/Scenes_State/Launch_Build.unity")) {
            // 抛出 BuildFailedException 来中止构建
            throw new BuildFailedException ("构建验证失败, 当前打开的场景不正确");
        }

        BuildApp.GetGameSettingsScript ();

        // 获取构建完成的目标平台
        BuildTarget target = report.summary.platform;

        HandleProductName ();
        Debug.Log ($"___________ PlayerSettings.productName:{PlayerSettings.productName}, server type:{BuildApp.gameSettings.serverType}");

        if (target == BuildTarget.Android) {
            // 检查构建条件
            if (Application.version != BuildApp.gameSettings.AppVersion) {
                throw new BuildFailedException ("构建被中断: 版本号{Application.version}设置不对, 应该是{gameSettings.AppVersion}");
            }

            HandleSymbols ();
            if (!BuildApp.gameSettings.isUseAutoBuildMachine) {
                HandleAPKVersion ();
            }
        }

        // // 你可以在此处添加更多的预处理操作，例如修改其他 PlayerSettings 或者检查资源等
        // // 例如，修改应用的包名
        // PlayerSettings.applicationIdentifier = "com.yourcompany.yourapp";
        // // 或者修改最低 API 级别
        // PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;

        Debug.Log ("Pre-build processing completed.");
    }

    void HandleSymbols () {
        // 获取当前的 BuildTargetGroup，这里假设是 Android，你可以根据需要修改为其他平台
        BuildTargetGroup targetGroup = BuildTargetGroup.Android;
        // 获取当前的 Scripting Define Symbols 列表
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup (targetGroup);
        // 你想要删除的符号列表，这里以 "DEBUG" 和 "TEST_MODE" 为例
        string[] symbolsToRemove = null;
        if (BuildApp.gameSettings.serverType is LoginServerType.Release or LoginServerType.Review)
            symbolsToRemove = new string[] { "PROJECT_LOG", "PROJECT_LOG_TEST" };
        else
            symbolsToRemove = new string[] { "PROJECT_LOG" }; //,"ADDRESSABLES_LOG_ALL","DEBUG_PLAYASSETDELIVERY"
        foreach (string symbol in symbolsToRemove) {
            // 使用 StringReplace 函数将符号替换为空字符串，从而实现删除操作
            Debug.Log ("Removed symbols: " + string.Join (", ", symbol));
            defines = defines.Replace (symbol, "");
        }
        // 更新 PlayerSettings 中的 Scripting Define Symbols
        PlayerSettings.SetScriptingDefineSymbolsForGroup (targetGroup, defines);
    }

    void HandleAPKVersion () {
        if (BuildApp.gameSettings.serverType == LoginServerType.Release) {
            if (BuildApp.gameSettings.loginChannelType is LoginChannelType.NONE) {
                throw new BuildFailedException ("release版本的渠道不可能为NONE");
            }
            return;
        }
        PlayerSettings.bundleVersion = "0.0.00";
        AssetDatabase.SaveAssets ();
        AssetDatabase.Refresh ();
    }
    void HandleProductName () {
        if (BuildApp.gameSettings.serverType == LoginServerType.Release) {
            if (BuildApp.gameSettings.loginChannelType is LoginChannelType.NONE) {
                throw new BuildFailedException ("release版本的渠道不可能为NONE");
            }

            PlayerSettings.productName = "YourAppName";
            return;
        }

        if (BuildApp.gameSettings.serverType == LoginServerType.Test_Inner)
            PlayerSettings.productName = "YourAppName_TestInner";
        else if (BuildApp.gameSettings.serverType == LoginServerType.Test_Outer)
            PlayerSettings.productName = "YourAppName_TestOuter";

        AssetDatabase.SaveAssets ();
        AssetDatabase.Refresh ();
    }
}