using System;
using UnityEngine;

public enum PortalDirection
{
    None,
    Up,
    Down,
    Left,
    Right
}

public class Portal : MonoBehaviour
{
    public int PortalIndex;
    public PortalDirection PortalD;
    public SceneType NextScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.Index = PortalIndex;

            GameManager.Instance.PortalInfo = GetOppositDirection(PortalD);

            SaveManager.Instance.UserData.SceneNumber = Math.Max(0, (int)NextScene - 1);

            InitallSpawner.Instance.Generate_Area_Vessel(SaveManager.Instance.UserData.PlayerVessel);

            SceneLoadManager.Instance.LoadScene(NextScene);
        }
    }

    private PortalDirection GetOppositDirection(PortalDirection direction)
    {
        return direction switch
        {
            PortalDirection.Up => PortalDirection.Down,
            PortalDirection.Down => PortalDirection.Up,
            PortalDirection.Left => PortalDirection.Right,
            PortalDirection.Right => PortalDirection.Left,
            _ => PortalDirection.None,
        };
    }
}
