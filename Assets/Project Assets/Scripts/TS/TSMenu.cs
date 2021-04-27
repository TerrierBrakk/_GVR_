using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class TSMenu : MonoBehaviour {

    public Transform wrap;
    public List<GameObject> views = new List<GameObject>();
    public CanvasGroup viewsWrapCG;
    public CanvasGroup mainMenuCG;
    public Vector3 activeViewsWrapPosition;
    public CanvasGroup canvasCG;

    private Vector3 defaultWrapTransformPos;
    private int currentViewIndex;

    void Start() {

        defaultWrapTransformPos = wrap.localPosition;
    }

    public void ChangeView(int viewIndex) {

        if (viewIndex < 0) {

            mainMenuCG.DOFade(1.0f, 0.25f);
            viewsWrapCG.DOFade(0.0f, 0.25f).OnComplete(delegate {

                views[currentViewIndex].SetActive(false);
            });

            wrap.DOLocalMove(defaultWrapTransformPos, 0.25f);
            wrap.DORotate(new Vector3(0, -16.242f, 0.0f), 0.25f);

        } else {

            mainMenuCG.DOFade(0.0f, 0.25f);
            viewsWrapCG.DOFade(1.0f, 0.25f);

            views[currentViewIndex].SetActive(false);
            views[currentViewIndex = viewIndex].SetActive(true);

            wrap.DOLocalMove(activeViewsWrapPosition, 0.25f);
            wrap.DORotate(new Vector3(0, 10.2f, 0.0f), 0.25f);
        }
    }

    public void LoadWorld() {

        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        canvasCG.DOFade(0.0f, 0.25f).OnComplete(delegate {

            canvasCG.gameObject.SetActive(false);
        });
    }
}
