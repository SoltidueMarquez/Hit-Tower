using Enemy;
using UnityEngine;
using Utils;

public class GameManager : Singleton<GameManager>
{
    public EnemyManager m_EnemyManager;
    public void Start()
    {
        m_EnemyManager = GetComponentInChildren<EnemyManager>();
        if (m_EnemyManager != null) m_EnemyManager.Init();
    }
}