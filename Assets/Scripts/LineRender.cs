using UnityEngine;
using UnityEngine.InputSystem;

public class LineRender : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = cam.ScreenToWorldPoint(mousePos);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            lineRenderer.enabled = col.OverlapPoint(worldPos);
        }
    }
}
