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

//Most of it is awfully hard-coded for the sake of speed completion
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
    public CanvasGroup menuPanel;
    public RectTransform arrow;
    public List<Button> inGameMenuButtons = new List<Button>();
    private bool gameInitialized = false;
    private Vector3 agentDefaultPos;
    public bool inGameMenuEnabled = false;
    private int currentInGameMenuButtonSelectionIndex;
    private bool restarting = false;

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

            if (Input.GetAxisRaw("10") != 0) { //same as ESC

                if (!_10InUse) { 

                    if (gameInitialized && menuLight.intensity == 0.0f && !restarting)
                        DisplayMenu();
                    _10InUse = true;
                }
            } else if (_10InUse) _10InUse = false;

            if(inGameMenuEnabled) {

                if (Input.GetAxisRaw("6th Axis") != 0) { //up/down

                    if (!_6thAxisInUse) {

                        if (Input.GetAxisRaw("6th Axis") < 0) //up
                            SelectFromInGameMenu(currentInGameMenuButtonSelectionIndex - 1);
                        else //down
                            SelectFromInGameMenu(currentInGameMenuButtonSelectionIndex + 1);

                        _6thAxisInUse = true;
                    }

                } else {

                    if (_6thAxisInUse) _6thAxisInUse = false;

                    if (Input.GetAxisRaw("0") != 0) { //enter

                        if (!_0InUse) {

                            inGameMenuButtons[currentInGameMenuButtonSelectionIndex].onClick.Invoke();
                            _0InUse = true;
                        }

                    } else if (_0InUse) _0InUse = false;
                }
            }
        }

#endif

#if UNITY_EDITOR
        if (!gameInitialized) {

            if (!enteredSelection) {

                if (!inGameMenuEnabled) {

                    if (Input.GetKeyDown(KeyCode.W))
                        SelectFromMenu(currentSelectableIndex - 1);
                    else if (Input.GetKeyDown(KeyCode.S))
                        SelectFromMenu(currentSelectableIndex + 1);
                    else if (Input.GetKeyDown(KeyCode.Return))
                        EnterSelection(true);

                } else {

                    Debug.Log("called");


                }


            } else if (Input.GetKeyDown(KeyCode.Escape))
                EnterSelection(false);

        } else {

            if (Input.GetKeyDown(KeyCode.Escape)) {

                if(gameInitialized && menuLight.intensity == 0.0f && !restarting)
                    DisplayMenu();
            }

            if(inGameMenuEnabled) {

                //control in-game menu
                if (Input.GetKeyDown(KeyCode.W))
                    SelectFromInGameMenu(currentInGameMenuButtonSelectionIndex - 1);
                else if (Input.GetKeyDown(KeyCode.S))
                    SelectFromInGameMenu(currentInGameMenuButtonSelectionIndex + 1);
                else if (Input.GetKeyDown(KeyCode.Return))
                    inGameMenuButtons[currentInGameMenuButtonSelectionIndex].onClick.Invoke();
            }
        }
#endif
    }

    public void DisplayMenu() {//ingame menu

        if (!inGameMenuEnabled) {

            menuPanel.DOFade(1.0f, 0.25f);
            inGameMenuEnabled = true;
            SelectFromInGameMenu(0);
            fadeCanvas.DOFade(0.5f, 0.25f);
            
        } else {

            menuPanel.DOFade(0.0f, 0.25f);
            inGameMenuEnabled = false;
            fadeCanvas.DOFade(0.0f, 0.25f);
        }
    }

    public void SelectFromInGameMenu(int index) {

        currentInGameMenuButtonSelectionIndex = index;
        if (index > inGameMenuButtons.Count - 1) currentInGameMenuButtonSelectionIndex = 0;
        else if (index < 0) currentInGameMenuButtonSelectionIndex = inGameMenuButtons.Count - 1;

        arrow.DOLocalMove(inGameMenuButtons[currentInGameMenuButtonSelectionIndex].GetComponent<RectTransform>().localPosition, 0.25f);
    }

    public void _RestartGame() {
        StartCoroutine(RestartGame());
        menuPanel.DOFade(0.0f, 0.25f);
        inGameMenuEnabled = false;
        restarting = true;
    }
    public IEnumerator RestartGame() {

        fadeCanvas.gameObject.SetActive(true);

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
            restarting = false;
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

    public void QuitApplication() {

        Application.Quit();

        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #endif
    }
}
