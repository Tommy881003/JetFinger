using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDisk : MonoBehaviour
{
    [SerializeField]
    private LineRenderer outline;
    [SerializeField]
    private GameObject disk;
    [SerializeField]
    private MeshRenderer mr;
    [SerializeField]
    private Easing easing;

    private float radius;
    public float hitBeat { get; private set; }

    public void SetupDisk(Material material, float beat, float r)
    {
        outline.material = material;
        mr.material = material;
        hitBeat = beat;
        radius = r;

        outline.positionCount = 31;
        for (int i = 0; i < 31; i++)
            outline.SetPosition(i, new Vector3(Mathf.Cos(i / 30f * Mathf.PI * 2), 0, Mathf.Sin(i / 30f * Mathf.PI * 2)) * radius);
    }

    public void SetDiskProgress(float progress)
    {
        float trueProgress = EaseLibrary.CallEaseFunction(easing, progress);
        disk.transform.localScale = new Vector3(2 * radius * trueProgress, disk.transform.localScale.y, 2 * radius * trueProgress);
    }
}
