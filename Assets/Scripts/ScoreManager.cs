using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // ----- NOTE STAT INFO -----
    public int notesSpawned = 0;
    public int notesHit = 0;
    public int notesMissed = 0;
    public int currentStreak = 0;
    public int highestStreak = 0;
    public int missClicks = 0;

    public int multiplier = 1;
    public int currentScore = 0;

    public int deafaultPoints = 25;

    [SerializeField] TextMeshProUGUI streakText;
    [SerializeField] TextMeshProUGUI scoreText;

    public static ScoreManager Instance;

    private void Awake()
    {
        // Delete GameManager copies
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OnMissClick()
    {
        ++missClicks;
        currentStreak = 0;

        CalculateMuliplier();

        UpdateUI();
    }

    public void OnMissedNote()
    {
        ++notesMissed;
        currentStreak = 0;

        CalculateMuliplier();

        UpdateUI();
    }

    public void OnNoteHit()
    {
        ++notesHit;
        ++currentStreak;

        CheckHighestStreak();
        CalculateMuliplier();
        CalculateScore();

        UpdateUI();
    }

    public void OnNoteSpawned()
    {
        ++notesSpawned;
    }

    private void CalculateMuliplier()
    {
        if (currentStreak > 40)
        {
            multiplier = 4;
        } else if (currentStreak > 30)
        {
            multiplier = 3;
        } else if (currentStreak > 20)
        {
            multiplier = 2;
        } else
        {
            multiplier = 1;
        }
    }

    private void CheckHighestStreak()
    {
        if (currentStreak > highestStreak)
        {
            highestStreak = currentStreak;
        }
    }

    private void CalculateScore()
    {
        currentScore += deafaultPoints * multiplier;
    }

    private void UpdateUI()
    {
        SetScore();
        SetStreak();
    }

    private void SetScore()
    {
        scoreText.text = $"{currentScore}";
    }

    private void SetStreak()
    {
        streakText.text = $"x{multiplier}";
    }

    public void ResetStats()
    {
        notesSpawned = 0;
        notesHit = 0;
        notesMissed = 0;
        currentStreak = 0;
        highestStreak = 0;
        missClicks = 0;
        multiplier = 1;
        currentScore = 0;
        GameManager.Instance.selectedSongName = "";
    }
}
