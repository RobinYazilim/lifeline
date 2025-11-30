using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager inst;
    public GameObject hitEffectPrefab;
    public GameObject stunEffectPrefab;
    public GameObject debuffEffectPrefab;
    public GameObject bombEffectPrefab;
    public GameObject buffEffectPrefab;
    void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

    public void spawnHitEffect(GameObject parent) => spawnAndDestroy(hitEffectPrefab, parent);
    public void spawnStunEffect(GameObject parent) => spawnAndDestroy(stunEffectPrefab, parent);
    public void spawnDebuffEffect(GameObject parent) => spawnAndDestroy(debuffEffectPrefab, parent);
    public void spawnBombEffect(GameObject parent) => spawnAndDestroy(bombEffectPrefab, parent);
    public void spawnUnscaledExplosionEffect(Vector3 position)
    {
        GameObject effect = Instantiate(bombEffectPrefab, gameObject.transform);
        effect.transform.position = position;

        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.useUnscaledTime = true;
        
        if (ps != null)
        {
            ps.Play();
            Destroy(effect, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
            Destroy(effect, 3f);
    }
    public void spawnBuffEffect(GameObject parent)
    {
        bool hasEffect = parent.transform.Find(buffEffectPrefab.name + "(Clone)") != null;
        if (hasEffect) return;
        GameObject effect = Instantiate(buffEffectPrefab, parent.transform);

        ParticleSystem ps = effect.GetComponent<ParticleSystem>();

        if (ps != null)
        {
            ps.Play();
        }
    }
    public void removeBuffEffect(GameObject parent)
    {
        Transform effectTransform = parent.transform.Find(buffEffectPrefab.name + "(Clone)");
        if (effectTransform != null)
        {
            Destroy(effectTransform.gameObject);
        }
    }

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
