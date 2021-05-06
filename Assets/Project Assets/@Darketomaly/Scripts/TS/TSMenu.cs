using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

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

    private int currentSelectableIndex;
    private bool enteredSelection;

    private bool _7thAxisInUse = false;
    private bool _0thAxisInUse = false;
    private bool _1thAxisInUse = false;

    public void Start() {

        SelectFromMenu(0);
    }

    public void Update() {

        if (!enteredSelection) {

            if (Input.GetAxisRaw("7th Axis") != 0) { //up/down

                if (!_7thAxisInUse) {

                    if (Input.GetAxisRaw("7th Axis") > 0) //up
                        SelectFromMenu(currentSelectableIndex - 1);
                    else //down
                        SelectFromMenu(currentSelectableIndex + 1);

                    _7thAxisInUse = true;
                }

            } else {

                if (_7thAxisInUse) _7thAxisInUse = false;

                if (Input.GetAxisRaw("0") != 0) { //enter

                    if (!_0thAxisInUse) {

                        EnterSelection(true);
                        _0thAxisInUse = true;
                    }

                } else 
                    if (_0thAxisInUse) _0thAxisInUse = false;
            }

        } else if (Input.GetAxisRaw("1") != 0) { //return

            if (!_1thAxisInUse) {

                EnterSelection(false);
                _1thAxisInUse = true;
            }

        } else 
            if (_1thAxisInUse) _1thAxisInUse = false;
        
    }

    public void SelectFromMenu(int desiredSelectableIndex) {

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

        door.DORotate(new Vector3(0.0f, -77.3f, 0.0f), 0.45f).OnComplete(delegate {

            Destroy(gameObject);
        });
    }

    public void QuitApplication() { }
}
