using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    private static AreaManager _instance;

    public List<Portal> Potals;
    [SerializeField]
    private int _myAreaIndex;
    [SerializeField]
    private Transform _itemParents;

    public string SceneName;

    private bool isRunnig = true;

    public CanvasGroup StageCanvas;
    public CanvasGroup AreaCanvas;

    
    [Header("Fade Times")]
    public float fadeInDuration = 1.5f;
    public float stayDuration = 2.0f;
    public float fadeOutDuration = 1.5f;

    public TextMeshProUGUI SceneTxt;
    public TextMeshProUGUI AreaTxt;



    // === 로드 될때까지 대기 후 스타트 ===
    private IEnumerator Start()
    {
        _instance = this;

        yield return new WaitUntil(()=>SaveManager.Instance.IsLoadCompleted);

        SaveManager.Instance.UserData.SceneNumber = _myAreaIndex;

        ItemSpawnManager.Instance.GenerateAreaItem(_myAreaIndex, _itemParents);
        ItemSpawnManager.Instance.GenerateAreaNpc(_myAreaIndex, _itemParents);

        for (int i = 0; i < Potals.Count; i++)
        {
            if (Potals[i].PortalD == GameManager.Instance.PortalInfo && Potals[i].PortalIndex == GameManager.Instance.Index)
            {
                GameManager.Instance.PlayerPos = Potals[i].transform.position;
            }
        }

        StageCanvas = gameObject.GetComponentInChildren<CanvasGroup>();

        SceneTxt.text = SceneName;

        StartCoroutine(FadeIn(fadeInDuration, stayDuration, fadeOutDuration));
    }

    // AreaManager.GenerateGroundItem(생성할 아이템 아이디, 생성할 위치)
    public static void GenerateGroundItem(int ItemId, Vector2 pos)
    {
        var newInfo = SaveManager.Instance.UserData.PlayerItemData.GenerateNewItemInfo(ItemId);
        newInfo.PosX = pos.x;
        newInfo.PosY = pos.y;

        if(!SaveManager.Instance.UserData.PlayerItemData.GroundItem.ContainsKey(_instance._myAreaIndex))
        {
            SaveManager.Instance.UserData.PlayerItemData.GroundItem[_instance._myAreaIndex] = new List<ItemInfo>();
        }

        SaveManager.Instance.UserData.PlayerItemData.GroundItem[_instance._myAreaIndex].Add(newInfo);

        ItemSpawnManager.Instance.GenerateGroundItem(_instance._itemParents, newInfo);
    }

    public IEnumerator FadeIn(float fadeinduration, float stayduration, float outduration)
    {
        if (isRunnig == true)
        {
            isRunnig = false;

            yield return new WaitForSeconds(0.5f);

            float timer = 0f;

            while (timer < fadeinduration)
            {
                StageCanvas.alpha = Mathf.Lerp(0f, 1f, timer / fadeinduration);
                timer += Time.deltaTime;
                yield return null;
            }

            StageCanvas.alpha = 1f;

            yield return new WaitForSeconds(stayduration);

            timer = 0f;
            while (timer < outduration)
            {
                StageCanvas.alpha = Mathf.Lerp(1f, 0f, timer / fadeinduration);
                timer += Time.deltaTime;
                yield return null;
            }
            StageCanvas.alpha = 0f;

            isRunnig = true;
        }
    }

    public IEnumerator FadeIn(float fadeinduration, float stayduration, float outduration, string Name)
    {
        if (isRunnig == true)
        {
            AreaTxt.text = Name;
            isRunnig = false;

            yield return new WaitForSeconds(0.5f);

            float timer = 0f;

            while (timer < fadeinduration)
            {
                AreaCanvas.alpha = Mathf.Lerp(0f, 1f, timer / fadeinduration);
                timer += Time.deltaTime;
                yield return null;
            }

            AreaCanvas.alpha = 1f;

            yield return new WaitForSeconds(stayduration);

            timer = 0f;
            while (timer < outduration)
            {
                AreaCanvas.alpha = Mathf.Lerp(1f, 0f, timer / fadeinduration);
                timer += Time.deltaTime;
                yield return null;
            }
            AreaCanvas.alpha = 0f;

            isRunnig = true;
        }
    }
}
