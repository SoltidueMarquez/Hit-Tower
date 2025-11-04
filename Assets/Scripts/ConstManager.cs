/// <summary>
/// 这个游戏中所有的常量都在这里
/// </summary>
public static class ConstManager
{
    /// <summary>
    /// 网格放置系统的对齐常量
    /// </summary>
    public const int k_GridSize = 100;

    /// <summary>
    /// 倍速设置
    /// </summary>
    public static readonly int[] timeSettings = new[] { 1, 2, 3 };
    
    /// <summary>
    /// 开始界面场景名
    /// </summary>
    public const string k_StartSceneName = "Start";
    
    /// <summary>
    /// 游戏界面场景名
    /// </summary>
    public const string k_GameSceneName = "Sample";
    
    /// <summary>
    /// 建筑物范围计算周期
    /// </summary>
    public const float k_UpdateInterval = 0.2f;

    /// <summary>
    /// 帧数debug更新间隔
    /// </summary>
    public const float k_FPSUpdateInterval = 0.2f;
    
    /// <summary>
    /// 网格地块的Y坐标高度
    /// </summary>
    public const float k_PreviewYOffset = 0.06f;
}