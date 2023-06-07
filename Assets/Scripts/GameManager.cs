using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int notesSpawned = 0;
    public int notesHit = 0;
    public int notesMissed = 0;
    public int currentStreak = 0;
    public int missClicks = 0;

    public void OnMissClick()
    {
        ++missClicks;
        currentStreak = 0;
    }

    public void OnMissedNote()
    {
        ++notesMissed;
        currentStreak = 0;
    }

    public void OnNoteHit()
    {
        ++notesHit;
        ++currentStreak;
    }

    public void OnNoteSpawned()
    {
        ++notesSpawned;
    }
}
