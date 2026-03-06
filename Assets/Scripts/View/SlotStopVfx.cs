using AxGrid;
using AxGrid.Base;
using UnityEngine;

public class SlotStopVfx : Binder
{
    [SerializeField] private ParticleSystem stopParticles;

    [OnAwake]
    private void AwakeThis()
    {
        if (stopParticles == null)
            stopParticles = GetComponent<ParticleSystem>();

        Log.Debug("SlotStopVfx Awake. Particles found: " + (stopParticles != null));
    }

    [OnStart]
    private void StartThis()
    {
        Log.Debug("SlotStopVfx Start. Model==Settings.Model ? " + ReferenceEquals(Model, Settings.Model));
        Model.EventManager.AddAction("OnSlotStopFxStart", PlayStopFx);
        Model.EventManager.AddAction("OnSlotStopFxEnd", StopStopFx);
    }

    [OnDestroy]
    private void DestroyThis()
    {
        Model.EventManager.RemoveAction("OnSlotStopFxStart", PlayStopFx);
        Model.EventManager.RemoveAction("OnSlotStopFxEnd", StopStopFx);
    }

    private void PlayStopFx()
    {
        Log.Debug("SlotStopVfx PlayStopFx called");

        if (stopParticles == null)
        {
            Log.Debug("SlotStopVfx: particle system is NULL");
            return;
        }

        stopParticles.Clear();
        stopParticles.Play();
        Log.Debug("SlotStopVfx: particle play triggered");
    }

    private void StopStopFx()
    {
        Log.Debug("SlotStopVfx StopStopFx called");

        if (stopParticles == null)
            return;

        stopParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
