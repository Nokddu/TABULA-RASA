using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PlayerForm
{
    [field: SerializeField] public CurrentState FormType; //이걸로 초상화 참조
    [field: SerializeField] public string name;
    [field: SerializeField] public RuntimeAnimatorController Animator { get; private set; }
    [field: SerializeField] public Vector2 ColliderSize { get; private set; }
    [field: SerializeField] public Vector2 ColliderOffset { get; private set; }
}

[CreateAssetMenu(fileName = "FormSo", menuName = "Character/Form")]
public class FormSO : ScriptableObject
{
    [field: SerializeField] public PlayerForm FormData { get; private set; }
}
