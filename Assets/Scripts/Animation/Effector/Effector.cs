using UnityEngine;

public class Effector : MonoBehaviour {
    private const string EFFECTOR_TAG = "Effector";

    private void Start() {
        gameObject.tag = EFFECTOR_TAG;
    }
}
