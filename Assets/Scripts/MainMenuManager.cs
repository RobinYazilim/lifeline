using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuManager : MonoBehaviour
{
   public GameObject MainMenuCanvas;
    public GameObject SettingsCanvas;
    public void OnPlayButtonClicked()
    {
    SceneManager.LoadScene("SampleScene");
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