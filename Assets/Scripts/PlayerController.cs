using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public int maxHealth;
    private int _health;
    public bool Dead { get; private set; }

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

    void Awake()
    {
        HealthDisplaySystem.DisplayHP(true);
        HealthDisplaySystem.SetMaxHP(maxHealth, true);
        _health = maxHealth;

        transform.Find("Dead").gameObject.SetActive(false);
    }

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

                    Vector2 displacement = pipe.transform.position - transform.position;

                    const float maxDistance = 1f;
                    if (displacement.sqrMagnitude <= maxDistance * maxDistance)
                    {

                        transform.position = pipe.transform.position;
                        _onPipe = pipe;
                        _velocity = Vector3.zero;
                        transform.Find("Body").rotation = Quaternion.AngleAxis(_shakeValue = 0, Vector3.forward);
                        
                        FindObjectOfType<CameraController>().AdjustZoomTOverTime(1, 0.75f);

                        IgnoreCollisionsWithWorld(true);

                        break;
                    }
                }
            }
            else
            {
                if (!_movingOnPipe)
                {
                    _onPipe = null;
                    FindObjectOfType<CameraController>().AdjustZoomTOverTime(0, 0.75f);

                    IgnoreCollisionsWithWorld(false);
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

                if (delta.sqrMagnitude <= 0.001f)
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
            transform.Find("Body").rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Movement code
            Vector3 targetVelocity = (input.Equals(Vector3.zero) ? Vector3.zero : input) * walkSpeed;
            _velocity = Vector3.MoveTowards(_velocity, targetVelocity, acceleration * Time.deltaTime);

            transform.position = transform.position + _velocity * Time.deltaTime;
        }
    }

    private void IgnoreCollisionsWithWorld(bool value)
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int wallLayer = LayerMask.NameToLayer("Walls");
        int waterLayer = LayerMask.NameToLayer("MyWater");

        Physics2D.IgnoreLayerCollision(playerLayer, wallLayer, value);
        Physics2D.IgnoreLayerCollision(playerLayer, waterLayer, value);
    }

    public void AdjustHealth(int value, Vector3 direction = default)
    {
        if (Dead)
            return;

        _health = Mathf.Clamp(_health + value, 0, maxHealth);
        HealthDisplaySystem.SetCurrentHP(_health);
        FindObjectOfType<CameraController>().ApplyCameraShake();

        if (_health == 0)
        {
            Dead = true;
            StartCoroutine(OnDead(direction));
        }
    }

    private IEnumerator OnDead(Vector3 direction)
    {
        Destroy(GetComponent<PlayerInput>());

        _onPipe = null;
        _movingOnPipe = false;

        transform.Find("Body").gameObject.SetActive(false);
        transform.Find("Dead").gameObject.SetActive(true);

        const float launchSpeed = 15f;
        const float friction = 50f;

        float speed = launchSpeed;

        int rotateDirection = Random.value > 0.5f ? 1 : -1;
        const float rotateSpeed = 180;

        while (true)
        {
            speed = Mathf.MoveTowards(speed, 0, friction * Time.deltaTime);
            if (speed <= 0)
                break;
            
            transform.position += direction * speed * Time.deltaTime;
            transform.Rotate(Vector3.forward, rotateSpeed * rotateDirection * Time.deltaTime);

            yield return null;
        }

        yield return new WaitForSeconds(2.25f);

        GameManager.ReloadLevel();
    }
}
