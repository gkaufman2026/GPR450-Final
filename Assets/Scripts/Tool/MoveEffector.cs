using UnityEngine;

public class MoveEffector : MonoBehaviour {

    [SerializeField] private GameObject prefab;
    [SerializeField] private Camera cam;

    private GameObject spawnedPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Vector3 mousePixelPos = Input.mousePosition;
            mousePixelPos.z = 20f;

            Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(mousePixelPos);
            mouseWorldPosition.z = 0f;

            if (spawnedPrefab == null) {
                spawnedPrefab = Instantiate(prefab, mouseWorldPosition, Quaternion.identity);
            } else {
                spawnedPrefab.transform.position = mouseWorldPosition;
            }
        }
    }
}
