using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    private int _health;
    public UnityEvent OnDestroyEvent { get; set; } = new UnityEvent();

    void Awake()
    {
        _health = maxHealth;
    }

    public void AdjustHealth(int value)
    {
        _health = Mathf.Clamp(_health + value, 0, maxHealth);
        if (_health == 0)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
    }
}
