using System;
using System.Runtime.InteropServices;
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

    public static void ReleaseClipPool(ClipPool clipPool) {
        if (clipPool != null && clipPool.clips != null) {
            int clipCount = clipPool.clips.Length;
            clipPool.clips = null;
            clipPool.keyframes = null;
            clipPool.samples = null;
        }
    }
    public static void ClipTransitionInit(ClipTransition transition, ClipTransitionFlag flag, int offset, Clip clip) {
        if (transition != null && clip != null) {
            transition.offset = offset;
            transition.flag = flag;
            transition.clipIndex = clip.index;
        }
    }

    public static void ClipInit(Clip outClip, string name, Keyframe first, Keyframe final) {
        if (outClip != null && outClip.index >= 0) {
            if (first != null && first.index >= 0 && final != null && final.index >= 0) {
                outClip.firstIndex = first.index;
                outClip.finalIndex = final.index;
                outClip.keyframeCount = outClip.finalIndex - outClip.firstIndex;
                outClip.keyframeDirection = ((outClip.keyframeCount) != 0 ? ((outClip.keyframeCount) > 0 ? +1 : -1) : 0);
                outClip.keyframeCount = 1 + outClip.keyframeCount * outClip.keyframeDirection;
                ClipTransitionInit(outClip.forward, ClipTransitionFlag.STOP, 0, outClip);
                ClipTransitionInit(outClip.reverse, ClipTransitionFlag.STOP, 0, outClip);
            }
        }
    }

    public static int sampleInit(Sample outSample, int timeStep, float playbackStep) {
        if (outSample != null && outSample.index >= 0 && playbackStep > 0) {
            outSample.timeStep = timeStep;
            outSample.timeSec = (int)(timeStep / playbackStep);
            return outSample.index;
        }
        return 0;
    }

    public static int keyframeInit(Keyframe outKeyframe, Sample sample0, Sample sample1, float playbackStep) {
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

    public static int GetIndexInPool(ClipPool clipPool, string name) {
        if (clipPool != null && clipPool.clips != null) {
            for (int i = 0; i < clipPool.clips.Length; i++) {
                if (clipPool.clips[i].name.Equals(name)) return i;
            }
        }
        return -1;
    }

    public static int GetClipDuration(ClipPool clipPool, int clipIndex, float playbackInSec) {
        if (clipPool != null && clipPool.clips != null && clipIndex < clipPool.clips.Length && playbackInSec > 0.0) {
            Clip clip = clipPool.clips[clipIndex];
            int i, k;
            clip.durationSec = clip.durationInv = 0.0f;

            for (i = 0, k = clip.firstIndex; i < clip.keyframeCount; ++i, k += clip.keyframeDirection) {
                clip.durationInStep += clipPool.keyframes[k].durationInSteps;
            }

            clip.durationSec = clip.durationSec / playbackInSec;
            clip.durationInv = clip.durationSec != 0 ? 1 / clip.durationSec : 0;
            return clip.index;
        }
        return -1;
    }

    public static int DistributeClipDuration(ClipPool clipPool, int clipIndex, float playbackInSec) {
        if (clipPool != null && clipPool.clips != null && clipIndex < clipPool.clips.Length && playbackInSec > 0.0) {
            Clip clip = clipPool.clips[clipIndex];
            Keyframe keyframe;
            Sample sample0, sample1;
            int i, k;
            for (i = 0, k = clip.firstIndex; i < clip.keyframeCount; ++i, k += clip.keyframeDirection) {
                keyframe = clipPool.keyframes[k];
                sample0 = clipPool.samples[keyframe.sampleIndex0];
                sample1 = clipPool.samples[keyframe.sampleIndex1];
                sampleInit(sample0, ((sample0.index * clip.durationInStep) / clip.keyframeCount), playbackInSec);
                sampleInit(sample1, ((sample1.index * clip.durationInStep) / clip.keyframeCount), playbackInSec);
                keyframeInit(clipPool.keyframes[k], sample0, sample1, playbackInSec);
            }
            return clip.index;
        }
        return -1;
    }
}
