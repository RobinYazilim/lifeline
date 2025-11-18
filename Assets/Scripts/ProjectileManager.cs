using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using System;


public class Projectile
{
    public float remainingTime = 5f;
    public GameObject physical;
    public ITarget target;
    public Action onHit;
    
    public Projectile(float remainingTime, ITarget target, GameObject physical, Action onHit)
    {
        this.remainingTime = remainingTime;
        this.target = target;
        this.physical = physical;
        this.onHit = onHit;
    }
}
public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager inst;
   
    private List<Projectile> projectiles;
    private List<Projectile> projectilesToRemove;

    void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);


        this.projectiles = new List<Projectile>();
        this.projectilesToRemove = new List<Projectile>();
    }
    
    public void spawnProjectile(float remainingTime, Vector3 position, ITarget target, GameObject physical, Action onHit)
    {
        GameObject newPhysical = Instantiate(physical, transform);
        newPhysical.transform.position = position;
        Projectile newProjectile = new Projectile(remainingTime, target, newPhysical, onHit);
        projectiles.Add(newProjectile);
        Debug.Assert(newProjectile.physical != null, "Projectile physical is null!");
    }

    public void deleteProjectile(Projectile projectile)
    {
        Destroy(projectile.physical);
        projectiles.Remove(projectile);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        foreach (Projectile proj in projectiles)
        {
            if (proj.target == null || !proj.target.exists())
            {
                projectilesToRemove.Add(proj);
                continue;
            }

            if (proj.remainingTime <= dt)
            {
                proj.onHit?.Invoke();

                projectilesToRemove.Add(proj);
                continue;
            }

            // bilerek normalized degil cunku gidicegi yolun uzunluguna gore hiz ayarlicak
            Vector3 direction = proj.target.getPosition() - proj.physical.transform.position;
            Vector3 move = direction * (dt / proj.remainingTime);

            proj.physical.transform.position += move;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            proj.physical.transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

            proj.remainingTime -= dt;
        }
        
    }
    void LateUpdate()
    {
        foreach (Projectile proj in projectilesToRemove)
        {
            deleteProjectile(proj);
        }
        projectilesToRemove.Clear();
    }
    
}