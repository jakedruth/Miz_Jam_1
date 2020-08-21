using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour
{
    private Camera _cam;
    public Transform target;
    private Vector3 _pos;
    private Vector3 _offset;

    [Header("Zoom Values")]
    public float minZoom;
    public float maxZoom;
    private float _zoomT = 0;
    private Coroutine _zoomCoroutine;
    public float speed;
    
    [Header("Shake Values")]
    public int shakeCount;
    public float shakeSpeed;
    public float maxDelta;
    

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
            _pos = Vector3.MoveTowards(_pos, target.transform.position, speed * Time.deltaTime);
            transform.position = _pos + _offset;
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

    [ContextMenu("Shake Camera")]
    public void ApplyCameraShake()
    {
        StopAllCoroutines();
        StartCoroutine(HandleCameraShake());
    }

    private IEnumerator HandleCameraShake()
    {
        List<Vector3> shakePoints = new List<Vector3>();

        float randAngle = Random.Range(0, Mathf.PI * 2);

        for (float i = shakeCount; i >= 0; i--)
        {
            float t = i / shakeCount;
            randAngle += Mathf.PI + Random.Range(-Mathf.PI * 0.25f, Mathf.PI * 0.25f) * 0.5f;
            randAngle %= 2 * Mathf.PI;
            float mag = t * maxDelta;
            Vector3 pos = new Vector3(Mathf.Cos(randAngle) * mag, Mathf.Sin(randAngle) * mag);
            shakePoints.Add(pos);
        }

        while (shakePoints.Count > 0)
        {
            _offset = Vector3.MoveTowards(_offset, shakePoints[0], shakeSpeed * Time.deltaTime);
            
            if(_offset == shakePoints[0])
                shakePoints.RemoveAt(0);

            yield return null;
        }
    }
}
