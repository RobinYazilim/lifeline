using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    public float speed = 10;
    public float health = 10;
    public float damage = 10;

    private Transform[] path;
    private int currentIndex = 0;


    public void SetPath(Path pathObj)
    {
        path = pathObj.points;
        currentIndex = 0;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (path == null || path.Length == 0)
        {
            return;
        }
        float dt = Time.deltaTime;
        

    }
    
    public bool TakeDamage(float dmgTaken)
    {
        health -= dmgTaken;
        if (health <= 0)
        {
            health = 0;
            return true;
        }
        return false;
    }
}
