using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _cam;
    public Transform target;
    public float speed;
    public float minZoom;
    public float maxZoom;
    private float _zoomT = 0;
    private Coroutine _zoomCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        _cam = transform.GetComponentInChildren<Camera>();
        if (_cam == null)
            Debug.LogError("Could not find camera component in children");

        transform.position = target.position;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }

        UpdateCameraSize();

        /* Debug Code
        float scrollValue = -Input.mouseScrollDelta.y;
        _zoomT = Mathf.Clamp01(_zoomT + scrollValue * 0.1f);
        float zoom = Mathf.Lerp(minZoom, maxZoom, _zoomT);
        _cam.orthographicSize = zoom; 
        /* End Debug Code */
    }

    private void UpdateCameraSize()
    {
        float zoom = Mathf.Lerp(minZoom, maxZoom, _zoomT);
        _cam.orthographicSize = zoom;
    }

    public void SetZoomT(float t)
    {
        _zoomT = Mathf.Clamp01(t);
    }

    public float GetZoomT()
    {
        return _zoomT;
    }

    public void AdjustZoomTOverTime(float t, float time)
    {
        if(_zoomCoroutine != null)
            StopCoroutine(_zoomCoroutine);

        _zoomCoroutine = StartCoroutine(LerpZoomTOverTime(t, time));
    }

    private IEnumerator LerpZoomTOverTime(float t, float time)
    {
        float start = _zoomT;
        float timer = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;

            _zoomT = Mathf.Lerp(start, t, timer / time);

            yield return null;
        }

        _zoomT = t;

        _zoomCoroutine = null;
    }
}
