using UnityEngine;

public class Effector : MonoBehaviour {
    private const string EFFECTOR_TAG = "Effector";

    // Ensuring any effector game object has the tag
    private void Start() {
        gameObject.tag = EFFECTOR_TAG;
    }
}
