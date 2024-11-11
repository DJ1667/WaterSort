using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMono : MonoBehaviour
{
    public const float Height = 3.6f;
    public const float OriginY = 0.7f;

    ColorType _color;
    public ColorType Color => _color;

    public void Init(ColorType color)
    {
        _color = color;

        ChangeColor();
    }

    public void ChangeOrder(int order)
    {
        transform.Find("body").GetComponent<SpriteRenderer>().sortingOrder = order;
    }

    private void ChangeColor()
    {
        Color color = UnityEngine.Color.white;
        switch (_color)
        {
            case ColorType.None:
                break;
            case ColorType.Disable:
                break;
            case ColorType.绿:
                color = new Color(0, 1, 0);
                break;
            case ColorType.红:
                color = new Color(1, 0, 0);
                break;
            case ColorType.黄:
                color = new Color(1, 1, 0);
                break;
            case ColorType.深绿:
                color = new Color(0, 0.5f, 0);
                break;
            case ColorType.白:
                color = new Color(1, 1, 1);
                break;
            case ColorType.蓝:
                color = new Color(0.2f, 0.4f, 0f);
                break;
            case ColorType.深蓝:
                color = new Color(0.25f, 0.25f, 0f);
                break;
            case ColorType.橙色:
                color = new Color(1, 0.5f, 0);
                break;
            case ColorType.紫色:
                color = new Color(0.5f, 0, 0);
                break;
            case ColorType.粉色:
                color = new Color(1, 0, 0);
                break;
        }

        transform.Find("body").GetComponent<SpriteRenderer>().color = color;
    }
}
