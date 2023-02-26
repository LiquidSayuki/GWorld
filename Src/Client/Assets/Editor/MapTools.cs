using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using Common.Data;
using Assets.Scripts.Managers;

/// <summary>
/// 将场景中的传送点的位置保存到传送点定义文件中
/// </summary>
public class MapTools {

    [MenuItem("Map Tools/Export Teleporters")]
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }

        List<TeleporterObject> allTeleporters = new List<TeleporterObject>();

        foreach( var map in DataManager.Instance.Maps)
        {
            // 打开场景文件
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene {0} not existed", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            // 检查每一个场景中的传送点的ID，在表格中是否存在
            TeleporterObject[] teleporters = GameObject.FindObjectsOfType<TeleporterObject>();  
            foreach(var teleporter in teleporters)
            {
                if (!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID))
                {
                    EditorUtility.DisplayDialog("", string.Format("地图{0} 中配置的Teleproter[{1}]不正确", map.Value.ID, teleporter.ID), "确定");
                    return;
                }
                // 检查传送点对应的地图ID是否正确
                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.ID];
                if(def.MapID != map.Value.ID)
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图{0} 中配置的Teleproter[{1}]不正确",map.Value.ID, teleporter.ID), "确定");
                    return;
                }
                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
            }
        }
        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("提示", string.Format("传送点导出完成"), "确定");
    }
}
