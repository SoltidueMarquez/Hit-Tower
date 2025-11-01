using Enemy;
using Player;
using UI_Framework.Scripts;
using UI_Framework.Scripts.Tools;
using UI_Framework.UI;
using UI_Framework.UI.UIGameSettings;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Utils.Singleton<GameManager>
{
    public EnemyManager EnemyManager { get; private set; }
    public PlayerManager PlayerManager { get; private set; }
    public void Start()
    {
        InitTimeScale();
        
        PlayerManager = GetComponentInChildren<PlayerManager>();
        if (PlayerManager != null) PlayerManager.Init();
        PlayerManager.playerLogic.OnDie += GameOverLose;
        
        // enemy的移动状态依赖于player的building位置信息
        EnemyManager = GetComponentInChildren<EnemyManager>();
        if (EnemyManager != null) EnemyManager.Init();
        EnemyManager.OnEnemyClear += CheckGameWin;

        #region UI最后再创建
        UIMgr.Instance.CreateUI<PlayerInfoUI>();
        
        // 最上层的是设置UI
        UIMgr.Instance.CreateUI<GameSettingsUI>();
        #endregion
    }

    #region 胜负判定
    private void GameOverLose()
    {
        Debug.Log("游戏结束，玩家死亡");
    }

    private void CheckGameWin()
    {
        if (EnemyManager.enemySpawner.IsSpawnOver()) GameOverWine();
    }
    private void GameOverWine()
    {
        Debug.Log("游戏胜利");
    }
    #endregion

    #region 倍速设置
    private int m_CurrentSpeedIndex; // 记录当前速度在TimeSettings中的索引

    private void InitTimeScale()
    {
        Time.timeScale = 1;
        m_CurrentSpeedIndex = 0;
    }
    
    public void SwitchGameSpeed()
    {
        // 循环递增索引，到达末尾则回到0
        m_CurrentSpeedIndex = (m_CurrentSpeedIndex + 1) % ConstManager.TimeSettings.Length;
    
        // 设置新的时间缩放
        Time.timeScale = ConstManager.TimeSettings[m_CurrentSpeedIndex];
    
        // 保持物理模拟的一致性
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void Pause()
    {
        // 设置新的时间缩放
        Time.timeScale = 0f;
    
        // 保持物理模拟的一致性
        Time.fixedDeltaTime = 0f;
    }

    public void Continue()
    {
        // 设置时间缩放
        Time.timeScale = ConstManager.TimeSettings[m_CurrentSpeedIndex];
    
        // 保持物理模拟的一致性
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void Restart()
    {
        SceneChangeHelper.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        // TODO:等待实现
        Debug.Log("主界面还没搭");
    }
    #endregion
}