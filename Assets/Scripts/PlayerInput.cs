using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class PlayerInput : MonoBehaviour
{
    private PlayerController _playerController;
    private GunController _gunController;
    Plane _plane = new Plane(Vector3.back, Vector3.zero);

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _gunController = GetComponent<GunController>();
    }

    void Update()
    {
        // Movement Code
        Vector3 input = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            input.y += 1;
        if (Input.GetKey(KeyCode.S))
            input.y -= 1;
        if (Input.GetKey(KeyCode.A))
            input.x -= 1;
        if (Input.GetKey(KeyCode.D))
            input.x += 1;

        input.Normalize();
        _playerController.Move(input);

        // Gun Code
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (_plane.Raycast(ray, out float enter))
        {
            Vector3 mouse = ray.GetPoint(enter);
            _gunController.AimAt(mouse);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _gunController.FireGun();
        }
    }
}
