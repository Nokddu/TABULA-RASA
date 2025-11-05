using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteraction
{
   void Interact();
}

public interface IPuzzle
{
    void Interact(int SuccessId, int FailId);
}

public class Npc : MonoBehaviour
{
    public NpcData NpcData;

    public bool flag;
    
    public void Init(NpcData NpcData)
    {
        this.NpcData = NpcData;
    }

    public void Dialog(CurrentState CurrentState)
    {
        DialogManager.Instance.InitByEntity(this.NpcData.Id, this.gameObject, CurrentState, NpcData.Name); // ghost, human, dog, ... 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (flag)
        {
            collision.GetComponent<Player>().InteractOn(gameObject.transform.position, true, true, true);
            SaveManager.Instance.UserData.DoorInteracted[NpcData.Id] = true;
            DialogManager.Instance.InitByEntity(NpcData.Id,gameObject);
            GameManager.Instance.Player.TargetNpc = this;
            gameObject.SetActive(false);
        }
    }
}
