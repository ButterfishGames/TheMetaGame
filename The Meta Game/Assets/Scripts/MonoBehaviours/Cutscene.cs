using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NewCutscene", menuName = "ScriptableObjects/Cutscene", order = 1)]
public class Cutscene : ScriptableObject
{
    public Transform[] transforms = new Transform[0];
        public int transformsSize = 0;
        public bool transformsExpanded = false;

    public Animator[] animators = new Animator[0];
        public int animatorsSize = 0;
        public bool animatorsExpanded = false;

    public RuntimeAnimatorController[] gameplayControllers = new RuntimeAnimatorController[0];
        public int gameplayControllersSize = 0;
        public bool gameplayControllersExpanded = false;

    public RuntimeAnimatorController[] cutsceneControllers = new RuntimeAnimatorController[0];
        public int cutsceneControllersSize = 0;
        public bool cutsceneControllersExpanded = false;

    public CutsceneStep[] steps = new CutsceneStep[0];
        public int stepsSize = 0;
        public bool stepsExpanded = false;
        public bool[] stepExpanded = new bool[0];
}

[System.Serializable]
public struct CutsceneStep
{
    public StepType stepType;

    // Motion parameters
    public int tranInd;
    public Vector3 mov;
    public Vector3 rot;
    public Vector3 scl;

    // Dialogue parameters
    public Dialogue dialogue;

    // Animation parameters
    public int animInd;
    public int state;

    // Wait parameters
    public float wait;

    // Song parameters
    public AudioClip song;

    // Load Scene parameters
    public int scene;
}

[System.Serializable]
public enum StepType
{
    motion,
    cameraMotion,
    dialogue,
    animation,
    wait,
    song,
    lockCam,
    loadScene
}

#if UNITY_EDITOR
[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Cutscene cutscene = target as Cutscene;

        cutscene.transformsExpanded = EditorGUILayout.Foldout(cutscene.transformsExpanded, "Transforms");
        if (cutscene.transformsExpanded)
        {
            EditorGUI.indentLevel++;
            cutscene.transformsSize = EditorGUILayout.DelayedIntField("Size", cutscene.transformsSize);
            if (cutscene.transforms.Length != cutscene.transformsSize)
            {
                Transform[] newArray = new Transform[cutscene.transformsSize];
                for (int i = 0; i < cutscene.transformsSize; i++)
                {
                    if (cutscene.transforms.Length > i)
                    {
                        newArray[i] = cutscene.transforms[i];
                    }
                }
                cutscene.transforms = newArray;
            }

            for (int i = 0; i < cutscene.transformsSize; i++)
            {
                cutscene.transforms[i] = (Transform)EditorGUILayout.ObjectField(cutscene.transforms[i], typeof(Transform), true);
            }

            EditorGUI.indentLevel--;
        }

        cutscene.animatorsExpanded = EditorGUILayout.Foldout(cutscene.animatorsExpanded, "Animators");
        if (cutscene.animatorsExpanded)
        {
            EditorGUI.indentLevel++;
            cutscene.animatorsSize = EditorGUILayout.DelayedIntField("Size", cutscene.animatorsSize);
            if (cutscene.animators.Length != cutscene.animatorsSize)
            {
                Animator[] newArray = new Animator[cutscene.animatorsSize];
                for (int i = 0; i < cutscene.animatorsSize; i++)
                {
                    if (cutscene.animators.Length > i)
                    {
                        newArray[i] = cutscene.animators[i];
                    }
                }
                cutscene.animators = newArray;
            }

            for (int i = 0; i < cutscene.animatorsSize; i++)
            {
                cutscene.animators[i] = (Animator)EditorGUILayout.ObjectField(cutscene.animators[i], typeof(Animator), true);
            }

            EditorGUI.indentLevel--;
        }

        cutscene.gameplayControllersExpanded = EditorGUILayout.Foldout(cutscene.gameplayControllersExpanded, "Gameplay Animator Controllers");
        if (cutscene.gameplayControllersExpanded)
        {
            EditorGUI.indentLevel++;
            cutscene.gameplayControllersSize = EditorGUILayout.DelayedIntField("Size", cutscene.gameplayControllersSize);
            if (cutscene.gameplayControllers.Length != cutscene.gameplayControllersSize)
            {
                RuntimeAnimatorController[] newArray = new RuntimeAnimatorController[cutscene.gameplayControllersSize];
                for (int i = 0; i < cutscene.gameplayControllersSize; i++)
                {
                    if (cutscene.gameplayControllers.Length > i)
                    {
                        newArray[i] = cutscene.gameplayControllers[i];
                    }
                }
                cutscene.gameplayControllers = newArray;
            }

            for (int i = 0; i < cutscene.gameplayControllersSize; i++)
            {
                cutscene.gameplayControllers[i] = (RuntimeAnimatorController)EditorGUILayout.ObjectField(cutscene.gameplayControllers[i], typeof(RuntimeAnimatorController), false);
            }

            EditorGUI.indentLevel--;
        }

        cutscene.cutsceneControllersExpanded = EditorGUILayout.Foldout(cutscene.cutsceneControllersExpanded, "Cutscene Animator Controllers");
        if (cutscene.cutsceneControllersExpanded)
        {
            EditorGUI.indentLevel++;
            cutscene.cutsceneControllersSize = EditorGUILayout.DelayedIntField("Size", cutscene.cutsceneControllersSize);
            if (cutscene.cutsceneControllers.Length != cutscene.cutsceneControllersSize)
            {
                RuntimeAnimatorController[] newArray = new RuntimeAnimatorController[cutscene.cutsceneControllersSize];
                for (int i = 0; i < cutscene.cutsceneControllersSize; i++)
                {
                    if (cutscene.cutsceneControllers.Length > i)
                    {
                        newArray[i] = cutscene.cutsceneControllers[i];
                    }
                }
                cutscene.cutsceneControllers = newArray;
            }

            for (int i = 0; i < cutscene.cutsceneControllersSize; i++)
            {
                cutscene.cutsceneControllers[i] = (RuntimeAnimatorController)EditorGUILayout.ObjectField(cutscene.cutsceneControllers[i], typeof(RuntimeAnimatorController), false);
            }

            EditorGUI.indentLevel--;
        }

        cutscene.stepsExpanded = EditorGUILayout.Foldout(cutscene.stepsExpanded, "Steps");
        if (cutscene.stepsExpanded)
        {
            EditorGUI.indentLevel++;
            cutscene.stepsSize = EditorGUILayout.DelayedIntField("Size", cutscene.stepsSize);
            if (cutscene.steps.Length != cutscene.stepsSize)
            {
                CutsceneStep[] newArray = new CutsceneStep[cutscene.stepsSize];
                for (int i = 0; i < cutscene.stepsSize; i++)
                {
                    if (cutscene.steps.Length > i)
                    {
                        newArray[i] = cutscene.steps[i];
                    }
                }
                cutscene.steps = newArray;

                bool[] newArray2 = new bool[cutscene.stepsSize];
                for (int i = 0; i < cutscene.stepsSize; i++)
                {
                    if (cutscene.stepExpanded.Length > i)
                    {
                        newArray2[i] = cutscene.stepExpanded[i];
                    }
                    else
                    {
                        newArray2[i] = false;
                    }
                }
                cutscene.stepExpanded = newArray2;
            }

            for (int i = 0; i < cutscene.steps.Length; i++)
            {
                cutscene.stepExpanded[i] = EditorGUILayout.Foldout(cutscene.stepExpanded[i], "Step " + i);
                if (cutscene.stepExpanded[i])
                {
                    EditorGUI.indentLevel++;
                    cutscene.steps[i].stepType = (StepType)EditorGUILayout.EnumPopup("Step Type", cutscene.steps[i].stepType);

                    switch (cutscene.steps[i].stepType)
                    {
                        case StepType.motion:
                            cutscene.steps[i].tranInd = EditorGUILayout.DelayedIntField("Transform Index", cutscene.steps[i].tranInd);
                            cutscene.steps[i].mov = EditorGUILayout.Vector3Field("Movement", cutscene.steps[i].mov);
                            cutscene.steps[i].rot = EditorGUILayout.Vector3Field("Rotation", cutscene.steps[i].rot);
                            cutscene.steps[i].scl = EditorGUILayout.Vector3Field("Scale", cutscene.steps[i].scl);
                            cutscene.steps[i].dialogue = null;
                            cutscene.steps[i].animInd = 0;
                            cutscene.steps[i].state = 0;
                            cutscene.steps[i].wait = EditorGUILayout.DelayedFloatField("Move Time", cutscene.steps[i].wait);
                            cutscene.steps[i].song = null;
                            cutscene.steps[i].scene = 0;
                            break;

                        case StepType.cameraMotion:
                            cutscene.steps[i].tranInd = 0;
                            cutscene.steps[i].mov = EditorGUILayout.Vector3Field("Movement", cutscene.steps[i].mov);
                            cutscene.steps[i].rot = Vector3.zero;
                            cutscene.steps[i].scl = Vector3.zero;
                            cutscene.steps[i].dialogue = null;
                            cutscene.steps[i].animInd = 0;
                            cutscene.steps[i].state = 0;
                            cutscene.steps[i].wait = EditorGUILayout.DelayedFloatField("Move Time", cutscene.steps[i].wait);
                            cutscene.steps[i].song = null;
                            cutscene.steps[i].scene = 0;
                            break;

                        case StepType.dialogue:
                            cutscene.steps[i].tranInd = 0;
                            cutscene.steps[i].mov = Vector3.zero;
                            cutscene.steps[i].rot = Vector3.zero;
                            cutscene.steps[i].scl = Vector3.zero;
                            cutscene.steps[i].dialogue = (Dialogue)EditorGUILayout.ObjectField("Dialogue", cutscene.steps[i].dialogue, typeof(Dialogue), true);
                            cutscene.steps[i].animInd = 0;
                            cutscene.steps[i].state = 0;
                            cutscene.steps[i].wait = 0;
                            cutscene.steps[i].song = null;
                            cutscene.steps[i].scene = 0;
                            break;

                        case StepType.animation:
                            cutscene.steps[i].tranInd = 0;
                            cutscene.steps[i].mov = Vector3.zero;
                            cutscene.steps[i].rot = Vector3.zero;
                            cutscene.steps[i].scl = Vector3.zero;
                            cutscene.steps[i].dialogue = null;
                            cutscene.steps[i].animInd = EditorGUILayout.DelayedIntField("Animator Index", cutscene.steps[i].animInd);
                            cutscene.steps[i].state = EditorGUILayout.DelayedIntField("Animation State", cutscene.steps[i].state);
                            cutscene.steps[i].wait = 0;
                            cutscene.steps[i].song = null;
                            cutscene.steps[i].scene = 0;
                            break;

                        case StepType.wait:
                            cutscene.steps[i].tranInd = 0;
                            cutscene.steps[i].mov = Vector3.zero;
                            cutscene.steps[i].rot = Vector3.zero;
                            cutscene.steps[i].scl = Vector3.zero;
                            cutscene.steps[i].dialogue = null;
                            cutscene.steps[i].animInd = 0;
                            cutscene.steps[i].state = 0;
                            cutscene.steps[i].wait = EditorGUILayout.DelayedFloatField("Wait Time", cutscene.steps[i].wait);
                            cutscene.steps[i].song = null;
                            cutscene.steps[i].scene = 0;
                            break;

                        case StepType.song:
                            cutscene.steps[i].tranInd = 0;
                            cutscene.steps[i].mov = Vector3.zero;
                            cutscene.steps[i].rot = Vector3.zero;
                            cutscene.steps[i].scl = Vector3.zero;
                            cutscene.steps[i].dialogue = null;
                            cutscene.steps[i].animInd = 0;
                            cutscene.steps[i].state = 0;
                            cutscene.steps[i].wait = 0;
                            cutscene.steps[i].song = (AudioClip)EditorGUILayout.ObjectField(cutscene.steps[i].song, typeof(AudioClip), false);
                            cutscene.steps[i].scene = 0;
                            break;

                        case StepType.loadScene:
                            cutscene.steps[i].tranInd = 0;
                            cutscene.steps[i].mov = Vector3.zero;
                            cutscene.steps[i].rot = Vector3.zero;
                            cutscene.steps[i].scl = Vector3.zero;
                            cutscene.steps[i].dialogue = null;
                            cutscene.steps[i].animInd = 0;
                            cutscene.steps[i].state = 0;
                            cutscene.steps[i].wait = 0;
                            cutscene.steps[i].song = null;
                            cutscene.steps[i].scene = EditorGUILayout.DelayedIntField("Scene Index", cutscene.steps[i].scene);
                            break;

                        default:
                            cutscene.steps[i].tranInd = 0;
                            cutscene.steps[i].mov = Vector3.zero;
                            cutscene.steps[i].rot = Vector3.zero;
                            cutscene.steps[i].scl = Vector3.zero;
                            cutscene.steps[i].dialogue = null;
                            cutscene.steps[i].animInd = 0;
                            cutscene.steps[i].state = 0;
                            cutscene.steps[i].wait = 0;
                            cutscene.steps[i].song = null;
                            cutscene.steps[i].scene = 0;
                            break;
                    }
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUI.indentLevel--;
        }
    }
}
#endif