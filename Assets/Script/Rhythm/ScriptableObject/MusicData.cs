using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MusicData : ScriptableObject
{
    [SerializeField]
    private AudioClip _music;
    public AudioClip music { get { return _music; } }
    
    [SerializeField]
    private float _bpm;
    public float bpm { get { return _bpm; } }

    [SerializeField]
    private double _offset;
    public double offset { get { return _offset; } }
}
