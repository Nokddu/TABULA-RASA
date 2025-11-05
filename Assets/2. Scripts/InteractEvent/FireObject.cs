using UnityEngine;

public class FireObject : MonoBehaviour
{
    private void Start()
    {
        DialogManager.Instance.ChangeField += FireOff;

        if (!SaveManager.Instance.UserData.QuestInfo.ContainsKey(21))
        {
            return;
        }
        if(SaveManager.Instance.UserData.QuestInfo[21] == Condition.QuestCompleted)
        {
            FireOff();
            return;
        }

    }

    private void OnDestroy()
    {
        DialogManager.Instance.ChangeField -= FireOff;
    }

    public void FireOff()
    {
        Destroy(gameObject);
    }
}
