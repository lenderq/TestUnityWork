using System;
using AxGrid;
using AxGrid.FSM;

public abstract class SlotStateBase : IState
{
    public FSM Parent { get; set; }

    public void __EnterState() => Enter();
    public void __ExitState() => Exit();
    public void __UpdateState(float deltaTime) => Update(deltaTime);

    public void __Invoke(string eventName, object[] args)
    {
        if (eventName == "OnBtn")
        {
            var btn = (args != null && args.Length > 0) ? args[0] as string : null;
            OnBtn(btn);
            return;
        }

        if (eventName == "OnReelStopped")
        {
            OnReelStopped();
            return;
        }
    }

    protected virtual void OnReelStopped() { }


    public void __InvokeDelayAsync(float delay, string eventName, object[] args)
    {

    }

    public virtual void Dispose() { }

    protected virtual void Enter() { }
    protected virtual void Exit() { }
    protected virtual void Update(float dt) { }
    protected virtual void OnBtn(string buttonName) { }
}
