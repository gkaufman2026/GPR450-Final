using UnityEngine;
using static KeyframeAnimController;

public class KeyframeManager : MonoBehaviour {

    [Header("Animation")]
    public ClipController clipController;

    [Header("Settings")]
    public bool isPlaying = true;
    public int keyframeStartOffset = 4;
    
    private int hClipCount, hSampleCount, hKeyframeCount;
    private int index;
    private const int FRAME_RATE = 24;

    void Start() {
        hClipCount = clipController.clip.finalIndex;
        hSampleCount = clipController.clipPool.samples.Length;

        init();
    }

    /// <summary>
    /// Handles Setup of Clip Controller - Jerry
    /// </summary>
    private void init() {
        for (int i = 0; i < hSampleCount - 1; ++i) {
            KeyframeController.sampleInit(clipController.clipPool.samples[i], i, FRAME_RATE);
        }
        for (int i = 0; i < hKeyframeCount - 1; ++i) {
            KeyframeController.keyframeInit(clipController.clipPool.keyframes[i],
                clipController.clipPool.samples[i], clipController.clipPool.samples[i + 1], FRAME_RATE);
        }
        for (int i = 0; i < hClipCount - 1; ++i) {
            KeyframeController.ClipInit(clipController.clipPool.clips[i], clipController.clipPool.clips[i].name,
                clipController.clipPool.keyframes[i],
                clipController.clipPool.keyframes[i]);

            KeyframeController.GetClipDuration(clipController.clipPool, i, FRAME_RATE);
        }

        index = KeyframeController.GetIndexInPool(clipController.clipPool, clipController.name);
        Init(clipController, clipController.clipPool.clips[index].name + "_ctrl", clipController.clipPool,
            index, clipController.playbackStep, clipController.playbackStepPerSec);
    }

    void FixedUpdate() {
        // Allowing the clip time to be paused
        if (isPlaying) {
            KeyframeAnimController.Update(clipController, Time.fixedDeltaTime);
        }
    }
}
