using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using JuanIsometric2D.Combat;
using JuanIsometric2D.StateMachine.Player;


namespace JuanIsometric2D.InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        const string INVENTORY_SLOT_CONTAINER = "ItemSlotContainer";
        const string INVENTORY_SLOT = "ItemSlot";
        const string INVENTORY_SLOT_IMAGE = "Image";
        const string INVENTORY_AMOUNT = "Amount";
        const string INVENTORY_SELECTOR = "Selector";


        [Header("UI References")]
        [SerializeField] Transform itemSlotContainer;
        [SerializeField] Transform itemSlotPrefab;
        [SerializeField] RectTransform selector;
        [SerializeField] float slotSpacing = 100f;

        [Header("Navigation Settings")]
        [SerializeField] float navigationCooldown = 0.15f;
        [SerializeField] PlayerInputActions inputActions;


        int currentSelectedIndex = 0;


        float lastNavigationTime;


        Inventory inventory;


        void Awake()
        {
            if (itemSlotContainer == null)
            {
                itemSlotContainer = transform.Find(INVENTORY_SLOT_CONTAINER);
            }
            if (itemSlotPrefab == null)
            {
                itemSlotPrefab = itemSlotContainer.Find(INVENTORY_SLOT);

                if (itemSlotPrefab != null)
                {
                    itemSlotPrefab.gameObject.SetActive(false);
                }
            }
            if(selector == null)
            {
                selector = transform.Find(INVENTORY_SELECTOR) as RectTransform;
            }

            inputActions = new PlayerInputActions();
        }

        void OnEnable()
        {
            inputActions.UI.Enable();


            inputActions.UI.Navigate.performed += OnNavigate;

            inputActions.UI.Point.performed += OnPointerMove;

            inputActions.UI.Enter.performed += OnEnterPressed;
        }

        void OnDisable()
        {
            inputActions.UI.Navigate.performed -= OnNavigate;

            inputActions.UI.Point.performed -= OnPointerMove;

            inputActions.UI.Enter.performed -= OnEnterPressed;


            inputActions.UI.Disable();
        }

        void OnNavigate(InputAction.CallbackContext context)
        {
            if (Time.time - lastNavigationTime < navigationCooldown)
            {
                return;
            }

            Vector2 input = context.ReadValue<Vector2>();

            if (input.y != 0)
            {
                int direction = input.y > 0 ? -1 : 1; 

                UpdateSelection(currentSelectedIndex + direction);
                lastNavigationTime = Time.time;
            }
        }

        void OnPointerMove(InputAction.CallbackContext context)
        {
            if (!gameObject.activeInHierarchy) 
            {
                return;
            }

            Vector2 mousePos = context.ReadValue<Vector2>();

            RectTransform containerRect = itemSlotContainer as RectTransform;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(containerRect, mousePos, null, out Vector2 localPoint))
            {
                int index = Mathf.FloorToInt(-localPoint.y / slotSpacing);

                if (index >= 0 && index < inventory.Items.Count)
                {
                    UpdateSelection(index);
                }
            }
        }

        void OnEnterPressed(InputAction.CallbackContext context)
        {
            if (!gameObject.activeInHierarchy) return;

            UseSelectedItem();
        }

        void UseSelectedItem()
        {
            if (currentSelectedIndex < 0 || currentSelectedIndex >= inventory.Items.Count)
            {
                return;

            }

            var selectedItem = inventory.Items[currentSelectedIndex];

            if (selectedItem.ItemType == Item.ItemType.HealthPotion)
            {
                var player = GameObject.FindFirstObjectByType<PlayerStateMachine>();

                if (player != null)
                {
                    var playerHealth = player.GetComponent<HealthSystem>();

                    if (playerHealth != null)
                    {
                        if (inventory.UseHealthPotion(playerHealth))
                        {
                            Debug.Log("Health Potion Used!");
                        }
                    }
                }
            }
        }

        void UpdateSelection(int newIndex)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            int itemCount = inventory.Items.Count;

            if (itemCount == 0) 
            {
                return;
            }

            newIndex = (newIndex + itemCount) % itemCount;
            currentSelectedIndex = newIndex;

            if (selector != null)
            {
                Vector2 position = new Vector2(0, -currentSelectedIndex * slotSpacing);
                selector.anchoredPosition = position;
            }
        }

        public void SetInventory(Inventory inventory)
        {
            this.inventory = inventory;
            inventory.OnInventoryChanged += HandleInventoryChanged;

            RefreshUI();
        }

        void HandleInventoryChanged(object sender, System.EventArgs e)
        {
            RefreshUI();
        }

        void RefreshUI()
        {
            foreach (Transform child in itemSlotContainer)
            {
                if (child == itemSlotPrefab || child == selector) 
                {
                    continue;
                }

                Destroy(child.gameObject);
            }

            int index = 0;

            foreach (var item in inventory.Items)
            {
                var slot = Instantiate(itemSlotPrefab, itemSlotContainer);
                slot.gameObject.SetActive(true);

                var rectTransform = slot.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -index * slotSpacing);

                var image = slot.Find(INVENTORY_SLOT_IMAGE).GetComponent<Image>();
                image.sprite = item.ItemSprite;

                var amountText = slot.Find(INVENTORY_AMOUNT).GetComponent<TextMeshProUGUI>();
                amountText.text = item.Amount > 1 ? item.Amount.ToString() : "";

                index++;
            }

            if (inventory.Items.Count > 0)
            {
                UpdateSelection(currentSelectedIndex);
            }
        }

        void OnDestroy()
        {
            if (inventory != null)
            {
                inventory.OnInventoryChanged -= HandleInventoryChanged;
            }
        }
    }
}
