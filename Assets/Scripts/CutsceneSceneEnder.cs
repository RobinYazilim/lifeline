using Unity.IO.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public class CutsceneSceneEnder : MonoBehaviour
{
    public VideoPlayer vid;
    public void OnCutsceneEnd()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
    private void Start()
    {
        if (vid != null)
        {
            vid.time = 0;
            vid.frame = 0;
            vid.Play();
        }
    }
}
