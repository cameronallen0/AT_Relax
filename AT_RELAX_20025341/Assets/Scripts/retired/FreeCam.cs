using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCam : MonoBehaviour
{
    private PlayerControls inputActions;
    private CharacterController controller;
    public BuildSystem gridScript;
    public InventoryManager inventoryManager;

    public Camera cam;
    private float movementSpeed = 10f;
    public float lookSensitivity = 30f;
    private float drag = 0.9f;
    private float minVelocityMagnitude = 0.1f;
    private bool isMoving;

    public LayerMask groundLayer;
    private float xRotation = 0f;

    private Vector3 velocity;
    private bool grounded;

    public int reach;

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
        if (!isMoving)
        {
            DoMovement();
        }
        DoLooking();
        DoFire();
        DoRemove();
        DoInventory();
    }

    private void DoInventory()
    {
        if(isOpen == false)
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
        if(!isOpen)
        {
            Vector2 looking = GetPlayerLook();
            float lookX = looking.x * lookSensitivity * Time.deltaTime;
            float lookY = looking.y * lookSensitivity * Time.deltaTime;

            xRotation -= lookY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp to prevent over-rotation

            cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * lookX);
        }
    }

    private void DoMovement()
    {
        if(!isOpen)
        {
            grounded = controller.isGrounded;
            isMoving = true;

            Vector2 movement = GetPlayerMovement();
            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 move = camForward * movement.y + camRight * movement.x;
            velocity *= drag;

            controller.Move(move * movementSpeed * Time.deltaTime);
            if (velocity.magnitude < minVelocityMagnitude)
            {
                isMoving = false;
                velocity = Vector3.zero;
            }
        }
    }

    private void DoFire()
    {
        if (!isOpen)
        {
            if (inputActions.PlayerController.Fire.triggered)
            {
                gridScript.BuildBlock();
            }
        }
    }

    private void DoRemove()
    {
        if(!isOpen)
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

    public Vector2 GetPlayerLook()
    {
        return inputActions.PlayerController.Look.ReadValue<Vector2>();
    }
    public Vector2 GetPlayerMovement()
    {
        return inputActions.PlayerController.Move.ReadValue<Vector2>();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
}
