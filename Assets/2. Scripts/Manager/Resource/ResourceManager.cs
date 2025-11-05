using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Resources.Load로 리소스를 가져오고, Instantiate로 오브젝트를 생성해주는 클래스

/// <summary>
/// (사용법)
/// 
/// 1. 프리팹 오브젝트를 만들려는 위치로 이동한다
/// 2. ResourceManager.Instance.오브젝트생성메서드<T>(Prefab.상수명);
/// 
/// ex) (플레이어 프리팹 생성)
///     ResourceManager.Instance.CreateCharacter<Player>(Prefab.Player); 
/// 
/// </summary>
/// 

public class ResourceManager : Singleton<ResourceManager>
{
    // 만들고 싶은 오브젝트 종류에 맞는 메서드를 정의 (맵이면 CreateMap, UI이면 CreateUI로)

    // 캐릭터 프리팹 생성/반환
    public T Create_Character<T>(string prefabName, Vector2 Position, Transform parent = null) where T : Object // T 타입이 Object야 함 
    {
        return Create_PathAndObject<T>(ResourcePath.CHARACTER, prefabName, Position, parent);
    }

    public T Create_Map<T>(string prefabName, Vector2 Position, Transform parent = null) where T : Object
    {
        return Create_PathAndObject<T>(ResourcePath.MAP, prefabName, Position, parent);
    }

    public T[] Create_BGM<T>() where T : Object
    {
        return Create_PathAndLoadResources<T>(ResourcePath.BGM);
    }

    public T[] Create_SFX<T>() where T : Object
    {
        return Create_PathAndLoadResources<T>(ResourcePath.SFX); 
    }

    public T[] Create_AMBIENCE<T>() where T : Object
    {
        return Create_PathAndLoadResources<T>(ResourcePath.AMBIENCE);
    }

    public T Create_UI<T>(string prefabName, Vector2 Position, Transform parent = null) where T : Object
    {
        return Create_PathAndObject<T>(ResourcePath.UI, prefabName, Position, parent);
    }

    // 경로를 만들고, 프리팹 생성/반환 
    public T Create_PathAndObject<T>(string resourcePath, string prefabName, Vector2 Position, Transform parent = null) where T : Object
    {
        string path = resourcePath + prefabName;
        return Create_Object<T>(path, Position, parent);
    }

    public T[] Create_PathAndLoadResources<T>(string resourcePath) where T : Object
    {
        string path = resourcePath;  
        return LoadResources<T>(path); 
    }

    // 프리팹 오브젝트 생성 
    public T Create_Object<T>(string path, Vector2 Position, Transform parent = null) where T : Object
    {
        T resource = LoadResource<T>(path);

        if(resource == null)
        {
            Debug.Log($"프리팹이 없습니다 : {path}");
            return default;
        }

        T obj = Instantiate(resource, Position, Quaternion.identity, parent);
        return obj;
    }

    public T LoadResource<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public T[] LoadResources<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }
}
