using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TSMenuSelection : MonoBehaviour {

    public List<Button> buttons = new List<Button>();
    public List<RectTransform> selectionPositions = new List<RectTransform>();
    public RectTransform selectionImage;

    private int currentSelectedButton;

    void Start() {

        UpdateSelection(currentSelectedButton = 0);
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.S)) UpdateSelection(currentSelectedButton++);
        else if (Input.GetKeyDown(KeyCode.W)) UpdateSelection(currentSelectedButton--);
        else if (Input.GetKeyDown(KeyCode.Return)) buttons[currentSelectedButton].onClick.Invoke();
    }

    private void UpdateSelection(int updatedIndex) {

        if (currentSelectedButton > buttons.Count - 1) currentSelectedButton = 0;
        else if (currentSelectedButton < 0) currentSelectedButton = buttons.Count - 1;
        selectionImage.DOAnchorPos(selectionPositions[currentSelectedButton].anchoredPosition, 0.25f);
    }
}
