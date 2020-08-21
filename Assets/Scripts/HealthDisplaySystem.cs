using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplaySystem : MonoBehaviour
{
    public static HealthDisplaySystem instance;

    public Sprite emptyHeart;
    public Sprite halfHeart;
    public Sprite fullHeart;
    public Color heartColor;

    private int MaxHP { get; set; }
    private int CurrentHP { get; set; }


    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;

        MaxHP = 6;
        CurrentHP = MaxHP;

        UpdateRender();

        DisplayHP(false);
    }

    public static void DisplayHP(bool value)
    {
        instance.gameObject.SetActive(value);
        if (value)
            UpdateRender();
    }

    public static void SetMaxHP(int value, bool setHpToFull = false)
    {
        if (value % 2 != 0)
        {
            throw new ArgumentException($"value {{value}} must be an even number");
        }

        instance.MaxHP = value;

        if (setHpToFull)
            instance.CurrentHP = value;

        UpdateRender();
    }

    public static void AdjustCurrentHP(int value)
    {
        SetCurrentHP(instance.CurrentHP + value);
    }

    public static void SetCurrentHP(int value)
    {
        instance.CurrentHP = Mathf.Clamp(value, 0, instance.MaxHP);
        UpdateRender();
    }

    private static void UpdateRender()
    {
        for (int i = instance.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(instance.transform.GetChild(i).gameObject);
        }

        int heartCount = instance.MaxHP / 2;
        int filled = instance.CurrentHP / 2;
        bool halfHeart = instance.CurrentHP % 2 == 1;

        GameObject heartPiece = new GameObject("Heart Piece");

        for (int i = 0; i < heartCount; i++)
        {
            GameObject heartHolder = Instantiate(heartPiece, instance.transform);
            Image heartImage = heartHolder.AddComponent<Image>();
            heartImage.color = instance.heartColor;

            if (i < filled)
            {
                heartImage.sprite = instance.fullHeart;
            } 
            else if (i == filled && halfHeart)
            {
                heartImage.sprite = instance.halfHeart;
            }
            else
            {
                heartImage.sprite = instance.emptyHeart;
            }
        }

        Destroy(heartPiece);
    }

    public static int GetMaxHP()
    {
        return instance.MaxHP;
    }

    public static int GetCurrentHP()
    {
        return instance.CurrentHP;
    }
}
