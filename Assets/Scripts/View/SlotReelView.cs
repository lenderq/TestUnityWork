using System.Collections.Generic;
using AxGrid;
using AxGrid.Base;
using UnityEngine;
using UnityEngine.UI;

public class SlotReelView : Binder
{
    [Header("Wiring")]
    [SerializeField] private RectTransform reelContainer;

    [Header("Items")]
    [SerializeField] private List<RectTransform> items = new List<RectTransform>();
    [SerializeField] private List<Image> itemImages = new List<Image>();
    [SerializeField] private Sprite[] icons;

    [Header("Layout")]
    [SerializeField] private float itemHeight = 100f;

    [Header("Spin")]
    [SerializeField] private float maxSpeed = 900f;
    [SerializeField] private float accel = 1800f;
    [SerializeField] private float decel = 2200f;
    [SerializeField] private float snapDuration = 0.25f;

    private enum Mode { Idle, Accelerating, Spinning, Decelerating, Snapping }
    private Mode mode = Mode.Idle;

    private float speed;
    private float snapTime;
    private float snapDeltaY;

    [OnAwake]
    private void AwakeThis()
    {
        if (reelContainer == null)
            reelContainer = (RectTransform)transform;

        if (items.Count == 0)
        {
            items.Clear();
            itemImages.Clear();

            for (int i = 0; i < reelContainer.childCount; i++)
            {
                var rt = reelContainer.GetChild(i) as RectTransform;
                if (rt == null) continue;

                var img = rt.GetComponent<Image>();
                if (img == null) continue;

                items.Add(rt);
                itemImages.Add(img);
            }
        }
    }

    [OnStart]
    private void StartThis()
    {
        Model.EventManager.AddAction("OnSlotSpinStart", OnSpinStart);
        Model.EventManager.AddAction("OnSlotSpinStop", OnSpinStop);
    }

    [OnDestroy]
    private void DestroyThis()
    {
        Model.EventManager.RemoveAction("OnSlotSpinStart", OnSpinStart);
        Model.EventManager.RemoveAction("OnSlotSpinStop", OnSpinStop);
    }

    private void OnSpinStart()
    {
        speed = 0f;
        mode = Mode.Accelerating;
    }

    private void OnSpinStop()
    {
        if (mode == Mode.Idle) return;

        Model.EventManager.Invoke("OnSlotStopFxStart");
        mode = Mode.Decelerating;
    }


    [OnUpdate]
    private void UpdateThis()
    {
        float dt = Time.deltaTime;

        if (mode == Mode.Accelerating)
        {
            speed = Mathf.MoveTowards(speed, maxSpeed, accel * dt);
            MoveItems(dt);
            if (Mathf.Approximately(speed, maxSpeed))
                mode = Mode.Spinning;
        }
        else if (mode == Mode.Spinning)
        {
            MoveItems(dt);
        }
        else if (mode == Mode.Decelerating)
        {
            speed = Mathf.MoveTowards(speed, 0f, decel * dt);
            MoveItems(dt);

            if (speed <= 60f)
                BeginSnap();
        }
        else if (mode == Mode.Snapping)
        {
            snapTime += dt;
            float t = Mathf.Clamp01(snapTime / snapDuration);
            float step = Mathf.Lerp(0f, snapDeltaY, t);

            ApplyGlobalDelta(step);
            snapDeltaY -= step;

            if (t >= 1f)
                FinishStop();
        }
    }

    private void MoveItems(float dt)
    {
        float dy = speed * dt;
        for (int i = 0; i < items.Count; i++)
        {
            var p = items[i].anchoredPosition;
            p.y -= dy;
            items[i].anchoredPosition = p;
        }

        RecycleIfNeeded();
    }

    private void RecycleIfNeeded()
    {
        float maxY = float.MinValue;
        for (int i = 0; i < items.Count; i++)
            if (items[i].anchoredPosition.y > maxY)
                maxY = items[i].anchoredPosition.y;

        float bottomLimit = -2f * itemHeight - itemHeight * 0.6f;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].anchoredPosition.y < bottomLimit)
            {
                var p = items[i].anchoredPosition;
                p.y = maxY + itemHeight;
                items[i].anchoredPosition = p;

                maxY = p.y;
                AssignRandomIcon(i);
            }
        }
    }

    private void AssignRandomIcon(int index)
    {
        if (icons == null || icons.Length == 0) return;
        itemImages[index].sprite = icons[Random.Range(0, icons.Length)];
        itemImages[index].preserveAspect = true;
    }

    private void BeginSnap()
    {
        mode = Mode.Snapping;
        snapTime = 0f;
        
        float bestAbs = float.MaxValue;
        float bestY = 0f;

        for (int i = 0; i < items.Count; i++)
        {
            float y = items[i].anchoredPosition.y;
            float a = Mathf.Abs(y);
            if (a < bestAbs)
            {
                bestAbs = a;
                bestY = y;
            }
        }

        snapDeltaY = -bestY;
    }

    private void ApplyGlobalDelta(float dy)
    {
        for (int i = 0; i < items.Count; i++)
        {
            var p = items[i].anchoredPosition;
            p.y += dy;
            items[i].anchoredPosition = p;
        }
    }

    private void FinishStop()
    {
        speed = 0f;
        mode = Mode.Idle;

        Model.EventManager.Invoke("OnSlotStopFxEnd");
        Settings.Fsm?.Invoke("OnReelStopped");
    }

}
