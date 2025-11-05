using UnityEngine;

public class BoundaryLimit : MonoBehaviour
{
    public Transform High;
    public Transform Low;

    private void LateUpdate()
    {
        Vector3 pos = GameManager.Instance.Player.transform.position;

        float clampedX = Mathf.Clamp(pos.x, Low.position.x, High.position.x);
        float clampedY = Mathf.Clamp(pos.y, Low.position.y, High.position.y);

        GameManager.Instance.Player.transform.position = new Vector3(clampedX, clampedY, 0);
    }
}
