using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour
{
    private bool _isOn;

    public bool IsOn
    {
        get { return _isOn; }
        set
        {
            _isOn = value;
            onSwitchChanged?.Invoke(_isOn);
            foreach (OnSwitchListener l in listeningPipes)
            {
                l.HandleOnSwitchChanged(_isOn);
            }
        }
    }

    public UnityEvent<bool> onSwitchChanged;
    public OnSwitchListener[] listeningPipes;

    [ContextMenu("Toggle Switch")]
    public void ToggleSwitch()
    {
        IsOn = !IsOn;
    }

    void Awake()
    {
        onSwitchChanged.AddListener(value =>
        {
            transform.GetChild(0).gameObject.SetActive(!value);
            transform.GetChild(1).gameObject.SetActive(value);
        });

        IsOn = false;

        /* Debug Code
        onSwitchChanged.AddListener(isOn => { Debug.Log($"Switch Toggled: {isOn}"); });
        /* End Debug Code */
    }
}
