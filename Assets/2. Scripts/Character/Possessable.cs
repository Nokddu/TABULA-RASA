using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Possessable : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController animatorControll;
    [SerializeField] private Vector2 colliderSize;
    [SerializeField] private Vector2 colliderOffset;
    [SerializeField] private Canvas text;
    [field : SerializeField] public string Pooltag { get; private set; }

    private Outline _outline;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        if (_outline != null)
        {
            if (_outline != null)
            {
                _outline.enabled = false;

            }
        }
    }

    public CurrentState GetFormState()
    {
        switch (Pooltag)
        {
            case "Human": return CurrentState.Human;
            case "Cat": return CurrentState.Cat;
            case "Dog":return CurrentState.Dog;
            default: return CurrentState.Ghost;
        }
    }
    public void OnText()
    {
        text.enabled = true;
    }

    public void OffText()
    {
        text.enabled = false;
    }
}
