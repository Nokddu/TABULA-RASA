using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Password : MonoBehaviour, IPuzzle, IInteraction
{
    [SerializeField] private GameObject _passwordUi;
    [SerializeField] private UIPasswordPanel _uiPasswordPanel;
    [SerializeField] private string _answer;

    private int _successId;
    private int _failId;

    public void Interact(int SuccessId, int FailId)
    {
        _successId = SuccessId;
        _failId = FailId;

        StartCoroutine(InteractRoutine());
    }

    public void Interact()
    {
        SoundManager.Instance.Play_Sfx(SFX.Door);

        gameObject.SetActive(false);

        PlayerStatusUI.ChangePortrait(3, "Sprite/Portraits/Player_Hat");
    }
    private IEnumerator InteractRoutine()
    {
        UIDialog.OffPanel();

        yield return new WaitForSeconds(0.2f);

        _passwordUi.SetActive(true);

        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            GameManager.Instance.Player.InteractOn(Vector2.zero, true, true, true);
        }
    }    

    public bool CheckPassword(string submittedPassword)
    {
        if (_answer.Equals(submittedPassword))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TriggerSuccess()
    {
        SoundManager.Instance.Play_Sfx(SFX.DoorClear);
        DialogManager.Instance.PlayDialogNode(DataManager.Instance.DialogNodes[_successId]);
        gameObject.SetActive(false);
    }

    public void TriggerFailure()
    {
        DialogManager.Instance.PlayDialogNode(DataManager.Instance.DialogNodes[_failId]);
    }
}