using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

public class DemoStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LevelController.Instance.LoadLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CheckBottle();
        }
    }

    #region 用户输入检测

    BottleMono _lastBottleMono = null; //上一个点击的瓶子

    public void CheckBottle()
    {
        // 2D中射线检测用户点击的位置是否在瓶子上
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1000);
        if (hit.collider != null)
        {
            var bottleMono = hit.collider.GetComponent<BottleMono>();
            if (bottleMono != null)
            {
                var lastBottleMono = _lastBottleMono;
                bottleMono.OnBottleClick(lastBottleMono);

                _lastBottleMono = bottleMono;
            }
        }
    }

    #endregion
}