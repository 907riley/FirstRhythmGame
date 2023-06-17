using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGameDisplay : MonoBehaviour
{
    // prefabs that get set on display
    [SerializeField] TextMeshProUGUI score;
    [SerializeField] TextMeshProUGUI hitPercentage;
    [SerializeField] TextMeshProUGUI notesHit;
    [SerializeField] TextMeshProUGUI highestStreak;
    [SerializeField] TextMeshProUGUI notesMissed;

    /// <summary>
    /// When about to be displayed get all the information
    /// </summary>
    void Start()
    {
        float hitPercent = ((float)ScoreManager.Instance.notesHit / (float)ScoreManager.Instance.notesSpawned) * 100;
        Debug.Log($"notesHit: {ScoreManager.Instance.notesHit} and notesSpawned: {ScoreManager.Instance.notesSpawned}");
        Debug.Log($"percentage: {hitPercent}");

        score.text = $"{ScoreManager.Instance.currentScore}";
        hitPercentage.text = $"{System.Math.Round(hitPercent, 2)}";
        notesHit.text = $"{ScoreManager.Instance.notesHit}";
        highestStreak.text = $"{ScoreManager.Instance.highestStreak}";
        notesMissed.text = $"{ScoreManager.Instance.notesMissed}";
    }

    /// <summary>
    /// Called when the exit button is clicked
    /// </summary>
    public void OnExitClick()
    {
        // Load the Menu scene
        StartCoroutine(Utils.LoadAsyncScene("Menu"));
        gameObject.SetActive(false);
        ScoreManager.Instance.ResetStats();
    }
}
