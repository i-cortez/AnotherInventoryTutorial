// SC_PickItem.cs
//
// Ismael Cortez
// 12-10-2020
// Simple Inventory System
//
// Adapted from Sharp Coder:
// https://sharpcoderblog.com/blog/unity-3d-coding-a-simple-inventory-system-with-ui-drag-and-drop
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_PickItem : MonoBehaviour
{
   // Each item must have a unique name.
   public string itemName = "My Item";
   public Texture itemPreview;

   void Start()
   {
      // Change item tag to Respawn to detect when we look at it.
      gameObject.tag = "Respawn";
   }

   public void PickItem()
   {
      Destroy(gameObject);
   }
}

