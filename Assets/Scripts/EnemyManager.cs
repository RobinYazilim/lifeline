using System.Collections.Generic;
using UnityEngine;


public enum EnemyState
{
    Walking,
    Attacking,
    Stunned
}
public enum EnemyType
{
    Basic,
    Fast,
    Tank,
    Debuff,
    Kamikaze,
    Boss,
}
public class Enemy
{
    public float speed = 1f;
    public int id;
    public float health = 10f;
    public float damage = 10f;
    public float attackCooldown = 1f;
    public GameObject physical;
    public float t = 0f;
    public int currentIndex = 1;

    public EnemyState state = EnemyState.Walking;
    public EnemyType type;

    public Enemy(float speed, int id, float health, float damage, float attackCooldown, EnemyType type, GameObject physical)
    {
        this.speed = speed;
        this.id = id;
        this.health = health;
        this.damage = damage;
        this.physical = physical;
        this.attackCooldown = attackCooldown;
        this.type = type;
    }

    public bool takeDamage(float dmgTaken)
    {
        this.health -= dmgTaken;
        if (this.health <= 0)
        {
            this.health = 0;
            EnemyManager.inst.deleteEnemy(this);
            return true;
        }
        return false;
    }
}

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager inst;
    private Transform[] path;
    public List<Enemy> enemies;

    void Start()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);

        this.enemies = new List<Enemy>();
    }

    public void SetPath(Transform[] path)
    {
        this.path = path;
    }

    public void spawnEnemy(float speed, float health, float damage, float attackCooldown, EnemyType type, GameObject physical)
    {
        switch(type)
        {
            case EnemyType.Fast:
                speed *= 1.5f;
                health *= 0.75f;
                break;
            case EnemyType.Tank:
                speed *= 0.75f;
                health *= 1.5f;
                break;
            case EnemyType.Kamikaze:
                damage *= 2f;
                health *= 0.5f;
                speed *= 1.5f;
                break;
            case EnemyType.Boss:
                speed *= 0.5f;
                health *= 3.5f;
                damage *= 2f;
                break;
            case EnemyType.Debuff:
                // sonra yaparız
                break;

            case EnemyType.Basic:
            default:
                break;
        }
        
        GameObject newPhysical = Object.Instantiate(physical);
        newPhysical.transform.position = new Vector3(1000f, 1000f, 1000f);
        Enemy newEnemy = new Enemy(speed, enemies.Count, health, damage, attackCooldown, type, newPhysical);

        enemies.Add(newEnemy);
    }

    public void deleteEnemy(Enemy enemy)
    {
        Destroy(enemy.physical);
        enemies.Remove(enemy);
    }
    void Update()
    {
        if (path == null || path.Length == 0)
        {
            return;
        }
        float dt = Time.deltaTime;

        foreach (var enemy in enemies)
        {
            if (enemy.state == EnemyState.Walking)
            {
                enemy.t += dt * enemy.speed / Vector3.Distance(path[enemy.currentIndex - 1].position, path[enemy.currentIndex].position);
                enemy.physical.transform.position = Vector3.Lerp(path[enemy.currentIndex - 1].position, path[enemy.currentIndex].position, enemy.t);
                if (enemy.t >= 1f)
                {
                    enemy.t = 0f;
                    enemy.currentIndex++;
                    if (enemy.currentIndex >= path.Length)
                    {
                        enemy.state = EnemyState.Attacking;
                    }
                }
            }
            else if (enemy.state == EnemyState.Attacking)
            {
                enemy.t += dt;
                if (enemy.t >= enemy.attackCooldown)
                {
                        Debug.Log("Attacking the home base thing");
                    enemy.t -= enemy.attackCooldown;
                    bool dead = HomeManager.inst.home.takeDamage(enemy, enemy.damage);
                    if (dead)
                    {
                        Debug.Log("GGEZ ur dead");
                    }
                }
            }
        }
    }
}
