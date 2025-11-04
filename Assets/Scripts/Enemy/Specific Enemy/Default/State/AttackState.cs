using Utils.StateMachine;

namespace Enemy.State
{
    public class AttackState : IState
    {
        private StateMachine stateMachine { get; set; }
        private EnemyMono m_Mono;
        
        public void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void OnEnter()
        { 
            // 不用空检测，有requireComponent约束
            GameManager.Instance.
                    playerManager.playerLogic.ModifyCurrentHealth(-m_Mono.enemyLogic.EnemyInfo.attack.Value);
            m_Mono.enemyLogic.SetDie();
        }

        public void OnTick()
        {
        }

        public void OnExit()
        {
        }

        public void Init()
        {
            m_Mono = stateMachine.gameObject.GetComponent<EnemyMono>();
        }
    }
}