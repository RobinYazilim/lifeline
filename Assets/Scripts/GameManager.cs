using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    public GameObject[] enemyPrefabs;
    public GameObject[] turretPrefabs;
    public GameObject[] homePrefabs;
    public GameObject[] projectilePrefabs;
    public int totalSingletons = 5; // EnemyManager, TurretManager, HomeManager, ProjectileManager, TurretMenu

    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

    private IEnumerator Start()
    {
        
        //TurretManager.inst.spawnTurret(new Vector3(-0.7f, 1.23f, 0f), 2f, 0.3f, 2f, TurretType.Basic, turretPrefabs[0]);
        //TurretManager.inst.spawnTurret(new Vector3(-0.7f, 1.23f, 0f), TurretType.Basic, turretPrefabs[0]);
        HomeManager.inst.spawnHome(new Vector3(-6f, -1.54f, 0f), 500f, 5f, 0.5f, homePrefabs[0]);

        for (int i = 1; i <= 7; i++)
        {
            //EnemyManager.inst.spawnEnemy(2, 10, 10, 1, EnemyType.Basic, enemyPrefabs[0]);
            EnemyManager.inst.spawnEnemy(EnemyType.Basic, enemyPrefabs[0]);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1f);
        for (int i = 1; i <= 3; i++)
        {
            //EnemyManager.inst.spawnEnemy(2, 10, 10, 1, EnemyType.Basic, enemyPrefabs[0]);
            EnemyManager.inst.spawnEnemy(EnemyType.Fast, enemyPrefabs[0]);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
