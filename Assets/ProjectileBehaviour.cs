using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float speed;
    public Transform target;

    public void Init(float speed, Transform target)
    {
        this.speed = speed;
        this.target = target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        float step = speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) <= 0.5f)
        {
    
            Destroy(gameObject);
            return;
        }
        else
        {
            //Debug.Log(Vector3.Distance(transform.position, target.position));
            //Debug.Log("Step:"+step);
            transform.position = Vector3.Lerp(transform.position, target.position, step);
        }
        
    }
}

