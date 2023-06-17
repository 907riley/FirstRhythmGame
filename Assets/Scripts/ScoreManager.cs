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

    // displayed info
    public int multiplier = 1;
    public int currentScore = 0;

    // default point amount
    public int deafaultPoints = 25;

    // text boxes to display streak and score
    [SerializeField] TextMeshProUGUI streakText;
    [SerializeField] TextMeshProUGUI scoreText;

    // singleton
    public static ScoreManager Instance;

    private void Awake()
    {
        // Delete scoremanager copies
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Called when a fingerbutton is clicked and no note is there to play
    /// </summary>
    public void OnMissClick()
    {
        ++missClicks;
        currentStreak = 0;

        CalculateMuliplier();

        UpdateUI();
    }

    /// <summary>
    /// Called when a note falls off screen, never played
    /// </summary>
    public void OnMissedNote()
    {
        ++notesMissed;
        currentStreak = 0;

        CalculateMuliplier();

        UpdateUI();
    }

    /// <summary>
    /// Called when a note is succesfully played
    /// </summary>
    public void OnNoteHit()
    {
        ++notesHit;
        ++currentStreak;

        CheckHighestStreak();
        CalculateMuliplier();
        CalculateScore();

        UpdateUI();
    }

    /// <summary>
    /// Called when a note is spawned
    /// </summary>
    public void OnNoteSpawned()
    {
        ++notesSpawned;
    }

    /// <summary>
    /// Sets the current multiplier based on currentStreak
    /// </summary>
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

    /// <summary>
    /// Helper function for checking to see if the current streak is a new highest streak
    /// </summary>
    private void CheckHighestStreak()
    {
        if (currentStreak > highestStreak)
        {
            highestStreak = currentStreak;
        }
    }

    /// <summary>
    /// Helper function for updating the current score
    /// </summary>
    private void CalculateScore()
    {
        currentScore += deafaultPoints * multiplier;
    }

    /// <summary>
    /// Helper for updating all of the UI
    /// </summary>
    private void UpdateUI()
    {
        SetScore();
        SetStreak();
    }

    /// <summary>
    /// Set the text value for score
    /// </summary>
    private void SetScore()
    {
        scoreText.text = $"{currentScore}";
    }

    /// <summary>
    /// Set the text value for current multiplier
    /// </summary>
    private void SetStreak()
    {
        streakText.text = $"x{multiplier}";
    }

    /// <summary>
    /// Reset all the stats for the next song to play
    /// </summary>
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
    }
}
