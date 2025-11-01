using Enemy;
using Player;
using UnityEngine;
using Utils;

public class GameManager : Singleton<GameManager>
{
    public EnemyManager EnemyManager { get; private set; }
    public PlayerManager PlayerManager { get; private set; }
    public void Start()
    {
        PlayerManager = GetComponentInChildren<PlayerManager>();
        if (PlayerManager != null) PlayerManager.Init();
        PlayerManager.playerLogic.OnDie += GameOverLose;
        
        // enemy的移动状态依赖于player的building位置信息
        EnemyManager = GetComponentInChildren<EnemyManager>();
        if (EnemyManager != null) EnemyManager.Init();
    }
    
    private void GameOverLose()
    {
        Debug.Log("游戏结束，玩家死亡");
    }
    
    // TODO:所有的波次结束且场上没怪为胜利
}