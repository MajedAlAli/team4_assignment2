using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TMP_Text HomeScore;
    public TMP_Text AwayScore;
    public TMP_Text TimerText;
    int homeCount = 0;
    int awayCount = 0;
    public float timer = 180f;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HomeScore.text = homeCount.ToString();
        AwayScore.text = awayCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
         if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            timer = 0;
            Debug.Log("Time's up!");
        }
    }

    public void AddPointHome()
    {
        homeCount++;
        HomeScore.text = homeCount.ToString();
    }

    public void AddPointAway()
    {
        awayCount++;
        AwayScore.text = awayCount.ToString();
    }

     void UpdateTimerUI()
    {
        if (TimerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            TimerText.text = string.Format("{0}:{1:00}", minutes, seconds);
        }
    }
}
