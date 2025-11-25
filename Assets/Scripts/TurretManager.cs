using System;
using System.Collections.Generic;
using UnityEngine;

public enum TurretState
{
    Attacking,
    Idle,
}

public enum TurretType
{
    Basic,
    AOE,
    Stun,
    AllInOne,
    Reverse,
    Buff,
    Debuff
}
public class Turret
{
    public int id;
    public float damage = 10f;
    public float attackCooldown = 1f;
    public float reach = 3f;
    public GameObject physical;
    public TurretState state;
    public TurretType type;
    public float t = 0;
    public Enemy target;
    public Projectile projectile;
    public Action<Enemy> onHit; // bunu kullanarak ozel seyleri yapariz MUHTEMELEN
    // stun fln seylerini iste // yaptim bile :p




    public Turret(int id, float damage, float attackCooldown, float reach, TurretType type, GameObject physical, Action<Enemy> onHit = null)
    {
        this.id = id;
        this.damage = damage;
        this.physical = physical;
        this.attackCooldown = attackCooldown;
        this.reach = reach;
        this.state = TurretState.Idle;
        this.type = type;
        this.onHit = onHit;

        LineRender lineRender = physical.GetComponent<LineRender>();
        if (lineRender != null)
        {
            lineRender.setUpRenderer(reach);
        }
    }
}

public class TurretManager : MonoBehaviour
{
    public static TurretManager inst;

    private List<Turret> turrets;
    private List<Turret> turretsToRemove;
    
    void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);

        turrets = new List<Turret>();
        turretsToRemove = new List<Turret>();
    }

    private System.Collections.IEnumerator RemoveStun(Enemy target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target == null) yield break;
        target.stunned -= 1;
    }
    private System.Collections.IEnumerator RemoveDebuff(Enemy target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target == null) yield break;
        target.debuffed -= 1;
    }

    private void setOnHit(Turret turret)
    {
        switch (turret.type)
        {
            case TurretType.Basic:
                turret.onHit = target =>
                {
                    target.takeDamage(turret.damage);
                };
                break;
            case TurretType.AOE:
                turret.onHit = target =>
                {
                    Vector3 targetPos = target.physical.transform.position;
                    foreach (var enemy in EnemyManager.inst.enemies)
                    {
                        float dist = Vector3.Distance(targetPos, enemy.physical.transform.position);
                        if (dist <= 2f) // aoe ne kadar olsun
                        {
                            enemy.takeDamage(turret.damage);
                        }
                    }
                    // target.takeDamage(turret.damage); galiba ustteki kod bunu yapiyo zaten ama GALIBA
                };
                break;
            case TurretType.Stun:
                turret.onHit = target =>
                {
                    StartCoroutine(RemoveStun(target, 0.5f));
                    target.stunned += 1;
                };
                break;
            case TurretType.AllInOne:
                turret.onHit = target =>
                {
                    target.takeDamage(turret.damage);
                    StartCoroutine(RemoveStun(target, 0.5f));
                    target.stunned += 1;
                };
                break;
            case TurretType.Reverse:
                turret.onHit = target =>
                {
                    // custom logic lazim buna yeah
                    // simdilik kendini yok edicek
                    turretsToRemove.Add(turret);
                };
                break;
            case TurretType.Buff:
                turret.onHit = target =>
                {
                    //bunu da sonra yapicaz
                };
                break;
            case TurretType.Debuff:
                turret.onHit = target =>
                {
                    StartCoroutine(RemoveDebuff(target, 0.5f));
                    target.debuffed += 1;
                };
                break;
            default:
                turret.onHit = target =>
                {
                    target.takeDamage(turret.damage);
                };
                break;
        }
    }

    public void spawnTurret(Vector3 position, float damage, float attackCooldown, float reach, TurretType type, GameObject physical)
    {
        GameObject newPhysical = Instantiate(physical, transform);
        newPhysical.transform.position = position;
        Turret newTurret = new Turret(turrets.Count, damage, attackCooldown, reach, type, newPhysical);
        
        setOnHit(newTurret);

        turrets.Add(newTurret);
    }
    public (float, float, float) getTurretData(TurretType type)
    {
        float damage, attackCooldown, reach;
        switch (type) //şimdilik boş sonra tamamlayacağım // ben tamamladim
        {
            case TurretType.Basic:
                damage = 2f;
                attackCooldown = 0.3f;
                reach = 2f;
                break;
            case TurretType.AOE: 
                damage = 1f;
                attackCooldown = 0.5f;
                reach = 3f;
                break;
            case TurretType.Stun:
                damage = 0f;
                attackCooldown = 1f;
                reach = 4f;
                break;
            case TurretType.AllInOne:
                damage = 3f;
                attackCooldown = 1f;
                reach = 3f;
                break;
            case TurretType.Reverse:
                damage = 0f;
                attackCooldown = 15f; // uzun cunku custom eklicez vakit olursa olmazsa olmicak bu turret
                reach = 0.5f;
                break;
            case TurretType.Buff:
                damage = 0f;
                attackCooldown = 100f;
                reach = 2f;
                break;
            case TurretType.Debuff:
                damage = 0f;
                attackCooldown = 1f;
                reach = 2f;
                break;
            default:
                damage = 1f;
                attackCooldown = 1f;
                reach = 1f;
                break;
        }
        return (damage, attackCooldown, reach);
    }
    public void spawnTurret(Vector3 position, TurretType type)
    {
        (float damage, float attackCooldown, float reach) = getTurretData(type);
        
        GameObject physical = GameManager.inst.turretPrefabs[(int) type];
        GameObject newPhysical = Instantiate(physical, transform);
        newPhysical.transform.position = position;
        Turret newTurret = new Turret(turrets.Count, damage, attackCooldown, reach, type, newPhysical);
        setOnHit(newTurret);

        turrets.Add(newTurret);
    }

    public void deleteTurret(Turret turret)
    {
        Destroy(turret.physical);
        turrets.Remove(turret);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        foreach (var turret in turrets)
        {
            if (turret.state == TurretState.Idle)
            {
                foreach (var enemy in EnemyManager.inst.enemies)
                {
                    if (enemy.dead) continue;
                    float dist = Vector3.Distance(turret.physical.transform.position, enemy.physical.transform.position);
                    if (dist <= turret.reach)
                    {
                        turret.target = enemy;
                        turret.state = TurretState.Attacking;
                        break;
                    }
                }
            }
            else if (turret.state == TurretState.Attacking)
            {
                if (turret.target == null || turret.target.dead)
                {
                    turret.target = null;
                    turret.state = TurretState.Idle;
                    continue;
                }
                float dist = Vector3.Distance(turret.physical.transform.position, turret.target.physical.transform.position);
                if (dist > turret.reach)
                {
                    turret.state = TurretState.Idle;
                    continue;
                }

                turret.t += dt;
                if (turret.t >= turret.attackCooldown)
                {
                    Enemy snapshotTarget = turret.target;
                    ProjectileManager.inst.spawnProjectile(
                        turret.attackCooldown / 2f,
                        turret.physical.transform.position,
                        turret.target,
                        GameManager.inst.projectilePrefabs[0],
                        () => turret.onHit?.Invoke(snapshotTarget)
                    );
                    turret.t -= turret.attackCooldown;
                    bool dead = turret.target.health <= turret.damage;
                    if (dead)
                    {
                        turret.state = TurretState.Idle;
                        turret.target.dead = true;
                        turret.target = null;
                        continue;
                    }
                }
            }
        }
        
    }
    void LateUpdate()
    {
        foreach (var turret in turretsToRemove)
        {
            turrets.Remove(turret);
        }
        turretsToRemove.Clear();
    }

}
