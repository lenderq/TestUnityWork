using System;
using AxGrid;
using AxGrid.Base;

public class ModelDebugProbe : Binder
{
    [OnStart]
    private void StartProbe()
    {
        Log.Debug("Probe: Model==Settings.Model ? " + ReferenceEquals(Model, Settings.Model));
        Log.Debug("Probe: BtnStartEnable now = " + Model.GetBool("BtnStartEnable", true));

        Model.EventManager.AddAction("OnBtnStartEnableChanged", OnStartEnableChanged);
        Model.EventManager.AddAction("OnBtnStopEnableChanged", OnStopEnableChanged);
    }

    private void OnStartEnableChanged()
    {
        Log.Debug("Probe event: StartEnable -> " + Model.GetBool("BtnStartEnable", true));
    }

    private void OnStopEnableChanged()
    {
        Log.Debug("Probe event: StopEnable -> " + Model.GetBool("BtnStopEnable", false));
    }

    [OnDestroy]
    private void DestroyProbe()
    {
        Model.EventManager.RemoveAction("OnBtnStartEnableChanged", OnStartEnableChanged);
        Model.EventManager.RemoveAction("OnBtnStopEnableChanged", OnStopEnableChanged);
    }
}
