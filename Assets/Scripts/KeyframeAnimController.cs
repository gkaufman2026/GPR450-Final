using System;
using UnityEngine;

public class KeyframeAnimController { 
    [Serializable]
    public class ClipController {
        public string name;
        public int clipIndex, keyframeIndex;
        public int clipTimeStep, keyframeTimeStep, playbackStep;
        public float clipTimeSec, keyframeSec, playbackSec, playbackStepPerSec, playbackSecPerStep;
        public float clipParam, keyframeParam;

        public KeyframeController.ClipPool clipPool;
        public KeyframeController.Clip clip;
        public KeyframeController.Keyframe keyframe;
    }

    public static int Init(ClipController ctrlOut, string name, KeyframeController.ClipPool pool, int clipPoolIndex, int playbackStep, float playbackStepSec) {
        int clip = SetControllerClip(ctrlOut, name, pool, clipPoolIndex, playbackStep, playbackStepSec);
        if (clip >= 0) {
            ctrlOut.name = name;
            return clip;
        }
        return -1;
    }

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

    public static void Update(ClipController clipCtrl, float dt) {
        if (clipCtrl != null && clipCtrl.clipPool != null) {
            float overstep;

            dt *= clipCtrl.playbackSec;
            clipCtrl.clipTimeSec += dt;
            clipCtrl.keyframeSec += dt;

            while ((overstep = clipCtrl.keyframeSec - clipCtrl.keyframe.durationSec) >= 0.0) {
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

            while ((overstep = clipCtrl.keyframeSec) < 0.0) {
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
