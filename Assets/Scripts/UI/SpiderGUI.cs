using ImGuiNET;
using System;
using UImGui;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.InputSystem;

public class SpiderGUI : MonoBehaviour {
    [SerializeField] private UImGui.UImGui instance;
    [SerializeField] private KeyframeManager kfManager;
    [SerializeField] private bool isOpen = true;

    private void Awake() {
        if (instance == null) {
            Debug.LogError("Missing hook into U Im Gui");
        }

        instance.Layout += OnLayout;
        instance.OnInitialize += OnInitialize;
        instance.OnDeinitialize += OnDeinitialize;
    }

    private void OnLayout(UImGui.UImGui obj) {
        // Mouse Scroll is too slow, cant find fix so removing it entirely
        if (!ImGui.Begin("Adv Anim Final", ref isOpen, ImGuiWindowFlags.NoScrollWithMouse)) {
            ImGui.End();
            return;
        }

        init();

        ImGui.End();
    }

    private void init() {
        ImGui.Checkbox("Is Playing", ref kfManager.isPlaying);

        if (ImGui.CollapsingHeader("Clip Controller")) {
            initClipController();
        }
    }

    private void initClipController() {
        ImGui.Text("Name: " + kfManager.clipController.name);
        ImGui.Text("Keyframe Index: " + kfManager.clipController.keyframeIndex);
        ImGui.Text("Clip Time Sec: " + kfManager.clipController.clipTimeSec.ToString("f"));
        ImGui.Text("Keyframe Time Sec: " + kfManager.clipController.keyframeSec.ToString("f"));
        ImGui.SliderFloat("Playback Sec", ref kfManager.clipController.playbackSec, 1, 100);
        ImGui.Text("Playback Step Per Sec: " + kfManager.clipController.playbackStepPerSec);
        ImGui.Text("Playback Sec Per Step: " + kfManager.clipController.playbackSecPerStep);
        ImGui.Text("Playback Step: " + kfManager.clipController.playbackStep);
        ImGui.Text("Clip Param: " + kfManager.clipController.clipParam.ToString("f"));
        ImGui.Text("Keyframe Param: " + kfManager.clipController.keyframeParam.ToString("f"));

        if (ImGui.CollapsingHeader("Clip Pool")) {
            ImGui.Text("Clip Length: " + kfManager.clipController.clipPool.clips.Length);
            ImGui.Text("Keyframes Length: " + kfManager.clipController.clipPool.keyframes.Length);
            ImGui.Text("Samples Length: " + kfManager.clipController.clipPool.samples.Length);
        }

        if (ImGui.CollapsingHeader("Current Clip")) {
            ImGui.Text("Name: " + kfManager.clipController.clip.name);
            ImGui.Text("Index: " + kfManager.clipController.clip.index);
            ImGui.Text("First Index: " + kfManager.clipController.clip.firstIndex);
            ImGui.Text("Final Index: " + kfManager.clipController.clip.finalIndex);
            ImGui.Text("Total Keyframes: " + kfManager.clipController.clip.keyframeCount);
            ImGui.Text("Keyframe Direction: " + kfManager.clipController.clip.keyframeDirection);
            ImGui.Text("Duration (Steps): " + kfManager.clipController.clip.durationInStep);
            ImGui.Text("Duration (Secs): " + kfManager.clipController.clip.durationSec);
            ImGui.Text("Duration (Inverse): " + kfManager.clipController.clip.durationInv);

            ImGui.SeparatorText("Forward Transition:");
            ImGui.BulletText("Offset: " + kfManager.clipController.clip.forward.offset);
            ImGui.BulletText("Clip Index: " + kfManager.clipController.clip.forward.clipIndex);
            ImGui.BulletText("Flag: " + kfManager.clipController.clip.forward.flag);

            ImGui.SeparatorText("Reverse Transition:");
            ImGui.BulletText("Offset: " + kfManager.clipController.clip.reverse.offset);
            ImGui.BulletText("Clip Index: " + kfManager.clipController.clip.reverse.clipIndex);
            ImGui.BulletText("Flag: " + kfManager.clipController.clip.reverse.flag);
        }

        if (ImGui.CollapsingHeader("Current Keyframe")) {
            ImGui.Text("Index: " + kfManager.clipController.keyframe.index);
            ImGui.Text("Sample 0 Index: " + kfManager.clipController.keyframe.sampleIndex0);
            ImGui.Text("Sample 1 Index: " + kfManager.clipController.keyframe.sampleIndex1);
            ImGui.Text("Duration In Steps: " + kfManager.clipController.keyframe.durationInSteps);
            ImGui.Text("Duration In Sec: " + kfManager.clipController.keyframe.durationSec);
            ImGui.Text("Duration Inverse: " + kfManager.clipController.keyframe.durationInv);
        }
    }

    private void OnInitialize(UImGui.UImGui obj) {
        // runs after UImGui.OnEnable();
    }

    private void OnDeinitialize(UImGui.UImGui obj) {
        // runs after UImGui.OnDisable();
    }

    private void OnDisable() {
        instance.Layout -= OnLayout;
        instance.OnInitialize -= OnInitialize;
        instance.OnDeinitialize -= OnDeinitialize;
    }
}
