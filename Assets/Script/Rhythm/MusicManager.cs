using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance { get; private set; } = null;

    [SerializeField]
    private bool _autoPlay;
    public bool autoPlay { get { return _autoPlay; } }
    [SerializeField]
    private bool _useKeyboard;
    public bool useKeyboard { get { return _useKeyboard; } }

    [SerializeField]
    private float _ignoreThreshold;
    public float ignoreThreshold { get { return _ignoreThreshold; } }
    [SerializeField]
    private float _acceptRange;
    public float acceptRange { get { return _acceptRange; } }
    
    [Space(20),SerializeField]
    private ChartData _chart;

    [SerializeField]
    private AudioSource _source;
    private AudioClip _music;       //音樂
    private double _startTime;
    private float _musicLength;     //音樂時長
    private float _bpm;             //BPM
    private double _offset;         //曲子開始的時間差
    public double crotchet { get; private set; }
    private double _songPosition;   //歌曲的位置
    public double songBeat { get { return _songPosition / crotchet; } }     //歌曲現在是第幾拍

    public List<TrackNoteData>[] notes { get; private set; }

    private int _combo = 0;
    public int combo { get { return _combo; } }
    private int _highestCombo = 0;
    public int highestCombo { get { return _highestCombo; } }
    private int _hitCount = 0;
    public int hitCount { get { return _hitCount; } }
    private int _missCount = 0;
    public int missCount { get { return _missCount; } }

    public UnityEvent OnSongStart = new UnityEvent();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);

        ResetSongData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetSongData();
            _source.Play();
            _startTime = AudioSettings.dspTime;
        }  
    }

    public void ResetSongData()
    {
        _source.Stop();

        _music = _chart.musicData.music;
        _musicLength = _music.length;
        _source.clip = _music;
        _bpm = _chart.musicData.bpm;
        crotchet = (double)60 / _bpm;
        _offset = _chart.musicData.offset;
        _songPosition = 0;

        _combo = 0;
        _highestCombo = 0;
        _hitCount = 0;
        _missCount = 0;

        notes = _chart.GetNotes();

        OnSongStart.Invoke();
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        _songPosition = (AudioSettings.dspTime - _startTime) - _offset;
    }

    public bool CheckToIgnore(float beatToCheck)
    {
        double beatTime = beatToCheck * crotchet;
        return (beatTime - _songPosition >= _ignoreThreshold);
    }

    public bool CheckInAcceptRange(float beatToCheck)
    {
        double beatTime = beatToCheck * crotchet;
        return (Mathf.Abs((float)beatTime - (float)_songPosition) < _acceptRange);
    }

    public bool CheckOverRange(float beatToCheck)
    {
        double beatTime = beatToCheck * crotchet;
        return (_songPosition - beatTime >= _acceptRange);
    }

    public void UpdateScoreData(bool hitBeat)
    {
        _hitCount += (hitBeat) ? 1 : 0;
        _missCount += (hitBeat) ? 0 : 1;
        _combo = (hitBeat) ? _combo + 1 : 0;
        _highestCombo = (_combo > _highestCombo) ? _combo : _highestCombo;
    }
}
