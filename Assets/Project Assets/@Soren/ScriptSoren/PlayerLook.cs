using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayerLook : MonoBehaviour
{
    public Transform slender = null;
    public bool isLooking;
    public static float timeNotLooking;
    public CanvasGroup warningCG;
    public TSMenu tsMenu;

    private float lookingTime;
    private bool warningMessageDisplayed = false;
    private bool gameOver = false;

    void Start() {

        SceneManager.sceneLoaded += OnSceneChange;
    }

    public void OnSceneChange(Scene scene, LoadSceneMode mode) {

        EnemyBehaviour tmp = FindObjectOfType<EnemyBehaviour>();
        if(tmp)
            slender = tmp.transform;
    }

    void Update() {

        if (!slender) return;
        if (tsMenu.inGameMenuEnabled) return;
        if (!tsMenu.gameInitialized) return;

        float distance = Vector3.Distance(slender.position, transform.position);
        float angle = Vector3.Angle(transform.forward, slender.position - transform.position);

        if (angle < 60 && distance < 15)
        {
            timeNotLooking = 0;
            lookingTime += Time.deltaTime;
            if(!warningMessageDisplayed && tsMenu.gameInitialized && !tsMenu.restarting)
                DisplayWarningMessage(true);
        }
        else
        {
            isLooking = false;
            timeNotLooking += Time.deltaTime;
            lookingTime = 0.0f;

            if (warningMessageDisplayed)
                DisplayWarningMessage(false);
        }

        if (lookingTime >= 4.0f)
            GameOver();

    }

    private void DisplayWarningMessage(bool value) {

        Debug.Log("Displaying:" + value);

        if (value) warningCG.DOFade(1.0f, 0.25f);
        else warningCG.DOFade(0.0f, 0.25f);

        warningMessageDisplayed = value;
    }

    private void GameOver() {

        timeNotLooking = 0.0f;
        lookingTime = 0.0f;

        DisplayWarningMessage(false);
        tsMenu._RestartGame();
    }
}
