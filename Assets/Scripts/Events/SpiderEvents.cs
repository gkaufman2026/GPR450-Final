using UnityEngine;
using UnityEngine.Events;

public class SpiderEvents : MonoBehaviour {
    public static void UpdateKeyframeIndex() => OnUpdatedKFIndex?.Invoke();
    public static event UnityAction OnUpdatedKFIndex;
}
