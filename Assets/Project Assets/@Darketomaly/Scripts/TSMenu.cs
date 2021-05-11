using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEditor;

[System.Serializable]
public class Selectable {

    public Button menuButton;
    public CanvasGroup view;
}

public class TSMenu : MonoBehaviour {

    public Image underline;
    public List<Selectable> selectables = new List<Selectable>();

    public Light menuLight;
    public Light viewLight;
    public Material menuLightMat;
    public Material viewLightMat;
    public CanvasGroup menuCanvasGroup;

    public Transform door;
    public Controller controller;

    [Header("Audio")]
    public AudioClip selectionClip;
    public AudioSource selectionAudioSource;
    public AudioClip doorOpenClip;
    public AudioClip doorCloseClip;
    public AudioSource doorAudioSource;

    private int currentSelectableIndex;
    private bool enteredSelection;

    [Header("In-game Menu")]
    public CanvasGroup fadeCanvas;
    private bool gameInitialized = false;
    private Vector3 agentDefaultPos;

    private bool _6thAxisInUse = false; //PC 7 Axis, Android 6
    private bool _0InUse = false;
    private bool _1InUse = false;
    private bool _10InUse = false; //PC 7, Android 10

    private Tween doorOpenTween;

    public void Start() {

        SelectFromMenu(0);
        agentDefaultPos = controller.gameObject.transform.position;
    }

    public void Update() {

#if UNITY_ANDROID

        if (!gameInitialized) {

            if (!enteredSelection) {

                if (Input.GetAxisRaw("6th Axis") != 0) { //up/down

                    if (!_6thAxisInUse) {

                        if (Input.GetAxisRaw("6th Axis") < 0) //up
                            SelectFromMenu(currentSelectableIndex - 1);
                        else //down
                            SelectFromMenu(currentSelectableIndex + 1);

                        _6thAxisInUse = true;
                    }

                } else {

                    if (_6thAxisInUse) _6thAxisInUse = false;

                    if (Input.GetAxisRaw("0") != 0) { //enter

                        if (!_0InUse) {

                            EnterSelection(true);
                            _0InUse = true;
                        }

                    } else
                        if (_0InUse) _0InUse = false;
                }

            } else if (Input.GetAxisRaw("1") != 0) { //return

                if (!_1InUse) {

                    EnterSelection(false);
                    _1InUse = true;
                }

            } else
                if (_1InUse) _1InUse = false;

        } else {

            if (Input.GetAxisRaw("10") != 0) {

                if (!_10InUse) {

                    StartCoroutine(RestartGame());
                    _10InUse = true;
                }
            } else if (_10InUse) _10InUse = false;
        }

#endif

#if UNITY_EDITOR

        if (!gameInitialized) {

            if (!enteredSelection) {

                if (Input.GetKeyDown(KeyCode.W))
                    SelectFromMenu(currentSelectableIndex - 1);
                else if (Input.GetKeyDown(KeyCode.S))
                    SelectFromMenu(currentSelectableIndex + 1);
                else if (Input.GetKeyDown(KeyCode.Return))
                    EnterSelection(true);
            } else if (Input.GetKeyDown(KeyCode.Escape))
                EnterSelection(false);

        } else if (Input.GetKeyDown(KeyCode.Escape))
            StartCoroutine(RestartGame());
#endif
    }

        public IEnumerator RestartGame() {

        fadeCanvas.gameObject.SetActive(true);

        //control in-game menu
        fadeCanvas.DOFade(1.0f, 0.25f);
        yield return new WaitForSeconds(0.25f);

        controller.agent.Warp(agentDefaultPos);

        while (controller.gameObject.transform.position != agentDefaultPos)
            yield return null; //agent things...

        controller.enabled = false;
        doorOpenTween.Kill();
        door.rotation = new Quaternion();
        AsyncOperation op = SceneManager.UnloadSceneAsync(1);
        while(!op.isDone)
            yield return null;

        menuCanvasGroup.alpha = 1.0f;
        menuLight.intensity = 1.0f;
        menuLightMat.DOColor(Color.white * 1.0f, "_EmissionColor", 0.0f);

        doorAudioSource.PlayOneShot(doorCloseClip);

        fadeCanvas.DOFade(0.0f, 0.25f).OnComplete(delegate {

            gameInitialized = false;
            enteredSelection = false;
            fadeCanvas.gameObject.SetActive(false);
        });
    }

    public void SelectFromMenu(int desiredSelectableIndex) {

        selectionAudioSource.PlayOneShot(selectionClip);

        if (desiredSelectableIndex > selectables.Count - 1) currentSelectableIndex = 0;
        else if (desiredSelectableIndex < 0) currentSelectableIndex = selectables.Count - 1;
        else currentSelectableIndex = desiredSelectableIndex;

        underline.GetComponent<RectTransform>().localPosition = 
            selectables[currentSelectableIndex].menuButton.GetComponent<RectTransform>().localPosition;

        underline.fillAmount = 0.0f;
        underline.DOKill();
        underline.DOFillAmount(1.0f, 0.30f);
    }
    
    public void EnterSelection(bool value) {

        selectables[currentSelectableIndex].menuButton.onClick.Invoke();
        if (currentSelectableIndex == 0 || currentSelectableIndex == selectables.Count - 1) return;
            
        enteredSelection = value;

        float menuLightIntensity = value ? 0.0f : 1.0f;
        float viewLightIntensity = value ? 1.0f : 0.0f;

        menuLight.DOIntensity(menuLightIntensity, 0.25f);
        viewLight.DOIntensity(viewLightIntensity, 0.25f);

        menuLightMat.DOColor(Color.white * menuLightIntensity, "_EmissionColor", 0.25f);
        viewLightMat.DOColor(Color.white * viewLightIntensity, "_EmissionColor", 0.25f);

        selectables[currentSelectableIndex].view.DOFade(viewLightIntensity, 0.25f);
    }

    public void LoadWorld() {

        enteredSelection = true;
        StartCoroutine(_LoadWorld());

        menuLight.DOIntensity(0.0f, 0.25f);
        viewLight.DOIntensity(0.0f, 0.25f);

        menuLightMat.DOColor(Color.white * 0.0f, "_EmissionColor", 0.25f);
        viewLightMat.DOColor(Color.white * 0.0f, "_EmissionColor", 0.25f);

        selectables[currentSelectableIndex].view.DOFade(0.0f, 0.25f);
        menuCanvasGroup.DOFade(0.0f, 0.25f);
    }

    private IEnumerator _LoadWorld() {

        AsyncOperation op = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        while (!op.isDone) 
            yield return null;

        controller.enabled = true;
        gameInitialized = true;

        doorOpenTween = door.DORotate(new Vector3(0.0f, -77.3f, 0.0f), 2.4f);
        doorAudioSource.PlayOneShot(doorOpenClip);
    }

    public void OnDestroy() {

        //Reset lights
        if(menuLightMat && viewLightMat && menuLight && viewLight) {

            menuLightMat.SetColor("_EmissionColor", Color.white * (menuLight.intensity = 1.0f));
            viewLightMat.SetColor("_EmissionColor", Color.white * (viewLight.intensity = 0.0f));
        }
    }

    public void QuitApplication() {

        Application.Quit();

        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #endif
    }
}
