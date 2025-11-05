using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleMenu_DOTween : MonoBehaviour
{
    [Header("0. 커튼 설정")]
    public RectTransform leftCurtain;
    public RectTransform rightCurtain;
    public float curtainDuration = 1.0f;

    [Header("1. 패널 설정")]
    public CanvasGroup panelCanvasGroup;
    public RectTransform panelRectTransform;

    [Header("2. 버튼 설정")]
    public CanvasGroup[] menuButtons;

    [Header("3. 애니메이션 값")]
    public float panelSlideDuration = 0.5f;
    public float panelStartXOffset = -300f;
    public float buttonStartXOffset = -200f;
    public float buttonMoveDuration = 0.4f;
    public float buttonFadeDuration = 0.3f;
    public float delayBetweenButtons = 0.1f;
    public float initialDelay = 0.5f;

    private Vector2 panelTargetPosition;
    private List<Vector2> buttonTargetPositions = new List<Vector2>();

    private Vector2 leftCurtainClosedPos;
    private Vector2 rightCurtainClosedPos;
    private bool isExiting = false;


    private Vector2 leftCurtainOpenPos; 
    private Vector2 rightCurtainOpenPos;
    private static bool s_isFirstLoad = true; 

    void Start()
    {
        Time.timeScale = 1f;

        if (leftCurtain != null)
        {
            leftCurtainClosedPos = leftCurtain.anchoredPosition;
            leftCurtainOpenPos = new Vector2(leftCurtainClosedPos.x - leftCurtain.rect.width, leftCurtainClosedPos.y);
        }
        if (rightCurtain != null)
        {
            rightCurtainClosedPos = rightCurtain.anchoredPosition;
            rightCurtainOpenPos = new Vector2(rightCurtainClosedPos.x + rightCurtain.rect.width, rightCurtainClosedPos.y);
        }

        if (panelRectTransform != null)
        {
            panelTargetPosition = panelRectTransform.anchoredPosition;
        }
        else
        {
            Debug.LogError("Panel RectTransform이 할당되지 않았습니다!");
            return;
        }

        buttonTargetPositions.Clear();
        foreach (var buttonCG in menuButtons)
        {
            if (buttonCG == null) { buttonTargetPositions.Add(Vector2.zero); continue; }
            RectTransform buttonRect = buttonCG.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                buttonTargetPositions.Add(buttonRect.anchoredPosition);
            }
            else
            {
                buttonTargetPositions.Add(Vector2.zero);
            }
        }

        if (s_isFirstLoad)
        {
            // 첫 로드: 인트로 애니메이션 준비
            s_isFirstLoad = false;

            // 패널 초기 상태 설정 (화면 밖, 투명)
            panelRectTransform.anchoredPosition = new Vector2(panelStartXOffset, panelTargetPosition.y);
            if (panelCanvasGroup != null) panelCanvasGroup.alpha = 0f;

            // 버튼 초기 상태 설정 (화면 밖, 투명)
            for (int i = 0; i < menuButtons.Length; i++)
            {
                if (menuButtons[i] == null) continue;
                menuButtons[i].alpha = 0f;
                RectTransform buttonRect = menuButtons[i].GetComponent<RectTransform>();
                if (buttonRect != null && i < buttonTargetPositions.Count)
                {
                    buttonRect.anchoredPosition = new Vector2(buttonTargetPositions[i].x + buttonStartXOffset, buttonTargetPositions[i].y);
                }
            }

            // 커튼 초기 상태 설정 (닫힌 위치)
            if (leftCurtain != null) leftCurtain.anchoredPosition = leftCurtainClosedPos;
            if (rightCurtain != null) rightCurtain.anchoredPosition = rightCurtainClosedPos;

            // 인트로 애니메이션 코루틴 시작
            if (leftCurtain == null || rightCurtain == null)
            {
                StartCoroutine(PlayIntroAnimation(false));
            }
            else
            {
                StartCoroutine(PlayIntroAnimation(true));
            }
        }
        else
        {

            // 커튼을 즉시 '열린' 위치로 설정
            if (leftCurtain != null) leftCurtain.anchoredPosition = leftCurtainOpenPos;
            if (rightCurtain != null) rightCurtain.anchoredPosition = rightCurtainOpenPos;

            // 패널을 즉시 '최종' 위치/알파로 설정
            panelRectTransform.anchoredPosition = panelTargetPosition;
            if (panelCanvasGroup != null) panelCanvasGroup.alpha = 1f;

            // 버튼들을 즉시 '최종' 위치/알파로 설정
            for (int i = 0; i < menuButtons.Length; i++)
            {
                if (menuButtons[i] == null) continue;
                menuButtons[i].alpha = 1f;
                RectTransform buttonRect = menuButtons[i].GetComponent<RectTransform>();
                if (buttonRect != null && i < buttonTargetPositions.Count)
                {
                    buttonRect.anchoredPosition = buttonTargetPositions[i];
                }
            }
        }
    }
    IEnumerator PlayIntroAnimation(bool playCurtain)
    {
        yield return new WaitForSeconds(initialDelay);

        if (playCurtain)
        {
            float leftCurtainWidth = leftCurtain.rect.width;
            float rightCurtainWidth = rightCurtain.rect.width;

            Sequence curtainSequence = DOTween.Sequence();

            // 왼쪽으로 사라짐
            curtainSequence.Append(leftCurtain.DOAnchorPos(
                                        new Vector2(leftCurtainClosedPos.x - leftCurtainWidth, leftCurtainClosedPos.y),
                                        curtainDuration)
                                    .SetEase(Ease.InOutExpo));
            // 오른쪽으로 사라짐
            curtainSequence.Insert(0f, rightCurtain.DOAnchorPos(
                                        new Vector2(rightCurtainClosedPos.x + rightCurtainWidth, rightCurtainClosedPos.y),
                                        curtainDuration)
                                    .SetEase(Ease.InOutExpo));

            yield return curtainSequence.WaitForCompletion();
        }

        if (panelRectTransform != null)
        {
            panelRectTransform.DOAnchorPos(panelTargetPosition, panelSlideDuration)
                                .SetEase(Ease.OutQuad);
        }
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.DOFade(1f, panelSlideDuration)
                            .SetEase(Ease.OutQuad);
        }

        yield return new WaitForSeconds(panelSlideDuration * 0.7f);

        for (int i = 0; i < menuButtons.Length; i++)
        {
            CanvasGroup buttonCG = menuButtons[i];
            if (buttonCG == null) continue;

            RectTransform buttonRect = buttonCG.GetComponent<RectTransform>();

            if (buttonRect != null && i < buttonTargetPositions.Count && buttonTargetPositions[i] != Vector2.zero)
            {
                buttonRect.DOAnchorPos(buttonTargetPositions[i], buttonMoveDuration)
                            .SetEase(Ease.OutCubic);
            }

            buttonCG.DOFade(1f, buttonFadeDuration)
                    .SetEase(Ease.OutQuad);

            yield return new WaitForSeconds(delayBetweenButtons);
        }
    }
    public void StartExitAnimation() // 종료 버튼 애니메이션
    {
        if (isExiting) return;
        isExiting = true;

        foreach (var buttonCG in menuButtons)
        {
            buttonCG.DOFade(0f, 0.3f);
        }
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.DOFade(0f, 0.3f);
        }

        Sequence exitSequence = DOTween.Sequence();

        exitSequence.Append(leftCurtain.DOAnchorPos(leftCurtainClosedPos, curtainDuration)
                                        .SetEase(Ease.InOutExpo));

        exitSequence.Insert(0f, rightCurtain.DOAnchorPos(rightCurtainClosedPos, curtainDuration)
                                            .SetEase(Ease.InOutExpo));

        exitSequence.OnComplete(QuitGame);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 빌드된 게임에서는 어플리케이션 종료
        Application.Quit();
#endif
    }
    private void OnApplicationQuit()
    {
        s_isFirstLoad = true;
    }
}