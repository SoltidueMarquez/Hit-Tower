using Buildings.Specific_Building.AOETower.State;
using Utils.StateMachine;

namespace Buildings.Specific_Building.AOETower
{
    public class AoeTower : BuildingMono
    {
        protected override void InitStateMachine()
        {
            // 添加状态机组
            stateMachine = GetComponent<StateMachine>();

            stateMachine.ClearStates();
            // 注册状态，如果已经注册过了就什么都不会做
            stateMachine.RegisterState(new AoeTowerIdleState());
            stateMachine.RegisterState(new AoeTowerAttackState());
            
            // 设置初始状态
            stateMachine.SwitchTo<AoeTowerIdleState>();
        }
    }
}