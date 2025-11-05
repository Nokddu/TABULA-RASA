using UnityEngine;

public class CardKey : MonoBehaviour, IInteraction
{
    public bool IsInsertKey;

    public bool IsSuccess;

    SpriteRenderer Sprite;

    private void Start()
    {
        Sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void Interact()
    {
        SoundManager.Instance.Play_Sfx(SFX.CardKey);

        IsInsertKey = true;

        CheckCardKey.AddCardKey(this);

        Sprite.color = new Color(0.5f, 1f, 0.5f);
    }

    public void keyFalse()
    {
        IsInsertKey = false;
        if (Sprite == null) return;


        Sprite.color = Color.white;
    }
}
