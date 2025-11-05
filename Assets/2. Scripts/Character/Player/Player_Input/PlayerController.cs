using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Player_Input PlayerInput { get; private set; }
    public Player_Input.PlayerInputActions PlayerActions { get; private set; }

    public event Action OnPossessEvent;
    public event Action OnInventoryEvent;
    public event Action OnInteractEvent;
    public event Action OnItemGetEvent;
    public event Action OnMenuEvent;
    private void OnEnable()
    {
        PlayerInput.PlayerInput.Enable();
        PlayerInput.PlayerInput.Possession.performed += OnPossession;
        PlayerInput.PlayerInput.Inventory.performed += OnInventory;
        PlayerInput.PlayerInput.Interact.performed += OnInteract;
        PlayerInput.PlayerInput.ItemGet.performed += OnItemGet;
        PlayerInput.PlayerInput.OpenMenu.performed += OnMenu;
    }
    private void OnDisable()
    {
        PlayerInput.PlayerInput.Disable();
        PlayerInput.PlayerInput.Possession.performed -= OnPossession;
        PlayerInput.PlayerInput.Inventory.performed -= OnInventory;
        PlayerInput.PlayerInput.Interact.performed -= OnInteract;
        PlayerInput.PlayerInput.ItemGet.performed -= OnItemGet;
        PlayerInput.PlayerInput.OpenMenu.performed -= OnMenu;
    }

    private void Awake()
    {
        PlayerInput = new Player_Input();
        PlayerActions = PlayerInput.PlayerInput;
    }
    // 빙의 키 Q 누르면 이벤트 실행//
    private void OnPossession(InputAction.CallbackContext context)
    {
        OnPossessEvent?.Invoke();
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        OnInventoryEvent?.Invoke();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        OnInteractEvent?.Invoke();
    }

    private void OnItemGet(InputAction.CallbackContext context)
    {
        OnItemGetEvent?.Invoke();
    }
    private void OnMenu(InputAction.CallbackContext context)
    {
        OnMenuEvent?.Invoke();
    }
}