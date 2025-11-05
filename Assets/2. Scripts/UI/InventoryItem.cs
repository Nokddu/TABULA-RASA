using UnityEngine;

public class InventoryItem
{
    public ItemInfo ItemInfo { get; private set; } // 실시간 변경되는 아이템 정보
    public ItemData ItemData { get; private set; } // 변경 X 아이템 정보

    public int width { get; private set; }
    public int height { get; private set; }
    public bool[,] occupiedCells; // 아이템의 실제 모양
    public int rotationAngle = 0;

    // 인벤토리 내에서의 위치
    public int GridX;
    public int GridY;

    public InventoryItem(ItemInfo itemInfo, ItemData itemData)
    {
        this.ItemInfo = itemInfo;
        this.ItemData = itemData;

        // ItemData의 shape 문자열을 bool[,] 2차원 배열로 변환
        ParseShape(itemData.shape);

        if (itemInfo.Rotation > 0)
        {
            int timesToRotate = itemInfo.Rotation / 90;
            for (int i = 0; i < timesToRotate; i++)
            {
                Rotate();
            }
        }
    }

    private void ParseShape(string[] shapeStrings)
    {
        if (shapeStrings == null || shapeStrings.Length == 0)
        {
            // shape 정보가 없으면 ItemData의 Width/Height로 사각형을 만듦
            height = this.ItemData.Height;
            width = this.ItemData.Width;
            occupiedCells = new bool[height, width];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    occupiedCells[y, x] = true;
        }
        else
        {
            height = shapeStrings.Length;
            width = shapeStrings[0].Length;
            occupiedCells = new bool[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    occupiedCells[y, x] = shapeStrings[y][x] == '1';
                }
            }
        }
    }

    // 90도씩 회전시키는 함수
    public void Rotate()
    {
        rotationAngle = (rotationAngle + 90) % 360;

        bool[,] newShape = new bool[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                newShape[x, (height - 1) - y] = occupiedCells[y, x];
            }
        }

        // 크기 스왑 및 모양 업데이트
        int temp = width;
        width = height;
        height = temp;
        occupiedCells = newShape;
    }
}