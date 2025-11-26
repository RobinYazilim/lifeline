using UnityEngine;
using System.Collections;

public class PathManager : MonoBehaviour
{
    public Transform[] points;
    public float radius = 0.75f;

    public static PathManager inst;

    void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        EnemyManager.inst.SetPath(points);
    }

    public bool isOnPath(Vector3 position)
    {   
        if (points == null || points.Length < 2)
            return false;

        float r2 = radius * radius;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 a = points[i].position;
            Vector3 b = points[i + 1].position;

            Vector3 ap = position - a;
            Vector3 ab = b - a;

            float abLengthSquared = ab.sqrMagnitude;
            if (abLengthSquared == 0)
                continue; // ne..
            float t = Vector3.Dot(ap, ab) / abLengthSquared;
            t = Mathf.Clamp01(t);

            Vector3 point = a + t * ab;

            float distSquared = (position - point).sqrMagnitude;
            if (distSquared <= r2)
            {
                return true;
            }

            // gereksiz mat yes
        }
        return false;
    }
}
