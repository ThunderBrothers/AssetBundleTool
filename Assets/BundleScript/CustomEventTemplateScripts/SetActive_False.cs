using UnityEngine.EventSystems;
/// <summary>
/// 实例脚本
/// 设置设置物体SetActive
/// </summary>
public class SetActive_False : BundleEventInfoBase, BundleActionHandler {
    public void OnBundleAction(PointerEventData eventData) {
        gameObject.SetActive(false);
    }
}

