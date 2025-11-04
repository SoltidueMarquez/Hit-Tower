using Utils.StateMachine;

namespace Buildings.Specific_Building.DOTTower
{
    public class DotTower : BuildingMono
    {
        protected override void InitStateMachine()
        {
            // 添加状态机组
            stateMachine = GetComponent<StateMachine>();
        }
    }
}