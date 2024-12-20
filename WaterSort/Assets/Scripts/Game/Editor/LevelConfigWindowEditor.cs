using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class LevelConfigWindowEditor : OdinMenuEditorWindow
{
    const string PathLevel = "Assets/AssetBundle/ScriptableObjects/LevelConfig";

    [MenuItem("Tools/关卡编辑器")]
    private static void Open()
    {
        var window = GetWindow<LevelConfigWindowEditor>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
    }


    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree(true);
        tree.DefaultMenuStyle.IconSize = 28.00f;
        tree.Config.DrawSearchToolbar = true;

        // Adds the level overview table.
        tree.Add("LevelConfigs", new LevelConfigCreateByExcelData());
        // Adds all characters.
        tree.AddAllAssetsAtPath("LevelConfigs", PathLevel, typeof(LevelConfig), true, true).SortMenuItemsByName();
        return tree;
    }

    protected override void OnBeginDrawEditors()
    {
        var selected = this.MenuTree.Selection.FirstOrDefault();
        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            if (selected != null)
            {
                GUILayout.Label(selected.Name);
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("新建关卡配置")))
            {
                var newId = -1;
                var guids = AssetDatabase.FindAssets("t:LevelConfig", new string[] { PathLevel });

                for (int i = 0; i < guids.Length; i++)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    var name = DUtils.GetFileName(path);
                    var id = int.Parse(name.Replace("LevelConfig", ""));
                    if (id != i + 1)
                    {
                        newId = i + 1;
                        break;
                    }
                }

                if (newId == -1)
                    newId = guids.Length + 1;

                var obj = ScriptableObject.CreateInstance(typeof(LevelConfig)) as LevelConfig;
                var dest = $"Assets/AssetBundle/ScriptableObjects/LevelConfig/LevelConfig{newId}.asset";
                AssetDatabase.DeleteAsset(dest);
                AssetDatabase.CreateAsset(obj, dest);
                AssetDatabase.Refresh();
                Debug.Log($"新建关卡配置 ID: {newId}");

                base.TrySelectMenuItemWithObject(obj);
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("定位资源")))
            {
                var assetPos = selected.Value as Object;
                if (assetPos != null)
                {
                    Selection.activeObject = assetPos;
                    EditorGUIUtility.PingObject(Selection.activeObject);
                }
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }
}
