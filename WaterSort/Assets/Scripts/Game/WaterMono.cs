using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMono : MonoBehaviour
{
    public Color32 Color;

    public void Init(Color32 color)
    {
        Color = color;
        transform.Find("WaterVisual").GetComponent<SpriteRenderer>().color = color;
    }
}
