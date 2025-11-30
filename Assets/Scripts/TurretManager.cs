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
    Basic, //favorimiz
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
    public Action<Enemy, float> onHit; // bunu kullanarak ozel seyleri yapariz MUHTEMELEN
    // stun fln seylerini iste // yaptim bile :p
    public int buffed = 0;
    public int debuffed = 0;



    public Turret(int id, float damage, float attackCooldown, float reach, TurretType type, GameObject physical, Action<Enemy, float> onHit = null)
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

    public List<Turret> turrets;

    public int maxTurretCount = 15;
    public List<Turret> turretsToRemove;
    
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

    private System.Collections.IEnumerator TurretRemoveDebuff(Turret target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target == null) yield break;
        target.debuffed -= 1;
    }
    public bool turretPosBlocked(Vector3 pos)
    {
        bool isOnPath = PathManager.inst.isOnPath(pos);
        if (isOnPath)
            return true;
        
        foreach (var turret in turrets)
        {
            if (turret == null || turret.physical == null) continue;
            float dist = Vector3.Distance(pos, turret.physical.transform.position);
            if (dist < 1f) // turretlar arasi minimum mesafe
                return true;
        }

        return false;
    }

    public bool canSpawnTurret()
    {
        return turrets.Count < maxTurretCount;
    }

    private float getTurretBuffMultiplier(Turret turret)
    {
        int diff = turret.debuffed - turret.buffed;
        if (diff > 0)
        {
            return 0.5f;
        }
        else if (diff < 0)
        {
            return 1.5f;
        }
        else
        {
            return 1f;
        }
    }

    private void doTurretDebuff(Turret turret)
    {
        if (turret.target == null) return;

        if (turret.target.type == EnemyType.Debuff)
        {
            EffectManager.inst.spawnDebuffEffect(turret.physical);
            StartCoroutine(TurretRemoveDebuff(turret, 1f));
            turret.debuffed += 1;
        }
    }

    private void setOnHit(Turret turret)
    {
        switch (turret.type)
        {
            case TurretType.Basic:
                turret.onHit = (target, damage) =>
                {
                    target.takeDamage(damage);
                    doTurretDebuff(turret);
                };
                break;
            case TurretType.AOE:
                turret.onHit = (target, damage) =>
                {
                    Vector3 targetPos = target.physical.transform.position;
                    EffectManager.inst.spawnBombEffect(target.physical);
                    foreach (var enemy in EnemyManager.inst.enemies)
                    {
                        float dist = Vector3.Distance(targetPos, enemy.physical.transform.position);
                        if (dist <= 2f) // aoe ne kadar olsun
                        {
                            enemy.takeDamage(damage);
                        }
                    }
                    // target.takeDamage(turret.damage); galiba ustteki kod bunu yapiyo zaten ama GALIBA
                    doTurretDebuff(turret);
                };
                break;
            case TurretType.Stun:
                turret.onHit = (target, damage) =>
                {
                    EffectManager.inst.spawnStunEffect(target.physical);
                    StartCoroutine(RemoveStun(target, 0.2f));
                    target.stunned += 1;
                    doTurretDebuff(turret);
                };
                break;
            case TurretType.AllInOne:
                turret.onHit = (target, damage) =>
                {
                    Vector3 targetPos = target.physical.transform.position;
                    foreach (var enemy in EnemyManager.inst.enemies)
                    {
                        if (enemy != target && enemy.dead) continue;
                        float dist = Vector3.Distance(targetPos, enemy.physical.transform.position);
                        if (dist <= 2f) // aoe ne kadar olsun
                        {
                            enemy.takeDamage(damage);
                        }
                    }
                    
                    EffectManager.inst.spawnStunEffect(target.physical);
                    EffectManager.inst.spawnBombEffect(target.physical);
                    EffectManager.inst.spawnDebuffEffect(target.physical);
                    StartCoroutine(RemoveStun(target, 0.1f));
                    target.stunned += 1;

                    StartCoroutine(RemoveDebuff(target, 0.2f));
                    target.debuffed += 1;
                    doTurretDebuff(turret);
                };
                break;
            case TurretType.Reverse:
                turret.onHit = (target, damage) =>
                {
                    // custom logic lazim buna yeah
                    // simdilik kendini yok edicek
                    turretsToRemove.Add(turret);
                    doTurretDebuff(turret);
                };
                break;
            case TurretType.Buff:
                turret.onHit = (target, damage) =>
                {
                    //bunu da sonra yapicaz
                    doTurretDebuff(turret);
                };
                break;
            case TurretType.Debuff:
                turret.onHit = (target, damage) =>
                {
                    EffectManager.inst.spawnDebuffEffect(target.physical);
                    StartCoroutine(RemoveDebuff(target, 0.5f));
                    target.debuffed += 1;
                    doTurretDebuff(turret);
                };
                break;
            default:
                turret.onHit = (target, damage) =>
                {
                    target.takeDamage(damage);
                    doTurretDebuff(turret);
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
                float rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < 0.1f)
                {
                    damage = 2f;
                    attackCooldown = 0.33f;
                }
                else
                {
                    damage = 1f;
                    attackCooldown = 0.5f;
                }
                reach = 2f;
                break;
            case TurretType.AOE: 
                damage = 4f;
                attackCooldown = 1.5f;
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
            turret.buffed = 0;
        }

        foreach (var turret in turrets)
        {
            if (turret.type != TurretType.Buff) continue;
            foreach (var otherTurret in turrets)
            {
                if (otherTurret == turret) continue;
                if (otherTurret.type == TurretType.Buff) continue;

                float dist = Vector3.Distance(turret.physical.transform.position, otherTurret.physical.transform.position);
                if (dist <= turret.reach)
                {
                    otherTurret.buffed += 1;
                    if (otherTurret.buffed == 1)
                    {
                        EffectManager.inst.spawnBuffEffect(otherTurret.physical);
                    }
                }
            }
            continue;
        }

        foreach (var turret in turrets)
        {
            if (turret.type == TurretType.Buff) continue;
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
                float mult = getTurretBuffMultiplier(turret);

                turret.t += dt * mult;
                if (turret.t >= turret.attackCooldown)
                {
                    Enemy snapshotTarget = turret.target;
                    float predictedDamage = turret.damage * mult;
                    ProjectileManager.inst.spawnProjectile(
                        turret.attackCooldown / 2f,
                        turret.physical.transform.position,
                        turret.target,
                        GameManager.inst.projectilePrefabs[0],
                        () => turret.onHit?.Invoke(snapshotTarget, predictedDamage)
                    );
                    turret.t -= turret.attackCooldown;
                    bool dead = turret.target.health <= predictedDamage;
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
