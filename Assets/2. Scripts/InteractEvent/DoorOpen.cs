using UnityEngine;

public class DoorOpen : MonoBehaviour, IInteraction
{
    public void Interact()
    {
        SoundManager.Instance.Play_Sfx(SFX.Door);

        gameObject.SetActive(false);

        PlayerStatusUI.ChangePortrait(3, "Sprite/Portraits/Player_Hat");
    }
}
