using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "5. Data/SO")]
public class Dialogs : ScriptableObject
{
    public List<Dialog> EntityDialogMap;
    public List<DialogText> DialogText;
    public List<DialogLine> DialogLines;
    public List<EndAction> EndActions;
    public List<DialogImage> DialogImages;
    public List<MileStones> MileStones;
}
