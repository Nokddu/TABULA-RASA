using UnityEngine;

public class UIPassword : MonoBehaviour
{
    [SerializeField] private UIPasswordPanel parent;
    [SerializeField] private string _password;

    public void OnPasswordButtonClicked()
    {
        parent.Password += _password;
    }
}
