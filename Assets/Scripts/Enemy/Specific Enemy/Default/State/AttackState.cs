using Utils.StateMachine;

namespace Enemy.State
{
    public class AttackState : IState
    {
        private StateMachine stateMachine { get; set; }
        
        public void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            // TODO:测试用的，直接杀死自己
            var mono = stateMachine.gameObject.GetComponent<EnemyMono>();
            if (mono != null)
            {
                GameManager.Instance.
                    playerManager.playerLogic.ModifyCurrentHealth(-mono.EnemyLogicMono.EnemyInfo.attack.Value);
                mono.EnemyLogicMono.SetDie();
            }
        }

        public void OnTick()
        {
        }

        public void OnExit()
        {
        }
    }
}