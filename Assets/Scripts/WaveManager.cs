using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager inst;
    public int currentWave = 0;
    public bool spawningFinished = false;
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
    }

    public void spawnNextWave()
    {
        currentWave += 1;
        StartCoroutine(waveFunction());
    }
}
