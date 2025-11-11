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
public class Turret
{
    public int id;
    public float damage = 10f;
    public float attackCooldown = 1f;
    public float reach = 3f;
    public GameObject physical;
    public TurretState state;
    public float t = 0;
    public Enemy target;


    public Turret(int id, float damage, float attackCooldown, float reach, GameObject physical)
    {
        this.id = id;
        this.damage = damage;
        this.physical = physical;
        this.attackCooldown = attackCooldown;
        this.reach = reach;
        this.state = TurretState.Idle;
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

    public void spawnTurret(Vector3 position, float damage, float attackCooldown, float reach, GameObject physical)
    {
        GameObject newPhysical = Object.Instantiate(physical);
        newPhysical.transform.position = position;
        Turret newTurret = new Turret(turrets.Count, damage, attackCooldown, reach, newPhysical);

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
                    Debug.Log(dist);
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
