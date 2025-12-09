using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class Effector : MonoBehaviour {
    private const string EFFECTOR_TAG = "Effector";
    private GameObject body;

    // Ensuring any effector game object has the tag
    private void Start() {
        gameObject.tag = EFFECTOR_TAG;
        
        body = GameObject.FindGameObjectWithTag("Body");
    }

    public void Pull(float dt, float speed) {
        if (body != null) {
            Vector3 currentPos = body.transform.position;
            Vector3 targetPos = gameObject.transform.position;
            Vector3 flatTargetPos = new(targetPos.x, currentPos.y, targetPos.z);

            body.transform.position = Vector3.MoveTowards(currentPos, flatTargetPos, (dt * speed) / 100);
        }
    }
}