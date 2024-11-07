using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

public class ExcelTools : EditorWindow
{
    private static string ExcelPath = "";

    /// <summary>
    /// Excel文件列表
    /// </summary>
    private static List<string> excelList;

    /// <summary>
    /// Excel文件列表 选中状态
    /// </summary>
    private static List<bool> excelSelectList;

    /// <summary>
    /// 滚动窗口初始位置
    /// </summary>
    private static Vector2 scrollPos;

    /// <summary>
    /// 输出格式索引
    /// </summary>
    private static int indexOfFormat = 0;

    /// <summary>
    /// 输出格式
    /// </summary>
    //private static string[] formatOption=new string[]{"JSON","CSV","XML"};
    private static string[] formatOption = new string[] {"Asset", "Json", "CSV", "Xml"};

    /// <summary>
    /// 编码索引
    /// </summary>
    private static int indexOfEncoding = 0;

    private static bool _haveAnyExcel = false; //是否存在Excel文件

    private static bool _isUseI2LLocalization = true;

    public static bool _isUseSanYiExcel = true; //是否使用叁一Excel格式（第一行写表名，需要忽略第一行）

    /// <summary>
    /// 显示当前窗口	
    /// </summary>
    [MenuItem("Tools/Excel转换工具")]
    static void ShowExcelTools()
    {
        var window = EditorWindow.GetWindow<ExcelTools>(true, "Excel数据转换", true);
        scrollPos = new Vector2(window.position.x, window.position.y + 75);
        Init();
        window.Show();
    }

    void OnEnable()
    {
        Init();
    }

    private static void Init()
    {
        //初始化
        _isUseI2LLocalization = EditorPrefs.GetBool("_isUseI2Localization", true);
        _isUseSanYiExcel = EditorPrefs.GetBool("_isUseSanYiExcel", true);

        excelList = new List<string>();
        excelSelectList = new List<bool>();
        LoadExcel();
    }

    /// <summary>
    /// 加载Excel 选中式
    /// </summary>
    private static void LoadExcel()
    {
        excelList.Clear();
        excelSelectList.Clear();

        ExcelPath = EditorPrefs.GetString("ExcelPath", "");
        if (string.IsNullOrEmpty(ExcelPath))
        {
            ExcelPath = Application.dataPath + "/Excel";
        }

        var path = ExcelPath;

        if (!Directory.Exists(path))
        {
            _haveAnyExcel = false;
            return;
        }

        string[] excelFileFullPaths = Directory.GetFiles(path, "*.xlsx");

        if (excelFileFullPaths == null || excelFileFullPaths.Length == 0)
        {
            _haveAnyExcel = false;
            return;
        }

        excelList.AddRange(excelFileFullPaths);
        for (int i = 0; i < excelList.Count; i++)
        {
            excelSelectList.Add(false);
        }
    }

    void OnGUI()
    {
        DrawOptions();
        DrawExport();
    }

    /// <summary>
    /// 绘制插件界面配置项
    /// </summary>
    private void DrawOptions()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Excel路径:", GUILayout.Width(80));
        EditorGUILayout.LabelField(ExcelPath, new GUIStyle("OL Ping"));
        if (GUILayout.Button("浏览", GUILayout.Width(100f)))
        {
            //param1：窗口标题
            //param2: 打开窗口 默认路径
            //param3：打开窗口 文件夹默认名称
            ExcelPath = EditorUtility.OpenFolderPanel("窗口标题", Application.dataPath, "");
            EditorPrefs.SetString("ExcelPath", ExcelPath);
            Init();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        var tempToggle = GUILayout.Toggle(_isUseSanYiExcel, "是否兼容第一行填表名的Excel");
        if (tempToggle != _isUseSanYiExcel)
        {
            _isUseSanYiExcel = tempToggle;
            EditorPrefs.SetBool("_isUseSanYiExcel", _isUseSanYiExcel);
        }

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("请选择格式类型:", GUILayout.Width(85));
        indexOfFormat = EditorGUILayout.Popup(indexOfFormat, formatOption, GUILayout.Width(125));

        GUILayout.Space(200);
        if (GUILayout.Button("刷新", GUILayout.Height(25)))
        {
            Init();
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.EndVertical();
    }

    /// <summary>
    /// 绘制插件界面输出项
    /// </summary>
    private void DrawExport()
    {
        if (excelList == null) return;
        if (excelList.Count < 1)
        {
            EditorGUILayout.LabelField($"没有检测到Excel文件哦!  --->   检查Excel路径是否正确 （{ExcelPath}）");
        }
        else
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("检测到Excel如下：");
            if (GUILayout.Button("全选", GUILayout.Height(25)))
            {
                for (int i = 0; i < excelSelectList.Count; i++)
                {
                    excelSelectList[i] = true;
                }
            }

            if (GUILayout.Button("全不选", GUILayout.Height(25)))
            {
                for (int i = 0; i < excelSelectList.Count; i++)
                {
                    excelSelectList[i] = false;
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(250));

            for (int i = 0; i < excelList.Count; i++)
            {
                var s1 = Path.GetFileName(excelList[i]);
                var s2 = $"   |   全路径->   |   ";
                var s = s1 + s2 + excelList[i];
                GUILayout.BeginHorizontal();
                excelSelectList[i] = GUILayout.Toggle(excelSelectList[i], s);
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.Space(10);

            //输出
            GUILayout.BeginVertical();

            if (indexOfFormat == 0)
            {
                GUILayout.Box("转换成Asset要先生成CS脚本哦", "MeTransitionBlock", GUILayout.MinWidth(200), GUILayout.Height(50));

                var tempToggle = GUILayout.Toggle(_isUseI2LLocalization, "多语言->是否使用I2LLocalization插件");
                if (tempToggle != _isUseI2LLocalization)
                {
                    _isUseI2LLocalization = tempToggle;
                    EditorPrefs.SetBool("_isUseI2Localization", _isUseI2LLocalization);
                }

                GUILayout.Space(10);
            }

            GUILayout.BeginHorizontal();

            switch (indexOfFormat)
            {
                case 0:
                    if (GUILayout.Button("生成C#脚本", GUILayout.Height(50)))
                    {
                        Convert("cs");
                    }

                    if (GUILayout.Button("转换为Asset", GUILayout.Height(50)))
                    {
                        Convert("asset");
                    }

                    break;
                case 1:
                    if (GUILayout.Button("转换为Json", GUILayout.Height(50)))
                    {
                        Convert("json");
                    }

                    break;
                case 2:
                    if (GUILayout.Button("转换为CSV", GUILayout.Height(50)))
                    {
                        Convert("csv");
                    }

                    break;
                case 3:
                    if (GUILayout.Button("转换为Xml", GUILayout.Height(50)))
                    {
                        Convert("xml");
                    }

                    break;
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }

    private static void Convert(string type)
    {
        for (int i = 0; i < excelList.Count; i++)
        {
            if (!excelSelectList[i]) continue;

            var excelPath = excelList[i];

            //构造Excel工具类
            ExcelUtilityEx excelUtilityEx = new ExcelUtilityEx(excelPath);

            //判断输出类型
            switch (type)
            {
                case "cs":
                    excelUtilityEx.ConvertToCS(excelPath);
                    break;
                case "asset":
                    if (_isUseI2LLocalization && Path.GetFileNameWithoutExtension(excelPath) == "Language")
                        excelUtilityEx.ConvertToI2LocalizationCSV(excelPath);
                    else
                        excelUtilityEx.ConvertToAsset(excelPath);
                    break;
                case "json":
                    excelUtilityEx.ConvertToJson(excelPath);
                    break;
                case "csv":
                    excelUtilityEx.ConvertToCSV(excelPath);
                    break;
                case "xml":
                    excelUtilityEx.ConvertToXml(excelPath);
                    break;
            }
        }

        //刷新本地资源
        AssetDatabase.Refresh();
    }
}