using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryItemController : MonoBehaviour
{
    public Item item;

    public void AddItem(Item newItem)
    {
        item = newItem;
    }

    public void UseItem()
    {
        if(item != null) 
        {
            switch (item.itemType)
            {
                case Item.ItemType.Blocks:
                    BuildSystem.Instance.SetObject(item.itemObject);
                    BuildSystem.Instance.ChangeText(item.itemName);
                    BuildSystem.Instance.ChangeAudio(item.placeClip, item.pitch, item.volume);
                    break;
            }
        }
        else
        {
            Debug.LogWarning("No item selected to use.");
        }  
    }
}
