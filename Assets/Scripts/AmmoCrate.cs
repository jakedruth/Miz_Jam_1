using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCrate : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Here");
            GunController gun = collision.GetComponent<GunController>();
            gun.PickUpAmmo(gun.maxAmmo);
        }
    }

}
