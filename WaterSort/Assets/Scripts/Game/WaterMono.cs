using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMono : MonoBehaviour
{
    public const float Height = 3.6f;
    public const float OriginY = 0.7f;

    public int _order;
    public int Order => _order;

    [SerializeField]
    ColorType _color;
    public ColorType Color => _color;

    public void Init(ColorType color)
    {
        _color = color;

        ChangeColor();
    }

    public void ChangeOrder(int order)
    {
        _order = order;
        transform.Find("body").GetComponent<SpriteRenderer>().sortingOrder = order;
    }

    private void ChangeColor()
    {
        Color32 color = UnityEngine.Color.white;
        switch (_color)
        {
            case ColorType.None:
                break;
            case ColorType.Disable:
                break;
            case ColorType.绿:
                color = new Color32(34, 148, 83, 255);
                break;
            case ColorType.红:
                color = new Color32(238, 39, 70, 255);
                break;
            case ColorType.黄:
                color = new Color32(210, 180, 44, 255);
                break;
            case ColorType.深绿:
                color = new Color32(37, 61, 36, 255);
                break;
            case ColorType.白:
                color = new Color32(255, 255, 255, 255);
                break;
            case ColorType.蓝:
                color = new Color32(36, 134, 185, 255);
                break;
            case ColorType.深蓝:
                color = new Color32(21, 85, 154, 255);
                break;
            case ColorType.橙:
                color = new Color32(251, 164, 20, 255);
                break;
            case ColorType.紫:
                color = new Color32(128, 109, 158, 255);
                break;
            case ColorType.粉:
                color = new Color32(236, 155, 173, 255);
                break;
            case ColorType.黑:
                color = new Color32(0, 0, 0, 255);
                break;
            case ColorType.浅粉:
                color = new Color32(233, 204, 211, 255);
                break;
        }

        transform.Find("body").GetComponent<SpriteRenderer>().color = color;
    }
}
