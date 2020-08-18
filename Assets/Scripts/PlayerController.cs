using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float maxSpeed;
    public float acceleration;

    public float maxTilt;
    public float tiltPerSecond;

    private Vector3 _velocity;
    private float _shakeValue;

    public bool onPipe;
    public bool movingOnPipe;

    public void Move(Vector3 input)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!onPipe)
            {
                PipePiece[] pipes = FindObjectsOfType<PipePiece>();
                foreach (PipePiece pipe in pipes)
                {
                    if (!pipe.isPad)
                        continue;

                    Vector3 displacement = pipe.transform.position - transform.position;
                    float threshHold = 1f;
                    if (displacement.sqrMagnitude <= threshHold)
                    {
                        transform.position = pipe.transform.position;
                        onPipe = true;
                        break;
                    }
                }
            }
            else
            {
                onPipe = false;
            }
        }


        if (onPipe)
        {
            if (input.sqrMagnitude >= 0.1f)
            {
                if (!movingOnPipe)
                {
                    // convert input to a direction
                    int dir = 0;
                    if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) // horizontal
                        dir = input.x > 0 ? 1 : 3;
                    else
                        dir = input.y > 0 ? 0 : 2;

                    // Get a path

                    // Start moving along path


                }
            }
        }
        else
        {
            // Swaying while walking
            float targetSway = (input.sqrMagnitude < 0.1) ? 0 : maxTilt;
            _shakeValue = Mathf.MoveTowards(_shakeValue, targetSway, acceleration * Time.deltaTime);
            float angle = Mathf.Sin(Time.time * tiltPerSecond) * _shakeValue;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Movement code
            Vector3 targetVelocity = (input.Equals(Vector3.zero) ? Vector3.zero : input) * maxSpeed;
            _velocity = Vector3.MoveTowards(_velocity, targetVelocity, acceleration * Time.deltaTime);
            transform.position = transform.position + _velocity * Time.deltaTime;

        }
    }
}
