using System.Collections;
using UnityEngine;

public class SavePoint : MonoBehaviour, IInteraction
{
    private SaveManager _save;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveManager.Instance.IsLoadCompleted);

        _save = SaveManager.Instance;
    }

    public void Interact()
    {
        SoundManager.Instance.Play_Sfx(SFX.Save);

        _save.UserData.PlayerPosX = transform.position.x;
        _save.UserData.PlayerPosY = transform.position.y;

        GameManager.Instance.PlayerPos.x = transform.position.x;
        GameManager.Instance.PlayerPos.y = transform.position.y;

        _save.UserData.CurrentMilestone = UIManager.Instance.CurrentTxt.text;

        _save.Save_Data(_save.UserData, _save.SystemData);
    }
}
