using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 색상 변경을 위해 추가

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons; 
    public TabButton selectedTab; 

    [Header("활성화/비활성화 색상")]
    public Color activeTabColor = Color.green;
    public Color inactiveTabColor = Color.white;

    void Start()
    {
        foreach (TabButton button in tabButtons)
        {
            button.tabGroup = this;
        }

        if (tabButtons != null && tabButtons.Count > 0)
        {
            OnTabSelected(tabButtons[0]);
        }
    }

    public void OnTabSelected(TabButton button)
    {
        if (selectedTab == button) return;

        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }

        selectedTab = button;
        selectedTab.Select();
    }
}