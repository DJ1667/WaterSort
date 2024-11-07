using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

public class ExcelCodeCreater
{
    public static string CreateCodeStrByExcelData(ExcelData excelData)
    {
        if (excelData == null)
            return null;

        //开始生成类
        StringBuilder classSource = new StringBuilder();
        classSource.AppendLine("/*Auto Create, Don't Edit !!!*/");
        classSource.AppendLine();
        //添加引用
        classSource.AppendLine("using UnityEngine;");
        classSource.AppendLine("using System.Collections.Generic;");
        classSource.AppendLine("using System;");
        classSource.AppendLine("using System.IO;");
        classSource.AppendLine();
        classSource.AppendLine("[Serializable]");
        classSource.AppendLine($"public class {excelData.ClassName} : ScriptableObject");
        classSource.AppendLine("{");

        classSource.AppendLine("\tpublic void Init()");
        classSource.AppendLine("\t{");
        classSource.AppendLine("\t#InitContent");
        classSource.AppendLine("\t}");
        classSource.AppendLine();

        StringBuilder initContent = new StringBuilder();
        foreach (var kv in excelData._excelDataDict)
        {
            var sheetName = kv.Key;
            var sheetData = kv.Value;

            classSource.AppendLine(
                $"\tpublic List<{excelData.ExcelNameAfterSplit}_{sheetName}_Config> {sheetName}ConfigList = new List<{excelData.ExcelNameAfterSplit}_{sheetName}_Config>();");
            classSource.AppendLine();
            var idTypeStr = sheetData[0].Values.First().ToString();
            var idNameStr = sheetData[0].Keys.First().ToString();
            classSource.AppendLine(
                $"\tpublic Dictionary<{idTypeStr},{excelData.ExcelNameAfterSplit}_{sheetName}_Config> {sheetName}ConfigDict = new Dictionary<{idTypeStr},{excelData.ExcelNameAfterSplit}_{sheetName}_Config>();");
            classSource.AppendLine();
            //字段初始化方法
            initContent.AppendLine($"\t\t{sheetName}ConfigDict.Clear();");
            initContent.AppendLine($"\t\tif({sheetName}ConfigList != null && {sheetName}ConfigList.Count > 0)");
            initContent.AppendLine("\t\t{");
            initContent.AppendLine($"\t\t\tfor(int i = 0; i < {sheetName}ConfigList.Count; i++)");
            initContent.AppendLine("\t\t\t{");
            initContent.AppendLine(
                $"\t\t\t\t{sheetName}ConfigDict.Add({sheetName}ConfigList[i].{idNameStr}, {sheetName}ConfigList[i]);");
            initContent.AppendLine("\t\t\t}");
            initContent.AppendLine("\t\t}");
            initContent.AppendLine();
            //字典获取方法
            classSource.AppendLine(
                $"\tpublic {excelData.ExcelNameAfterSplit}_{sheetName}_Config Get{sheetName}_Config({idTypeStr} id)");
            classSource.AppendLine("\t{");
            classSource.AppendLine($"\t\tif({sheetName}ConfigDict.ContainsKey(id))");
            classSource.AppendLine($"\t\t\treturn {sheetName}ConfigDict[id];");
            classSource.AppendLine("\t\telse");
            classSource.AppendLine("\t\t\treturn null;");
            classSource.AppendLine("\t}");
        }

        classSource.Replace("#InitContent", initContent.ToString());

        foreach (var kv in excelData._excelDataDict)
        {
            var sheetName = kv.Key;
            var sheetData = kv.Value;

            classSource.AppendLine(CreateExcelConfigGetMethod(excelData, sheetName, sheetData));
        }

        classSource.AppendLine("}");

        foreach (var kv in excelData._excelDataDict)
        {
            var sheetName = kv.Key;
            var sheetData = kv.Value;

            classSource.AppendLine(CreateExcelConfigClass(excelData, sheetName, sheetData));
        }

        return classSource.ToString();
    }

    private static string CreateExcelConfigClass(ExcelData excelData, string sheetName,
        List<Dictionary<string, object>> sheetData)
    {
        //声明所有字段  （只取两行 类型和注释）
        //类型
        var typeList = sheetData[0].Values.ToList();
        //注释
        var tipsList = sheetData[1].Values.ToList();
        //属性名
        var propertyList = sheetData[0].Keys.ToList();

        StringBuilder classSource = new StringBuilder();
        classSource.AppendLine("[Serializable]");
        classSource.AppendLine($"public class {excelData.ExcelNameAfterSplit}_{sheetName}_Config");
        classSource.AppendLine("{");
        //========================字段========================
        for (int i = 0; i < propertyList.Count; i++)
        {
            //添加注释
            classSource.AppendLine("\t/// <summary>");
            classSource.AppendLine($"\t/// {tipsList[i]}");
            classSource.AppendLine("\t/// </summary>>");

            if (typeList[i].ToString().StartsWith("enum"))
            {
                var strAtt = typeList[i].ToString().Split('|');

                classSource.AppendLine(strAtt[0] == "enum"
                    ? $"\tpublic {strAtt[1]} {propertyList[i]};"
                    : $"\tpublic {strAtt[1]}[] {propertyList[i]};");
            }
            else
            {
                classSource.AppendLine($"\tpublic {typeList[i]} {propertyList[i]};");
            }
        }

        //========================字段========================
        classSource.AppendLine("}");
        return classSource.ToString();
    }

    private static string CreateExcelConfigGetMethod(ExcelData excelData, string sheetName,
        List<Dictionary<string, object>> sheetData)
    {
        //声明所有字段  （只取两行 类型和注释）
        //类型
        var typeList = sheetData[0].Values.ToList();
        //属性名
        var propertyList = sheetData[0].Keys.ToList();

        StringBuilder classSource = new StringBuilder();
        //======================方法==========================
        classSource.AppendLine("\t#region --- Get Method ---");

        for (int i = 1; i < propertyList.Count; i++)
        {
            string itemNameStr = propertyList[i].FirstOrDefault().ToString().ToUpper() + propertyList[i].Substring(1);

            if (typeList[i].ToString().StartsWith("enum"))
            {
                var strAtt = typeList[i].ToString().Split('|');

                classSource.AppendLine(strAtt[0] == "enum"
                    ? $"\tpublic {strAtt[1]} Get_{sheetName}_{itemNameStr}({typeList[0]} id)"
                    : $"\tpublic {strAtt[1]}[] Get_{sheetName}_{itemNameStr}({typeList[0]} id)");
            }
            else
            {
                classSource.AppendLine($"\tpublic {typeList[i]} Get_{sheetName}_{itemNameStr}({typeList[0]} id)");
            }

            classSource.AppendLine("\t{");
            classSource.AppendLine($"\t\tvar config = Get{sheetName}_Config(id);");
            classSource.AppendLine("\t\tif(config == null)");
            classSource.AppendLine("\t\t\treturn default;");
            classSource.AppendLine($"\t\treturn config.{propertyList[i]};");
            classSource.AppendLine("\t}");
        }

        classSource.AppendLine("\t#endregion");
        //======================方法==========================
        return classSource.ToString();
    }
}