using System;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyState
{
    Walking,
    Attacking
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
public class Enemy : ITarget
{
    public bool exists() => physical != null;
    public Vector3 getPosition() => physical.transform.position;
    public float speed = 1f;
    public int id;
    public float health = 10f;
    public float damage = 10f;
    public float attackCooldown = 1f;
    public GameObject physical;
    public float t = 0f;
    public bool dead = false;
    public int currentIndex = 1;
    public int stunned = 0; // 0dan farkliysa stun var + - yapiyoz cunku oyle 6 yillik gamedev experiencim var trust me bro
    public int debuffed = 0; // ayni sey

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
            EnemyManager.inst.enemiesToRemove.Add(this);
            return true;
        }
        if (physical != null)
            EnemyManager.inst.StartCoroutine(EnemyManager.inst.DamageFlash(physical));

        return false;
    }
}

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager inst;
    private Transform[] path;
    public List<Enemy> enemies;
    public List<Enemy> enemiesToRemove;

    void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);

        this.enemies = new List<Enemy>();
        this.enemiesToRemove = new List<Enemy>();
    }

    public void SetPath(Transform[] path)
    {
        this.path = path;
    }

    public void spawnEnemy(float speed, float health, float damage, float attackCooldown, EnemyType type, GameObject physical)
    {
        GameObject newPhysical = Instantiate(physical, transform);
        newPhysical.transform.position = new Vector3(1000f, 1000f, 1000f);
        Enemy newEnemy = new Enemy(speed, enemies.Count, health, damage, attackCooldown, type, newPhysical);

        enemies.Add(newEnemy);
    }
    public void spawnEnemy(EnemyType type)
    {
        GameObject physical = GameManager.inst.enemyPrefabs[(int) type];
        float speed, health, damage, attackCooldown;
        switch(type)
        {
            case EnemyType.Fast:
                speed = 5f;
                health = 5f;
                damage = 2f;
                attackCooldown = 0.33f;
                break;
            case EnemyType.Tank:
                speed = 1f;
                health = 15f;
                damage = 5f;
                attackCooldown = 3f;
                break;
            case EnemyType.Kamikaze:
                speed = 5f;
                health = 1f;
                damage = 20f;
                attackCooldown = 30f;
                break;
            case EnemyType.Boss:
                speed = 1f;
                health = 100f;
                damage = 10f;
                attackCooldown = 1f;
                break;
            case EnemyType.Debuff:
                speed = 2f;
                health = 5f;
                damage = 10f;
                attackCooldown = 2f;
                break;
            case EnemyType.Basic:
                speed = 2f;
                health = 5f;
                damage = 2f;
                attackCooldown = 2f;
                break;
            default:
                speed = 1f;
                health = 1f;
                damage = 1f;
                attackCooldown = 1f;
                break;
        }
        
        GameObject newPhysical = Instantiate(physical, transform);
        newPhysical.transform.position = new Vector3(1000f, 1000f, 1000f);
        Enemy newEnemy = new Enemy(speed, enemies.Count, health, damage, attackCooldown, type, newPhysical);

        enemies.Add(newEnemy);
    }
    public void deleteEnemy(Enemy enemy)
    {
        Debug.Log("Deleting enemy ID: " + enemy.id);
        Destroy(enemy.physical);
        enemy.dead = true;
        enemies.Remove(enemy);
        
    }
    public System.Collections.IEnumerator DamageFlash(GameObject physical)
    {
        SpriteRenderer sr = physical.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;
        
        Color originalColor = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        if (sr == null) yield break;
        sr.color = originalColor;
    }
    void Update()
    {
        if (path == null || path.Length == 0)
        {
            return;
        }
        float dt = Time.deltaTime;
        if (enemies.Count == 0 && WaveManager.inst.spawningFinished && !ShopManager.inst.shopVisible)
        {
            ShopManager.inst.showShop();
        }

        foreach (var enemy in enemies)
        {
            if (enemy.dead) continue;
            if (enemy.stunned > 0)
            {
                continue;
            }
            if (enemy.state == EnemyState.Walking)
            {
                enemy.t += dt * (enemy.speed * (enemy.debuffed != 0 ? 0.5f : 1f)) / Vector3.Distance(path[enemy.currentIndex - 1].position, path[enemy.currentIndex].position);
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
    void LateUpdate()
    {
        foreach (var enemy in enemiesToRemove)
        {
            deleteEnemy(enemy);
        }
        enemiesToRemove.Clear();
    }
}
