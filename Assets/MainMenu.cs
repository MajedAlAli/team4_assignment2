using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject menuPanel; // Drag the MenuPanel here in Inspector
    public GameObject mainmenu;
    public GameObject howtoplaymenu;

    void Start()
    {
        Time.timeScale = 0f; // Pause the game until Play is pressed
        menuPanel.SetActive(true); // Show menu at start
        mainmenu.SetActive(true);
        howtoplaymenu.SetActive(false);
    }

    public void PlayGame()
    {
        menuPanel.SetActive(false); // Hide menu
        Time.timeScale = 1f; // Resume game
        FindAnyObjectByType<AudioManager>().Play("StadiumAmbience");
        FindAnyObjectByType<AudioManager>().Play("StartWhistle");
    }

    public void HowToPlay()
    {
        mainmenu.SetActive(false);
        howtoplaymenu.SetActive(true);
    }

    public void Back()
    {
        howtoplaymenu.SetActive(false);
        mainmenu.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit(); // Quits the game (only works in build)

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops the game in the editor
        #endif
    }

}
