using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoDisplaySystem : MonoBehaviour
{
    public static AmmoDisplaySystem instance;
    private TMP_Text Text { get; set; }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        Text = GetComponentInChildren<TMP_Text>();

        DisplayAmmo(false);
    }

    public static void DisplayAmmo(bool value)
    {
        instance.gameObject.SetActive(value);
    }

    public static void SetAmmo(int ammo)
    {
        instance.Text.text = $"Ammo: {Mathf.Clamp(ammo, 0, int.MaxValue):D2}";
    }
}