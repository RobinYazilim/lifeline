using UnityEngine;
using System.Collections;

public class Path : MonoBehaviour
{
    public Transform[] points;

    IEnumerator Start()
    {
        while (EnemyManager.inst == null)
            yield return null;
        EnemyManager.inst.SetPath(points);
    }
}
