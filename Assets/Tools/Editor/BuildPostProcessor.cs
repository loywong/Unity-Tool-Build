using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildPostProcessor : IPostprocessBuildWithReport {
    // 构建后执行的顺序，数字越小，越先执行
    public int callbackOrder => 100;

    public void OnPostprocessBuild (BuildReport report) {
        Debug.Log ("OnPostprocessBuild");

        BuildApp.HandleSceneForBuild_Post ();

        // 获取构建完成的目标平台
        BuildTarget target = report.summary.platform;

        if (target == BuildTarget.Android) {
            // 在构建 Android 时进行的操作
            Debug.Log ("Build completed for Android. Performing post-build operations...");
            HandleSymbols ();

            EditorApplication.delayCall += () => {
                HandleAPKName ();
            };
        }

        Debug.Log ("Post-build processing completed.");
    }

    void HandleSymbols () {
        BuildTargetGroup targetGroup = BuildTargetGroup.Android;
        // 获取当前的 Scripting Define Symbols 列表
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup (targetGroup);
        // 你想要删除的符号列表，这里以 "DEBUG" 和 "TEST_MODE" 为例
        string[] symbolsToAdd = new string[] { "PROJECT_LOG", "PROJECT_LOG_TEST" };
        foreach (string symbol in symbolsToAdd) {
            // 检查是否已经包含该符号，如果不包含，则添加
            if (!defines.Contains (symbol)) {
                Debug.Log ("Added symbols: " + string.Join (", ", symbol));
                if (defines.Length > 0) {
                    defines += ";" + symbol;
                } else {
                    defines = symbol;
                }
            }
        }
        // 更新 PlayerSettings 中的 Scripting Define Symbols
        PlayerSettings.SetScriptingDefineSymbolsForGroup (targetGroup, defines);
    }

    // 处理apk名字
    string newPath;
    void HandleAPKName () {
        // string projectRoot = Directory.GetParent (Directory.GetParent (Application.dataPath).FullName).FullName;
        string projectRoot = Directory.GetParent (Application.dataPath).FullName;
        string buildFolder = Path.Combine (projectRoot, "Build");

        string oldApkPath;
        bool isAAB = EditorUserBuildSettings.buildAppBundle;
        if (isAAB) oldApkPath = Path.Combine (buildFolder, "build.aab");
        else oldApkPath = Path.Combine (buildFolder, "build.apk");

        if (!File.Exists (oldApkPath)) {
            Debug.LogError ($"APK不存在:{oldApkPath}");
            return;
        }

        // 1. 产品名（PlayerSettings.productName 允许带空格，这里保留）
        string productName = "YourAppName";

        // 2. 服务端类型：通过 ScriptingDefineSymbols 里定义 SERVER_xxx 宏
        string serverType = BuildApp.gameSettings.serverType.ToString ();

        // 3. 版本号（PlayerSettings 里的 version）
        string version = PlayerSettings.bundleVersion;

        // 4. 资源版本号
        string resVersion = PlayerSettings.Android.bundleVersionCode.ToString ();

        // 5. 发布渠道
        string channel = BuildApp.gameSettings.loginChannelType.ToString ();

        // 6. 包名
        string packageName = PlayerSettings.GetApplicationIdentifier (BuildTargetGroup.Android);

        // 7. 日期-时间
        string dateTime = System.DateTime.Now.ToString ("yyyyMMdd[HHmmss]");

        // 组合新文件名
        string fileFormat = isAAB? ".aab": ".apk";
        string newFileName;
        if (BuildApp.gameSettings.isInternalMemberLogin)
            newFileName = $"{productName}_{serverType}_v{version}_vbc{resVersion}_{dateTime}_{channel}[{packageName}]_XXXInternal{fileFormat}";
        else
            newFileName = $"{productName}_{serverType}_v{version}_vbc{resVersion}_{dateTime}_{channel}[{packageName}]{fileFormat}";
        // 去掉非法文件名字符
        newFileName = string.Concat (newFileName.Split (Path.GetInvalidFileNameChars ()));

        newPath = Path.Combine (buildFolder, newFileName);

        // 重命名
        File.Move (oldApkPath, newPath);

        Debug.Log ($"APK 重命名完成：{newPath}");
    }
}