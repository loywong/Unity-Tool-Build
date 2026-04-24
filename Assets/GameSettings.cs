using Sirenix.OdinInspector;
using UnityEngine;

public enum LoginServerType {
    NONE = 0,
    Release,
    Review, // 审核服, 除了做为过审之后，后续可以作为线上的备份模拟服，提供一些高级账号对外展示之用等
    Test_Outer,
    Test_Inner,
    Dev_Localhsot,
    Dev_ZhangSan,
}

// 引用发布平台-渠道
public enum LoginChannelType {
    NONE = 0, // 不通过平台下载，开发测试过程中 默认选项
    GooglePlay = 1,
    TapTap = 2,
}

public class GameSettings : MonoBehaviour {
    public static GameSettings _instance;

    [Header ("游戏配置Client ----------")]
    [LabelText ("是否启用新手引导"), SerializeField] bool ifHandleNewerGuid;
    [LabelText ("是否启用系统引导"), SerializeField] public bool ifHandleSystemGuid;

    [Header ("纯测试工具 --------------")]
    [LabelText ("显示硬/软/网 环境信息"), SerializeField] bool isShowEnvInfo;
    [LabelText ("DebugConsole-GUI调试"), SerializeField] public bool ifShowGUITest;
    [LabelText ("LogViewer"), SerializeField] public bool IsUseLogViewer;
    [LabelText ("启动本地调试Log"), SerializeField] public bool m_IsDebuglog;
    [LabelText ("[白名单] 测试账号密码登录_4正式服")] bool _isInternalMemberLogin;
    public bool isInternalMemberLogin => _isInternalMemberLogin;
    [LabelText("性能测试包")] public bool isBuildPerfTestPackage;

    [Header ("游戏配置Server ----------")]
    [LabelText ("服务器环境"), SerializeField] public LoginServerType serverType = LoginServerType.Test_Inner;
    [LabelText ("Ip_Dev_Localhost"), SerializeField] string hostIp_dev_Localhost = "127.0.0.1";
    [LabelText ("Ip_Dev_ZhangSan"), SerializeField] string hostIp_dev_ZhangSan = "10.0.0.5";
    [LabelText ("Ip_Test_Inner"), SerializeField] string hostIp_test_inner = "10.0.0.6";
    [LabelText ("Ip_Test_Outer"), SerializeField] string hostIp_test_outer = "1.1.1.1";
    [LabelText ("Ip_Release"), ReadOnly, SerializeField] string hostIp_release = "1.1.1.1";
    [LabelText ("Ip_Review"), ReadOnly, SerializeField] string hostIp_review = "1.1.1.1";
    [LabelText ("Port_Android"), SerializeField] int _hostPort_android = 43000;
    [LabelText ("Port_iOS"), SerializeField] int _hostPort_ios = 48000;
    public int HostPort => _hostPort_android;
    public int HostPort_iOS => _hostPort_android;
    public string HostIp_dev_Localhost => hostIp_dev_Localhost;
    public string HostIp_dev_ZhanSan => hostIp_dev_ZhangSan;
    public string HostIp_test_inner => hostIp_test_inner;
    public string HostIp_test_outer => hostIp_test_outer;
    public string HostIp_release => hostIp_release;
    public string HostIp_review => hostIp_review;

    [LabelText ("发布渠道"), SerializeField] LoginChannelType _loginChannelType;
    [LabelText ("Web中心-App名"), SerializeField] string _AppName = "YourAppName";
    [LabelText ("Web中心-Platform名"), SerializeField] string _VersionName = "Android";
    [LabelText ("APP版本号"), SerializeField] string _AppVersion = "0.0.00";

    public LoginChannelType loginChannelType => _loginChannelType;
    public string AppName => _AppName;
    public string VersionName => _VersionName;

    public string AppVersion => _AppVersion;
    void Awake () {
        DontDestroyOnLoad (gameObject);
        _instance = this;
    }
}