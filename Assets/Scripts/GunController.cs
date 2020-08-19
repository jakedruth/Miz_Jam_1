using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private Transform _gunHolder;
    private Transform _bulletSpawn;

    void Awake()
    {
        _gunHolder = transform.GetChild(1);
        _bulletSpawn = _gunHolder.GetChild(0);
    }

    public void AimAt(Vector3 pos)
    {
        Vector3 delta = pos - transform.position;

        float angle = Vector3.SignedAngle(Vector3.right, delta, Vector3.forward);
        _gunHolder.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 scale = _gunHolder.localScale;
        scale.y = Mathf.Sign(delta.x);
        _gunHolder.localScale = scale;
    }

    public void FireGun()
    {
        float angle = _gunHolder.rotation.eulerAngles.z;
        Bullet prefab = Resources.Load<Bullet>("Prefabs/Bullet");
        Bullet bullet = Instantiate(prefab, _bulletSpawn.position, Quaternion.AngleAxis(angle, Vector3.forward));
    }
}
