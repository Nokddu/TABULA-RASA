using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour, IInteraction
{
    public string KeyDescription;
    public string InputKey;
    public string KeyImagePath;
    public void Interact()
    {
        Show_Tutorial.Show_TutorialPopUp(InputKey,KeyDescription, KeyImagePath);
    }

    

}
