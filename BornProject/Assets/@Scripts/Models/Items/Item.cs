using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{

    #region Properties

    public ItemData Data { get; protected set; }
    public Creature Owner { get; protected set; }

    public string Key => Data.Key;
    public ItemType Type => Data.Type;
    public string Description => Data.Description;
    public float Cost => Data.Cost;
    public List<StatModifier> Modifiers { get; protected set; }
    public int RemainStack => Data == null ? -1 : Data.MaxStack - Stack; // 남은 스택공간.
    public int MaxStack => Data.MaxStack; // 최대 스택공간.
    public int Stack
    {
        get => _stack;
        set
        {
            if (value == _stack) return;
            if (value == 0)
            {
                _stack = 0;
                OnStackZero?.Invoke(this); // 스택에 없다면 Item을 넣음.
            }
            else
            {
                _stack = value;
                OnChangedStack?.Invoke(_stack); // 스택에 있다면 값을 변경.
            }
        }
    }


    #endregion

    #region Fields

    private int _stack;

    public event Action<int> OnChangedStack;
    public event Action<Item> OnStackZero;

    #endregion
    public Item(ItemData data, Creature owner = null, int stack = 1)
    {
        this.Data = data;
        this.Owner = owner;

        Modifiers = Data.Modifiers.ConvertAll(x => x.Copy());
        Stack = stack;
    }

    public Item Copy(int stack = 0) => new(Data, Owner, stack == 0 ? Stack : stack);

    public int TryAddStack(int stack)
    {
        if (Stack + stack > MaxStack) // Add시 최대Stack 초과 한다면
        {
            stack -= (MaxStack - Stack);
            Stack = MaxStack;
            return stack;
        }
        else
        {
            Stack += stack;
            return 0;
        }
    }
    public int TryRemoveStack(int stack)
    {
        if (Stack >= stack)
        {
            Stack -= stack;
            return 0;
        }
        else
        {
            stack -= Stack;
            Stack = 0;
            return stack;
        }
    }
}