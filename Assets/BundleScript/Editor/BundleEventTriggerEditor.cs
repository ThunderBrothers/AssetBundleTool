using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using static BundleEventTrigger;

[CustomEditor(typeof(BundleEventTriggerDesigner))]
public class BundleEventTriggerEditor : Editor
{
    ReorderableList eventInfo;
    private bool isMethodVoid = false;
    //构建目标物体以修改事件

    private bool init = false;

    private void OnEnable() {
        SetupBundleEventTriggerList();
    }


    /// <summary>
    /// 显示List界面
    /// </summary>
    void SetupBundleEventTriggerList() {
        SerializedProperty serializedProperty = serializedObject.FindProperty("bundleEventTriggerInfos");
        if (serializedProperty != null)
        {
            eventInfo = new ReorderableList(serializedObject, serializedProperty, true, true, true, true);
            //标题
            eventInfo.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Bundle触发器设置       Target物体      执行脚本         触发类型");
            };
            //元素绘制
            eventInfo.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = eventInfo.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                //脚本
                SerializedProperty _method = element.FindPropertyRelative("method");
                //参数类型
                SerializedProperty _mode = element.FindPropertyRelative("triggerType");
                //目标物体
                SerializedProperty _target = element.FindPropertyRelative("target");
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight), _target, GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + 120, rect.y, rect.width - 120 - 120, EditorGUIUtility.singleLineHeight), _method, GUIContent.none);
                //根据有无脚本绘制界面
                if(_method.objectReferenceValue != null){
                    EditorGUI.PropertyField(new Rect(rect.x + rect.width - 120, rect.y, 120, EditorGUIUtility.singleLineHeight), _mode, GUIContent.none);
                    //执行
                    if (_mode.enumValueIndex > 1)
                    {
                        //根据选择参数类型填入参数
                        //待续
                        switch (_mode.enumValueIndex)
                        {
                            case (int)PersistentListenerMode.EventDefined:;break;
                            default:;return;
                        }
                    }
                } 
            };
            //添加 + 按钮的下拉菜单
            //菜单内容根据文件夹结构自动适配
            eventInfo.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) =>
            {
                //一个目录
                var menu = new GenericMenu();
                //Mod文件夹下的资产GUID
                //得到GUIDs字符串数组
                var guids = AssetDatabase.FindAssets("", new[] { "Assets/BundleScript/CustomEventTemplateScripts" });
                //遍历CustomEventTemplateScripts文件夹下的子目录
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    //增加一个目录项
                    //AddItem会自动把WaveCreationParams传入ClickHandLer函数中
                    menu.AddItem(new GUIContent("CustomEventTemplateScripts/" + System.IO.Path.GetFileNameWithoutExtension(path)), false,
                        ClickHandLer, new EventInfoCreationParams() { target = null, scriptPath = path, mode = BundleEventTriggerType.GazeClick});
                }
                menu.ShowAsContext();
            };
            //移除菜单
            eventInfo.onRemoveCallback = (ReorderableList list) =>
            {
                if (EditorUtility.DisplayDialog("警告", "删除该元素？", "是", "否"))
                {
                    //脚本处理
                    //待续
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
            };
        }
    }
    /// <summary>
    /// 下拉菜单系统选择点击的回调
    /// </summary>
    /// <param name="target"></param>
    private void ClickHandLer(object target) {
        var data = (EventInfoCreationParams)target;
        eventInfo.serializedProperty.arraySize++;
        eventInfo.index = eventInfo.serializedProperty.arraySize - 1;
        SerializedProperty element = eventInfo.serializedProperty.GetArrayElementAtIndex(eventInfo.index);
        element.FindPropertyRelative("target").objectReferenceValue = null;
        element.FindPropertyRelative("method").objectReferenceValue = AssetDatabase.LoadAssetAtPath(data.scriptPath, typeof(UnityEngine.Object)) as UnityEngine.Object;
        //这里自定义了添加 所以要应用界面逆向修改List数组
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// 选择回调
    /// </summary>
    private struct EventInfoCreationParams {
        public GameObject target;
        public BundleEventTriggerType mode;
        public string scriptPath;
    }
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        serializedObject.Update();
        if (eventInfo != null)
        {
            eventInfo.DoLayoutList();
        }
        //for (int i =0;i< bundleEventTriggerInfos?.Count && i < lastTriggerInfos?.Count; i++)
        //{
        //    if (lastTriggerInfos[i]?.target != bundleEventTriggerInfos[i]?.target)
        //    {
        //        if (lastTriggerInfos[i].target != null)
        //        { 
        //            //重置物体上的增加的逻辑脚本
        //            BundleEventInfoBase bundleAction = lastTriggerInfos[i].target.GetComponent<BundleEventInfoBase>();
        //            if (bundleAction != null)
        //            {
        //                //生成之前物体脚本
        //                DestroyImmediate(bundleAction);
        //            }
        //        }
        //        UnityEngine.Object @object = bundleEventTriggerInfos[i].method;
        //        Type type = ((MonoScript)@object).GetClass();
        //        bundleEventTriggerInfos[i].target.AddComponent(type);
        //        lastTriggerInfos[i].target = bundleEventTriggerInfos[i].target;
        //    }
        //}
       
        serializedObject.ApplyModifiedProperties();
    }
}
