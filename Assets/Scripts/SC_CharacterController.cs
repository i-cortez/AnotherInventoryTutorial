// SC_CharacterController.cs
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

// Automatically adds required components as dependencies.
[RequireComponent(typeof(CharacterController))]

public class SC_CharacterController : MonoBehaviour
{
   public float speed = 7.5f;
   public float jumpSpeed = 8.0f;
   public float gravity = 20.0f;
   public Camera playerCamera;
   public float lookSpeed = 2.0f;
   public float lookXLimit = 60.0f;

   // A CharacterController allows you to easily do movement constrained by collisions without having to deal with a rigidbody.
   CharacterController characterController;
   Vector3 moveDirection = Vector3.zero;
   Vector2 rotation = Vector2.zero;

   // Makes a variable not show up in the inspector but be serialized.
   [HideInInspector]
   public bool canMove = true;

   void Start()
   {
      characterController = GetComponent<CharacterController>();
      rotation.y = transform.eulerAngles.y;
   }

   void Update()
   {
      if(characterController.isGrounded)
      {
         // We are grounded, so recalculate move direction based on axes.
         Vector3 forward = transform.TransformDirection(Vector3.forward);
         Vector3 right = transform.TransformDirection(Vector3.right);
         float curSpeedX = speed * Input.GetAxis("Vertical");
         float curSpeedY = speed * Input.GetAxis("Horizontal");
         moveDirection = (forward * curSpeedX) + (right * curSpeedY);

         if(Input.GetButton("Jump"))
         {
            moveDirection.y = jumpSpeed;
         }
      }

      // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below when the moveDirection is multiplied by deltaTime).
      // This is because gravity should be applied as an acceleration (ms^2)
      moveDirection.y -= gravity * Time.deltaTime;

      // Player and Camera rotation
      if(canMove)
      {
         // Move the controller.
         characterController.Move(moveDirection * Time.deltaTime);

         // Allow for character and camera rotation.
         rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
         rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
         rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
         playerCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
         transform.eulerAngles = new Vector2(0, rotation.y);
      }
   }
}

