using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory
{

    #region Properties

    public float Gold
    {
        get => _gold;
        set
        {
            _gold = value;
            OnGoldChanged?.Invoke(_gold);
        }
    }
    public int Count => _slots.Count;
    public int NotEmptyCount => _slots.Where(slot => !slot.IsEmpty()).Count();

    #endregion

    #region Fields

    private float _gold;

    // Collections.
    private List<InventorySlot> _slots = new();

    // Callbacks.
    public event Action<float> OnGoldChanged;
    public event Action OnChanged;

    #endregion

    public Inventory(int count = 99)
    {
        _slots = new();
        for (int i = 0; i < count; i++) _slots.Add(new());
    }
    public InventorySlot this[int index]
    {
        get
        {
            if (index >= _slots.Count) return null;
            return _slots[index];
        }
    }

    public List<InventorySlot> GetAllSlots() => _slots;
    public InventorySlot GetEmptySlot() => _slots.FirstOrDefault(slot => slot.IsEmpty());
    public List<InventorySlot> GetEmptySlots() => _slots.Where(slot => slot.IsEmpty()).ToList();
    public InventorySlot GetSlot(ItemData data) => _slots.FirstOrDefault(slot => slot.IsItem(data));
    public List<InventorySlot> GetSlots(ItemData data, bool containEmpty) => _slots.Where(slot => slot.IsItem(data) || (containEmpty && slot.IsEmpty())).OrderBy(slot => !slot.IsItem(data)).ToList();

    public bool CanAdd(ItemData itemData, int stack = 1)
    {
        List<InventorySlot> slots = GetSlots(itemData, true);
        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];
            stack -= slot.IsEmpty() ? itemData.MaxStack : slot.Item.RemainStack;
            if (stack <= 0) return true;
        }
        return false;
    }

    public void AddSlot(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _slots.Add(new());
        }
        OnChanged?.Invoke();
    }
    public bool RemoveSlot(int count)
    {
        if (count > Count) return false;
        for (int i = Count - 1; i >= 0; i--)
        {
            _slots.RemoveAt(i);
        }
        OnChanged?.Invoke();
        return true;

    }

    public int TryAdd(ItemData itemData, int stack = 1)
    {
        List<InventorySlot> slots = GetSlots(itemData, true);
        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];
            int remainStack = slot.TryAdd(itemData, stack);
            if (remainStack == -1) return -1;
            else if (remainStack == 0) return 0;
            stack = remainStack;
        }
        return stack;
    }
    public int TryAdd(Item item)
    {
        int remainStack = item.Stack;
        List<InventorySlot> slots = GetSlots(item.Data, true);
        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];
            remainStack = slot.TryAdd(item);
            if (remainStack == -1) return -1;
            else if (remainStack == 0) return 0;
            item.Stack = remainStack;
        }
        return remainStack;
    }
    public int TryRemove(ItemData itemData, int stack = 1)
    {
        List<InventorySlot> slots = GetSlots(itemData, false);
        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];
            int remainStack = slot.TryRemove(itemData, stack);
            if (remainStack == -1) return -1;
            else if (remainStack == 0) return 0;
            stack -= remainStack;
        }
        return stack;
    }

    //public bool Add(Item item, int stack = 1) {
    //    InventorySlot slot = GetSlot(item.Data) ?? GetEmptySlot();
    //    if (slot == null) return false;
    //    return slot.AddItem(item.Data, stack);
    //}

    //public bool Remove(ItemData itemData, int stack = 1) {
    //    InventorySlot slot = GetSlot(itemData);
    //    if (slot == null) return false;
    //    return slot.RemoveItem(itemData, stack);
    //}
    //public bool Remove(Item item, int stack = 1) {
    //    InventorySlot slot = GetSlot(item.Data);
    //    if (slot == null) return false;
    //    return slot.RemoveItem(item.Data, stack);
    //}

}