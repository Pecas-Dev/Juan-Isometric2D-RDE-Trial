using JuanIsometric2D.Animation.Player;
using JuanIsometric2D.GameInputSystem;
using JuanIsometric2D.InventorySystem;

using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;


namespace JuanIsometric2D.StateMachine.Player
{
    public class PlayerStateMachine : StateMachine
    {
        public enum PlayerState
        {
            Idle,
            Movement
        }

        readonly string[] scenesWithoutAudio = { "Isometric2D_Task2", "Isometric2D_Task3" };
        readonly string[] scenesWithoutInventory = { "Isometric2D_Task2", "Isometric2D_Task3" };

        bool isAudioDisabled;
        bool isInventoryDisabled;


        [Header("Player State")]
        [SerializeField, ReadOnlyInspector] PlayerState playerCurrentState;
        public PlayerState CurrentState => playerCurrentState;


        [Header("Component References")]
        [SerializeField] Rigidbody2D playerRigidbody2D;
        [SerializeField] CapsuleCollider2D playerCapsuleCollider2D;
        [SerializeField] Animator playerMotionAnimator;
        public Rigidbody2D PlayerRigidbody2D => playerRigidbody2D;
        public CapsuleCollider2D PlayerCapsuleCollider2D => playerCapsuleCollider2D;
        public Animator PlayerAnimator => playerMotionAnimator;


        [Header("Script References")]
        [SerializeField] GameInputSO playerGameInputSO;
        [SerializeField] PlayerAnimator playerAnimatorScript;
        public GameInputSO PlayerGameInputSO => playerGameInputSO;
        public PlayerAnimator PlayerAnimatorScript => playerAnimatorScript;


        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 5f;
        public float MoveSpeed => moveSpeed;


        [Header("Isometric Settings")]
        [SerializeField] float isometricAngle = 45f;
        public float IsometricAngle => isometricAngle;


        [Header("Audio Settings")]
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip[] hurtSounds;
        [SerializeField] AudioClip swooshSound;
        [SerializeField] AudioClip projectileShotSound;


        [Header("Inventory")]
        [SerializeField] InventoryUI inventoryUI;


        Inventory inventory;


        float currentSpeedMultiplier = 1f;
        public float CurrentSpeedMultiplier => currentSpeedMultiplier;


        bool isInventoryVisible = false;


        void Awake()
        {
            if (playerGameInputSO != null)
            {
                playerGameInputSO.Initialize();
            }

            playerRigidbody2D = GetComponent<Rigidbody2D>();
            playerCapsuleCollider2D = GetComponent<CapsuleCollider2D>();
            playerAnimatorScript = GetComponent<PlayerAnimator>();
            playerMotionAnimator = GetComponentInChildren<Animator>();
            audioSource = GetComponent<AudioSource>();

            isInventoryDisabled = scenesWithoutInventory.Contains(SceneManager.GetActiveScene().name);

            if (!isInventoryDisabled)
            {
                inventoryUI = FindFirstObjectByType<InventoryUI>();
                InventorySettings();
            }
        }

        void Start()
        {
            LogNullReferenceErrors();
            SwitchState(new PlayerIdleState(this));
        }

        void OnDisable()
        {
            playerGameInputSO.Uninitialize();
        }

        void LogNullReferenceErrors()
        {
            if (playerRigidbody2D == null)
            {
                Debug.LogError("Rigidbody2D (" + gameObject.name + ") - Reference is missing!");
            }
            if (playerCapsuleCollider2D == null)
            {
                Debug.LogError("CapsuleCollider2D (" + gameObject.name + ") - Reference is missing!");
            }
            if (playerGameInputSO == null)
            {
                Debug.LogError("GameInputSO (" + gameObject.name + ") - Reference is missing!");
            }
            if (playerAnimatorScript == null)
            {
                Debug.LogError("PlayerAnimator script (" + gameObject.name + ") - Reference is missing!");
            }
            if (playerMotionAnimator == null)
            {
                Debug.LogError("Animator (" + gameObject.name + ") - Reference is missing!");
            }

            isAudioDisabled = scenesWithoutAudio.Contains(SceneManager.GetActiveScene().name);

            if (!isAudioDisabled)
            {
                if (audioSource == null)
                {
                    Debug.LogError("AudioSource (" + gameObject.name + ") - Reference is missing!");
                }
                if (hurtSounds == null || hurtSounds.Length == 0)
                {
                    Debug.LogError("Hurt Sounds (" + gameObject.name + ") - References are missing!");
                }
                if (swooshSound == null)
                {
                    Debug.LogError("Swoosh Sound (" + gameObject.name + ") - Reference is missing!");
                }
                if (projectileShotSound == null)
                {
                    Debug.LogError("Projectile Shot Sound (" + gameObject.name + ") - Reference is missing!");
                }
            }
        }

        void InventorySettings()
        {
            if (isInventoryDisabled) 
            {
                return;
            }

            inventory = new Inventory();
            inventoryUI.SetInventory(inventory);

            if (inventoryUI != null)
            {
                inventoryUI.gameObject.SetActive(false);
            }
        }

        public void HandleInventoryInput()
        {
            if (isInventoryDisabled)
            {
                return;
            }

            if (playerGameInputSO.InventoryTogglePressed)
            {
                ToggleInventory();
                playerGameInputSO.ResetInventoryToggle();
            }
        }

        void ToggleInventory()
        {
            if (isInventoryDisabled)
            {
                return;
            }

            if (inventoryUI != null)
            {
                isInventoryVisible = !isInventoryVisible;
                inventoryUI.gameObject.SetActive(isInventoryVisible);
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isInventoryDisabled)
            {
                Item item = collision.GetComponent<Item>();

                if (item != null && item.gameObject.activeInHierarchy)
                {
                    item.gameObject.SetActive(false);
                    inventory.AddItem(item);
                    Destroy(collision.gameObject);
                    return;
                }
            }

            if (collision.CompareTag("GrassZone"))
            {
                var grassZone = collision.GetComponent<GrassZone>();

                if (grassZone != null)
                {
                    currentSpeedMultiplier = grassZone.GrassZoneSO.SpeedMultiplier;

                    if (playerCurrentState == PlayerState.Movement && currentState is PlayerMovementState movementState)
                    {
                        movementState.SetSpeedMultiplier(currentSpeedMultiplier);
                    }
                }
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("GrassZone"))
            {
                currentSpeedMultiplier = 1f;

                if (playerCurrentState == PlayerState.Movement && currentState is PlayerMovementState movementState)
                {
                    movementState.SetSpeedMultiplier(1f);
                }
            }
        }

        public Inventory GetInventory()
        {
            if (isInventoryDisabled)
            {
                return null;
            }

            return inventory;
        }

        public void PlayHurtSound()
        {
            if (audioSource != null && hurtSounds != null && hurtSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, hurtSounds.Length);

                audioSource.clip = hurtSounds[randomIndex];
                audioSource.Play();
            }
        }

        public void PlaySwooshSound()
        {
            if (audioSource != null && swooshSound != null)
            {
                audioSource.clip = swooshSound;
                audioSource.Play();
            }
        }

        public void PlayProjectileShotSound()
        {
            if (audioSource != null && projectileShotSound != null)
            {
                audioSource.clip = projectileShotSound;
                audioSource.Play();
            }
        }

        internal void SetCurrentState(PlayerState newState)
        {
            playerCurrentState = newState;
        }
    }
}