using System;

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

    public static int init(ClipController ctrlOut, string name, KeyframeController.ClipPool pool, int clipPoolIndex, int playbackStep, float playbackStepSec) {
        int clip = SetControllerClip(ctrlOut, name, pool, clipPoolIndex, playbackStep, playbackStepSec);
        return clip >= 0 ? clip : 0;
    }

    public static int SetControllerClip(ClipController clipCtrl, string name, KeyframeController.ClipPool clipPool, int clipIndex, int playback, float playbackStep) {
        if (clipCtrl != null && clipPool.clips != null && clipIndex < clipPool.clipCount && playbackStep > 0) {
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
}
