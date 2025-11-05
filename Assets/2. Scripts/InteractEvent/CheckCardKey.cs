using UnityEngine;
using System.Collections.Generic;

public class CheckCardKey : MonoBehaviour, IPuzzle
{
    private static List<CardKey> cards = new List<CardKey>(0);

    public static void AddCardKey(CardKey cardKey)
    {
        cards.Add(cardKey);
    }

    public void SetFalse()
    {
        foreach(CardKey cardKey in cards)
        {
            cardKey.keyFalse();
        }

        cards.Clear();
    }

    public bool IsOk()
    {
        bool ret = false;

        if(cards.Count == 2)
        {
            foreach (CardKey cardKey in cards)
            {
                if (cardKey.IsInsertKey && cardKey.IsSuccess)
                {
                    if (SaveManager.Instance.UserData.NpcRewards.ContainsKey(32108) == false)
                    {
                        SoundManager.Instance.Play_Sfx(SFX.DropAntibiotics);
                    }

                    ret = true;
                }
                else if (cardKey.IsInsertKey && !cardKey.IsSuccess)
                {
                    ret = false;
                    SetFalse();
                    break;
                }
            }
        }
        else
        {
            SetFalse();
        }

        return ret;
    }

    public void Interact(int SuccessId, int FailId)
    {
        if (IsOk())
        {
            DialogManager.Instance.PlayDialogNode(DataManager.Instance.DialogNodes[SuccessId]);
        }
        else
        {
            DialogManager.Instance.PlayDialogNode(DataManager.Instance.DialogNodes[FailId]);
        }
    }
}
