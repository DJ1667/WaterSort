using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Excel;
using I2.Loc;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Excel数据处理类
/// 支持类型 -> 表中定义格式
/// "string" -> string
/// "bool" -> bool
/// "byte" -> byte
/// "int" -> int
/// "short" -> short
/// "long" -> long
/// "float" -> float
/// "double" -> double
/// "Vector2" -> Vector2
/// "Vector3" -> Vector3
/// "Color" -> Color
/// "Color32" -> Color32
/// "string[]" -> string[]
/// "bool[]" -> bool[]
/// "byte[]" -> byte[]
/// "int[]" -> int[]
/// "short[]" -> short[]
/// "long[]" -> long[]
/// "float[]" -> float[]
/// "double[]" -> double[]
/// "Vector2[]" -> Vector2[]
/// "Vector3[]" -> Vector3[]
/// "Color[]" -> Color[]
/// "Color32[]" -> Color32[]
/// "enum" ->  "enum|xxx" xxx指的是你的枚举类型               ------------------需要注意的特殊类型-----------------------
/// "enum[]" ->  "enum[]|xxx" xxx指的是你的枚举类型           ------------------需要注意的特殊类型-----------------------
/// </summary>
public class ExcelUtilityEx
{
    private const string CScriptPath = "Assets/Scripts/Config";
    private const string AssetPath = "Assets/Resources/Data/AssetConfig";
    private const string JsonPath = "Assets/Resources/Data/Json";
    private const string CSVPath = "Assets/Resources/Data/CSV";
    private const string XmlPath = "Assets/Resources/Data/Xml";
    private const string TxtPath = "Assets/Font/Txt";

    private ExcelData _excelData;

    public ExcelUtilityEx(string excelFile)
    {
        _excelData = new ExcelData();
        FileStream mStream = File.Open(excelFile, FileMode.Open, FileAccess.Read);
        IExcelDataReader mExcelReader = ExcelReaderFactory.CreateOpenXmlReader(mStream);
        _excelData.resultSet = mExcelReader.AsDataSet();
    }

    #region Excel合法性检查和数据读取

    private bool InitExcelDataForI2L(string excelPath)
    {
        CompleteExcelData(excelPath);

        if (!CheckExcelIsLegal(excelPath)) return false;

        for (int t = 0; t < _excelData.resultSet.Tables.Count; t++)
        {
            //默认读取第一个数据表
            DataTable mSheet = _excelData.resultSet.Tables[t];

            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1)
            {
                DebugEx(DebugType.LogWarning, $"{_excelData.ExcelName} - {mSheet.TableName} 表中不存在数据");
                continue;
            }

            if (!CheckSheetIsLegal(mSheet, false)) continue;

            //读取数据表行数和列数
            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            //准备一个列表存储整个表的数据
            List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

            //读取数据
            for (int i = 1; i < rowCount; i++)
            {
                //舍弃掉id为空的行
                if (i > 2)
                {
                    if (mSheet.Rows[i][0] == null || string.IsNullOrEmpty(mSheet.Rows[i][0].ToString()))
                    {
                        DebugEx(DebugType.LogWarning,
                            $"{_excelData.ExcelName} - {mSheet.TableName}  第{i + 1}行第1列 为空,id不能为空,已在结果中舍去这一行");
                        continue;
                    }
                }

                //准备一个字典存储每一行的数据
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int j = 0; j < colCount; j++)
                {
                    //读取第1行数据作为表头字段
                    string field = mSheet.Rows[0][j].ToString();
                    //忽略空的列
                    if (string.IsNullOrEmpty(field)) continue;

                    //Key-Value对应
                    if (row.ContainsKey(field))
                    {
                        DebugEx(DebugType.LogWarning,
                            $"{_excelData.ExcelName} - {mSheet.TableName}   {field} 第1行第" + (j + 1) +
                            "列 属性字段名重复,已在结果中舍去这一列");
                    }
                    else
                    {
                        row[field] = mSheet.Rows[i][j];
                    }
                }

                //添加到表数据中
                table.Add(row);
            }

            _excelData._excelDataDict.Add(mSheet.TableName, table);
        }

        return true;
    }

    private bool InitExcelData(string excelPath)
    {
        CompleteExcelData(excelPath);

        if (!CheckExcelIsLegal(excelPath)) return false;

        for (int t = 0; t < _excelData.resultSet.Tables.Count; t++)
        {
            //默认读取第一个数据表
            DataTable mSheet = _excelData.resultSet.Tables[t];

            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1)
            {
                DebugEx(DebugType.LogError, $"{_excelData.ExcelName} - {mSheet.TableName} 表中不存在数据");
                continue;
            }

            if (!CheckSheetIsLegal(mSheet)) continue;

            //读取数据表行数和列数
            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            //准备一个列表存储整个表的数据
            List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

            int titleRow = ExcelTools._isUseSanYiExcel ? 1 : 0;
            int typeRow = ExcelTools._isUseSanYiExcel ? 2 : 1;
            int tipsRow = ExcelTools._isUseSanYiExcel ? 3 : 2;

            //读取数据
            for (int i = typeRow; i < rowCount; i++)
            {
                //舍弃掉id为空的行
                if (i > tipsRow)
                {
                    if (mSheet.Rows[i][0] == null || string.IsNullOrEmpty(mSheet.Rows[i][0].ToString()))
                    {
                        DebugEx(DebugType.LogWarning,
                            $"{_excelData.ExcelName} - {mSheet.TableName}  第{i + 1}行第1列 为空,id不能为空,已在结果中舍去这一行");
                        continue;
                    }
                }

                //准备一个字典存储每一行的数据
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int j = 0; j < colCount; j++)
                {
                    //读取第1行数据作为表头字段
                    string field = mSheet.Rows[titleRow][j].ToString();
                    //忽略空的列
                    if (string.IsNullOrEmpty(field)) continue;

                    //Key-Value对应
                    if (row.ContainsKey(field))
                    {
                        DebugEx(DebugType.LogWarning,
                            $"{_excelData.ExcelName} - {mSheet.TableName}   {field} 第1行第" + (j + 1) +
                            "列 属性字段名重复,已在结果中舍去这一列");
                    }
                    else
                    {
                        row[field] = mSheet.Rows[i][j];
                    }
                }

                //添加到表数据中
                table.Add(row);
            }

            _excelData._excelDataDict.Add(mSheet.TableName, table);
        }

        return true;
    }

    private bool CheckExcelIsLegal(string excelPath)
    {
        var excelName = GetFileName(excelPath);

        //检查合法性
        if (!Regex.IsMatch(excelName, "^[A-Za-z_][A-Za-z_0-9]+$"))
        {
            DebugEx(DebugType.LogError, $"{excelName}   Excel表 命名不规范，只能包含字母数字和下划线且不能以数字开头");
            return false;
        }

        //判断Excel文件中是否存在数据表
        if (_excelData.resultSet.Tables.Count < 1)
        {
            DebugEx(DebugType.LogError, "Excel中不存在数据表");
            return false;
        }

        return true;
    }

    private bool CheckSheetIsLegal(DataTable mSheet, bool checkPropertyName = true)
    {
        int colCount = mSheet.Columns.Count; //列数

        //检查表的第一行是否符合规范
        int titleRow = ExcelTools._isUseSanYiExcel ? 1 : 0;
        for (int i = 0; i < colCount; i++)
        {
            var title = mSheet.Rows[titleRow][i].ToString();
            //忽略空的列
            if (string.IsNullOrEmpty(title))
            {
                DebugEx(DebugType.LogWarning,
                    $"{_excelData.ExcelName} - {mSheet.TableName}    第1行第{(i + 1)}列的字段为空,已在结果中舍去这一列");
                continue;
            }

            if (checkPropertyName)
            {
                //检查合法性
                if (!Regex.IsMatch(title, "^[A-Za-z_][A-Za-z_0-9]+$"))
                {
                    DebugEx(DebugType.LogError,
                        $"{_excelData.ExcelName} - {mSheet.TableName} 表被剔除 ------>  {title}   第1行第{(i + 1)}列的字段命名不规范，只能包含字母数字和下划线且不能以数字开头");
                    return false;
                }
            }
        }

        int typeRow = ExcelTools._isUseSanYiExcel ? 2 : 1;
        //处理表的第二行  定义变量的类型
        for (int i = 0; i < colCount; i++)
        {
            var strTemp = mSheet.Rows[typeRow][i].ToString();
            var strType = strTemp;

            //忽略的列剔除
            var title = mSheet.Rows[titleRow][i].ToString();
            if (string.IsNullOrEmpty(title))
            {
                continue;
            }

            if (strType.StartsWith("enum"))
            {
                var strArr = strType.Split('|');
                var enumType = strArr[1];

                Assembly assembly = Assembly.Load("Assembly-CSharp");
                var t = assembly.GetType(enumType);
                if (!_excelData._enumTypeDict.ContainsKey(strType))
                    _excelData._enumTypeDict.Add(strType, t);

                if (t == null || !t.IsEnum)
                {
                    DebugEx(DebugType.LogError,
                        $"{_excelData.ExcelName} - {mSheet.TableName} 表被剔除 -------> 数据类型不合法 ====== 枚举不存在 ====== 第2行 第{i + 1}列 内容:{strType}");
                    return false;
                }
            }
            else
            {
                switch (strType)
                {
                    case "string":
                    case "bool":
                    case "byte":
                    case "int":
                    case "short":
                    case "long":
                    case "float":
                    case "double":
                    case "Vector2":
                    case "Vector3":
                    case "Color":
                    case "Color32":
                    case "string[]":
                    case "bool[]":
                    case "byte[]":
                    case "int[]":
                    case "short[]":
                    case "long[]":
                    case "float[]":
                    case "double[]":
                    case "Vector2[]":
                    case "Vector3[]":
                    case "Color[]":
                    case "Color32[]":
                        break;
                    default:
                        DebugEx(DebugType.LogError,
                            $"{_excelData.ExcelName} - {mSheet.TableName} 表被剔除 -------> 数据类型不合法 ============ 第2行 第{i + 1}列 内容:{strType}");
                        return false;
                }
            }
        }

        return true;
    }

    private string GetFileName(string path)
    {
        path = Path.GetFileNameWithoutExtension(path);
        var tempIndex = path.LastIndexOf("/");
        var fileName = path.Substring(tempIndex + 1);

        return fileName;
    }

    private void CompleteExcelData(string path)
    {
        var excelName = GetFileName(path);
        _excelData.ExcelName = excelName;
        var strArr = excelName.Split('_');
        _excelData.ExcelNameAfterSplit = strArr[0];
        _excelData.ClassName = strArr[0] + "_ExcelData";
        _excelData.AssetName = excelName + "_ExcelData";
        _excelData.ExcelPath = path;
    }

    #endregion

    #region 转换为Json

    public void ConvertToJson(string path)
    {
        if (!InitExcelData(path)) return;

        //生成Json字符串
        string json = JsonConvert.SerializeObject(_excelData._excelDataDict, Formatting.Indented);

        var fileName = GetFileName(path);
        WriteStringToFile(JsonPath, json, fileName + ".json");

        DebugEx(DebugType.Log, $"{_excelData.ExcelName}  生成Json成功");
    }

    #endregion

    #region 转换为Asset

    public void ConvertToCS(string path)
    {
        if (!InitExcelData(path)) return;

        var csStr = ExcelCodeCreater.CreateCodeStrByExcelData(_excelData);

        var fileName = _excelData.ClassName + ".cs";
        WriteStringToFile(CScriptPath, csStr, fileName);

        DebugEx(DebugType.Log, $"{_excelData.ExcelName}  生成CS脚本成功");
    }

    public void ConvertToAsset(string path)
    {
        if (!InitExcelData(path)) return;

        Assembly assembly = Assembly.Load("Assembly-CSharp");

        var type = assembly.GetType(_excelData.ClassName);
        if (type == null)
        {
            DebugEx(DebugType.LogError, $"{_excelData.ExcelName}   获取配置类出错,检查是否生成了脚本");
            return;
        }

        //ScriptableObject
        var sto = (UnityEngine.Object)Activator.CreateInstance(type);

        foreach (var kv in _excelData._excelDataDict)
        {
            var sheetName = kv.Key;
            var sheetData = kv.Value;

            //每张表中数据的数据类
            var configClassName = $"{_excelData.ExcelNameAfterSplit}_{sheetName}_Config";
            var classType = assembly.GetType(configClassName);

            //存放数据的list
            var listName = $"{sheetName}ConfigList";
            var listInfo = sto.GetType().GetField(listName);
            var listType = listInfo.FieldType;
            var listObj = Activator.CreateInstance(listType);
            var methodInfo = listType.GetMethod("Add");

            //类型
            var typeList = sheetData[0].Values.ToList();
            //属性名
            var propertyList = sheetData[0].Keys.ToList();

            //从数值行开始
            for (int i = 2; i < sheetData.Count; i++)
            {
                //创建一个class实例 表中一行的数据
                var classObj = Activator.CreateInstance(classType);

                for (int j = 0; j < propertyList.Count; j++)
                {
                    var propertyName = propertyList[j];
                    var fieldInfo = classType.GetField(propertyName);
                    SetObjectFiled(classObj, fieldInfo, typeList[j].ToString(), sheetData[i][propertyName].ToString());
                }

                //将class实例添加到List中
                methodInfo.Invoke(listObj, new object[] { classObj });
            }

            listInfo.SetValue(sto, listObj);
        }

        var assetPath = Application.dataPath.Replace("Assets", "/") + AssetPath;
        if (!Directory.Exists(assetPath))
        {
            Directory.CreateDirectory(assetPath);
        }

        AssetDatabase.CreateAsset(sto, AssetPath + "/" + _excelData.AssetName + ".asset");
        DebugEx(DebugType.Log, $"{_excelData.ExcelName}  生成Asset成功");
    }

    void SetObjectFiled(object obj, FieldInfo field, string type, string param)
    {
        if (param == null) param = "";

        if (type.StartsWith("enum"))
        {
            var strArr = type.Split('|');
            var enumType = strArr[1];

            Type t = null;
            if (_excelData._enumTypeDict.ContainsKey(type))
                t = _excelData._enumTypeDict[type];
            else
            {
                Assembly assembly = Assembly.Load("Assembly-CSharp");
                t = assembly.GetType(enumType);
            }

            if (strArr[0] == "enum")
            {
                field.SetValue(obj, DUtils.ParseToEnum(t, param));
            }
            else if (strArr[0] == "enum[]")
            {
                field.SetValue(obj, DUtils.ParseToArrayEnum(t, param));
            }
            else
            {
                DebugEx(DebugType.Log, $"枚举类型不对 ----- {strArr[0]} ------");
            }
        }
        else
        {
            switch (type)
            {
                case "string":
                    field.SetValue(obj, param);
                    break;
                case "bool":
                    field.SetValue(obj, DUtils.ParseToBool(param));
                    break;
                case "byte":
                    field.SetValue(obj, DUtils.ParseToByte(param));
                    break;
                case "int":
                    field.SetValue(obj, DUtils.ParseToInt(param));
                    break;
                case "short":
                    field.SetValue(obj, DUtils.ParseToShort(param));
                    break;
                case "long":
                    field.SetValue(obj, DUtils.ParseToLong(param));
                    break;
                case "float":
                    field.SetValue(obj, DUtils.ParseToFloat(param));
                    break;
                case "double":
                    field.SetValue(obj, DUtils.ParseToDouble(param));
                    break;
                case "Vector2":
                    field.SetValue(obj, DUtils.ParseToVector2(param));
                    break;
                case "Vector3":
                    field.SetValue(obj, DUtils.ParseToVector3(param));
                    break;
                case "Color":
                    field.SetValue(obj, DUtils.ParseToColor(param));
                    break;
                case "Color32":
                    field.SetValue(obj, DUtils.ParseToColor32(param));
                    break;
                case "string[]":
                    field.SetValue(obj, DUtils.ParseToArrayString(param));
                    break;
                case "bool[]":
                    field.SetValue(obj, DUtils.ParseToArrayBool(param));
                    break;
                case "byte[]":
                    field.SetValue(obj, DUtils.ParseToArrayByte(param));
                    break;
                case "int[]":
                    field.SetValue(obj, DUtils.ParseToArrayInt(param));
                    break;
                case "short[]":
                    field.SetValue(obj, DUtils.ParseToArrayShort(param));
                    break;
                case "long[]":
                    field.SetValue(obj, DUtils.ParseToArrayLong(param));
                    break;
                case "float[]":
                    field.SetValue(obj, DUtils.ParseToArrayFloat(param));
                    break;
                case "double[]":
                    field.SetValue(obj, DUtils.ParseToArrayDouble(param));
                    break;
                case "Vector2[]":
                    field.SetValue(obj, DUtils.ParseToArrayVector2(param));
                    break;
                case "Vector3[]":
                    field.SetValue(obj, DUtils.ParseToArrayVector3(param));
                    break;
                case "Color[]":
                    field.SetValue(obj, DUtils.ParseToArrayColor(param));
                    break;
                case "Color32[]":
                    field.SetValue(obj, DUtils.ParseToArrayColor32(param));
                    break;
                default:
                    Assembly assembly = Assembly.Load("Assembly-CSharp");
                    var t = assembly.GetType(type);
                    if (t != null)
                    {
                        if (t.IsEnum)
                        {
                            field.SetValue(obj, Enum.Parse(t, param));
                        }
                    }
                    else
                    {
                        field.SetValue(obj, param);
                    }

                    break;
            }
        }
    }

    #endregion

    #region 转换为CSV

    public void ConvertToCSV(string path)
    {
        if (!InitExcelData(path)) return;

        foreach (var kv in _excelData._excelDataDict)
        {
            var sheetName = kv.Key;
            var sheetData = kv.Value;

            StringBuilder stringBuilder = new StringBuilder();

            //处理属性 第一行
            foreach (var key in sheetData[0].Keys)
            {
                stringBuilder.Append(key + ",");
            }

            stringBuilder.Append("\r\n");

            //处理字段 其余行
            for (int i = 0; i < sheetData.Count; i++)
            {
                var data = sheetData[i];

                foreach (var val in data.Values)
                {
                    stringBuilder.Append(val + ",");
                }

                stringBuilder.Append("\r\n");
            }

            //写入文件
            WriteStringToFile(CSVPath, stringBuilder.ToString(), _excelData.AssetName + "_" + sheetName + ".csv");

            DebugEx(DebugType.Log, $"{_excelData.ExcelName}_{sheetName}  生成CSV成功");
        }
    }

    #endregion

    #region 转换为XML

    public void ConvertToXml(string path)
    {
        if (!InitExcelData(path)) return;

        StringBuilder stringBuilder = new StringBuilder();
        //创建Xml文件头
        stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        stringBuilder.Append("\r\n");

        foreach (var kv in _excelData._excelDataDict)
        {
            var sheetName = kv.Key;
            var sheetData = kv.Value;

            //创建根节点
            stringBuilder.Append($"<{sheetName}>");
            stringBuilder.Append("\r\n");

            //第一行 属性名
            var nameList = sheetData[0].Keys.ToList();

            //处理字段 其余行
            for (int i = 0; i < sheetData.Count; i++)
            {
                var dataList = sheetData[i].Values.ToList();

                //创建子节点
                stringBuilder.Append($"  <Row{i - 1}>");
                stringBuilder.Append("\r\n");

                for (int j = 0; j < dataList.Count; j++)
                {
                    stringBuilder.Append("   <" + nameList[j] + ">");
                    stringBuilder.Append(dataList[j]);
                    stringBuilder.Append("</" + nameList[j] + ">");
                    stringBuilder.Append("\r\n");
                }

                //使用换行符分割每一行
                stringBuilder.Append($"  </Row{i - 1}>");
                stringBuilder.Append("\r\n");
            }

            //闭合标签
            stringBuilder.Append($"</{sheetName}>");
            stringBuilder.Append("\r\n");
        }

        WriteStringToFile(XmlPath, stringBuilder.ToString(), _excelData.AssetName + ".xml");
        DebugEx(DebugType.Log, $"{_excelData.ExcelName} 生成Xml成功");
    }

    #endregion

    #region 转换为多语言表->I2Localization插件专用 //语言表名字必须是Language

    public void ConvertToI2LocalizationCSV(string path)
    {
        if (!InitExcelDataForI2L(path)) return;

        //读第一张表
        var sheetName = _excelData.resultSet.Tables[0].TableName;
        var sheetData = _excelData._excelDataDict[sheetName];

        StringBuilder stringBuilder = new StringBuilder();

        Dictionary<string, StringBuilder> languageDict = new Dictionary<string, StringBuilder>();

        //处理属性 第一行
        var idNameList = sheetData[0].Keys.ToList();
        List<string> cnNameList = sheetData[1].Values.Select(x => x as string ?? "").ToList(); //第三行，各种语言的中文备注
        for (int i = 0; i < idNameList.Count; i++)
        {
            if (i == 0)
            {
                stringBuilder.Append("Key" + "\t");
                stringBuilder.Append("Type" + "\t");
                stringBuilder.Append("Desc" + "\t");
            }
            else
            {
                stringBuilder.Append(idNameList[i] + "\t");
                languageDict.Add(cnNameList[i], new StringBuilder());
            }
        }

        stringBuilder.Append("\r\n");

        //处理字段 其余行 (排除类型行和注释航行)
        for (int i = 2; i < sheetData.Count; i++)
        {
            var data = sheetData[i];

            var valList = data.Values.ToList();
            for (int j = 0; j < valList.Count; j++)
            {
                if (j == 0)
                {
                    stringBuilder.Append(valList[j] + "\t");
                    stringBuilder.Append("Text" + "\t");
                    stringBuilder.Append("" + "\t");
                }
                else
                {
                    stringBuilder.Append(valList[j] + "\t");
                    languageDict[cnNameList[j]].Append(valList[j]);
                }
            }

            stringBuilder.Append("\r\n");
        }

        //写入文件
        WriteStringToFile(TxtPath, stringBuilder.ToString(), _excelData.ExcelName + "_" + sheetName + ".txt");

        //每个语言的txt文件
        foreach (var item in languageDict)
        {
            WriteStringToFile(TxtPath, item.Value.ToString(), item.Key + ".txt");
        }

        DebugEx(DebugType.Log, $"{_excelData.ExcelName}_{sheetName}  生成CSV成功");

        var fullPath = TxtPath + "/" + _excelData.ExcelName + "_" + sheetName + ".txt";
        ToI2Localization(fullPath);
    }


    private void ToI2Localization(string filePath)
    {
        EditorUtility.DisplayProgressBar("多语言程序", "开干!!! (ง •_•)ง", 0f);
        var CurrentExtension = LocalizationEditor.eLocalSpreadsheeet.CSV;
        var UpdateMode = eSpreadsheetUpdateMode.Replace;
        filePath = Application.dataPath.Replace("Assets", filePath);
        try
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                switch (CurrentExtension)
                {
                    case LocalizationEditor.eLocalSpreadsheeet.CSV:
                        Import_CSV(filePath, UpdateMode);
                        break;
                }
            }
            else
            {
                DebugEx(DebugType.LogError, $"路径错误：{filePath}");
            }
        }
        catch (System.Exception ex)
        {
            DebugEx(DebugType.LogError, ex.Message);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    private void Import_CSV(string FileName, eSpreadsheetUpdateMode mode)
    {
        EditorUtility.DisplayProgressBar("多语言程序", "正在转换多语言文件", 0.3f);

        LanguageSourceAsset sourceAsset = Resources.Load<LanguageSourceAsset>(LocalizationManager.GlobalSources[0]);
        var source = sourceAsset.mSource;

        EditorUtility.SetDirty(sourceAsset);

        var encoding = System.Text.Encoding.UTF8;
        string CSVstring = LocalizationReader.ReadCSVfile(FileName, encoding);
        EditorUtility.DisplayProgressBar("多语言程序", "CSV读表完成", 0.6f);
        char Separator = '\t';
        string sError = source.Import_CSV(string.Empty, CSVstring, mode, Separator);

        if (!string.IsNullOrEmpty(sError))
        {
            DebugEx(DebugType.LogError, sError);
        }
        else
        {
            //自定义需求
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = sourceAsset;
            AssetDatabase.Refresh();
        }

        EditorUtility.DisplayProgressBar("多语言程序", "转换多语言文件完成", 0.9f);
    }

    #endregion

    #region 文件操作

    private void WriteStringToFile(string savePath, string content, string fileName)
    {
        var path = Application.dataPath.Replace("Assets", "") + savePath;
        var fullFilePath = path + "/" + fileName;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //写入文件
        if (File.Exists(fullFilePath))
        {
            DebugEx(DebugType.Log, fullFilePath + "    文件已存在，将被替换");
        }
        else
        {
            DebugEx(DebugType.Log, fullFilePath + "    创建文件");
        }

        //补充 using(){} ()中的对象必须继承IDispose接口,在{}结束后会自动释放资源,也就是相当于帮你调用了Dispos()去释放资源
        using (FileStream fileStream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
        {
            using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
            {
                textWriter.Write(content);
            }
        }
    }

    #endregion

    #region 日志

    void DebugEx(DebugType type, string message)
    {
        switch (type)
        {
            case DebugType.Log:
                Debug.Log("ExcelConvert: " + message);
                break;
            case DebugType.LogWarning:
                Debug.LogWarning("ExcelConvert: " + message);
                break;
            case DebugType.LogError:
                Debug.LogError("ExcelConvert: " + message);
                break;
            default:
                break;
        }
    }

    enum DebugType
    {
        Log,
        LogWarning,
        LogError
    }

    #endregion

    #region 额外的

    public static string ExcelToJsonStr(string path)
    {
        FileStream mStream = File.Open(path, FileMode.Open, FileAccess.Read);
        IExcelDataReader mExcelReader = ExcelReaderFactory.CreateOpenXmlReader(mStream);
        var temp_mResultSet = mExcelReader.AsDataSet();

        //判断Excel文件中是否存在数据表
        if (temp_mResultSet.Tables.Count < 1)
        {
            Debug.LogError("Excel中不存在数据表");
            return "";
        }

        //存储整个Excel的数据
        Dictionary<string, object> excel = new Dictionary<string, object>();

        //遍历所有的表
        for (int t = 0; t < temp_mResultSet.Tables.Count; t++)
        {
            //默认读取第一个数据表
            DataTable mSheet = temp_mResultSet.Tables[t];

            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1)
            {
                Debug.LogError(mSheet.TableName + "表中不存在数据");
                continue;
            }

            //读取数据表行数和列数
            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            //检查表的第一行是否符合规范
            bool isLegal = true;
            for (int i = 0; i < colCount; i++)
            {
                var title = mSheet.Rows[0][i].ToString();
                //忽略空的列
                if (string.IsNullOrEmpty(title))
                {
                    Debug.LogError("第1行第" + (i + 1) + "列的字段为空,已在结果中舍去这一列");
                    continue;
                }

                //检查合法性
                if (!Regex.IsMatch(title, "^[A-Za-z_][A-Za-z_0-9]+$"))
                {
                    Debug.LogError(title + "   第1行第" + (i + 1) + "列的字段命名不规范，只能包含字母数字和下划线且不能以数字开头");
                    isLegal = false;
                    break;
                }
            }

            if (!isLegal) continue;

            //准备一个列表存储整个表的数据
            List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

            //读取数据
            for (int i = 1; i < rowCount; i++)
            {
                //准备一个字典存储每一行的数据
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int j = 0; j < colCount; j++)
                {
                    //读取第1行数据作为表头字段
                    string field = mSheet.Rows[0][j].ToString();
                    //忽略空的列
                    if (string.IsNullOrEmpty(field)) continue;

                    //Key-Value对应
                    if (row.ContainsKey(field))
                    {
                        Debug.LogError(field + " 第1行第" + (j + 1) + "列 属性字段名重复,已在结果中舍去这一列");
                    }
                    else
                    {
                        row[field] = mSheet.Rows[i][j];
                    }
                }

                //添加到表数据中
                table.Add(row);
            }

            excel.Add(mSheet.TableName, table);
        }

        //生成Json字符串
        string json = JsonConvert.SerializeObject(excel, Formatting.Indented);

        return json;
    }

    #endregion
}