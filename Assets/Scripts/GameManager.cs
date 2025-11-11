using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    public GameObject[] enemyPrefabs;
    public GameObject[] turretPrefabs;
    public GameObject[] homePrefabs;

    IEnumerator Start()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
        while (EnemyManager.inst == null || TurretManager.inst == null)
            yield return null;

        TurretManager.inst.spawnTurret(new Vector3(-0.7f, 1.23f, 0f), 5f, 0.3f, 2f, turretPrefabs[0]);
        HomeManager.inst.spawnHome(new Vector3(-6f, -1.54f, 0f), 500f, 5f, 0.5f, homePrefabs[0]);

        for (int i = 1; i <= 3; i++)
        {
            EnemyManager.inst.spawnEnemy(5, 10, 10, 1, enemyPrefabs[0]);
            yield return new WaitForSeconds(0.3f);
        }
    }

    void Update()
    {
        
    }
}
