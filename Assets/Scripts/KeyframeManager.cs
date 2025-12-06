using System;
using UnityEngine;

public class KeyframeManager : MonoBehaviour {
    // Variables 
    public KeyframeAnimController.ClipController clipController;
    private float dt;

    private int clipStart, keyframeStart, keyframeStartOffset;


    private void init() {
        for (int i = 0; i < 1; i++) {
            KeyframeController.sampleInit(clipController.clipPool.samples[i], i, 24);
        }

        for (int i = 0; i < 1; i++) {
            KeyframeController.keyframeInit(clipController.clipPool.keyframes[i],
                                            clipController.clipPool.samples[i],  // current sample
                                            clipController.clipPool.samples[i + 1], // sample after current
                                            24);
        }

        KeyframeController.ClipInit(clipController.clipPool.clips[clipStart], clipController.name, 
            clipController.clipPool.keyframes[keyframeStart],
            clipController.clipPool.keyframes[keyframeStart + keyframeStartOffset]);

        KeyframeController.GetClipDuration(clipController.clipPool, clipStart, 30);

        clipStart = KeyframeController.GetIndexInPool(clipController.clipPool, clipController.name);

        KeyframeAnimController.Init(clipController, clipController.name + "_ctrl", clipController.clipPool,
            clipController.clipIndex, clipController.playbackStep, clipController.playbackStepPerSec);
    }

    void Start() {
        init();
    }

    // Update is called once per frame
    void Update() {
        KeyframeAnimController.Update(clipController, dt);
    }
}
