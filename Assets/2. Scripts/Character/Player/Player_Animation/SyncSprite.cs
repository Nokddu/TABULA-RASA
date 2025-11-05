using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncSprite : MonoBehaviour
{
    public SpriteRenderer SourceSpriteRenderer;
    public SpriteRenderer SilhouetteSpriteRenderer;

    void LateUpdate()
    {
        if (SourceSpriteRenderer != null && SilhouetteSpriteRenderer != null)
        {
            SilhouetteSpriteRenderer.sprite = SourceSpriteRenderer.sprite;
        }
    }
}
