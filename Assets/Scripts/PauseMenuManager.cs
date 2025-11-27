using UnityEngine;
using UnityEngine.SceneManagement;

public class Pausemenumanager : MonoBehaviour
{
public GameObject PauseMenuCanvas;
public GameObject settingscanvas;

 public void OnbackButtonİnPauseClicked()
    {
     PauseMenuCanvas.SetActive(false);
     settingscanvas.SetActive(false);
    }

 public void OnBackİnSettingsButtonClicked()
    {
     PauseMenuCanvas.SetActive(true);
     settingscanvas.SetActive(false);
    }

     public void OnPauseButtonClicked()
    {
     PauseMenuCanvas.SetActive(true);
     settingscanvas.SetActive(false);
    }

    public void OnsettingsButtoninpauseClicked()
    {
     PauseMenuCanvas.SetActive(false);
     settingscanvas.SetActive(true);
    }

    public void OnQuitİnPauseButtonClicked()
    {
     SceneManager.LoadScene("MainMenuScence");
    }



}
