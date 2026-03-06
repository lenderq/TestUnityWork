using AxGrid;

[AxGrid.FSM.State("Idle")]
public class IdleState : SlotStateBase
{
    protected override void Enter()
    {
        Log.Debug("Enter Idle");
        Settings.Model.Set("BtnStartEnable", true);
        Settings.Model.Set("BtnStopEnable", false);
    }

    protected override void OnBtn(string buttonName)
    {
        Log.Debug($"Idle OnBtn: {buttonName}");
        if (buttonName == "Start")
            Parent.Change("Spinning", true);
    }
}

[AxGrid.FSM.State("Spinning")]
public class SpinningState : SlotStateBase
{
    private float t;

    protected override void Enter()
    {
        Log.Debug("Enter Spinning");
        t = 0f;

        Settings.Model.Set("BtnStartEnable", false);
        Settings.Model.Set("BtnStopEnable", false);

        Settings.Model.EventManager.Invoke("OnSlotSpinStart");
    }

    protected override void Update(float dt)
    {
        t += dt;
        if (t >= 3f)
        {
            Log.Debug("Spinning: 3 sec passed -> CanStop");
            Parent.Change("CanStop", true);
        }
    }

    protected override void OnBtn(string buttonName)
    {
        Log.Debug($"Spinning OnBtn: {buttonName} (ignored)");
    }
}

[AxGrid.FSM.State("CanStop")]
public class CanStopState : SlotStateBase
{
    protected override void Enter()
    {
        Log.Debug("Enter CanStop");
        Settings.Model.Set("BtnStartEnable", false);
        Settings.Model.Set("BtnStopEnable", true);
    }

    protected override void OnBtn(string buttonName)
    {
        Log.Debug($"CanStop OnBtn: {buttonName}");
        if (buttonName == "Stop")
            Parent.Change("Stopping", true);
    }
}

[AxGrid.FSM.State("Stopping")]
public class StoppingState : SlotStateBase
{
    protected override void Enter()
    {
        Log.Debug("Enter Stopping");

        Settings.Model.Set("BtnStartEnable", false);
        Settings.Model.Set("BtnStopEnable", false);

        Settings.Model.EventManager.Invoke("OnSlotSpinStop");
    }

    protected override void OnReelStopped()
    {
        Log.Debug("Stopping: reel stopped -> Idle");
        Parent.Change("Idle", true);
    }

    protected override void OnBtn(string buttonName)
    {
        Log.Debug($"Stopping OnBtn: {buttonName} (ignored)");
    }
}
