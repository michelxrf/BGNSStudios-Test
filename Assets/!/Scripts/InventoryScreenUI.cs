using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Handles inventory screen
/// </summary>
public class InventoryScreenUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [SerializeField]
    private ItemSlot[] itemSlots;

    private PlayerInput playerInput;
    private InputAction inventoryAction;
    private bool isVisible = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        inventoryAction = playerInput.actions["Inventory"];
        inventoryAction.performed += OnInventoryActionPerformed;

        Hide();
    }

    private void OnDestroy()
    {
        if (inventoryAction != null)
        {
            inventoryAction.performed -= OnInventoryActionPerformed;
        }
    }

    private void OnInventoryActionPerformed(InputAction.CallbackContext context)
    {
        if (isVisible)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        isVisible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InventorySystem.instance.ToggleInventory(false);
    }

    private void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        isVisible = true;
        RefreshItems();
        Cursor.lockState = CursorLockMode.None;
        InventorySystem.instance.ToggleInventory(true);
    }

    private void RefreshItems()
    {
        var inventory = InventorySystem.instance.Inventory;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < inventory.Count)
            {
                itemSlots[i].SetItem(inventory[i]);
            }
            else
            {
                itemSlots[i].Clear();
            }
        }
    }
}
