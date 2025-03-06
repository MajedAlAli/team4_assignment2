using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TMP_Text HomeScore;
    public TMP_Text AwayScore;
    public TMP_Text TimerText;
    public TMP_Text GoalHome;
    public TMP_Text GoalAway;

    int homeCount = 0;
    int awayCount = 0;
    public float timer = 180f;
    public float activeTime = 2f;


    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GoalHome.gameObject.SetActive(false);
        GoalAway.gameObject.SetActive(false);
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
            FindObjectOfType<AudioManager>().Play("FinalWhistle");
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

    public void DisplayMessageHome()
    {
        StartCoroutine(Display(GoalHome));
    }

    public void DisplayMessageAway()
    {
        StartCoroutine(Display(GoalAway));
    }

    IEnumerator Display(TMP_Text text)
    {
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(activeTime);
        text.gameObject.SetActive(false);
    }
}
