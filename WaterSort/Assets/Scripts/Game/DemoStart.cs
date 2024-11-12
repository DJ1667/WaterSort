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

    [ContextMenu("测试颜色随机打乱算法")]
    private void TestShuffleWithIntensity()
    {
        List<int> list1 = new List<int>();
        List<int> list2 = new List<int>();
        for (int i = 1; i <= 9; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                list1.Add(i);
                list2.Add(i);
            }
        }

        DUtils.ShuffleWithIntensity1(list1, 0.8f);
        DUtils.ShuffleWithIntensity2(list2, 0.8f);

        string str = "";
        for (int i = 0; i < list1.Count / 4; i++)
        {
            for (int j = i * 4; j < (i + 1) * 4; j++)
            {
                str += list1[j] + " ";
            }

            str += "    |    ";

            for (int j = i * 4; j < (i + 1) * 4; j++)
            {
                str += list2[j] + " ";
            }

            str += "\n";
        }

        Debug.Log(str);
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