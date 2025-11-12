using UnityEngine;

public enum HomeState
{
    Idle,
    Attacking
}
public class Home
{
    public float health = 10f;
    public float damage = 10f;
    public float attackCooldown = 1f;
    public GameObject physical;
    public float t = 0f;
    public Enemy target;

    public HomeState state = HomeState.Idle;

    public Home(float health, float damage, float attackCooldown, GameObject physical)
    {
        this.health = health;
        this.damage = damage;
        this.physical = physical;
        this.attackCooldown = attackCooldown;
    }

    public bool takeDamage(Enemy damager, float dmgTaken)
    {
        if (this.target == null)
        {
            Debug.Log("got new target!!!");
            this.target = damager;
            this.state = HomeState.Attacking;
        }
        this.health -= dmgTaken;
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
    public Home home;
    void Start()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);

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
                Debug.Log("HOME ATTACKED!!!");
                home.t -= home.attackCooldown;


                if (home.target != null)  //Bu defa da home target olmadan attack etmeyi deniyordu (mal mı biraz neyse)
                {
                    bool dead = home.target.takeDamage(home.damage);
                    if (dead)
                    {
                        home.state = HomeState.Idle;
                        home.target = null;
                        return;
                    }
                }
            }
        }
    }
}
