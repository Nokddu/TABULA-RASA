using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SoulTierUI : MonoBehaviour
{
    [Header("등급 아이콘 리스트")]
    // 인스펙터에서 3개의 삼각형 Image를 순서대로 연결
    public List<Sprite> tierIcons;

    public Image image;

    public void UpdateTierDisplay(int currentLevel)
    {
        image.sprite = tierIcons[currentLevel];
        Debug.Log($"Level Update{currentLevel}");
    }
}