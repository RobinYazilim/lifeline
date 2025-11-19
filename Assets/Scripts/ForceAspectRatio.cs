using UnityEngine;

public class ForceAspectRatio : MonoBehaviour
{
    public float targetAspect = 16f / 9f;

    void Update()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scale = windowAspect / targetAspect;

        Camera cam = Camera.main;

        if (scale > 1f)
        {
            float width = 1f / scale;
            float x = (1f - width) / 2f;
            cam.rect = new Rect(x, 0f, width, 1f);
        }
        else
        {
            float height = scale;
            float y = (1f - height) / 2f;
            cam.rect = new Rect(0f, y, 1f, height);
        }
    }
}
