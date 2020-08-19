using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private Transform _gunHolder;
    private Transform _bulletSpawn;
    private Transform _reloadDisplayHolder;
    private Transform _reloadDisplayFill;

    public float bulletSpray;
    public int ammoCount;
    public int clipSize;
    private int _ammoInClip;
    public float reloadTime;
    private float _reloadTimer;

    public bool IsReloading { get { return _reloadTimer > 0; } }

    void Awake()
    {
        _gunHolder = transform.GetChild(1);
        _bulletSpawn = _gunHolder.GetChild(0);
        _reloadDisplayHolder = transform.GetChild(2);
        _reloadDisplayFill = _reloadDisplayHolder.GetChild(2);

        Reload();
        _reloadTimer = 0;
        _reloadDisplayHolder.gameObject.SetActive(false);
    }

    void Update()
    {
        if (IsReloading)
        {
            _reloadTimer -= Time.deltaTime;

            if (_reloadTimer < 0)
            {
                _reloadTimer = 0;
                _reloadDisplayHolder.gameObject.SetActive(false);
            }

            float t = (reloadTime - _reloadTimer) / reloadTime;
            _reloadDisplayFill.localScale = new Vector3(t , 1 , 1);
        }
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
        if(IsReloading)
            return;

        if (_ammoInClip == 0)
        {
            Reload();
            return;
        }

        _ammoInClip--;

        float angle = _gunHolder.rotation.eulerAngles.z;
        angle += Random.Range(-bulletSpray, bulletSpray) * 0.5f;
        Bullet prefab = Resources.Load<Bullet>("Prefabs/Bullet");
        Bullet bullet = Instantiate(prefab, _bulletSpawn.position, Quaternion.AngleAxis(angle, Vector3.forward));
    }

    public void Reload()
    {
        int neededAmmo = clipSize - _ammoInClip;
        if(neededAmmo == 0)
            return;
        
        if (neededAmmo < ammoCount)
        {
            _ammoInClip += neededAmmo;
            ammoCount -= neededAmmo;
        }
        else
        {
            _ammoInClip += ammoCount;
            ammoCount = 0;
        }

        _reloadTimer = reloadTime;
        _reloadDisplayHolder.gameObject.SetActive(true);
    }
}
