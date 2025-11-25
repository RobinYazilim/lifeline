using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager inst;
    public int currentWave = 0;
    public bool spawningFinished = false;
    private Coroutine waveCoroutine;
    // menuden editlenebiliyo artik yippieeee

    [System.Serializable]
    public class SpawnData
    {
        public EnemyType type;
        public int count;
        public float delay;
        public float waitAfter;
    }
    [System.Serializable]
    public class Wave
    {
        public List<SpawnData> spawns = new List<SpawnData>();
    }
    public List<Wave> waves = new List<Wave>();

    // wave ile degisecek diger boyle seyleri de buraya yazak
    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

    IEnumerator waveFunction()
    {
        spawningFinished = false;
        switch (currentWave)
        {
            case 1:
                for (int i = 1; i <= 7; i++)
                {
                    EnemyManager.inst.spawnEnemy(EnemyType.Basic);
                    yield return new WaitForSeconds(1f);
                }
                yield return new WaitForSeconds(1f);
                for (int i = 1; i <= 3; i++)
                {
                    EnemyManager.inst.spawnEnemy(EnemyType.Basic);
                    yield return new WaitForSeconds(1f);
                }
                break;
            case 2:
                for (int i = 1; i <= 2; i++)
                {
                    EnemyManager.inst.spawnEnemy(EnemyType.Fast);
                    yield return new WaitForSeconds(0.3f);
                }
                for (int i = 1; i <= 1; i++)
                {
                    EnemyManager.inst.spawnEnemy(EnemyType.Tank);
                    yield return new WaitForSeconds(0.3f);
                }
                break;
            case 3:
                for (int i = 1; i <= 2; i++)
                {
                    EnemyManager.inst.spawnEnemy(EnemyType.Fast);
                    yield return new WaitForSeconds(0.3f);
                }
                for (int i = 1; i <= 1; i++)
                {
                    EnemyManager.inst.spawnEnemy(EnemyType.Tank);
                    yield return new WaitForSeconds(0.3f);
                }
                break;
            default:
                break;
        }
        spawningFinished = true;
        yield return new WaitForSeconds(20f);
        spawnNextWave();
    }

    public void spawnNextWave()
    {
        currentWave += 1;
        StartCoroutine(waveFunction());
    }

    private IEnumerator waveLoop()
    {
        while (currentWave < waves.Count)
        {
            spawningFinished = false;
            currentWave += 1;

            Wave wave = waves[currentWave - 1];
            foreach (var spawn in wave.spawns)
            {
                for (int i = 0; i < spawn.count; i++)
                {
                    EnemyManager.inst.spawnEnemy(spawn.type);
                    yield return new WaitForSeconds(spawn.delay);
                }
                yield return new WaitForSeconds(spawn.waitAfter);
            }

            // burda wave finished wallahi
            
            spawningFinished = true;
            
            yield return new WaitForSeconds(20f);

        }
        waveCoroutine = null;
    }

    public void startWaves()
    {
        if (waveCoroutine == null)
            waveCoroutine = StartCoroutine(waveLoop());
    }

}
