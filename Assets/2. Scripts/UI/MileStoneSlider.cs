using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MileStoneSlider : MonoBehaviour
{
    [Header("연결할 오브젝트")]
    public RectTransform milePanelRect;

    public RectTransform arrowButtonRect;

    public Button arrowButton;

    [Header("위치 값 (직접 설정)")]
    public Vector2 panelInPosition; 
    public Vector2 panelOutPosition; 

    [Header("애니메이션 설정")]
    public float slideDuration = 0.4f;

    private bool isPanelVisible = true;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DOTween.Init();
        arrowButton.onClick.AddListener(TogglePanel);
        CheckSceneVisibility(SceneManager.GetActiveScene());
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (milePanelRect) milePanelRect.DOKill();
        if (arrowButtonRect) arrowButtonRect.DOKill();
        if (arrowButton) arrowButton.onClick.RemoveListener(TogglePanel);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSceneVisibility(scene);
    }

    private void CheckSceneVisibility(Scene scene)
    {
        if (scene.name == "Title")
        {
            milePanelRect.gameObject.SetActive(false);
            arrowButton.gameObject.SetActive(false);
        }
        else
        {
            milePanelRect.gameObject.SetActive(true);
            arrowButton.gameObject.SetActive(true);

            milePanelRect.anchoredPosition = panelInPosition;

            arrowButtonRect.localRotation = Quaternion.Euler(0, 0, -90f);
            isPanelVisible = true;
        }
    }

    public void TogglePanel()
    {
        if (isPanelVisible)
        {
            milePanelRect.DOAnchorPos(panelOutPosition, slideDuration).SetEase(Ease.OutQuad);
            arrowButtonRect.DORotate(new Vector3(0, 0, 90f), slideDuration);
        }
        else
        {
            milePanelRect.DOAnchorPos(panelInPosition, slideDuration).SetEase(Ease.OutQuad);
            arrowButtonRect.DORotate(new Vector3(0, 0, -90f), slideDuration);
        }

        isPanelVisible = !isPanelVisible;
    }
}