using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LineRender : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private Camera cam;
    private Collider2D col;
    public bool real = true;

    private void Awake()
    {
        cam = Camera.main;
        col = GetComponent<Collider2D>();

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
            lineRenderer.enabled = false;
            
    }

    public void setUpRenderer(float reach)
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        int segments = 100;
        if (lr != null)
        {
            lr.loop = true;
            lr.useWorldSpace = false;
            Vector3[] points = new Vector3[segments];
            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
        
                float x = Mathf.Cos(angle) * reach;
                float y = Mathf.Sin(angle) * reach;

                points[i] = new Vector3(x, y, 0);
            }

            lr.positionCount = segments;
            lr.SetPositions(points);
        }
        
        if (!real)
        {
            lineRenderer.enabled = true;
        }
    }

    void Update()
    {
        if (!real) return;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = cam.ScreenToWorldPoint(mousePos);

        if (col != null && lineRenderer != null)
        {
            lineRenderer.enabled = col.OverlapPoint(worldPos);
        }
    }
}
