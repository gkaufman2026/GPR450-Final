using ImGuiNET;
using System;
using UnityEngine;

public class MoveEffector : MonoBehaviour {

    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private Camera cam;

    private GameObject spawnedPrefab;

    void Start() {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        // Preventing spawning of move effector if mouse is overlapping with ImGui UI - Jerry
        if (!ImGui.GetIO().WantCaptureMouse) {
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit)) {
                    Vector3 spawn = hit.point + spawnOffset;
                    if (hit.transform.CompareTag("Floor")) {
                        if (spawnedPrefab == null) {
                            spawnedPrefab = Instantiate(prefab, spawn, Quaternion.identity);
                        } else {
                            spawnedPrefab.transform.position = spawn;
                        }
                    }
                }
            }
        }

        // Quick way to quit out of engine (more profesh)
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
