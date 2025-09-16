using UnityEngine;
using System.Collections;

// This script is attached to the physical prefab that represents a dropped item in the world.
[RequireComponent(typeof(Collider))] // Needs a trigger collider to detect the player
public class LootPickup : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The unique tag of this loot prefab in the ObjectPooler.")]
    [SerializeField] private string poolTag = "LootDrop";
    [Tooltip("How long after spawning before the pickup can be collected.")]
    [SerializeField] private float pickupDelay = 0.5f;

    private Item _itemToDrop; // The specific item this pickup represents.
    private bool _canBePickedUp = false;

    private void OnEnable()
    {
        // Start a coroutine to enable pickup after a short delay.
        // This prevents the player from instantly collecting loot from an enemy they just killed.
        StartCoroutine(EnablePickupAfterDelay());
    }

    /// <summary>
    /// Initializes this pickup with the data of the item it should represent.
    /// This is called by the EnemyLootDrop script.
    /// </summary>
    public void Initialize(Item item)
    {
        _itemToDrop = item;
        // Here you could update the visual appearance based on the item,
        // e.g., changing a mesh, material, or icon displayed above the pickup.
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the pickup is ready and if the object that entered the trigger is the player.
        if (_canBePickedUp && other.CompareTag("Player"))
        {
            // If it is, try to add the item to the player's inventory.
            if (_itemToDrop != null && InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddItem(_itemToDrop);
                Debug.Log($"Player picked up: {_itemToDrop.itemName}");

                // After pickup, return this object to the pool.
                ReturnToPool();
            }
        }
    }

    private IEnumerator EnablePickupAfterDelay()
    {
        _canBePickedUp = false;
        yield return new WaitForSeconds(pickupDelay);
        _canBePickedUp = true;
    }

    private void ReturnToPool()
    {
        // Deactivate and return to the pool for reuse.
        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }
}