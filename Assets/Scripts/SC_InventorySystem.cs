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

public class SC_InventorySystem : MonoBehaviour
{
   public Texture crosshairTexture;
   public SC_CharacterController playerController;

   // List with Prefabs of all the available items.
   public SC_PickItem[] availableItems;

   // Available items slots.
   int[] itemSlots = new int[12];
   bool showInventory = false;
   float windowAnimation = 1;
   float animationTimer = 0;

   // UI Drag & Drop.
   int hoveringOverIndex = -1;
   int itemIndexToDrag = -1;
   Vector2 dragOffset = Vector2.zero;

   // Item pick up.
   SC_PickItem detectedItem;
   int detectedItemIndex;

   // Start is called before the first frame update.
   void Start()
   {
      // Determines whether the hardware pointer is visible or not.
      Cursor.visible = false;

      // Determines whether the hardware pointer is locked to the center of the view, constrained to the window, or not constrained at all.
      Cursor.lockState = CursorLockMode.Locked;

      // Initialize the Item Slots
      for(int i = 0; i < itemSlots.Length; ++i)
      {
         // Here -1 represents an empty slot
         itemSlots[i] = -1;
      }
   }

   // Update is called once per frame.
   void Update()
   {
      // Show or Hide inventory UI.
      if(Input.GetKeyDown(KeyCode.I))
      {
         showInventory = !showInventory;
         animationTimer = 0;

         if(showInventory)
         {
            // Allows the player to use the cursor while inventory is open.
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
         }

         else
         {
            // Hide and lock the cursor when inventory is not in use.
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
         }
      }

      if(animationTimer < 1)
      {
         animationTimer += Time.deltaTime;
      }

      if(showInventory)
      {
         // Open the inventory menu and freeze movement.
         windowAnimation = Mathf.Lerp(windowAnimation, 0, animationTimer);
         playerController.canMove = false;
      }

      else
      {
         // Close the inventory menu and allow movement
         windowAnimation = Mathf.Lerp(windowAnimation, 1f, animationTimer);
         playerController.canMove = true;
      }

      // Begin the item drag.
      if(Input.GetMouseButtonDown(0) && hoveringOverIndex > -1 && itemSlots[hoveringOverIndex] > -1)
      {
         itemIndexToDrag = hoveringOverIndex;
      }

      // Release the dragged item
      if(Input.GetMouseButtonUp(0) && itemIndexToDrag > -1)
      {
         if(hoveringOverIndex < 0)
         {
            // Drop the item outside
            Instantiate(availableItems[itemSlots[itemIndexToDrag]], playerController.playerCamera.transform.position + (playerController.playerCamera.transform.forward), Quaternion.identity);
            itemSlots[itemIndexToDrag] = -1;
         }

         else
         {
            // Switch items between the selected slot and the one we are hovering on
            int itemIndexTmp = itemSlots[itemIndexToDrag];
            itemSlots[itemIndexToDrag] = itemSlots[hoveringOverIndex];
            itemSlots[hoveringOverIndex] = itemIndexTmp;
         }

         // Clear the index
         itemIndexToDrag = -1;
      }

      // Item pick up.
      if(detectedItem && detectedItemIndex > -1)
      {
         if(Input.GetKeyDown(KeyCode.F))
         {
            // Add the item to inventory in first empty slot found
            int slotToAddTo = -1;
            for(int i = 0; i < itemSlots.Length; ++i)
            {
               if(itemSlots[i] == -1)
               {
                  slotToAddTo = i;
                  break;
               }
            }

            if(slotToAddTo > -1)
            {
               // Add the item to the inventory when empty slot is found
               itemSlots[slotToAddTo] = detectedItemIndex;
               detectedItem.PickItem();
            }

            else
            {
               Debug.Log("Inventory is full!");
            }
         }
      }
   }

   // Update phase in the native player loop.
   void FixedUpdate()
   {
      // Detect if the Player is looking at any item
      RaycastHit hit;
      Ray ray = playerController.playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

      if (Physics.Raycast(ray, out hit, 2.5f))
      {
         Transform objectHit = hit.transform;

         if (objectHit.CompareTag("Respawn"))
         {
            if ((detectedItem == null || detectedItem.transform != objectHit) && objectHit.GetComponent<SC_PickItem>() != null)
            {
               SC_PickItem itemTmp = objectHit.GetComponent<SC_PickItem>();

               // Check if item is in availableItemsList
               for (int i = 0; i < availableItems.Length; ++i)
               {
                  if (availableItems[i].itemName == itemTmp.itemName)
                  {
                     detectedItem = itemTmp;
                     detectedItemIndex = i;
                  }
               }
            }
         }

         else detectedItem = null;
      }

      else detectedItem = null;
   }

   void OnGUI()
   {
      // Inventory UI
      GUI.Label(new Rect(5, 5, 200, 25), "Press 'I' to open Inventory");

      // Draw the inventory window
      if(windowAnimation < 1)
      {
         GUILayout.BeginArea(new Rect(10 - (430 * windowAnimation), Screen.height / 2 - 200, 302, 430), GUI.skin.GetStyle("box"));
         GUILayout.Label("Inventory", GUILayout.Height(25));

         // Begin a vertical control group.
         // All controls rendered inside this element will be placed vertically below each other.
         GUILayout.BeginVertical();

         for(int i = 0; i < itemSlots.Length; i += 3)
         {
            // Begin a horizontal control group.
            // All controls rendered inside this element will be placed horizontally next to each other.
            GUILayout.BeginHorizontal();

            // Display 3 items in a row
            for(int j = 0; j < 3; ++j)
            {
               if(i + j < itemSlots.Length)
               {
                  if(itemIndexToDrag == i + j || (itemIndexToDrag > -1 && hoveringOverIndex == i + j))
                  {
                     GUI.enabled = false;
                  }

                  if(itemSlots[i + j] > -1)
                  {
                     if(availableItems[itemSlots[i + j]].itemPreview)
                     {
                        // Show the texture
                        GUILayout.Box(availableItems[itemSlots[i + j]].itemPreview, GUILayout.Width(95), GUILayout.Height(95));
                     }

                     else
                     {
                        // Show the item name if no texture is available
                        GUILayout.Box(availableItems[itemSlots[i + j]].itemName, GUILayout.Width(95), GUILayout.Height(95));
                     }
                  }

                  else
                  {
                     // Empty slot
                     GUILayout.Box("", GUILayout.Width(95), GUILayout.Height(95));
                  }

                  // Detect if the mouse cursor is hovering over item
                  Rect lastRect = GUILayoutUtility.GetLastRect();
                  Vector2 eventMousePosition = Event.current.mousePosition;
                  if(Event.current.type == EventType.Repaint && lastRect.Contains(eventMousePosition))
                  {
                     hoveringOverIndex = i + j;
                     if(itemIndexToDrag < 0)
                     {
                        dragOffset = new Vector2(lastRect.x - eventMousePosition.x, lastRect.y - eventMousePosition.y);
                     }
                  }

                  GUI.enabled = true;
               }
            }

            GUILayout.EndHorizontal();
         }

         GUILayout.EndVertical();

         if(Event.current.type == EventType.Repaint && !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
         {
            hoveringOverIndex = -1;
         }

         GUILayout.EndArea();
      }

      // Item dragging
      if(itemIndexToDrag > -1)
      {
         if(availableItems[itemSlots[itemIndexToDrag]].itemPreview)
         {
            // Show the image
            GUI.Box(new Rect(Input.mousePosition.x + dragOffset.x, Screen.height - Input.mousePosition.y + dragOffset.y, 95, 95), availableItems[itemSlots[itemIndexToDrag]].itemPreview);
         }

         else
         {
            // Show the text
            GUI.Box(new Rect(Input.mousePosition.x + dragOffset.x, Screen.height - Input.mousePosition.y + dragOffset.y, 95, 95), availableItems[itemSlots[itemIndexToDrag]].itemName);
         }
      }

      if(hoveringOverIndex > -1 && itemSlots[hoveringOverIndex] > -1 && itemIndexToDrag < 0)
      {
         GUI.Box(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y - 30, 100, 25), availableItems[itemSlots[hoveringOverIndex]].itemName);
      }

      if(!showInventory)
      {
         // Player crosshair
         GUI.color = detectedItem ? Color.green : Color.white;
         GUI.DrawTexture(new Rect(Screen.width / 2 - 4, Screen.height / 2 - 4, 8, 8), crosshairTexture);
         GUI.color = Color.white;

         // Pick up message
         if(detectedItem)
         {
            GUI.color = new Color(0, 0, 0, 0.84f);
            GUI.Label(new Rect(Screen.width / 2 - 75 + 1, Screen.height / 2 - 50 + 1, 200, 20), "Press 'F' to pick up '" + detectedItem.itemName + "'");
            GUI.color = Color.green;
            GUI.Label(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 50, 200, 20), "Press 'F' to pick up '" + detectedItem.itemName + "'");
         }
      }
   }
}

