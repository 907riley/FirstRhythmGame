using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int notesSpawned = 0;
    public int notesHit = 0;
    public int notesMissed = 0;
    public int currentStreak = 0;
    public int missClicks = 0;

    [SerializeField] TextMeshProUGUI streakText;

    public void OnMissClick()
    {
        ++missClicks;
        currentStreak = 0;
        SetStreak();
    }

    public void OnMissedNote()
    {
        ++notesMissed;
        currentStreak = 0;
        SetStreak();
    }

    public void OnNoteHit()
    {
        ++notesHit;
        ++currentStreak;
        SetStreak();
    }

    public void OnNoteSpawned()
    {
        ++notesSpawned;
    }

    private void SetStreak()
    {
        streakText.text = currentStreak.ToString();
    }
}
