namespace Buildings
{
    public class BuildingViewModelHelper
    {
        //- 引入MVVC与函数式编程思想优化UI与表现部分（借鉴MVVM的ModelView对View的单向绑定与抽象的函数值编程），增强UI的扩展性，部分UI更新采用数据向下流通消息向上流通的机制。例如：
        // - 设计 NodeSelectHelper（ViewModel层）管理节点选择状态，通过数据绑定自动触发UI刷新。
        // - 新增功能（如发送按钮验证）仅需扩展与切片订阅数据字段（如validityData），无需修改底层逻辑，显著降低耦合度。
        // - buff、节点等UI表现均采用此思想。
        
        public int currenrSelectedBuildingIndex;
    }
}