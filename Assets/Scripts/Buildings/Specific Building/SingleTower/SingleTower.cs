using Buildings.Specific_Building.SingleTower.State;
using Utils;
using Utils.StateMachine;

namespace Buildings.Specific_Building.SingleTower
{
    public class SingleTower : BuildingMono
    {
        public ValueChannel penetrateNum = new ValueChannel();
        public ValueChannel sputterRadius = new ValueChannel();

        protected override void InitStateMachine()
        {
            // 添加状态机组
            stateMachine = GetComponent<StateMachine>();

            stateMachine.ClearStates();
            // 注册状态，如果已经注册过了就什么都不会做
            stateMachine.RegisterState(new SingleTowerIdleMode());
            stateMachine.RegisterState(new SingleTowerAttackMode());

            // 设置初始状态
            stateMachine.SwitchTo<SingleTowerIdleMode>();
        }
    }
}