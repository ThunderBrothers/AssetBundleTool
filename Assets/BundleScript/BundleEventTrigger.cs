using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[DisallowMultipleComponent]
//暂定自动添加BoxCollider
[RequireComponent(typeof(BoxCollider))]
[AddComponentMenu("Bundle/BundleEventTrigger")]
public class BundleEventTrigger : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler {
    /// <summary>
    /// 单个触发器上的触发信息表
    /// </summary>
    [SerializeField]
    private List<BundleEventTriggerInfo> selfBundleEventTriggerInfo;
    private BundleEventTriggerDesigner selfDesigner;
    private GameObject[] targets;//被操作物体
    private BundleActionHandler[] bundleActionHandlers;//触发逻辑数组

    private void Start() {
        if (selfDesigner == null)
        {
            selfDesigner = GetComponent<BundleEventTriggerDesigner>();
        }
        selfBundleEventTriggerInfo = selfDesigner?.bundleEventTriggerInfos;
        if (selfBundleEventTriggerInfo == null)
        {
            Debug.Log(gameObject.name + "--->>>>上没有附带的BundleEventTriggerDesigner组件");
        }
        //延迟执行，在加载后所有执行脚本添加完成后执行获取信息
        StartCoroutine(GetAllBundleActionHandler());
    }
    IEnumerator GetAllBundleActionHandler() {
        yield return null;
        yield return null;
        //获取信息 selfBundleEventTriggerInfo
        //未完待续
    }

    public List<BundleEventTriggerInfo> triggers {
        get
        {
            if (selfBundleEventTriggerInfo == null)
                selfBundleEventTriggerInfo = new List<BundleEventTriggerInfo>();
            return selfBundleEventTriggerInfo;
        }
        set { selfBundleEventTriggerInfo = value; }
    }
    /// <summary>
    /// 执行事件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="eventData"></param>
    private void Execute(BundleEventTriggerType id, PointerEventData eventData) {
        for (int i = 0, imax = triggers.Count; i < imax; ++i)
        {
            var ent = triggers[i];
            if (ent.target != null && ent.triggerType == id && ent.method != null)
                ent.target.GetComponent<BundleActionHandler>()?.OnBundleAction(eventData);
        }
    }
    public virtual void OnPointerEnter(PointerEventData eventData) {
        Execute(BundleEventTriggerType.GazeOn, eventData);
    }

    public virtual void OnPointerExit(PointerEventData eventData) {
        Execute(BundleEventTriggerType.GazeOff, eventData);
    }

    public virtual void OnPointerClick(PointerEventData eventData) {
        Execute(BundleEventTriggerType.GazeClick, eventData);
    }

    public enum BundleEventTriggerType {

        GazeOn = 0,
        GazeOff = 1,
        GazeClick = 2,
    }
}
