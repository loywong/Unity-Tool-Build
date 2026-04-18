#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class StartupWithScene_Launch {
    public static void OpenLaunchScene () {
        OpenScene_Launch();
    }

    [MenuItem ("Assets/打开_Launch_Inner场景", priority = 7)]
    static void OpenScene_Launch () {
        // 获取当前选中的资源
        // Object selectedObject = Selection.activeObject;
        // if (selectedObject is SceneAsset) {
            string scenePath = "Assets/Scenes_State/Launch_Inner.unity"; //AssetDatabase.GetAssetPath(selectedObject);
            EditorSceneManager.OpenScene (scenePath);
        // } else {
        //     Debug.Log ("Please select a scene asset.");
        // }
    }
    [MenuItem ("Assets/打开_DemoNew场景", priority = 7)]
    static void OpenScene_Test () {
        string scenePath = "Assets/Scenes/demo_new.unity";
        EditorSceneManager.OpenScene (scenePath);
    }
    [MenuItem ("Assets/打开_World场景", priority = 7)]
    static void OpenScene_Test_Lobby () {
        string scenePath = "Assets/Scenes_State/World_New.unity";
        EditorSceneManager.OpenScene (scenePath);
    }

    [MenuItem ("Assets/启动_Launch场景", priority = 7)]
    static void LaunchGameModeWithFixedScene () {
        string fixedScenePath = "Assets/Scenes_State/Launch.unity";
        EditorSceneManager.OpenScene (fixedScenePath);
        EditorApplication.isPlaying = true;
    }

    [MenuItem ("Assets/@@@循环切换-Dev开发_Build打包-模式 ", priority = 7)]
    static void Tool_ChangeBuildOrDevMode () {
        // 切换Build和Dev模式时，同步设置Development Build选项
        BuildApp.Tool_ChangeBuildOrDevMode();

        // EditorApplication.delayCall += () => {
            BuildApp.GetGameSettingsScript();

            BuildApp.SetDevelopmentBuild();
            // 切换Build和Dev模式时，动态加载卸载 DebugUtil脚本
            BuildApp.HandleDebugUtil();

            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();
        // };
    }
}
#endif