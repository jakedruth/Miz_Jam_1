using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeginEncounter : MonoBehaviour
{
    public bool takeOverCamera;
    [Range(0, 1)]
    public float setZoomLevel;

    private int _enemiesRemaining;

    void Awake()
    {
       SetAllChildrenActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            CameraController cam = FindObjectOfType<CameraController>();
            float startZoom = cam.GetZoomT();
            if (takeOverCamera)
            {
                cam.target = transform;
                cam.AdjustZoomTOverTime(setZoomLevel, 0.75f);
            }

            BoxCollider2D c = GetComponent<BoxCollider2D>();

            Enemy[] enemies = Physics2D.OverlapBoxAll(c.bounds.center, c.size, 0, LayerMask.GetMask("Enemies")).Select(e => e.transform.GetComponent<Enemy>()).ToArray();
            _enemiesRemaining = enemies.Length;
            foreach (Enemy enemy in enemies)
            {
                enemy.OnDestroyEvent.AddListener(() =>
                {
                    _enemiesRemaining--;
                    
                    if (_enemiesRemaining > 0) 
                        return;
                    
                    Destroy(gameObject);
                    if (!takeOverCamera) 
                        return;

                    cam.target = collision.transform;
                    cam.AdjustZoomTOverTime(startZoom, 0.75f);
                });
            }

            Destroy(c);
            SetAllChildrenActive(true);
        }
    }

    void SetAllChildrenActive(bool value)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(value);
        }
    }

}
