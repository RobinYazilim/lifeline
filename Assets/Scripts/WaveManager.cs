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
