using Enemy;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void Start()
    {
        EnemyManager.Instance.Init();
    }
}