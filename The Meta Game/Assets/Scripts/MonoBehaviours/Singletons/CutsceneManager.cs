using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager singleton;
    public Cutscene currentScene;

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
        currentScene = Instantiate(cutscene);
        StartCoroutine(RunCutscene());
    }

    private IEnumerator RunCutscene()
    {
        bool lockCam = false;

        GameController.singleton.SetPaused(true);

        Camera.main.GetComponent<CameraScroll>().enabled = false;
        
        GameObject gTemp;
        Animator aTemp;

        for (int i = 0; i < currentScene.transformsSize; i++)
        {
            gTemp = GameObject.Find(currentScene.transNames[i]);
            if (gTemp != null)
            {
                currentScene.transforms[i] = gTemp.GetComponent<Transform>();
            }
        }

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
                        GameController.singleton.StartCoroutine(GameController.singleton.FadeAndLoad(currentScene.steps[i].scene));
                    }
                    StopAllCoroutines();
                    break;

                case StepType.switchMode:
                    GameController.singleton.SwitchMode(currentScene.steps[i].mode);
                    GameController.singleton.SetPaused(true);
                    Camera.main.GetComponent<CameraScroll>().enabled = false;
                    break;

                case StepType.unlockMode:
                    GameController.singleton.Unlock(currentScene.steps[i].mode);
                    GameController.singleton.ToggleSwitchPanel(true);
                    yield return new WaitForSeconds(GameController.singleton.unlockWaitTime + GameController.singleton.unlockFadeTime);
                    break;
            }
        }

        for (int i = 0; i < currentScene.animatorsSize; i++)
        {
            currentScene.animators[i].runtimeAnimatorController = currentScene.gameplayControllers[i];
            if (currentScene.animNames[i].Equals("Player"))
            {
                currentScene.animators[i].SetBool("platformer", true);
                currentScene.animators[i].SetBool("moving", false);
            }
        }
        
        Camera.main.GetComponent<CameraScroll>().enabled = !lockCam;

        GameController.singleton.SetPaused(false);

        yield return new WaitForEndOfFrame();
        currentScene = null;
        yield return null;
    }

    private IEnumerator TransformMove(Transform trans, Vector3 mov, Vector3 rot, Vector3 scl, float wait)
    {
        float invTime = 1.0f / wait;
        float maxDistMov = Vector3.Distance(trans.position, mov) * invTime;
        float maxDistRot = Vector3.Distance(trans.rotation.eulerAngles, rot) * invTime;
        float maxDistScl = Vector3.Distance(trans.localScale, scl) * invTime;

        for (float t = 0; t <= wait; t += Time.deltaTime)
        {
            trans.position = Vector3.MoveTowards(trans.position, mov, maxDistMov * Time.deltaTime);
            trans.rotation = Quaternion.Euler(Vector3.MoveTowards(trans.rotation.eulerAngles, rot, maxDistRot * Time.deltaTime));
            trans.localScale = Vector3.MoveTowards(trans.localScale, scl, maxDistScl);

            // trans.Translate(mov * invTime * Time.deltaTime);
            // trans.Rotate(rot * invTime * Time.deltaTime);
            // trans.localScale += scl * invTime * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator CameraMove(Vector3 mov, float wait)
    {
        float invTime = 1.0f / wait;
        float maxDist = Vector3.Distance(Camera.main.transform.position, mov) * invTime;

        for (float t = 0; t <= wait; t += Time.deltaTime)
        {
            float xDiff = -Camera.main.transform.position.x;
            float yDiff = -Camera.main.transform.position.y;

            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, mov, maxDist * Time.deltaTime);

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
