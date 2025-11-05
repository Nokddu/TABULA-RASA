using System;
using UnityEngine;
using DG.Tweening;

public abstract class UIBase : MonoBehaviour
{
    public event Action OnCloseAnimationEnd;
    public virtual void Init()
    {

    }

    public virtual void Open()
    {
        transform.SetAsLastSibling();

        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    public virtual void Close()
    {
        // 닫기 애니메이션이 끝나면(OnComplete), OnCloseAnimationEnd 이벤트를 호출(방송)합니다.
        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            // 구독자들에게 "애니메이션 끝났다!"고 알림
            OnCloseAnimationEnd?.Invoke();
        });
    }
}