using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager singleton;
    private Cutscene currentScene;

    private void Start()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartScene(Cutscene cutscene)
    {
        currentScene = cutscene;
        StartCoroutine(RunCutscene());
    }

    private IEnumerator RunCutscene()
    {
        bool lockCam = false;

        GameController.singleton.SetPaused(true);

        CameraScroll camScroll = Camera.main.GetComponent<CameraScroll>();
        camScroll.enabled = false;
        
        GameObject gTemp;
        Animator aTemp;

        for (int i = 0; i < currentScene.animatorsSize; i++)
        {
            gTemp = GameObject.Find(currentScene.animNames[i]);
            if (gTemp != null)
            {
                aTemp = gTemp.GetComponentInChildren<Animator>();
                if (aTemp != null)
                {
                    currentScene.animators[i] = aTemp;
                }
            }
        }

        for (int i = 0; i < currentScene.animatorsSize; i++)
        {
            currentScene.animators[i].runtimeAnimatorController = currentScene.cutsceneControllers[i];

            Rigidbody2D temp;
            if (currentScene.transforms[i].TryGetComponent(out temp))
            {
                temp.velocity = Vector2.zero;
            }
        }

        for (int i = 0; i < currentScene.stepsSize; i++)
        {
            Debug.Log(i + "/" + currentScene.stepsSize);
            switch (currentScene.steps[i].stepType)
            {
                case StepType.motion:
                    for (int j = 0; j < currentScene.transformsSize; j++)
                    {
                        gTemp = GameObject.Find(currentScene.transNames[j]);
                        if (gTemp != null)
                        {
                            currentScene.transforms[j] = gTemp.GetComponent<Transform>();
                        }
                    }

                    StartCoroutine(TransformMove(currentScene.transforms[currentScene.steps[i].tranInd], 
                        currentScene.steps[i].mov, 
                        currentScene.steps[i].rot, 
                        currentScene.steps[i].scl, 
                        currentScene.steps[i].wait
                        ));
                    break;

                case StepType.cameraMotion:
                    StartCoroutine(CameraMove(currentScene.steps[i].mov, currentScene.steps[i].wait));
                    break;

                case StepType.dialogue:
                    DialogueManager.singleton.StartDialogue(currentScene.steps[i].dialogue);
                    yield return new WaitUntil(() => !DialogueManager.singleton.GetDisplaying());
                    break;

                case StepType.animation:
                    for (int j = 0; j < currentScene.animatorsSize; j++)
                    {
                        gTemp = GameObject.Find(currentScene.animNames[j]);
                        if (gTemp != null)
                        {
                            aTemp = gTemp.GetComponentInChildren<Animator>();
                            if (aTemp != null)
                            {
                                currentScene.animators[j] = aTemp;
                            }
                        }
                    }

                    currentScene.animators[currentScene.steps[i].animInd].SetInteger("state", currentScene.steps[i].state);
                    break;

                case StepType.wait:
                    yield return new WaitForSeconds(currentScene.steps[i].wait);
                    break;

                case StepType.song:
                    AudioSource source = GameObject.Find("Song").GetComponent<AudioSource>();
                    source.clip = currentScene.steps[i].song;
                    source.Play();
                    break;

                case StepType.lockCam:
                    lockCam = true;
                    break;

                case StepType.loadScene:
                    if (currentScene.steps[i].scene == 0)
                    {
                        GameController.singleton.ReturnToMenu();
                    }
                    else
                    {
                        GameController.singleton.FadeAndLoad(currentScene.steps[i].scene);
                    }
                    break;
            }
        }

        Debug.Log("test");

        for (int i = 0; i < currentScene.animatorsSize; i++)
        {
            currentScene.animators[i].runtimeAnimatorController = currentScene.gameplayControllers[i];
            if (currentScene.animNames[i].Equals("Player"))
            {
                currentScene.animators[i].SetBool("platformer", true);
                currentScene.animators[i].SetBool("moving", false);
            }
        }

        if (!lockCam)
        {
            camScroll.enabled = true;
        }

        Debug.Log("Gets here?");
        GameController.singleton.SetPaused(false);

        currentScene = null;
        yield return null;
    }

    private IEnumerator TransformMove(Transform trans, Vector3 mov, Vector3 rot, Vector3 scl, float wait)
    {
        float invTime = 1.0f / wait;

        for (float t = 0; t <= wait; t += Time.deltaTime)
        {
            trans.Translate(mov * invTime * Time.deltaTime);
            trans.Rotate(rot * invTime * Time.deltaTime);
            trans.localScale += scl * invTime * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator CameraMove(Vector3 mov, float wait)
    {
        float invTime = 1.0f / wait;

        for (float t = 0; t <= wait; t += Time.deltaTime)
        {
            float xDiff = -Camera.main.transform.position.x;
            float yDiff = -Camera.main.transform.position.y;

            Camera.main.transform.Translate(mov * invTime * Time.deltaTime);

            xDiff += Camera.main.transform.position.x;
            yDiff += Camera.main.transform.position.y;

            Parallax[] pObjs = FindObjectsOfType<Parallax>();
            foreach (Parallax obj in pObjs)
            {
                obj.UpdatePos(xDiff, yDiff);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
