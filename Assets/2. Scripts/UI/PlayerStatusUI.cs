using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("초상화 UI")]
    public Image portraitImage; // 인스펙터에서 초상화 Image 컴포넌트를 연결

    private List<Sprite> portraitSprite = new List<Sprite>();

    public static Action<int, string> ImgChange;

    public static Action<int, string> ImgDataChange;

    [Header("정신력(Sanity) 바 UI")]
    public Slider sanitySlider; // 인스펙터에서 정신력 Slider 컴포넌트를 연결

    private void Start()
    {
        if (SaveManager.Instance.UserData.PlayerImg == null)
        {
            portraitImage.sprite = Resources.Load<Sprite>("Sprite/Portraits/WhiteGhost");
        }
        else
        {
            portraitImage.sprite = Resources.Load<Sprite>(SaveManager.Instance.UserData.PlayerImg);
        }
        ImgSetting();
        ImgChange += ChangeImg;
        ImgDataChange += ChangeImgData;
    }

    private void OnDestroy()
    {
        ImgChange -= ChangeImg;
        ImgDataChange -= ChangeImgData;
    }

    /// <summary>
    /// 0=ghost ,1=cat ,2=dog, 3=human
    /// </summary>
    /// <param name="state"></param>
    /// <param name="imgPath"></param>
    public void UpdatePortrait(int state)
    {
        if (state >= 0 && state < portraitSprite.Count)
        {
            portraitImage.sprite = portraitSprite[state];

            SaveManager.Instance.UserData.PlayerImg = SaveManager.Instance.UserData.SaveImgs[state];
        }
    }

    private void ImgSetting()
    {
        if (SaveManager.Instance.UserData.SaveImgs[0] == null)
        {
            
            portraitSprite.Add(Resources.Load<Sprite>("Sprite/Portraits/WhiteGhost"));
            SaveManager.Instance.UserData.SaveImgs[0] = "Sprite/Portraits/WhiteGhost";
            portraitSprite.Add(Resources.Load<Sprite>("Sprite/Portraits/CatState"));
            SaveManager.Instance.UserData.SaveImgs[1] = "Sprite/Portraits/CatState";
            portraitSprite.Add(Resources.Load<Sprite>("Sprite/Portraits/DogState"));
            SaveManager.Instance.UserData.SaveImgs[2] = "Sprite/Portraits/DogState";
            portraitSprite.Add(Resources.Load<Sprite>("Sprite/Portraits/HumanFace"));
            SaveManager.Instance.UserData.SaveImgs[3] = "Sprite/Portraits/HumanFace";
        }
        else
        {
            for (int i = 0; i < SaveManager.Instance.UserData.SaveImgs.Count; i++)
            {
                portraitSprite.Add(Resources.Load<Sprite>(SaveManager.Instance.UserData.SaveImgs[i]));
            }
        }
    }
    
    private void ChangeImg(int index, string imgPath)
    {
        portraitSprite[index] = Resources.Load<Sprite>(imgPath);
        SaveManager.Instance.UserData.SaveImgs[index] = imgPath;
        UpdatePortrait(index);
    }

    private void ChangeImgData(int index, string imgPath)
    {
        portraitSprite[index] = Resources.Load<Sprite>(imgPath);
        SaveManager.Instance.UserData.SaveImgs[index] = imgPath;
    }
    /// <summary>
    /// 0=ghost ,1=cat ,2=dog, 3=human
    /// </summary>
    /// <param name="state"></param>
    /// <param name="imgPath"></param>
    public static void ChangePortrait(int index, string imgPath)
    {
        ImgChange?.Invoke(index, imgPath);
    }

    public static void ChangePortraitData(int index, string imgPath)
    {
        ImgDataChange?.Invoke(index, imgPath);
    }


    // 정신력(Sanity) 값이 변경될 때 호출할 함수
    public void UpdateSanityBar(float currentSanity, float maxSanity)
    {
        sanitySlider.value = currentSanity / maxSanity;
    }
}