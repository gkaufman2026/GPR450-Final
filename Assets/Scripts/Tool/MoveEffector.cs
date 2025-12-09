using ImGuiNET;
using UnityEngine;

public class MoveEffector : MonoBehaviour {

    [SerializeField] private GameObject prefab;
    [SerializeField] public Vector3 spawnOffset;
    [SerializeField] private Camera cam;

    private GameObject spawnedPrefab;
    private Effector effector;

    public Effector Effector { get => effector; set => effector = value; }
    public Camera Cam { get => cam; set => cam = value; }

    void Start() {
        Cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        // Preventing spawning of move effector if mouse is overlapping with ImGui UI - Jerry
        if (!ImGui.GetIO().WantCaptureMouse) {
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                Ray ray = Cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit)) {
                    Vector3 spawn = hit.point + spawnOffset;
                    if (hit.transform.CompareTag("Floor")) {
                        if (spawnedPrefab == null) {
                            spawnedPrefab = Instantiate(prefab, spawn, Quaternion.identity);
                            Effector = spawnedPrefab.AddComponent<Effector>();
                            
                        } else {
                            Effector.transform.position = spawn;
                        }
                    }
                }
            }
        }

        // Quick way to quit out of engine (more profesh)
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.K)) {
            Destroy(spawnedPrefab);
            spawnedPrefab = null;
            effector = null;
            Effector = null;
        }
    }
}
