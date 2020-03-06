using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NewCutscene", menuName = "ScriptableObjects/Cutscene", order = 1)]
public class Cutscene : ScriptableObject
{
    public Transform[] transforms = new Transform[0];
        public string[] transNames = new string[0];
        public int transformsSize = 0;
        public bool transformsExpanded = false;

    public Animator[] animators = new Animator[0];
        public string[] animNames = new string[0];
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

    // Switch Mode parameters
    public GameController.GameMode mode;
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
    loadScene,
    switchMode
}

#if UNITY_EDITOR
[CustomEditor(typeof(Cutscene))]
[CanEditMultipleObjects]
public class CutsceneEditor : Editor
{

    SerializedProperty transformsProp;
    SerializedProperty tNamesProp;
    SerializedProperty tSizeProp;

    SerializedProperty animatorsProp;
    SerializedProperty aNamesProp;
    SerializedProperty aSizeProp;

    SerializedProperty gameplayConsProp;
    SerializedProperty gConSizeProp;

    SerializedProperty cutsceneConsProp;
    SerializedProperty cConSizeProp;

    SerializedProperty stepsProp;
    SerializedProperty stepsSizeProp;

    private void OnEnable()
    {
        transformsProp = serializedObject.FindProperty("transforms");
        tNamesProp = serializedObject.FindProperty("transNames");
        tSizeProp = serializedObject.FindProperty("transformsSize");

        animatorsProp = serializedObject.FindProperty("animators");
        aNamesProp = serializedObject.FindProperty("animNames");
        aSizeProp = serializedObject.FindProperty("animatorsSize");

        gameplayConsProp = serializedObject.FindProperty("gameplayControllers");
        gConSizeProp = serializedObject.FindProperty("gameplayControllersSize");

        cutsceneConsProp = serializedObject.FindProperty("cutsceneControllers");
        cConSizeProp = serializedObject.FindProperty("cutsceneControllersSize");

        stepsProp = serializedObject.FindProperty("steps");
        stepsSizeProp = serializedObject.FindProperty("stepsSize");
    }

    public override void OnInspectorGUI()
    {
        Cutscene cutscene = target as Cutscene;

        cutscene.transformsExpanded = EditorGUILayout.Foldout(cutscene.transformsExpanded, "Transforms");
        if (cutscene.transformsExpanded)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(tSizeProp, new GUIContent("Size"));
            if (transformsProp.arraySize != tSizeProp.intValue)
            {
                transformsProp.arraySize = tSizeProp.intValue;
                tNamesProp.arraySize = tSizeProp.intValue;
            }

            for (int i = 0; i < tSizeProp.intValue; i++)
            {
                EditorGUILayout.PropertyField(tNamesProp.GetArrayElementAtIndex(i), new GUIContent("Object Name"));
                GameObject temp = GameObject.Find(tNamesProp.GetArrayElementAtIndex(i).stringValue);
                if (temp != null)
                {
                    transformsProp.GetArrayElementAtIndex(i).objectReferenceValue = temp.GetComponent<Transform>();
                }
            }

            EditorGUI.indentLevel--;
        }

        cutscene.animatorsExpanded = EditorGUILayout.Foldout(cutscene.animatorsExpanded, "Animators");
        if (cutscene.animatorsExpanded)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(aSizeProp, new GUIContent("Size"));
            if (animatorsProp.arraySize != aSizeProp.intValue)
            {
                animatorsProp.arraySize = aSizeProp.intValue;
                aNamesProp.arraySize = aSizeProp.intValue;
            }

            for (int i = 0; i < aSizeProp.intValue; i++)
            {
                EditorGUILayout.PropertyField(aNamesProp.GetArrayElementAtIndex(i), new GUIContent("Object Name"));
                GameObject temp = GameObject.Find(aNamesProp.GetArrayElementAtIndex(i).stringValue);
                if (temp != null)
                {
                    Animator aTemp = temp.GetComponentInChildren<Animator>();
                    if (aTemp != null)
                    {
                        animatorsProp.GetArrayElementAtIndex(i).objectReferenceValue = aTemp;
                    }
                }
            }

            EditorGUI.indentLevel--;
        }

        cutscene.gameplayControllersExpanded = EditorGUILayout.Foldout(cutscene.gameplayControllersExpanded, "Gameplay Animator Controllers");
        if (cutscene.gameplayControllersExpanded)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(gConSizeProp, new GUIContent("Size"));
            if (gameplayConsProp.arraySize != gConSizeProp.intValue)
            {
                gameplayConsProp.arraySize = gConSizeProp.intValue;
            }

            for (int i = 0; i < gConSizeProp.intValue; i++)
            {
                EditorGUILayout.PropertyField(gameplayConsProp.GetArrayElementAtIndex(i));
            }

            EditorGUI.indentLevel--;
        }

        cutscene.cutsceneControllersExpanded = EditorGUILayout.Foldout(cutscene.cutsceneControllersExpanded, "Cutscene Animator Controllers");
        if (cutscene.cutsceneControllersExpanded)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(cConSizeProp, new GUIContent("Size"));
            if (cutsceneConsProp.arraySize != cConSizeProp.intValue)
            {
                cutsceneConsProp.arraySize = cConSizeProp.intValue;
            }

            for (int i = 0; i < cConSizeProp.intValue; i++)
            {
                EditorGUILayout.PropertyField(cutsceneConsProp.GetArrayElementAtIndex(i));
            }

            EditorGUI.indentLevel--;
        }

        CutsceneStep[] steps = new CutsceneStep[0];

        cutscene.stepsExpanded = EditorGUILayout.Foldout(cutscene.stepsExpanded, "Steps");
        if (cutscene.stepsExpanded)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(stepsSizeProp, new GUIContent("Size"));
            if (stepsProp.arraySize != stepsSizeProp.intValue)
            {
                stepsProp.arraySize = stepsSizeProp.intValue;
            }

            bool[] newArray2 = new bool[stepsSizeProp.intValue];
            for (int i = 0; i < stepsSizeProp.intValue; i++)
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

            for (int i = 0; i < stepsSizeProp.intValue; i++)
            {
                cutscene.stepExpanded[i] = EditorGUILayout.Foldout(cutscene.stepExpanded[i], "Step " + i);
                if (cutscene.stepExpanded[i])
                {
                    EditorGUI.indentLevel++;

                    SerializedProperty stepTypeProp = stepsProp.GetArrayElementAtIndex(i).FindPropertyRelative("stepType");

                    SerializedProperty tranIndProp = stepsProp.GetArrayElementAtIndex(i).FindPropertyRelative("tranInd");
                    SerializedProperty movProp = stepsProp.GetArrayElementAtIndex(i).FindPropertyRelative("mov");
                    SerializedProperty rotProp = stepsProp.GetArrayElementAtIndex(i).FindPropertyRelative("rot");
                    SerializedProperty sclProp = stepsProp.GetArrayElementAtIndex(i).FindPropertyRelative("scl");

                    SerializedProperty dialogueProp = stepsProp.GetArrayElementAtIndex(i).FindPropertyRelative("dialogue");

                    SerializedProperty animIndProp = stepsProp.GetArrayElementAtIndex(i).FindPropertyRelative("animInd");
                    SerializedProperty stateProp = stepsProp.GetArrayElementAtIndex(i).FindPropertyRelative("state");

                    SerializedProperty waitProp = stepsProp.GetArrayElementAtIndex(i).FindPropertyRelative("wait");

                    SerializedProperty songProp = stepsProp.GetArrayElementAtIndex(i).FindPropertyRelative("song");

                    SerializedProperty sceneProp = stepsProp.GetArrayElementAtIndex(i).FindPropertyRelative("scene");

                    SerializedProperty modeProp = stepsProp.GetArrayElementAtIndex(i).FindPropertyRelative("mode");

                    EditorGUILayout.PropertyField(stepTypeProp, new GUIContent("Step Type"));

                    switch ((StepType)stepTypeProp.enumValueIndex)
                    {
                        case StepType.motion:
                            EditorGUILayout.PropertyField(tranIndProp, new GUIContent("Transform Index"));
                            EditorGUILayout.PropertyField(movProp, new GUIContent("End Position"));
                            EditorGUILayout.PropertyField(rotProp, new GUIContent("End Rotation"));
                            EditorGUILayout.PropertyField(sclProp, new GUIContent("End Scale"));
                            dialogueProp.objectReferenceValue = null;
                            animIndProp.intValue = 0;
                            stateProp.intValue = 0;
                            EditorGUILayout.PropertyField(waitProp, new GUIContent("Move Time"));
                            songProp.objectReferenceValue = null;
                            sceneProp.intValue = 0;
                            modeProp.enumValueIndex = 0;
                            break;

                        case StepType.cameraMotion:
                            tranIndProp.intValue = 0;
                            EditorGUILayout.PropertyField(movProp, new GUIContent("End Position"));
                            rotProp.vector3Value = Vector3.zero;
                            sclProp.vector3Value = Vector3.one;
                            dialogueProp.objectReferenceValue = null;
                            animIndProp.intValue = 0;
                            stateProp.intValue = 0;
                            EditorGUILayout.PropertyField(waitProp, new GUIContent("Move Time"));
                            songProp.objectReferenceValue = null;
                            sceneProp.intValue = 0;
                            modeProp.enumValueIndex = 0;
                            break;

                        case StepType.dialogue:
                            tranIndProp.intValue = 0;
                            movProp.vector3Value = Vector3.zero;
                            rotProp.vector3Value = Vector3.zero;
                            sclProp.vector3Value = Vector3.one;
                            EditorGUILayout.PropertyField(dialogueProp, new GUIContent("Dialogue"));
                            animIndProp.intValue = 0;
                            stateProp.intValue = 0;
                            waitProp.floatValue = 0;
                            songProp.objectReferenceValue = null;
                            sceneProp.intValue = 0;
                            modeProp.enumValueIndex = 0;
                            break;

                        case StepType.animation:
                            tranIndProp.intValue = 0;
                            movProp.vector3Value = Vector3.zero;
                            rotProp.vector3Value = Vector3.zero;
                            sclProp.vector3Value = Vector3.one;
                            dialogueProp.objectReferenceValue = null;
                            EditorGUILayout.PropertyField(animIndProp, new GUIContent("Animator Index"));
                            EditorGUILayout.PropertyField(stateProp, new GUIContent("Animation State"));
                            waitProp.floatValue = 0;
                            songProp.objectReferenceValue = null;
                            sceneProp.intValue = 0;
                            modeProp.enumValueIndex = 0;
                            break;

                        case StepType.wait:
                            tranIndProp.intValue = 0;
                            movProp.vector3Value = Vector3.zero;
                            rotProp.vector3Value = Vector3.zero;
                            sclProp.vector3Value = Vector3.one;
                            dialogueProp.objectReferenceValue = null;
                            animIndProp.intValue = 0;
                            stateProp.intValue = 0;
                            EditorGUILayout.PropertyField(waitProp, new GUIContent("Wait Time"));
                            songProp.objectReferenceValue = null;
                            sceneProp.intValue = 0;
                            modeProp.enumValueIndex = 0;
                            break;

                        case StepType.song:
                            tranIndProp.intValue = 0;
                            movProp.vector3Value = Vector3.zero;
                            rotProp.vector3Value = Vector3.zero;
                            sclProp.vector3Value = Vector3.one;
                            dialogueProp.objectReferenceValue = null;
                            animIndProp.intValue = 0;
                            stateProp.intValue = 0;
                            waitProp.floatValue = 0;
                            EditorGUILayout.PropertyField(songProp, new GUIContent("Song Clip"));
                            sceneProp.intValue = 0;
                            modeProp.enumValueIndex = 0;
                            break;

                        case StepType.loadScene:
                            tranIndProp.intValue = 0;
                            movProp.vector3Value = Vector3.zero;
                            rotProp.vector3Value = Vector3.zero;
                            sclProp.vector3Value = Vector3.one;
                            dialogueProp.objectReferenceValue = null;
                            animIndProp.intValue = 0;
                            stateProp.intValue = 0;
                            waitProp.floatValue = 0;
                            songProp.objectReferenceValue = null;
                            EditorGUILayout.PropertyField(sceneProp, new GUIContent("Scene Index"));
                            modeProp.enumValueIndex = 0;
                            break;

                        case StepType.switchMode:
                            tranIndProp.intValue = 0;
                            movProp.vector3Value = Vector3.zero;
                            rotProp.vector3Value = Vector3.zero;
                            sclProp.vector3Value = Vector3.one;
                            dialogueProp.objectReferenceValue = null;
                            animIndProp.intValue = 0;
                            stateProp.intValue = 0;
                            waitProp.floatValue = 0;
                            songProp.objectReferenceValue = null;
                            sceneProp.intValue = 0;
                            EditorGUILayout.PropertyField(modeProp, new GUIContent("Mode"));
                            break;

                        default:
                            tranIndProp.intValue = 0;
                            movProp.vector3Value = Vector3.zero;
                            rotProp.vector3Value = Vector3.zero;
                            sclProp.vector3Value = Vector3.one;
                            dialogueProp.objectReferenceValue = null;
                            animIndProp.intValue = 0;
                            stateProp.intValue = 0;
                            waitProp.floatValue = 0;
                            songProp.objectReferenceValue = null;
                            sceneProp.intValue = 0;
                            modeProp.enumValueIndex = 0;
                            break;
                    }

                    EditorGUI.indentLevel--;
                }
            }
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif