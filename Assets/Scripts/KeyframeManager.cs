using System;
using UnityEngine;

public class KeyframeManager : MonoBehaviour {
    // Variables 
    public KeyframeAnimController.ClipController clipController;

    void Start() {
        KeyframeAnimController.init(clipController, clipController.name, clipController.clipPool, clipController.clipIndex, clipController.playbackStep, clipController.playbackStepPerSec);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
