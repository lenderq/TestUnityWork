using AxGrid;
using AxGrid.Base;

public class SlotRootBootstrapper : Binder
{
    [OnAwake]
    private void AwakeRoot()
    {
        Settings.Model = Model;
        Log.Debug("Root: Settings.Model aligned with Binder.Model");
    }
}
