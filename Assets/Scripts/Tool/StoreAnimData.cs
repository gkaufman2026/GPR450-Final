using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StoreAnimData : MonoBehaviour {
    [SerializeField]private List<AnimationClip> clips;

    void Start() {
        if (clips != null) {
           foreach (AnimationClip clip in clips) {
                LogAnimationClipData(clip);
            } 
        }
    }

    private void LogAnimationClipData(AnimationClip clip) {
        // clip.length
        // clip.frameRate

        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);
        foreach (EditorCurveBinding binding in curveBindings) {
            // binding.path
            // binding.type
            // binding.propertyName
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
            if (curve != null) {
                // curve.keys.Length
                foreach (Keyframe key in curve.keys) {
                    // key.time
                    // key.value
                }
            }
        }
    }
}
