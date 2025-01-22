using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    #region Variables

    [Header("Player Settings")]
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    [Header("Camera Settings")]
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    public bool canMove = true;

    private CharacterController characterController;

    #endregion

    #region Unity Methods

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleUI();
        HandleMovementAndJumping();
        HandleRotation();
    }

    #endregion

    #region Custom Methods

    private void HandleUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main Menu");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void HandleMovementAndJumping()
    {
        if (!canMove) 
            return;

        // Determine movement speed
        float speed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed = runSpeed;

        // Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector3 horizontalMovement = (forward * moveVertical + right * moveHorizontal) * speed;

        // Jumping
        if (characterController.isGrounded)
        {
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpPower; // Jump
            else
                moveDirection.y = 0; // Reset vertical movement on the ground
        }
        else
            moveDirection.y -= gravity * Time.deltaTime; // Apply gravity while in the air

        // Combine horizontal and vertical movement
        moveDirection.x = horizontalMovement.x;
        moveDirection.z = horizontalMovement.z;

        // Apply movement
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (!canMove) return;

        // Vertical camera rotation
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // Horizontal player rotation
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        transform.rotation *= Quaternion.Euler(0, mouseX, 0);
    }

    #endregion
}
