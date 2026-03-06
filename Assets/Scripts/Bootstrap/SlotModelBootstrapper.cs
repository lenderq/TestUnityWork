using UnityEngine;
using AxGrid.Base;

public class SlotModelBootstrapper : Binder
{
    [OnAwake]
    void Init()
    {
        Model.Set("BtnStartEnable", true);
        Model.Set("BtnStopEnable", false);
    }
}
