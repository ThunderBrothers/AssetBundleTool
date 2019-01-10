using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static BundleEventTrigger;

[AddComponentMenu("Bundle/BundleEventTriggerDesigner")]
/// <summary>
/// BundleEventTrigger设计师
/// 构建数据给BundleEventTrigger执行
/// bundleEventTriggerInfos保存此BundleEventTriggerDesigner上定制的所有事件信息
/// </summary>
public class BundleEventTriggerDesigner : MonoBehaviour
{
    [Header("可排序ObjList")]
    public List<BundleEventTriggerInfo> bundleEventTriggerInfos = new List<BundleEventTriggerInfo>();
    public List<BundleEventTriggerInfo> lastTriggerInfos = new List<BundleEventTriggerInfo>();

}
/// <summary>
/// 单个触发事件信息
/// </summary>
[Serializable]
public class BundleEventTriggerInfo {
    public GameObject target;
    public UnityEngine.Object method;
    public BundleEventTriggerType triggerType;
}

