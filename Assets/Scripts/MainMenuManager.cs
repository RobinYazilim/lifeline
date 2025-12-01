using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


public class MainMenuManager : MonoBehaviour
{
    public GameObject MainMenuCanvas;
    public GameObject SettingsCanvas;
    public AudioSource music;

    public void onVolumeChange(float value)
    {
        if (music != null)
        {
            music.volume = value * 0.6f;
        }
    }
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    public void OnSettingsButtonClicked()
    {
        SettingsCanvas.SetActive(true);
        MainMenuCanvas.SetActive(false);
    }

    public void OnBackButtonClicked()
    {
        SettingsCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }
}