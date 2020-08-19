﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float walkSpeed;
    public float pipeSpeed;
    public float pipeSpeedModifier;
    public float acceleration;

    public float maxTilt;
    public float tiltPerSecond;

    private Vector3 _velocity;
    private float _shakeValue;

    private int _pipeMoveDirection;
    private PipePiece _onPipe;
    private bool _movingOnPipe;

    public void Move(Vector3 input)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_onPipe == null)
            {
                PipePiece[] pipes = FindObjectsOfType<PipePiece>();
                foreach (PipePiece pipe in pipes)
                {
                    if (!pipe.isPad)
                        continue;

                    Vector3 displacement = pipe.transform.position - transform.position;
                    const float threshHold = 1f;
                    if (displacement.sqrMagnitude <= threshHold)
                    {
                        transform.position = pipe.transform.position;
                        _onPipe = pipe;
                        _velocity = Vector3.zero;
                        transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
                        break;
                    }
                }
            }
            else
            {
                if (!_movingOnPipe)
                {
                    _onPipe = null;
                }
            }
        }

        if (_onPipe != null)
        {
            if (!_movingOnPipe)
            {
                if (input.sqrMagnitude >= 0.1f)
                { 
                    // convert input to a direction
                    int dir;
                    if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) // horizontal
                        dir = input.x > 0 ? 1 : 3;
                    else
                        dir = input.y > 0 ? 0 : 2;

                    PipePiece nextPiece = _onPipe.GetPipe(dir);
                    if (nextPiece != null)
                    {
                        _movingOnPipe = true;
                        _onPipe = nextPiece;
                        _pipeMoveDirection = dir;
                    }
                }
            }
            else
            {
                Vector3 pos = transform.position;
                Vector3 targetPos = _onPipe.transform.position;
                Vector3 delta = targetPos - pos;
                Vector3 velocity = delta.normalized * pipeSpeed;

                Vector3 projectedInput = Vector3.Project(input, velocity);
                velocity += projectedInput * pipeSpeedModifier;

                pos = Vector3.MoveTowards(pos, targetPos, velocity.magnitude * Time.deltaTime);
                transform.position = pos;

                if (pos.Equals(targetPos))
                {
                    if (_onPipe.isPad)
                    {
                        _movingOnPipe = false;
                    }
                    else
                    {
                        bool foundNextPipe = false;
                        // Get the next pipe
                        for (int i = 0; i < 4; i++)
                        {
                            int dir = (_pipeMoveDirection + i) % 4;
                            if (i == 2 && _onPipe.GetPipeType != PipePiece.PipeType.END) 
                                continue;

                            PipePiece nextPiece = _onPipe.GetPipe(dir);
                            if (nextPiece != null)
                            {
                                foundNextPipe = true;
                                _onPipe = nextPiece;
                                _pipeMoveDirection = dir;
                                break;
                            }
                        }

                        if (!foundNextPipe)
                        {
                            _onPipe = null;
                            _movingOnPipe = false;
                            Debug.Log("Error? did not find a pipe piece");
                        }
                    }
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
            Vector3 targetVelocity = (input.Equals(Vector3.zero) ? Vector3.zero : input) * walkSpeed;
            _velocity = Vector3.MoveTowards(_velocity, targetVelocity, acceleration * Time.deltaTime);
            transform.position = transform.position + _velocity * Time.deltaTime;
        }
    }
}
