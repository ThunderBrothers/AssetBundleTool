using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///所有自定义脚本的基类
///这里先留着方便以后拓展
/// </summary>
public class BundleEventInfoBase : MonoBehaviour{

    
}
/// <summary>
/// 自定义脚本的方法执行接口
/// </summary>
public interface BundleActionHandler {
   
    void OnBundleAction(PointerEventData eventData);
}
