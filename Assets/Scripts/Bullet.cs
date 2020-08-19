using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;

    private float _lifeTime = 3;


    // Update is called once per frame
    void Update()
    {
        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0)
        {
            Destroy(gameObject);
            return;
        }

        float step = speed * Time.deltaTime;
        Vector3 vel = transform.right * step;
        transform.position += vel;

        RaycastHit2D[] hit2Ds = Physics2D.RaycastAll(transform.position, vel, step);
        foreach (RaycastHit2D hit in hit2Ds)
        {
            switch (hit.transform.tag)
            {
                default:
                    Debug.Log("Unknown Collision Type with object", hit.transform.gameObject);
                    break;
                case "Wall":
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
