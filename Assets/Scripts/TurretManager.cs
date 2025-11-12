using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering;
public enum TurretState
{
    Attacking,
    Stunned,
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



    public Turret(int id, float damage, float attackCooldown, float reach, TurretType type, GameObject physical)
    {
        this.id = id;
        this.damage = damage;
        this.physical = physical;
        this.attackCooldown = attackCooldown;
        this.reach = reach;
        this.state = TurretState.Idle;
        this.type = type;

    }
}

public class TurretManager : MonoBehaviour
{
    public static TurretManager inst;

    private List<Turret> turrets;
    
    void Start()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);

        turrets = new List<Turret>();
    }

    public void spawnTurret(Vector3 position, float damage, float attackCooldown, float reach,TurretType type, GameObject physical)
    {
        switch (type) //şimdilik boş sonra tamamlayacağım
        {
            case TurretType.Basic:
                default:
                break;
            case TurretType.AOE: 
                break;
            case TurretType.Stun:
                break;
            case TurretType.AllInOne:  
                break;
            case TurretType.Reverse: 
                break;
            case TurretType.Buff:
                break;
            case TurretType.Debuff:
                break;
        }
        GameObject newPhysical = Object.Instantiate(physical);
        newPhysical.transform.position = position;
        Turret newTurret = new Turret(turrets.Count, damage, attackCooldown, reach, type, newPhysical);

        turrets.Add(newTurret);
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
                    float dist = Vector3.Distance(turret.physical.transform.position, enemy.physical.transform.position);
                    if (dist <= turret.reach)
                    {
                        turret.target = enemy;
                        turret.state = TurretState.Attacking;
                        Debug.Log("FOUND ENEMY!!!");
                        break;
                    }
                }
            }
            else if (turret.state == TurretState.Attacking)
            {
                if (turret.target == null)
                {
                    turret.state = TurretState.Idle;
                    continue;
                }
                float dist = Vector3.Distance(turret.physical.transform.position, turret.target.physical.transform.position);
                if (dist > turret.reach)
                {
                    turret.state = TurretState.Idle;
                        Debug.Log("Enemy out of bounds..");
                    continue;
                }

                turret.t += dt;
                if (turret.t >= turret.attackCooldown)
                {
                    Debug.Log("ATTACKED!!!");
                    turret.t -= turret.attackCooldown;
                    bool dead = turret.target.takeDamage(turret.damage);
                    if (dead)
                    {
                        turret.state = TurretState.Idle;
                        continue;
                    }
                }
            }
        }
    }
}
