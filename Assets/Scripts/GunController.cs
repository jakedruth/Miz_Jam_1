using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private Transform _gunHolder;
    private Transform _bulletSpawn;
    private Transform _reloadDisplayHolder;
    private Transform _reloadDisplayFill;
    public int startingAmmo;

    public float bulletSpray;
    public int maxAmmo;
    public int AmmoCount { get; set; }
    public int clipSize;
    public int AmmoInClip { get; set; }
    public int TotalAmmo { get { return AmmoInClip + AmmoCount;} }
    public float reloadTime;
    private float _reloadTimer;

    public bool IsReloading { get { return _reloadTimer > 0; } }

    void Awake()
    {
        _gunHolder = transform.GetChild(1);
        _bulletSpawn = _gunHolder.GetChild(0);
        _reloadDisplayHolder = transform.GetChild(2);
        _reloadDisplayFill = _reloadDisplayHolder.GetChild(2);
        
        PickUpAmmo(startingAmmo);
        Reload(true);
        _reloadDisplayHolder.gameObject.SetActive(false);
    }

    void Update()
    {
        // TODO: Convert Gun To machine-gun

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

    public void PickUpAmmo(int amount)
    {
        int total = amount + TotalAmmo;

        _gunHolder.gameObject.SetActive(total > 0);

        if (amount == 0)
            return;

        AmmoCount = Mathf.Clamp(total, 0, maxAmmo);

        if (amount == total) 
            Reload();
    }

    public void AimAt(Vector3 pos)
    {
        if (TotalAmmo == 0)
            return;

        Vector2 delta = pos - transform.position;

        float angle = Vector2.SignedAngle(Vector2.right, delta);
        _gunHolder.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 scale = _gunHolder.localScale;
        scale.y = Mathf.Sign(delta.x);
        _gunHolder.localScale = scale;
    }

    public void FireGun()
    {
        // Check to see if you can fire
        if(IsReloading || TotalAmmo == 0)
            return;

        if (AmmoInClip == 0)
        {
            Reload();
            return;
        }

        // Remove a bullet from the clip
        AmmoInClip--;

        // Create a bullet with the angle of the gun
        float angle = _gunHolder.rotation.eulerAngles.z;
        angle += Random.Range(-bulletSpray, bulletSpray) * 0.5f;
        Bullet prefab = Resources.Load<Bullet>("Prefabs/Bullet");
        Bullet bullet = Instantiate(prefab, _bulletSpawn.position, Quaternion.AngleAxis(angle, Vector3.forward));

        // Check to see if the player has no more bullets
        if (TotalAmmo == 0) 
        {
            _gunHolder.gameObject.SetActive(false);
        }
    }

    public void Reload(bool ignoreReloadTime = false)
    {
        int neededAmmo = clipSize - AmmoInClip;
        if(neededAmmo == 0 || AmmoCount <= 0)
            return;
        
        if (neededAmmo < AmmoCount)
        {
            AmmoInClip += neededAmmo;
            AmmoCount -= neededAmmo;
        }
        else
        {
            AmmoInClip += AmmoCount;
            AmmoCount = 0;
        }

        _reloadDisplayHolder.gameObject.SetActive(!ignoreReloadTime);

        if (!ignoreReloadTime) 
            _reloadTimer = reloadTime;
    }
}
