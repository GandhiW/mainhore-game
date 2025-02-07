using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlotUI_Furniture : SlotUI
{
    public override void OnSlotTouched()
    {
        base.OnSlotTouched();
        if(FindObjectOfType<HouseManager>().IsInDecorationMode)
        {
            secondButton.gameObject.SetActive(true);
        }
    }
    public override void secondAction()
    {
        unpackItem();
    }

    //Place item in house
    private void unpackItem()
    {
        Debug.Log("Placing: " + currentItem.itemName);
        FindObjectOfType<InventoryManager>().OnUnpackFurniture.Invoke(currentItem as SO_Furniture);
        FindObjectOfType<InventoryManager>().RemoveItem(currentItem);
    }
}
