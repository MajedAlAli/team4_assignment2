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
    public TMP_Text Timesup;

    public GameObject player;
    public GameObject homegoal;
    public GameObject awaygoal;
    private Collider homecollider;
    private Collider awaycollider;

    int homeCount = 0;
    int awayCount = 0;
    public float timer = 180f;
    public float activeTime = 2f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GoalHome.gameObject.SetActive(false);
        GoalAway.gameObject.SetActive(false);
        Timesup.gameObject.SetActive(false);
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

            if (Mathf.FloorToInt(timer) == 10)
            {
                FindAnyObjectByType<AudioManager>().Play("CountDown");
            }
            else if (Mathf.FloorToInt(timer) == 1)
            {
                FindAnyObjectByType<AudioManager>().Stop("CountDown");
            }
        }
        else if (timer <= 0 && timer != -1)
        {
            timer = -1;
            StopGame();
            TimerText.text = "0:00";
            FindAnyObjectByType<AudioManager>().Play("FinalWhistle");
            Timesup.gameObject.SetActive(true);
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

    public void StopGame()
    {
        PlayerControllerX movementScript = player.GetComponent<PlayerControllerX>();
        homecollider = homegoal.GetComponent<Collider>();
        awaycollider = awaygoal.GetComponent<Collider>();
        GoalHome.gameObject.SetActive(false);
        GoalAway.gameObject.SetActive(false);

        if (movementScript != null)
        {
            movementScript.gameIsActive = false;
            movementScript.DeactivatePowerUp();
            movementScript.speed = 0f;
        }

        if (homecollider != null)
        {
            homecollider.enabled = false;
        }

        if (awaycollider != null)
        {
            awaycollider.enabled = false;
        }
    }
}
