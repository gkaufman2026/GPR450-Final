using UnityEngine;
using System;
using System.Collections.Generic;

public class KeyframeAnimController {
    /// <summary>
    /// Clip Controller Data Structure - Jerry
    /// </summary>
    [Serializable]
    public class ClipController {
        // Gets updated on runtime
        [Tooltip("Case-Sensitive String for clip lookup")] public string name;
        
        // The current index of the respective clip and keyframe playing
        public int clipIndex, keyframeIndex;

        // First two are not used in current instance and will be reset back to 0
        public int clipTimeStep;
        public int keyframeTimeStep;

        // Clip & Keyframe Time In Seconds
        [Tooltip("Clip Time in Seconds")] public float clipTimeSec;
        [Tooltip("Keyframe Time in Seconds")] public float keyframeSec;

        // Playback Speed In Seconds
        [Tooltip("Duration of a Second for playback"), Range(1,100)] public float playbackSec;
        public float playbackStepPerSec;
        public float playbackSecPerStep;
        [Tooltip("Product of StepPerSec & SecPerStep")] public int playbackStep;

        // Interpolation Param
        public float clipParam, keyframeParam;

        public KeyframeController.ClipPool clipPool;
        [Tooltip("Current Clip")] public KeyframeController.Clip clip;
        [Tooltip("Current Keyframe")] public KeyframeController.Keyframe keyframe;
    }

    /// <summary>
    /// Clip Controller Initialization - Jerry
    /// </summary>
    public static int Init(ClipController ctrlOut, string name, KeyframeController.ClipPool pool, int clipPoolIndex, int playbackStep, float playbackStepSec) {
        int clip = SetControllerClip(ctrlOut, name, pool, clipPoolIndex, playbackStep, playbackStepSec);
        if (clip >= 0) {
            ctrlOut.name = name;
            return clip;
        }
        return -1;
    }

    /// <summary>
    /// Set Clip Controller to be played - Jerry
    /// </summary>
    /// <param name="clipCtrl"> Clip Controller </param>
    /// <param name="name"> Name of the Clip, used as a lookup </param>
    /// <param name="clipPool"> Clip Pool </param>
    /// <param name="clipIndex"> Index of clip </param>
    /// <param name="playback"> Playback Time in Seconds </param>
    /// <param name="playbackStep"> Playback Step Time in Seconds </param>
    /// <returns> Returns the index of the clip with its applied settings onto clipController </returns>
    public static int SetControllerClip(ClipController clipCtrl, string name, KeyframeController.ClipPool clipPool, int clipIndex, int playback, float playbackStep) {
        if (clipCtrl != null && clipPool.clips != null && clipIndex < clipPool.clips.Length && playbackStep > 0) {
            clipCtrl.clipPool = clipPool;
            clipCtrl.clipIndex = clipIndex;
            clipCtrl.clip = clipPool.clips[clipIndex];

            clipCtrl.keyframeIndex = clipCtrl.clip.firstIndex;
            clipCtrl.keyframe = clipPool.keyframes[clipCtrl.keyframeIndex];
            clipCtrl.clipTimeStep = clipCtrl.keyframeTimeStep = 0;
            clipCtrl.clipTimeSec = clipCtrl.keyframeSec = 0;
            clipCtrl.clipParam = clipCtrl.keyframeParam = 0;

            if (clipCtrl != null && clipCtrl.clipPool != null && playbackStep > 0) {
                clipCtrl.playbackStep = playback;
                clipCtrl.playbackStepPerSec = playbackStep;
                clipCtrl.playbackSecPerStep = playbackStep;
                clipCtrl.playbackStep = (int)(playbackStep * clipCtrl.playbackSecPerStep);
            }

            return clipIndex;
        }
        return 0;
    }

    /// <summary>
    /// Clip Controller Update Loop - Jerry
    /// </summary>
    /// <param name="clipCtrl"> Clip Controller </param>
    /// <param name="dt"> Time.fixedDeltaTime </param>
    public static void Update(ClipController clipCtrl, float dt, List<FabrikIK> iKs) {
        if (clipCtrl != null && clipCtrl.clipPool != null) {
            float overstep;

            // Time Step
            dt *= clipCtrl.playbackSec;
            clipCtrl.clipTimeSec += dt;
            clipCtrl.keyframeSec += dt;

            foreach(FabrikIK toe in iKs) {
                toe.ResolveIK(dt);
            }

            // If the current keyframe time in seconds - current keyframes duration is >= 0
            while ((overstep = clipCtrl.keyframeSec - clipCtrl.keyframe.durationSec) >= 0.0) {
                // If current keyframe is the last one, reset back to first
                if (clipCtrl.keyframeIndex == clipCtrl.clip.finalIndex) {
                    clipCtrl.keyframeIndex = clipCtrl.clip.firstIndex;
                    clipCtrl.keyframe = clipCtrl.clipPool.keyframes[clipCtrl.keyframeIndex];
                    clipCtrl.keyframeSec = overstep;
                } else { 
                    clipCtrl.keyframeIndex += clipCtrl.clip.keyframeDirection;
                    clipCtrl.keyframe = clipCtrl.clipPool.keyframes[clipCtrl.keyframeIndex];
                    clipCtrl.keyframeSec = overstep;
                }
            }

            // If keyframe time in seconds is less than 0
            while ((overstep = clipCtrl.keyframeSec) < 0.0) {
                // Set the current keyframe to the first
                if (clipCtrl.keyframeIndex == clipCtrl.clip.firstIndex) {
                    clipCtrl.keyframeIndex = clipCtrl.clip.finalIndex;
                    clipCtrl.keyframe = clipCtrl.clipPool.keyframes[clipCtrl.keyframeIndex];
                    clipCtrl.keyframeSec = overstep + clipCtrl.keyframe.durationSec;
                } else {
                    clipCtrl.keyframeIndex -= clipCtrl.clip.keyframeDirection;
                    clipCtrl.keyframe = clipCtrl.clipPool.keyframes[clipCtrl.keyframeIndex];
                    clipCtrl.keyframeSec = overstep + clipCtrl.keyframe.durationSec;
                }
            }

            // Normalization 
            clipCtrl.keyframeParam = clipCtrl.keyframeSec * clipCtrl.keyframe.durationInv;
            clipCtrl.clipParam = clipCtrl.clipTimeSec * clipCtrl.clip.durationInv;

            return;
        }
    }
}
