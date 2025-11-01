namespace Enemy.Specific_Enemy
{
    public class NormalEnemy : EnemyMono
    {
        public override void Init(EnemyData enemyData, EnemyManager manager)
        {
            base.Init(enemyData, manager);
        }
        
        protected override void InitStateMachine()
        {
            base.InitStateMachine();
        }
    }
}