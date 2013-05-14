using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Inventory
{
    private List<Item> items;

    public IEnumerable<Item> Items
    {
        get
        {
            return items;
        }
    }

    public Item EquippedItem
    {
        get;
        private set;
    }

    public Inventory()
    {
        this.items = new List<Item>();
    }

    public bool AddItem(Item i)
    {
        this.items.Add(i);
        return true;
    }

    public void EquipItem(Item i)
    {
        if (this.items.Contains(i))
        {
            Debug.Log("Equipping " + i);
            this.EquippedItem = i;
        }
    }
}

