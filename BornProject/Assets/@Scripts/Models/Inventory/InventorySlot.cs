using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventorySlot
{

    #region Properties

    public Item Item
    {
        get => _item;
        set
        {
            if (value == _item) return;
            if (value == null)
            {
                _item.OnChangedStack -= OnChangedStack;
                _item.OnStackZero -= OnStackZero;
                _item = null;
            }
            else
            {
                _item = value;
                _item.OnChangedStack -= OnChangedStack;
                _item.OnStackZero -= OnStackZero;
                _item.OnChangedStack += OnChangedStack;
                _item.OnStackZero += OnStackZero;
            }
            OnChanged?.Invoke(this);
        }
    }
    public int Stack => Item == null ? -1 : Item.Stack;
    public bool IsCraftingResultSlot { get; set; } = false;

    #endregion

    #region Fields

    private Item _item;

    // Callbacks.
    public Action OnGetCraftingResult;
    public event Action<InventorySlot> OnChanged;

    #endregion

    public bool IsEmpty() => Item == null;
    public bool IsItem(ItemData data) => Item != null && Item.Data == data;
    public bool IsItem(Item item) => Item == item;

    public bool SetItem(Item item) // 인벤토리 슬롯에 아이템을 설정.
    {
        if (Item != null) return false;
        Item = item;

        return true;
    }

    public int TryAdd(int stack = 1) // 기존 슬롯에 있는 아이템에 일정 개수의 스택(수량)을 추가하려고 시도.
    {
        if (this.Item == null) return -1;
        return this.Item.TryAddStack(stack);
    }
    public int TryAdd(ItemData data, int stack = 1) // 'ItemData'로 지정된 새 항목을 슬롯에 추가하려고 시도.
    {
        if (this.Item == null)
        {
            if (stack > data.MaxStack)
            {
                SetItem(new(data, stack: data.MaxStack));
                return stack - data.MaxStack;
            }
            else
            {
                SetItem(new(data, stack: stack));
                return 0;
            }
        }
        else if (this.Item.Data != data) return -1;
        else return TryAdd(stack);
    }
    public int TryAdd(Item item) // 전체'Item'객체를 슬롯에 추가하려고 시도.
    {
        if (this.Item == null)
        {
            SetItem(item);
            return 0;
        }
        else if (this.Item.Data != item.Data) return -1;
        else return this.Item.TryAddStack(item.Stack);
    }

    public void Remove()
    {
        this.Item = null;
    }
    public int TryRemove(int stack = 1)
    {
        if (this.Item == null) return -1;
        return this.Item.TryRemoveStack(stack);
    }
    public int TryRemove(ItemData data, int stack = 1)
    {
        if (this.Item == null || this.Item.Data != data) return -1;
        return this.Item.TryRemoveStack(stack);
    }
    public int TryRemove(Item item, int stack = 1)
    {
        if (this.Item != item) return -1;
        return this.Item.TryRemoveStack(stack);
    }

    public void SwapSlot(InventorySlot slot)
    {
        (this.Item, slot.Item) = (slot.Item, this.Item);
    }
    public Item PopItem(int stack = 1)
    {
        if (this.Item == null || this.Stack < stack) return null;
        Item popItem;
        if (this.Stack == stack)
        {
            popItem = this.Item;
            Item = null;
        }
        else
        {
            popItem = Item.Copy(stack);
            Item.Stack -= stack;
        }
        return popItem;
    }

    #region Callbacks

    private void OnChangedStack(int stack)
    {
        OnChanged?.Invoke(this);
    }
    private void OnStackZero(Item item)
    {
        Item = null;
    }

    #endregion
}