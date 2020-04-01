using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffController : MonoBehaviour
{
    public static readonly int UP_NOTE = 0;
    public static readonly int LEFT_NOTE = 1;
    public static readonly int RIGHT_NOTE = 2;
    public static readonly int DOWN_NOTE = 3;

    public static readonly float[] NOTE_HEIGHTS = { 6.05f, 2.79f, -0.38f, -3.74f };

    [System.Serializable]
    public struct Song
    {
        public AudioClip song;
        public Note[] notes;
    };

    [System.Serializable]
    public struct Note
    {
        public int note;
        public float xDiff;
    };

    public Song[] songs;

    public float startX = 10;

    public GameObject notePrefab;

    public AudioSource source;

    private float yScaleFactor;

    private float xScaleFactor;

    private List<int> currentPlay;

    // Start is called before the first frame update
    void Start()
    {
        currentPlay = new List<int>();

        source = GetComponent<AudioSource>();
        yScaleFactor = 1.0f / transform.localScale.y;
        xScaleFactor = 1.0f / transform.localScale.x;

        Vector3 spawnPos = new Vector3(startX, 0, 0);
        int i = 0;
        foreach (Note note in songs[0].notes)
        {
            spawnPos.y = NOTE_HEIGHTS[note.note];

            GameObject temp = Instantiate(notePrefab, transform);
            temp.transform.localPosition = spawnPos;
            Vector3 scale = temp.transform.localScale;
            scale.x *= xScaleFactor;
            scale.y *= yScaleFactor;
            temp.transform.localScale = scale;
            temp.GetComponent<NoteController>().SetDir(note.note);
            temp.name += " " + i;

            spawnPos.x += note.xDiff;
            i++;
        }

        source.clip = songs[0].song;
    }

    public void StartSong()
    {
        source.Play();
    }

    public bool ProcInput(int d, int note)
    {
        currentPlay.Add(d);
        RhythmController temp = FindObjectOfType<RhythmController>();

        int ind = currentPlay.ToArray().Length - 1;
        if (songs[0].notes.Length > ind)
        {
            if (currentPlay[ind] != songs[0].notes[ind].note || ind != note)
            {
                temp.Miss();
                return false;
            }
            else
            {
                if (currentPlay.ToArray().Length == songs[0].notes.Length)
                {
                    temp.StartCoroutine(temp.Win());
                }
                return true;
            }
        }
        else
        {
            temp.StartCoroutine(temp.Miss());
            return false;
        }
    }

    public void FixPlay(int ind)
    {
        currentPlay = new List<int>();
        for (int i = 0; i <= ind; i++)
        {
            currentPlay.Add(songs[0].notes[i].note);
        }
    }
}
