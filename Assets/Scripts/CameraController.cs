using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _cam;
    public Transform target;
    public float maxSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _cam = transform.GetComponentInChildren<Camera>();
        if (_cam == null)
            Debug.LogError("Could not find camera component in children");
    }

    void Update()
    {
        if (target != null)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, target.transform.position, maxSpeed * Time.deltaTime);
        }
    }
}
