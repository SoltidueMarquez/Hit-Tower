namespace Utils.StateMachine
{
    public interface IState
    {
        // 设置状态机引用（新增）
        void SetStateMachine(StateMachine stateMachine);

        // 进入状态时调用
        void OnEnter();
    
        // 每帧更新时调用
        void OnTick();
    
        // 退出状态时调用
        void OnExit();

        void Init();
    }
}