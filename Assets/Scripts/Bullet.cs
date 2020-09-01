using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public int damage;
    private float _lifeTime = 3;

    public bool hurtPlayer;

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
                case "Player":
                    if (hurtPlayer)
                    {
                        hit.transform.GetComponent<PlayerController>().AdjustHealth(-damage, transform.right.normalized);
                        Destroy(gameObject);
                    }
                    break;
                case "Enemy":
                    if (!hurtPlayer)
                    {
                        hit.transform.GetComponent<Enemy>().AdjustHealth(-damage);
                        Destroy(gameObject);
                    }
                    break;
                case "Wall":
                    Destroy(gameObject);
                    break;
                case "Toggle":
                    Destroy(gameObject);
                    hit.transform.GetComponent<Switch>().ToggleSwitch();
                    break;
                case "Crate":
                    Destroy(gameObject);
                    Destroy(hit.transform.gameObject);
                    break;
                // Case Do Nothing
                case "Water":
                    break;
            }
        }
    }
}
