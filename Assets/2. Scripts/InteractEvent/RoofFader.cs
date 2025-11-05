using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofFader : MonoBehaviour
{
    [SerializeField] private TilemapRenderer _tilemapRenderer;
    [SerializeField] private float _fadeTime;

    private IEnumerator FadeOut()
    {
        yield return StartCoroutine(Fade(1.0f, 0.0f));
    }

    private IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(0.0f, 1.0f));
    }

    private IEnumerator Fade(float start, float End)
    {
        float CurrentTime = 0.0f;

        Material Mat = _tilemapRenderer.material;
        Color Color = Mat.color;

        // 페이드 인/아웃 
        while(CurrentTime < _fadeTime)
        {
            CurrentTime += Time.deltaTime;
            float t = CurrentTime / _fadeTime;

            Color.a = Mathf.Lerp(start, End, t);

            Mat.color = Color;
            yield return null;
        }

        // 원본 색상으로 되돌리기 
        Color.a = End;
        Mat.color = Color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopCoroutine(FadeIn());
            StartCoroutine(FadeOut());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopCoroutine(FadeOut());
            StartCoroutine(FadeIn());
        }
    }
}
