using Buildings.Specific_Building.DOTTower.State;
using Utils.StateMachine;

namespace Buildings.Specific_Building.DOTTower
{
    public class DotTower : BuildingMono
    {
        protected override void InitStateMachine()
        {
            // 添加状态机组
            stateMachine = GetComponent<StateMachine>();
            
            stateMachine.ClearStates();
            // 注册状态，如果已经注册过了就什么都不会做
            stateMachine.RegisterState(new DotTowerIdleState());
            stateMachine.RegisterState(new DotTowerAttackState());
            
            // 设置初始状态
            stateMachine.SwitchTo<DotTowerIdleState>();
        }
    }
}