using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StoreAnimData : MonoBehaviour {
    public class AnimData
    {
        public float clipLength;
        public float frameRate;
        public List<CurveBindingData> curveBindingData;

    }
    public class CurveBindingData
    {
        public string path;
        public string propertyName;
        public int keyListLen;
        public List<Keys> keyframes;
    }
    public class Keys
    {
        public float time;
        public float value;
    }

    [SerializeField]private List<AnimationClip> clips;

    [SerializeField] private List<AnimData> clipData;

    private AnimData clipDat;

    void Start() {
        if (clips != null) {
           foreach (AnimationClip clip in clips) {
                clipData.Add(LogAnimationClipData(clip));
            } 
        }
    }

    private AnimData LogAnimationClipData(AnimationClip clip) {
        clipDat.clipLength = clip.length;
        clipDat.frameRate = clip.frameRate;
        // clip.length
        // clip.frameRate
        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);
        foreach (EditorCurveBinding binding in curveBindings) {
            // binding.path
            // binding.type
            // binding.propertyName
            //clipDat.curveBindingData.Add(CurveBindingData(binding.path, binding.propertyName));
            CurveBindingData curveBindingData = new();
            curveBindingData.path = binding.path;
            curveBindingData.propertyName = binding.propertyName;
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
            if (curve != null) {
                // curve.keys.Length
                curveBindingData.keyListLen = curve.keys.Length;
                foreach (Keyframe key in curve.keys) {
                    // key.time
                    // key.value
                    Keys keys = new Keys();
                    keys.time = key.time;
                    keys.value = key.value;
                    curveBindingData.keyframes.Add(keys);
                }
            }
            clipDat.curveBindingData.Add(curveBindingData);
        }
        
        return clipDat;
    }
}
