using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class Turret : MonoBehaviour
{
    private Transform _gunHolder;
    private Transform _bulletSpawn;

    private Transform _player;

    public float rotationSpeed;
    public float bulletSpray;

    void Awake()
    {
        _gunHolder = transform.GetChild(1);
        _bulletSpawn = _gunHolder.GetChild(0);

        _player = FindObjectOfType<PlayerController>().transform;
    }

    void Update()
    {
        const float maxDistance = 15;
        Vector2 delta = _player.position - transform.position;
        if (delta.sqrMagnitude <= maxDistance * maxDistance)
        {
            int layerMask = LayerMask.GetMask("Walls", "Player");

            Ray2D ray = new Ray2D(transform.position, delta);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 15f, layerMask);
            if (hit)
            {
                if (hit.transform == _player)
                {
                    float angle = Vector2.SignedAngle(Vector2.right, delta);

                    Quaternion targetLookAt = Quaternion.AngleAxis(angle, Vector3.forward);
                    Quaternion rotation = Quaternion.RotateTowards(_gunHolder.rotation, targetLookAt, rotationSpeed * Time.deltaTime);
                    _gunHolder.rotation = rotation;
                }
            }
        }
    }

    private void Fire()
    {
        // Create a bullet with the angle of the gun
        float angle = _gunHolder.rotation.eulerAngles.z;
        angle += Random.Range(-bulletSpray, bulletSpray) * 0.5f;
        Bullet prefab = Resources.Load<Bullet>("Prefabs/Bullet");
        Bullet bullet = Instantiate(prefab, _bulletSpawn.position, Quaternion.AngleAxis(angle, Vector3.forward));
    }
}
