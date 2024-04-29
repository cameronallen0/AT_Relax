using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerControls inputActions;

    private CharacterController controller;

    public BuildSystem gridScript;
    public InventoryManager inventoryManager;

    //Camera Variables
    [SerializeField] public Camera cam;
    [SerializeField] public float lookSensitivity = 30f;

    public LayerMask groundLayer;

    private float xRotation = 0f;

    // Movement Variables
    private Vector3 velocity;
    private float gravity = -20f;
    private bool grounded;
    [SerializeField] private float movementSpeed;
    private float walkSpeed = 3f;
    private float runSpeed = 6f;
    private bool isRunning;

    //Jump Variables
    private float jumpHeight = 1.1f;
    private bool isJumping;


    //Placing Variables
    public float reach = 5f;

    private bool isOpen;

    private void Awake()
    {
        inputActions = new PlayerControls();
        isOpen = false;
    }
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void Update()
    {
        DoMovement();
        DoLooking();
        DoJump();
        DoFire();
        DoRemove();
        DoInventory();
        DoRun();
    }

    private void DoInventory()
    {
        if (isOpen == false)
        {
            if (inputActions.PlayerController.Inventory.triggered)
            {
                gridScript.inventory.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                isOpen = true;
            }
        }
        if (gridScript.inventory.activeInHierarchy == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            isOpen = false;
        }
    }

    private void DoLooking()
    {
        if (isOpen == false)
        {
            Vector2 looking = GetPlayerLook();
            float lookX = looking.x * lookSensitivity * Time.deltaTime;
            float lookY = looking.y * lookSensitivity * Time.deltaTime;

            xRotation -= lookY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * lookX);
        }
    }

    private void DoMovement()
    {
        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
            isJumping = false;
        }
        else
        {
            movementSpeed = walkSpeed;
        }

        Vector2 movement = GetPlayerMovement();
        Vector3 move = transform.right * movement.x + transform.forward * movement.y;
        controller.Move(move * movementSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void DoRun()
    {
        if (inputActions.PlayerController.Run.ReadValue<float>() > 0)
        {
            isRunning = !isRunning;
            movementSpeed = runSpeed;
        }
        else
        {
            movementSpeed = walkSpeed;
        }
    }

    private void DoFire()
    {
        if (isOpen == false)
        {
            if (inputActions.PlayerController.Fire.triggered)
            {
                gridScript.BuildBlock();
            }
        }
    }

    private void DoRemove()
    {
        if(isOpen == false)
        {
            if (inputActions.PlayerController.Remove.triggered)
            {
                gridScript.DestroyBlock();
            }
        }     
    }

    private Vector2 GetMousePositionInWorld()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            return hit.point;
        }

        return Vector2.zero;
    }

    private void DoJump()
    {
        if (grounded)
        {
            if (inputActions.PlayerController.Jump.triggered)
            {
                isJumping = !isJumping;
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return inputActions.PlayerController.Move.ReadValue<Vector2>();
    }

    public Vector2 GetPlayerLook()
    {
        return inputActions.PlayerController.Look.ReadValue<Vector2>();
    }
}
