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

    private int currentSelectableIndex;
    private bool enteredSelection;

    public void Start() {

        SelectFromMenu(0);
    }

    public void Update() {

        if (!enteredSelection) {

            if (Input.GetKeyDown(KeyCode.S))
                SelectFromMenu(currentSelectableIndex + 1);
            else if (Input.GetKeyDown(KeyCode.W))
                SelectFromMenu(currentSelectableIndex - 1);
            else if (Input.GetKeyDown(KeyCode.Return))
                EnterSelection(true);
        } else if (Input.GetKeyDown(KeyCode.Escape))
            EnterSelection(false);
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
        
        //Open door
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

        menuLight.DOIntensity(0.0f, 0.25f);
        viewLight.DOIntensity(0.0f, 0.25f);

        menuLightMat.DOColor(Color.white * 0.0f, "_EmissionColor", 0.25f);
        viewLightMat.DOColor(Color.white * 0.0f, "_EmissionColor", 0.25f);

        selectables[currentSelectableIndex].view.DOFade(0.0f, 0.25f).OnComplete(delegate {

            Destroy(gameObject);
        });
    }

    public void QuitApplication() { }
}
