using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject menuPanel; // Drag the MenuPanel here in Inspector

    void Start()
    {
        Time.timeScale = 0f; // Pause the game until Play is pressed
        menuPanel.SetActive(true); // Show menu at start
    }

    public void PlayGame()
    {
        menuPanel.SetActive(false); // Hide menu
        Time.timeScale = 1f; // Resume game
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
