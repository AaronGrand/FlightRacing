using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefs
{
    public Color color;

    public void Load()
    {
        float r = PlayerPrefs.GetFloat("color.r", 1f);
        float g = PlayerPrefs.GetFloat("color.g", 1f);
        float b = PlayerPrefs.GetFloat("color.b", 1f);
        float a = PlayerPrefs.GetFloat("color.a", 1f);
        color = new Color(r, g, b, a);
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("color.r", color.r);
        PlayerPrefs.SetFloat("color.g", color.g);
        PlayerPrefs.SetFloat("color.b", color.b);
        PlayerPrefs.SetFloat("color.a", color.a);
    }

    public void SetColor(ref Material material)
    {
        material.color = color;
    }
}
