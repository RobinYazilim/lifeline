using UnityEngine;
using System.Collections.Generic;


public class Projectile
    {
        public float speed = 5f;
        public GameObject physicalObject;
        public float dispawnTime = 5f;
        public Transform target;
        
        public Projectile(float speed, GameObject physicalObject, float dispawnTime, Transform target)
        {
            this.speed = speed;
            this.physicalObject = physicalObject;
            this.dispawnTime = dispawnTime;
            this.target = target;
        }
    }
public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager inst;
    public GameObject projectilePrefab;
   

    void Start()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);


    }
    
public void SpawnProjectile(float speed, Vector3 position, float dispawnTime, Transform target)
    { 
        GameObject obj = Instantiate(projectilePrefab, position, Quaternion.identity);
        ProjectileBehaviour projectilebehaviour = obj.GetComponent<ProjectileBehaviour>();
        projectilebehaviour.Init(speed, target);
        Destroy(obj, dispawnTime);
    }    

}
