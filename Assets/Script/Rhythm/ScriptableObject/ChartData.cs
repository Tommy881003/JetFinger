using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ChartData : ScriptableObject
{
    [SerializeField]
    private MusicData _musicData;
    public MusicData musicData { get { return _musicData; } }

    public List<TrackNoteData> _notes;
    public List<TrackNoteData> notes { get { return _notes; } }

    public void ClearAll()
    {
        _notes.Clear();
    }

    public void AddNote(TrackID ID, float beat)
    {
        TrackNoteData trackNoteData = new TrackNoteData { beatNumber = beat, id = ID };
        _notes.Add(trackNoteData);
    }

    public void AddConsecutiveNotes(TrackID ID, float start, float gap, int consecutive)
    {
        for(int i = 0; i < consecutive; i ++)
        {
            TrackNoteData trackNoteData = new TrackNoteData { beatNumber = start + i * gap, id = ID };
            _notes.Add(trackNoteData);
        }
    }

    public List<TrackNoteData>[] GetNotes()
    {
        int count = Enum.GetValues(typeof(TrackID)).Length;
        List<TrackNoteData>[] result = new List<TrackNoteData>[count];
        for (int i = 0; i < count; i++)
            result[i] = new List<TrackNoteData>();

        _notes.Sort(delegate (TrackNoteData c1, TrackNoteData c2) { return c1.beatNumber.CompareTo(c2.beatNumber); });

        foreach (TrackNoteData data in _notes)
            result[(int)data.id].Add(data);
        
        return result;
    }
}
