using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class KeyframeController {
    /// <summary>
    /// Clip Data Structure : Timeline of what will happen within animation - Jerry
    /// </summary>
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

    /// <summary>
    /// Sample Data Structure : Moment within the animation - Jerry
    /// </summary>
    [Serializable]
    public class Sample {
        public int index;
        public int timeStep, timeSec;
    }

    /// <summary>
    /// Keyframe Data Structure : Multiple moments at point of time - Jerry
    /// </summary>
    [Serializable]
    public class Keyframe {
        public int index;
        public int sampleIndex0, sampleIndex1;
        public int durationInSteps;
        public int durationSec, durationInv;
    }

    /// <summary>
    /// Clip Pool Data Structure : Collection of Clips within an animation - Jerry
    /// </summary>
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

    /// <summary>
    /// Clear Clip Pool - Jerry
    /// </summary>
    /// <param name="clipPool"> Clip Controller </param>
    public static void ReleaseClipPool(ClipPool clipPool) {
        if (clipPool != null && clipPool.clips != null) {
            int clipCount = clipPool.clips.Length;
            clipPool.clips = null;
            clipPool.keyframes = null;
            clipPool.samples = null;
        }
    }

    /// <summary>
    /// Handle Transition Between Clips - Jerry
    /// </summary>
    /// <param name="transition"> Transition Settings between clips </param>
    /// <param name="flag"> Desired transition action </param>
    /// <param name="offset"> Offset between clips </param>
    /// <param name="clip"> Clip to move to </param>
    public static void ClipTransitionInit(ClipTransition transition, ClipTransitionFlag flag, int offset, Clip clip) {
        if (transition != null && clip != null) {
            transition.offset = offset;
            transition.flag = flag;
            transition.clipIndex = clip.index;
        }
    }

    /// <summary>
    /// Handle Initalization of Clip - Jerry
    /// </summary>
    /// <param name="outClip"> The clip being initalized </param>
    /// <param name="name"> The name of new clip </param>
    /// <param name="first"> Start keyframe </param>
    /// <param name="final"> Final keyframe</param>
    public static void ClipInit(Clip outClip, string name, Keyframe first, Keyframe final) {
        if (outClip != null && outClip.index >= 0) {
            if (first != null && first.index >= 0 && final != null && final.index >= 0) {
                outClip.name = name;
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

    /// <summary>
    /// Handle Initalization of Sample - Jerry
    /// </summary>
    /// <param name="outSample"> The sample being initalized </param>
    /// <param name="timeStep"> Desired timestep for sample </param>
    /// <param name="playbackStep"> Desired playback step for sample </param>
    /// <returns> If valid sample, returns the index or -1 for out of bounds </returns>
    public static int sampleInit(Sample outSample, int timeStep, float playbackStep) {
        if (outSample != null && outSample.index >= 0 && playbackStep > 0) {
            outSample.timeStep = timeStep;
            outSample.timeSec = (int)(timeStep / playbackStep);
            return outSample.index;
        }
        return -1;
    }

    /// <summary>
    /// Handle Initalization of keyframe - Jerry
    /// </summary>
    /// <param name="outKeyframe"> The keyframe being initalized </param>
    /// <param name="sample0"> Sample Index for Start </param>
    /// <param name="sample1"> Sample Index for End </param>
    /// <param name="playbackStep"> Desired playback step for keyframe </param>
    /// <returns> If valid keyframe, returns the index or -1 for out of bounds </returns>
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
        return -1;
    }

    /// <summary>
    /// Finds Index of Clip by name - Jerry
    /// </summary>
    /// <param name="clipPool"> Clip Pool being searched </param>
    /// <param name="name"> Name of clip wanted </param>
    /// <returns> If name exists within clip pool, returns the clips index or -1 for out of bounds </returns>
    public static int GetIndexInPool(ClipPool clipPool, string name) {
        if (clipPool != null && clipPool.clips != null) {
            for (int i = 0; i < clipPool.clips.Length; i++) {
                if (clipPool.clips[i].name.Equals(name)) return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Get Clip Duration - Jerry
    /// </summary>
    /// <param name="clipPool"> Clip Pool to find all keyframes within pool </param>
    /// <param name="clipIndex"> Index of clip of desired duration </param>
    /// <param name="playbackInSec"> Playback in Seconds </param>
    /// <returns> The calculated duration of all the keyframes within clip pool </returns>
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

    /// <summary>
    /// Alternative Way to Calculate Clip Duration - Jerry
    /// </summary>
    /// <param name="clipPool"> Clip Pool to find all keyframes within pool </param>
    /// <param name="clipIndex"> Index of clip of desired duration </param>
    /// <param name="playbackInSec"> Playback in Seconds </param>
    /// <returns> The calculated duration of all the keyframes by the overall clip duration </returns>
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
