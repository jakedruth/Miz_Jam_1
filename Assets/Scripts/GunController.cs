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

    public float bulletsPerSecond;
    public float FireRate { get { return 1 / bulletsPerSecond; } }
    private float _fireTimer;

    public int clipSize;
    public int AmmoInClip { get; set; }
    public float reloadTime;
    private float _reloadTimer;

    public bool IsReloading { get { return _reloadTimer > 0; } }

    void Awake()
    {
        _gunHolder = transform.Find("GunHolder");
        _bulletSpawn = _gunHolder.GetChild(0);

        _reloadDisplayHolder = transform.Find("ReloadDisplay");
        _reloadDisplayFill = _reloadDisplayHolder.GetChild(2);

        AmmoInClip = clipSize;
        _reloadDisplayHolder.gameObject.SetActive(false);

        AmmoDisplaySystem.DisplayAmmo(true);
    }

    public void HandleInput(bool onDown, bool isDown)
    {
        if (IsReloading)
            return;

        if (AmmoInClip == 0 && onDown)
        {
            Reload();
            return;
        }

        if (onDown)
            _fireTimer = 0;

        if (isDown)
        {
            _fireTimer -= Time.deltaTime;
            if (_fireTimer <= 0)
            {
                FireGun();
            }
        }
    }

    public void HandleReloading(bool reload)
    {
        // TODO: Convert Gun To machine-gun

        if (reload)
            Reload();

        if (IsReloading)
        {
            _reloadTimer -= Time.deltaTime;

            if (_reloadTimer < 0)
            {

                AmmoInClip = clipSize;
                _reloadTimer = 0;
                _fireTimer = 0;

                AmmoDisplaySystem.SetAmmo(AmmoInClip);
                _reloadDisplayHolder.gameObject.SetActive(false);
            }

            float t = (reloadTime - _reloadTimer) / reloadTime;
            
            _reloadDisplayFill.localScale = new Vector3(t , 1 , 1);
        }
    }
    

    public void AimAt(Vector3 pos)
    {
        Vector2 delta = pos - transform.position;

        float angle = Vector2.SignedAngle(Vector2.right, delta);
        _gunHolder.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 scale = _gunHolder.localScale;
        scale.y = Mathf.Sign(delta.x);
        _gunHolder.localScale = scale;
    }

    private void FireGun()
    {
        // Check to see if you can fire
        if(IsReloading || AmmoInClip == 0)
            return;

        // Remove a bullet from the clip
        AmmoInClip--;
        AmmoDisplaySystem.SetAmmo(AmmoInClip);
        _fireTimer += FireRate;

        // Create a bullet with the angle of the gun
        float angle = _gunHolder.rotation.eulerAngles.z;
        angle += Random.Range(-bulletSpray, bulletSpray) * 0.5f;
        Bullet prefab = Resources.Load<Bullet>("Prefabs/Bullets/PlayerBullet");
        Bullet bullet = Instantiate(prefab, _bulletSpawn.position, Quaternion.AngleAxis(angle, Vector3.forward));
    }

    public void Reload(bool ignoreReloadTime = false)
    {
        int neededAmmo = clipSize - AmmoInClip;
        if(neededAmmo == 0)
            return;

        if (ignoreReloadTime)
        {
            _reloadDisplayHolder.gameObject.SetActive(false);
            _reloadTimer = 0;
        }
        else
        {
            _reloadDisplayHolder.gameObject.SetActive(true);
            _reloadTimer = reloadTime;
        }
    }

    
}
