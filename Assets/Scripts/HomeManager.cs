using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum HomeState
{
    Idle,
    Attacking
}
public class Home : ITarget
{
    public bool exists() => physical != null;
    public Vector3 getPosition() => physical.transform.position;
    public float health = 10f;
    public float damage = 10f;
    public float attackCooldown = 1f;
    public GameObject physical;
    public float t = 0f;
    public Enemy target;
    public bool dead = false;

    public HomeState state = HomeState.Idle;

    public Home(float health, float damage, float attackCooldown, GameObject physical)
    {
        this.health = health;
        this.damage = damage;
        this.physical = physical;
        this.attackCooldown = attackCooldown;
        HomeManager.inst.mainHomeHealth = this.health;
        HomeManager.inst.mainMaxHomeHealth = this.health;
        ShopManager.inst.HomeHealth(this.health);
    }

    public bool takeDamage(Enemy damager, float dmgTaken)
    {
        
        if (this.target == null)
        {
            this.target = damager;
            this.state = HomeState.Attacking;
        }
        this.health -= dmgTaken;
        ShopManager.inst.HomeHealth(this.health);
        HomeManager.inst.mainHomeHealth = this.health;
        
        if (this.health <= 0)
        {
            Debug.Log("im fr dead");
            this.health = 0;
            return true;
        }
        return false;
    }
}


public class HomeManager : MonoBehaviour
{
    public static HomeManager inst;
    public float mainHomeHealth = 100f;
    public float mainMaxHomeHealth = 100f;
    public Home home;
    public Volume volume;
    void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

    void Start()
    {
    }

    private IEnumerator lerpTimeToZero()
    {
        float duration = 3f;
        float start = Time.timeScale;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;

            float n = Mathf.Clamp01(t / duration);

            float eased = 1f - Mathf.Pow(1f - n, 3);
            Time.timeScale = Mathf.Lerp(start, 0f, eased);

            yield return null;
        }

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.5f);
        Debug.Log("waa");
        StartCoroutine(explodeAll());
    }

    public IEnumerator explodeAll()
    {
        // foreach (Projectile p in ProjectileManager.inst.projectiles)
        // {
        //     ProjectileManager.inst.projectilesToRemove.Add(p);
        // }

        List<object> targets = new List<object>();
        
        foreach (Turret t in TurretManager.inst.turrets)
            targets.Add(t);

        foreach (Enemy e in EnemyManager.inst.enemies)
            targets.Add(e);

        for (int i = 0; i < targets.Count; i++)
        {
            int rand = Random.Range(i, targets.Count);
            var temp = targets[i];
            targets[i] = targets[rand];
            targets[rand] = temp;
        }

        foreach (var obj in targets)
        {
            Vector3 pos;
            if (obj is Turret)
            {
                Turret t = (Turret)obj;
                pos = t.physical.transform.position;
                TurretManager.inst.turretsToRemove.Add(t);
            }
            else if (obj is Enemy)
            {
                Enemy e = (Enemy)obj;
                pos = e.physical.transform.position;
                EnemyManager.inst.enemiesToRemove.Add(e);
            }
            else continue;

            EffectManager.inst.spawnUnscaledExplosionEffect(pos);

            yield return new WaitForSecondsRealtime(0.1f);
        }
        
    }

    public void homeDead()
    {
        if (home == null) return;
        if (home.dead) return;

        home.dead = true;
        Debug.Log("GGEZ ur dead");

        // MAKE EVERYTHING EXPLODE!!!
        StartCoroutine(lerpTimeToZero());
    }
    public void spawnHome(Vector3 homePosition, float health, float damage, float attackCooldown, GameObject physical)
    {
        if (home != null)
        {
            Debug.LogError("burda bi sikinti var simdi");
            return;
        }
        GameObject newPhysical = Object.Instantiate(physical);
        newPhysical.transform.position = homePosition;
        home = new Home(health, damage, attackCooldown, newPhysical);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (home == null) //home oluşmadan attack etmeyi deniyordu error veriyordu bu yüzden de
        {
            return;
        }
        
        if (home.state == HomeState.Attacking)
        {
            if (home.target == null)
            {
                home.state = HomeState.Idle;
                return;
            }

            home.t += dt;
            if (home.t >= home.attackCooldown)
            {
                home.t -= home.attackCooldown;


                if (home.target != null)  //Bu defa da home target olmadan attack etmeyi deniyordu (mal mı biraz neyse)
                {
                    Enemy snapshotTarget = home.target;
                    ProjectileManager.inst.spawnProjectile(
                        home.attackCooldown / 2f,
                        home.physical.transform.position,
                        home.target,
                        GameManager.inst.projectilePrefabs[0],
                        () =>
                        {
                            snapshotTarget.takeDamage(home.damage);
                        }
                    );
                    home.t -= home.attackCooldown;
                    bool dead = home.target.health <= home.damage;
                    if (dead)
                    {                        
                        home.state = HomeState.Idle;
                        home.target.dead = true;
                        home.target = null;
                        return;
                    }
                }
            }
        }
    }
}
