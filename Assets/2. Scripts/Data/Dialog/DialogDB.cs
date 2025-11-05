using System.Collections.Generic;

public class DialogDB
{
    public readonly Dictionary<int, List<Dialog>> EntityDialogMap;
    public readonly Dictionary<int, DialogText> DialogTexts;
    public readonly Dictionary<string, DialogLine> DialogLines;
    public readonly Dictionary<string, EndAction> EndActions;
    public readonly Dictionary<int, List<DialogImage>> DialogImages;
    public readonly Dictionary<string, MileStones> MileStones;

    public DialogDB(Dialogs DialogsSO)
    {
        EntityDialogMap = new Dictionary<int, List<Dialog>>();
        DialogTexts = new Dictionary<int, DialogText>();
        DialogLines = new Dictionary<string, DialogLine>();
        EndActions = new Dictionary<string, EndAction>();
        DialogImages = new Dictionary<int, List<DialogImage>>();
        MileStones = new Dictionary<string, MileStones>();

        if (DialogsSO != null && DialogsSO.EntityDialogMap != null)
        {
            foreach(Dialog data in DialogsSO.EntityDialogMap)
            {
                if(data != null)
                {
                    if (!EntityDialogMap.ContainsKey(data.Id))
                    {
                        EntityDialogMap[data.Id] = new List<Dialog>();
                    }

                    EntityDialogMap[data.Id].Add(data);
                }
            }
        }

        if(DialogsSO != null && DialogsSO.DialogText != null)
        {
            foreach(DialogText data in DialogsSO.DialogText)
            {
                if (data != null)
                {
                    DialogTexts[data.TargetDialogId] = data;
                }
            }
        }

        if(DialogsSO != null && DialogsSO.DialogImages != null)
        {
            foreach (DialogImage data in DialogsSO.DialogImages)
            {
                if(data != null)
                {
                    if (!DialogImages.ContainsKey(data.Id))
                    {
                        DialogImages[data.Id] = new List<DialogImage>();
                    }

                    DialogImages[data.Id].Add(data);
                }
            }
        }

        if (DialogsSO != null && DialogsSO.MileStones != null)
        {
            foreach (MileStones data in DialogsSO.MileStones)
            {
                if (data != null)
                {
                    MileStones[data.TargetDialogId] = data;
                }
            }
        }

        if (DialogsSO != null && DialogsSO.EndActions != null)
        {
            foreach(EndAction data in DialogsSO.EndActions)
            {
                if (data != null)
                {
                    EndActions[data.Id] = data;
                }
            }
        }

        if(DialogsSO != null && DialogsSO.DialogLines != null)
        {
            foreach(DialogLine data in DialogsSO.DialogLines)
            {
                if(data != null)
                {
                    DialogLines[data.TargetDialogId] = data;
                }
            }
        }
    }
}
