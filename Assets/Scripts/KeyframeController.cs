using System;
using UnityEngine;

public class KeyframeController {
    // Data Structures - Jerry
    [Serializable]
    public class Clip {
        public string name;
        public int index;
        public int firstIndex;
        public int finalIndex;
        public int keyframeCount;
        [Tooltip("Step Direction")] public int keyframeDirection;

        public int durationInStep;
        public float durationSec, durationInv;

        public ClipTransition forward, reverse;
    }

    [Serializable]
    public class Sample {
        public int index;
        public int timeStep, timeSec;
    }

    int sampleInit(Sample outSample, int timeStep, float playbackStep) {
        if (outSample != null && outSample.index >= 0 && playbackStep > 0) {
            outSample.timeStep = timeStep;
            outSample.timeSec = (int)(timeStep / playbackStep);
            return outSample.index;
        }
        return 0;
    }

    int keyframeInit(Keyframe outKeyframe, Sample sample0, Sample sample1, float playbackStep) {
        if (outKeyframe != null && sample0 != null && sample1 != null && playbackStep > 0) {
            if (outKeyframe.index >= 0 && sample0.index >= 0 && sample1.index >= 0) {
                outKeyframe.sampleIndex0 = sample0.index;
                outKeyframe.sampleIndex1 = sample1.index;
                outKeyframe.durationInSteps = sample1.timeStep - sample0.timeStep;
                outKeyframe.durationSec = (int)(outKeyframe.durationInSteps / playbackStep);
                outKeyframe.durationInv = outKeyframe.durationSec;
                return outKeyframe.index;
            }
        }
        return 0;
    }

    [Serializable]
    public class Keyframe {
        public int index;
        public int sampleIndex0, sampleIndex1;
        public int durationInSteps;
        public int durationSec, durationInv;
    }

    [Serializable]
    public class ClipPool {
        public Clip[] clips;
        public Keyframe[] keyframes;
        public Sample[] samples;
        public int clipCount, keyframeCount, sampleCount;
    }

    [Serializable]
    public class ClipTransition {
        public int offset, clipIndex;
        public ClipTransitionFlag flag;
    }

    [Serializable]
    public enum ClipTransitionFlag {
        STOP,
        PLAY,
        REVERSE,
        SKIP,
        OVERSTEP,
        TERMINUS,
        OFFSET,
        CLIP,
        BRANCH
    }

    public void CreateClipPool(ClipPool clipPool, int clipCount, int keyframeCount, int sampleCount) {
         
    }

    public void ReleaseClipPool(ClipPool clipPool) {
    }
    public void ClipTransitionInit(ClipTransition transition, ClipTransitionFlag flg, int offset, Clip clip) {
    }

    public void ClipInit(Clip outClip, string name, Keyframe first, Keyframe final) {
    }

    public void GetIndexInPool(ClipPool clipPool, string name) {
    }

    public void GetClipDuration(ClipPool clipPool, int clipIndex, float playbackInSec) {
    }

    public void DistributeClipDuration(ClipPool clipPool, int clipIndex, float playbackInSec) {
    }
}
