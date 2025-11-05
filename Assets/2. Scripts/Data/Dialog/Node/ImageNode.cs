using System.Collections.Generic;

public class ImageNode
{
    public int Id;
    public int Value;
    public string Path;

    public List<DialogImage> DialogImageList;
    
    public ImageNode(List<DialogImage> DialogImageList)
    {
        this.DialogImageList = new List<DialogImage>();

        foreach(DialogImage DialogImage in DialogImageList)
        {
            this.DialogImageList.Add(DialogImage);
        }
    }
}