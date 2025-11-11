using UnityEngine;

public class Path : MonoBehaviour
{
    public Transform[] points;

    void Start()
    {
        EnemyManager.inst.SetPath(points);
    }
}
