using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    private PlayerController _playerController;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // TODO: Update input
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
    }
}
