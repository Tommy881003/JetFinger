using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Track : MonoBehaviour
{
    [SerializeField]
    private TrackID id;

    [Space(20), SerializeField]
    private GameObject hitDisk;
    [SerializeField]
    private Material trackMaterial;
    [Space(20), SerializeField]
    private Renderer highLightRenderer;
    [SerializeField]
    private Color highLightColor;
    [SerializeField]
    private float highLightTime;
    [Space(20), SerializeField]
    private GameObject hitParticle;
    [SerializeField]
    private GameObject missParticle;
    [Space(20), SerializeField]
    private KeyCode key;
    [SerializeField]
    private TextMeshProUGUI keyText;

    [Space(20), SerializeField]
    private float radius;
    [SerializeField]
    private float trackLength;
    [SerializeField]
    private LineRenderer leftLine;
    [SerializeField]
    private LineRenderer rightLine;
    [SerializeField]
    private LineRenderer circleLr;

    private static float trackSpeed = 5f;

    private MusicManager manager;
    [SerializeField]
    private AudioPlayer audioPlayer;

    private List<TrackNoteData> notes = new List<TrackNoteData>();
    private int nextNoteIndex;
    private Queue<HitDisk> disks = new Queue<HitDisk>();
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        highLightRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", Color.clear);
        highLightRenderer.SetPropertyBlock(propertyBlock);
    }

    private void Start()
    {
        manager = MusicManager.instance;

        notes = manager.notes[(int)id];
        nextNoteIndex = 0;     
        
        if (manager.useKeyboard)
            keyText.text = "[" + key.ToString() + "]";
        else
            keyText.text = "";
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        foreach(HitDisk disk in disks)
        {
            double delta = (disk.hitBeat - manager.songBeat) * manager.crotchet * trackSpeed;
            disk.gameObject.transform.localPosition = new Vector3(0, 0, (float)delta);
            disk.SetDiskProgress(1 - ((float)delta / trackLength));
        }

        if (manager.autoPlay)
            AutoPlay();
        else 
            ManualPlay();
    }

    private void AutoPlay()
    {
        if (disks.Count > 0)
        {
            HitDisk closestDisk = disks.Peek();
            while (manager.songBeat >= closestDisk.hitBeat)
            {
                CallFlash();
                audioPlayer.PlaySFX();
                disks.Dequeue();
                DestroyDisk(closestDisk.gameObject, true);
                if (disks.Count == 0)
                    break;
                closestDisk = disks.Peek();
            }
        }
    }

    private void ManualPlay()
    {
        if (manager.useKeyboard && Input.GetKeyDown(key))
            ManualCheckHit();
            
        if (disks.Count == 0)
            return;

        HitDisk closestDisk = disks.Peek();

        while (manager.CheckOverRange(closestDisk.hitBeat))
        {
            disks.Dequeue();
            DestroyDisk(closestDisk.gameObject, false);
            if (disks.Count == 0)
                break;
            closestDisk = disks.Peek();
        }
    }

    private void ManualCheckHit()
    {
        CallFlash();

        HitDisk closestDisk;
        audioPlayer.PlaySFX();
        if (disks.Count > 0)
        {
            closestDisk = disks.Peek();
            if (!manager.CheckToIgnore(closestDisk.hitBeat))
            {
                disks.Dequeue();
                DestroyDisk(closestDisk.gameObject, manager.CheckInAcceptRange(closestDisk.hitBeat));
            }

            if (disks.Count == 0)
                return;

            closestDisk = disks.Peek();

            while (manager.CheckOverRange(closestDisk.hitBeat))
            {
                disks.Dequeue();
                DestroyDisk(closestDisk.gameObject, false);
                if (disks.Count == 0)
                    break;
                closestDisk = disks.Peek();
            }
        }
    }

    private void DestroyDisk(GameObject disk ,bool hit)
    {
        if (hit)
            Instantiate(hitParticle, transform.position, Quaternion.Euler(-90, 0, 0));
        else
            Instantiate(missParticle, disk.transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(disk);
        manager.UpdateScoreData(hit);
    }

    private void LateUpdate()
    {
        if (nextNoteIndex >= notes.Count)
            return;

        double delta = (notes[nextNoteIndex].beatNumber - manager.songBeat) * manager.crotchet * trackSpeed;

        while(delta < trackLength && nextNoteIndex < notes.Count)
        {
            HitDisk newDisk = Instantiate(hitDisk, transform).GetComponent<HitDisk>();
            newDisk.transform.localPosition = new Vector3(0, 0, trackLength);
            newDisk.SetupDisk(trackMaterial, notes[nextNoteIndex].beatNumber, radius);
            disks.Enqueue(newDisk);

            nextNoteIndex++;
            if (nextNoteIndex >= notes.Count)
                break;
            delta = (notes[nextNoteIndex].beatNumber - manager.songBeat) * manager.crotchet * trackSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!manager.autoPlay && !manager.useKeyboard)
            ManualCheckHit();
    }

    private void CallFlash()
    {
        StopAllCoroutines();
        highLightRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", Color.clear);
        highLightRenderer.SetPropertyBlock(propertyBlock);
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        float timer = 0;
        while(timer < highLightTime)
        {
            highLightRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", new Color(highLightColor.r, highLightColor.g, highLightColor.b, 1 - (timer / highLightTime)));
            highLightRenderer.SetPropertyBlock(propertyBlock);
            timer += Time.deltaTime;
            yield return null;
        }

        highLightRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", Color.clear);
        highLightRenderer.SetPropertyBlock(propertyBlock);
    }

    /*#if UNITY_EDITOR

        // Update is called once per frame
        void Update()
        {
            if (leftLine != null)
            {
                leftLine.SetPosition(0, new Vector3(-radius, 0, 0));
                leftLine.SetPosition(1, new Vector3(-radius, 0, trackLength));
            }

            if (rightLine != null)
            {
                rightLine.SetPosition(0, new Vector3(radius, 0, 0));
                rightLine.SetPosition(1, new Vector3(radius, 0, trackLength));
            }
            if (circleLr != null)
            {
                circleLr.positionCount = 31;
                for (int i = 0; i < 31; i++)
                    circleLr.SetPosition(i, new Vector3(Mathf.Cos(i / 30f * Mathf.PI * 2), 0, Mathf.Sin(i / 30f * Mathf.PI * 2)) * radius);
            }
        }

    #endif*/
}
