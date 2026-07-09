# Architectural Specification: Inventory System

* **Status**: APPROVED
* **Date**: 2026-07-09
* **Engine Focus**: Unity 6 LTS

---

## 1. Design Intent & Requirements Traceability

The Inventory System manages the storage, query, combination, and serialization of all items, collectibles, and crafting materials:

* **Frictionless Crafting (Vision §2 & GDD §2.2.4)**: Children must not be blocked by inventory management during active learning loops. Crafting materials (e.g. driftwood, reed-fiber, brass shavings) are stored in an **unlimited, stack-free storage** layer, while quest and cosmetic items are restricted to a slot-based layout (24 initial slots).
* **Drag-and-Combine Math Workbench (GDD §2.6 & §2.4.1)**: To support fractions (e.g., combining two `1/4` planks to craft one `1/2` plank), the inventory system exposes an allocation-free item combining API integrated with our drag-and-drop Workbench UI.
* **Cooperative Social Gifting (Vision §11 & GDD §3.5)**: The inventory system supports async item replication and transfer, allowing players to leave gift packages for friends on the Bramble hub Board.

---

## 2. Item Registry & ScriptableObject Configurations

Item definitions are authored in the Unity Editor as static configurations using ScriptableObjects.

### 2.1 C# Data Definitions

```csharp
using UnityEngine;

namespace QuestBit.Systems.Inventory
{
    public enum ItemCategory
    {
        CraftMaterial,  // Unlimited storage (driftwood, brass)
        QuestItem,      // Slot-bound storage (keys, tools, logs)
        CosmeticCharm   // Slot-bound storage (outfits, badges)
    }

    [CreateAssetMenu(fileName = "so_item_default", menuName = "QuestBit/Item Definition")]
    public class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string _itemId = null!;
        [SerializeField] private string _displayNameKey = null!; // Localized key
        [SerializeField] private ItemCategory _category;
        [SerializeField] private Sprite _icon = null!;
        
        public string ItemId => _itemId;
        public string DisplayNameKey => _displayNameKey;
        public ItemCategory Category => _category;
        public Sprite Icon => _icon;
    }
}
```

---

## 3. Inventory System Interface & Implementation

The Inventory System separates the storage layers into a dictionary for materials and a fixed list for slot-bound items.

### 3.1 Interface Contracts

```csharp
using System.Collections.Generic;
using QuestBit.Core.EventBus;

namespace QuestBit.Systems.Inventory
{
    public struct InventorySlot
    {
        public string ItemId;
        public int Quantity;
        public bool IsEmpty => string.IsNullOrEmpty(ItemId);
    }

    public interface IInventorySystem
    {
        int MaxQuestSlots { get; }
        IReadOnlyList<InventorySlot> GetQuestSlots();
        IReadOnlyDictionary<string, int> GetMaterials();

        // Transaction APIs
        bool AddItem(string itemId, int quantity);
        bool RemoveItem(string itemId, int quantity);
        bool HasItem(string itemId, int quantity);
        
        // Workbench Crafting API (Drag-and-Combine)
        Result<string> CombineItems(string itemAId, string itemBId);
    }
}
```

### 3.2 Inventory Manager Class

```csharp
using System.Collections.Generic;
using VContainer;
using QuestBit.Core.EventBus;
using QuestBit.Gameplay.Events; // Matches Event Bus payloads

namespace QuestBit.Systems.Inventory
{
    public class InventoryManager : IInventorySystem
    {
        private readonly IEventBus _eventBus;
        private readonly Dictionary<string, int> _materials = new Dictionary<string, int>(16);
        private readonly List<InventorySlot> _questSlots = new List<InventorySlot>(24);
        
        private readonly Dictionary<string, string> _recipeBook = new Dictionary<string, string>(16);

        public int MaxQuestSlots { get; private set; } = 24;

        [Inject]
        public InventoryManager(IEventBus eventBus)
        {
            _eventBus = eventBus;
            InitializeDefaultSlots();
            RegisterWorkbenchRecipes();
        }

        public IReadOnlyList<InventorySlot> GetQuestSlots() => _questSlots;
        public IReadOnlyDictionary<string, int> GetMaterials() => _materials;

        public bool AddItem(string itemId, int quantity)
        {
            // Fetch category from item database (simulated lookup)
            var category = GetItemCategory(itemId);

            if (category == ItemCategory.CraftMaterial)
            {
                if (_materials.TryGetValue(itemId, out int currentQty))
                {
                    _materials[itemId] = currentQty + quantity;
                }
                else
                {
                    _materials[itemId] = quantity;
                }
                return true;
            }

            // Handle Slot-Bound Items (Quest/Cosmetic)
            return AddToQuestSlots(itemId, quantity);
        }

        public bool RemoveItem(string itemId, int quantity)
        {
            var category = GetItemCategory(itemId);

            if (category == ItemCategory.CraftMaterial)
            {
                if (_materials.TryGetValue(itemId, out int currentQty) && currentQty >= quantity)
                {
                    _materials[itemId] = currentQty - quantity;
                    if (_materials[itemId] == 0) _materials.Remove(itemId);
                    return true;
                }
                return false;
            }

            // Remove from Slot-Bound items
            for (int i = 0; i < _questSlots.Count; i++)
            {
                if (_questSlots[i].ItemId == itemId && _questSlots[i].Quantity >= quantity)
                {
                    var updatedSlot = new InventorySlot
                    {
                        ItemId = _questSlots[i].Quantity == quantity ? string.Empty : itemId,
                        Quantity = _questSlots[i].Quantity - quantity
                    };
                    _questSlots[i] = updatedSlot;
                    return true;
                }
            }
            return false;
        }

        public bool HasItem(string itemId, int quantity)
        {
            var category = GetItemCategory(itemId);
            if (category == ItemCategory.CraftMaterial)
            {
                return _materials.TryGetValue(itemId, out int qty) && qty >= quantity;
            }

            int count = 0;
            foreach (var slot in _questSlots)
            {
                if (slot.ItemId == itemId) count += slot.Quantity;
            }
            return count >= quantity;
        }

        public Result<string> CombineItems(string itemAId, string itemBId)
        {
            // Ensure both items are in inventory
            if (!HasItem(itemAId, 1) || !HasItem(itemBId, 1))
            {
                return Result<string>.Fail("Missing components in inventory.");
            }

            // Create sorting-independent recipe key (e.g. "plank_quarter+plank_quarter")
            string recipeKey = string.Compare(itemAId, itemBId) < 0 
                ? $"{itemAId}+{itemBId}" 
                : $"{itemBId}+{itemAId}";

            if (_recipeBook.TryGetValue(recipeKey, out string outputItemId))
            {
                // Remove ingredients and add resulting item
                RemoveItem(itemAId, 1);
                RemoveItem(itemBId, 1);
                AddItem(outputItemId, 1);
                
                return Result<string>.Ok(outputItemId);
            }

            return Result<string>.Fail("No recipe found for this combination.");
        }

        private void InitializeDefaultSlots()
        {
            for (int i = 0; i < MaxQuestSlots; i++)
            {
                _questSlots.Add(new InventorySlot { ItemId = string.Empty, Quantity = 0 });
            }
        }

        private void RegisterWorkbenchRecipes()
        {
            // Register fraction plank combinations (GDD §2.4.1)
            _recipeBook.Add("item_plank_quarter+item_plank_quarter", "item_plank_half");
            _recipeBook.Add("item_plank_quarter+item_plank_half", "item_plank_three_quarter");
            
            // Register traversal tool recipe combinations
            _recipeBook.Add("material_driftwood+material_reed_fiber", "item_rope_bridge_kit");
        }

        private ItemCategory GetItemCategory(string itemId)
        {
            if (itemId.StartsWith("material_")) return ItemCategory.CraftMaterial;
            if (itemId.StartsWith("item_")) return ItemCategory.QuestItem;
            return ItemCategory.CosmeticCharm;
        }
    }
}
```

---

## 4. Local-First Inventory Replication & Social Sync

1. **Instant Transactions**: Inventory changes occur on the local thread instantly, resolving gameplay events within **<1ms**.
2. **Local Auto-Save Trigger**: Any inventory change (adding a material or combining items) publishes an event that prompts the `ISaveSystem` to save the game data state locally, preventing data loss.
3. **Async Gifting**: Tapping "Gift Item" on the Bramble Hub board packages the selected items into a JSON payload. This is cached in the networking sync queue and sent asynchronously to our GCS server without blocking active play (Offline-First compliance).

---

## 5. Failure Modes & Edge Cases

### 1. Quest Slot Overflow (Inventory Full)
* **Symptom**: Player completes a quest yielding a Cosmetic Charm, but all 24 inventory slots are full.
* **Mitigation**: Inventory overflow must never result in item destruction. If quest rewards cannot fit in the inventory slot grid, the system:
  1. Spawns the reward item as a floating in-world package near the player character.
  2. Places a temporary placeholder item in the player's Bramble Social Mailbox.
  3. Displays a friendly prompt suggesting the player clear space or expand their slots at Gullhaven (GDD §3.2).

### 2. Crafting Loop Race Conditions
* **Symptom**: Dragging two planks onto the workbench in rapid succession causes double-processing, consuming ingredients twice but yielding one output item.
* **Mitigation**: The UI workbench slots lock interaction during combination computations (using a **300ms visual combine animation** lock).

---

## 6. Verification & Automated Tests

1. **Drag-and-Combine Validation Unit Test**:
   Assert that adding two `item_plank_quarter` items to the inventory and calling `CombineItems("item_plank_quarter", "item_plank_quarter")`:
   * Returns a successful result containing `item_plank_half`.
   * Asserts the quantities of `item_plank_quarter` in inventory decrease by 2.
   * Asserts the quantity of `item_plank_half` increases by 1.

2. **Unlimited Stack Overflow Test**:
   Verify that adding **1,000,000** units of `material_driftwood` to the inventory does not cause memory leaks or throw stack overflow errors.
