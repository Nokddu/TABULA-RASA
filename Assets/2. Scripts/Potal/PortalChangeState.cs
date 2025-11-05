using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalChangeState : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //SaveManager.Instance.UserData.SaveImgs[3] = "Sprite/Portraits/No_Player_Hat";
        PlayerStatusUI.ChangePortraitData(3, "Sprite/Portraits/No_Player_Hat");
    }
}
