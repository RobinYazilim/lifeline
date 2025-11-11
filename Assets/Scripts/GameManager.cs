using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    public GameObject[] enemyPrefabs;
    public GameObject[] turretPrefabs;

    void Start()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);

        EnemyManager.inst.spawnEnemy(5, 10, 10, 1, enemyPrefabs[0]);
        TurretManager.inst.spawnTurret(new Vector3(-0.7f, 1.23f, 0f), 5f, 0.3f, 2f, turretPrefabs[0]);
    }

    void Update()
    {
        
    }
}
