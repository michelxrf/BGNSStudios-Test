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

    private bool isVisible = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Hide();
        InventorySystem.instance.OnInventoryChanged += RefreshItems;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(isVisible)
            {
                Hide();
            }
            else
            {
                Show();
            }
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

    public void RefreshItems()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i] != null)
                itemSlots[i].SetItem(InventorySystem.instance.GetItemByIndex(i));
            else
                itemSlots[i].Clear();
        }
    }

    private void OnDestroy()
    {
        InventorySystem.instance.OnInventoryChanged -= RefreshItems;
    }
}
