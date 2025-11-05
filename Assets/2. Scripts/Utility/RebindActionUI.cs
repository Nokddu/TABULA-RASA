using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class RebindingManager : MonoBehaviour
{
    public InputActionAsset actions; 
    public Text feedbackText; 

    // 각 액션 이름에 대한 UI 버튼 텍스트를 관리하기 위한 딕셔너리
    private Dictionary<string, Text> actionButtonTexts = new Dictionary<string, Text>();

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private const string RebindsKey = "rebinds"; // PlayerPrefs에 저장할 때 사용할 키

    void Awake()
    {
        // 저장된 키 설정(오버라이드) 불러오기
        string rebinds = PlayerPrefs.GetString(RebindsKey, string.Empty);
        if (!string.IsNullOrEmpty(rebinds))
        {
            actions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    void OnEnable()
    {
        actions.Enable();
    }

    void OnDisable()
    {
        actions.Disable();
    }

    // UI 버튼 텍스트를 등록하는 함수 (UI 스크립트에서 호출)
    public void RegisterButtonText(string actionName, Text buttonText)
    {
        actionButtonTexts[actionName] = buttonText;
        UpdateButtonText(actionName);
    }

    // 특정 액션의 키 바인딩 UI 텍스트를 업데이트하는 함수
    public void UpdateButtonText(string actionName)
    {
        if (actionButtonTexts.ContainsKey(actionName))
        {
            var action = actions.FindAction(actionName);
            if (action != null)
            {
                // ToDisplayString()은 "W", "Space", "LMB" 같이 보기 좋은 문자열로 변환해 줍니다.
                actionButtonTexts[actionName].text = action.GetBindingDisplayString(0);
            }
        }
    }

    // 키 변경 시작 함수 (UI 버튼에서 호출될 함수)
    public void StartRebinding(string actionName)
    {
        // 먼저 기존 Rebinding Operation이 있다면 정리
        rebindingOperation?.Dispose();

        var action = actions.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError($"{actionName} 액션을 찾을 수 없습니다.");
            return;
        }

        if (feedbackText != null) feedbackText.gameObject.SetActive(true);
        actions.Disable(); // 리바인딩 중 다른 입력 방지

        rebindingOperation = action.PerformInteractiveRebinding()
            // WASD 같은 2D Vector의 특정 키 하나만 바꿀 때는 인덱스를 지정해야 합니다.
            // 예: .WithTargetBinding(1) -> 'W'에 해당하는 바인딩
            // 여기서는 첫 번째 바인딩(인덱스 0)을 대상으로 합니다.

            // 특정 종류의 입력을 제외할 수 있습니다.
            .WithControlsExcluding("Mouse") // 마우스 버튼은 제외

            .OnMatchWaitForAnother(0.1f) // 찰나의 입력은 무시

            .OnComplete(operation =>
            {
                actions.Enable();
                operation.Dispose();
                if (feedbackText != null) feedbackText.gameObject.SetActive(false);

                // UI 텍스트 업데이트
                UpdateButtonText(actionName);

                // 변경사항 저장
                SaveBindingOverrides();
            })
            .OnCancel(operation =>
            {
                actions.Enable();
                operation.Dispose();
                if (feedbackText != null) feedbackText.gameObject.SetActive(false);
            });

        rebindingOperation.Start();
    }

    // 변경된 키 설정을 PlayerPrefs에 저장
    private void SaveBindingOverrides()
    {
        string rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(RebindsKey, rebinds);
        PlayerPrefs.Save();
        Debug.Log("키 설정이 저장되었습니다.");
    }

    // 기본값으로 되돌리기
    public void ResetToDefaults()
    {
        actions.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(RebindsKey);
        Debug.Log("키 설정이 기본값으로 초기화되었습니다.");

        // UI 텍스트들도 모두 기본값으로 업데이트
        foreach (var actionName in actionButtonTexts.Keys)
        {
            UpdateButtonText(actionName);
        }
    }
}