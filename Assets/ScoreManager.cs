using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    // Singleton instance of the ScoreManager
    public static ScoreManager instance;

    // UI elements for displaying scores, timer, and messages
    public TMP_Text HomeScore;
    public TMP_Text AwayScore;
    public TMP_Text TimerText;
    public TMP_Text GoalHome;
    public TMP_Text GoalAway;
    public TMP_Text Timesup;

    // References to game objects involved in the scoring system
    public GameObject player;
    public GameObject homegoal;
    public GameObject awaygoal;
    private Collider homecollider;
    private Collider awaycollider;

    // Score counters
    int homeCount = 0;
    int awayCount = 0;
    
    // Timer variables
    public float timer = 180f; // Game starts with 180 seconds (3 minutes)
    public float activeTime = 2f; // Duration for goal message display

    private void Awake()
    {
        // Ensures only one instance of ScoreManager exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroys duplicate ScoreManager instances
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Hide goal messages and times-up text initially
        GoalHome.gameObject.SetActive(false);
        GoalAway.gameObject.SetActive(false);
        Timesup.gameObject.SetActive(false);
        
        // Initialize the score display
        HomeScore.text = homeCount.ToString();
        AwayScore.text = awayCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            // Decrease timer by the time elapsed since the last frame
            timer -= Time.deltaTime;
            UpdateTimerUI(); // Update the timer display

            // Play countdown sound when 10 seconds remain
            if (Mathf.FloorToInt(timer) == 10)
            {
                FindAnyObjectByType<AudioManager>().Play("CountDown");
            }
            // Stop countdown sound at 1 second remaining
            else if (Mathf.FloorToInt(timer) == 1)
            {
                FindAnyObjectByType<AudioManager>().Stop("CountDown");
            }
        }
        else if (timer <= 0 && timer != -1) // Ensures the game stops only once
        {
            timer = -1; // Prevents multiple calls to StopGame()
            StopGame(); // Stops the game logic
            TimerText.text = "0:00"; // Ensures timer reads 0:00
            FindAnyObjectByType<AudioManager>().Play("FinalWhistle"); // Plays final whistle sound
            Timesup.gameObject.SetActive(true); // Show time's up message
        }
    }

    // Increases the home team's score and updates UI
    public void AddPointHome()
    {
        homeCount++;
        HomeScore.text = homeCount.ToString();
    }

    // Increases the away team's score and updates UI
    public void AddPointAway()
    {
        awayCount++;
        AwayScore.text = awayCount.ToString();
    }

    // Updates the timer UI with minutes and seconds format
    void UpdateTimerUI()
    {
        if (TimerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            TimerText.text = string.Format("{0}:{1:00}", minutes, seconds);
        }
    }

    // Displays the goal message for the home team temporarily
    public void DisplayMessageHome()
    {
        StartCoroutine(Display(GoalHome));
    }

    // Displays the goal message for the away team temporarily
    public void DisplayMessageAway()
    {
        StartCoroutine(Display(GoalAway));
    }

    // Coroutine to display a UI message for a limited time
    IEnumerator Display(TMP_Text text)
    {
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(activeTime);
        text.gameObject.SetActive(false);
    }

    // Stops the game and disables movement and goal colliders
    public void StopGame()
    {
        // Gets the player's movement script
        PlayerControllerX movementScript = player.GetComponent<PlayerControllerX>();
        
        // Gets goal colliders
        homecollider = homegoal.GetComponent<Collider>();
        awaycollider = awaygoal.GetComponent<Collider>();
        
        // Hides goal messages
        GoalHome.gameObject.SetActive(false);
        GoalAway.gameObject.SetActive(false);
        
        // Disables player movement
        if (movementScript != null)
        {
            movementScript.gameIsActive = false;
            movementScript.DeactivatePowerUp(); // Ensures power-ups are disabled
            movementScript.speed = 0f;
        }
        
        // Disables goal colliders to prevent further scoring
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
