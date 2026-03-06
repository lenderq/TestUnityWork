using AxGrid;
using AxGrid.Base;
using AxGrid.FSM;
using UnityEngine;

public class SlotFsmBootstrapper : Binder
{
    [OnAwake]
    private void Init()
    {
        Settings.Fsm = new FSM();
        Settings.Fsm.Add(new IdleState(), new SpinningState(), new CanStopState(), new StoppingState());
        Log.Debug("FSM: states added");
    }

    [OnStart]
    private void StartFsm()
    {
        Settings.Fsm.Start("Idle");
        Log.Debug("FSM: started Idle");
    }

    [OnUpdate]
    private void UpdateFsm()
    {
        Settings.Fsm.Update(Time.deltaTime);
    }
}
