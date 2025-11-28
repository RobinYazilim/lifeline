using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager inst;
    public GameObject hitEffectPrefab;
    public GameObject stunEffectPrefab;
    public GameObject debuffEffectPrefab;
    public GameObject bombEffectPrefab;
    void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

    public void spawnHitEffect(GameObject parent) => spawnAndDestroy(hitEffectPrefab, parent);
    public void spawnStunEffect(GameObject parent) => spawnAndDestroy(stunEffectPrefab, parent);
    public void spawnDebuffEffect(GameObject parent) => spawnAndDestroy(debuffEffectPrefab, parent);
    public void spawnBombEffect(GameObject parent) => spawnAndDestroy(bombEffectPrefab, parent);

    private void spawnAndDestroy(GameObject prefab, GameObject parent)
    {
        GameObject effect = Instantiate(prefab, parent.transform);

        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            Destroy(effect, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
            Destroy(effect, 3f);
    }
}
