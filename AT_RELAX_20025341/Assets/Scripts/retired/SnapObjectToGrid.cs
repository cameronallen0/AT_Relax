using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridSnapObjectSpawner : MonoBehaviour
{
    private static GridSnapObjectSpawner _instance;
    public static GridSnapObjectSpawner Instance {  get { return _instance; } }

    public GameObject currentObject;
    public GameObject inventory;
    public float gridCellSize = 1f;
    public int maxObjects = 100;
    public LayerMask objectLayers;

    public TextMeshProUGUI uiText;

    public FreeCam playerScript;
    private List<Vector3> occupiedGridPositions = new List<Vector3>();
    private List<GameObject> gridObjects = new List<GameObject>();

    public void Awake()
    {
        inventory.gameObject.SetActive(false);
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        uiText.text = "";
    }

    public void SetObject(GameObject itemObject)
    {
        if(currentObject != null)
        {
            currentObject = null;
        }
        
        currentObject = itemObject;
    }

    public void SpawnOnGrid()
    {
        if (occupiedGridPositions.Count >= maxObjects)
        {
            Debug.LogWarning("Maximum Objects Reached");
            return;
        }
        if (currentObject == null)
        {
            Debug.LogWarning("Select an Item");
            return;
        }

        Ray ray = playerScript.cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, playerScript.reach, playerScript.groundLayer))
        {
            Vector3 hitPoint = hit.point;
            Vector3 snappedPosition = SnapToGrid(hitPoint);

            if (IsGridPositionOccupied(snappedPosition))
            {
                // If the position is occupied, find the nearest empty position
                snappedPosition = FindNearestEmptyPosition(hitPoint);
            }

            if (snappedPosition == Vector3.zero)
            {
                Debug.LogWarning("No available positions nearby");
                return;
            }

            Vector3 directionToPlayer = playerScript.transform.position - snappedPosition;
            directionToPlayer.y = 0;
            Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer, Vector3.up);

            Vector3 eulerRotation = rotationToPlayer.eulerAngles;
            eulerRotation.y = Mathf.Round(eulerRotation.y / 90f) * 90f;
            rotationToPlayer = Quaternion.Euler(eulerRotation);

            GameObject gridObject = Instantiate(currentObject, snappedPosition, rotationToPlayer);
            occupiedGridPositions.Add(snappedPosition);
            gridObjects.Add(gridObject);
        }
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        Vector3 snappedPosition = new Vector3(
            Mathf.Round(position.x / gridCellSize) * gridCellSize,
            Mathf.Round(position.y / gridCellSize) * gridCellSize,
            Mathf.Round(position.z / gridCellSize) * gridCellSize
        );

        return snappedPosition;
    }

    private Vector3 FindNearestEmptyPosition(Vector3 hitPoint)
    {
        float searchRadius = gridCellSize;

        while (searchRadius < playerScript.reach)
        {
            // Search in a circular pattern around the hit point
            for (float angle = 0; angle < 360; angle += 10)
            {
                Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * searchRadius;
                Vector3 newPosition = hitPoint + offset;
                Vector3 snappedPosition = SnapToGrid(newPosition);

                if (!IsGridPositionOccupied(snappedPosition))
                {
                    // Perform a downward raycast to find the ground or an object
                    RaycastHit hit;
                    Ray downRay = new Ray(newPosition + Vector3.up * playerScript.reach, Vector3.down);
                    if (Physics.Raycast(downRay, out hit, playerScript.reach, playerScript.groundLayer))
                    {
                        // If the ray hits an object or ground layer, return the position
                        return hit.point;
                    }
                    else
                    {
                        // If the ray doesn't hit anything, return the snapped position
                        return snappedPosition;
                    }
                }
            }

            searchRadius += gridCellSize;
        }

        return Vector3.zero;
    }



    public void RemoveFromGrid()
    {
        Ray ray = playerScript.cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, playerScript.reach, objectLayers))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.transform.parent != null)
            {
                hitObject = hitObject.transform.parent.gameObject;
            }

            if (gridObjects.Contains(hitObject))
            {
                int index = gridObjects.IndexOf(hitObject);

                gridObjects.Remove(hitObject);

                if (index >= 0 && index < occupiedGridPositions.Count)
                {
                    occupiedGridPositions.RemoveAt(index);
                }
                Destroy(hitObject);
            }
        }
    }

    private bool IsGridPositionOccupied(Vector3 position)
    {
        foreach (Vector3 occupiedPosition in occupiedGridPositions)
        {
            if (position == occupiedPosition)
            {
                return true;
            }
        }
        return false;
    }

    public void ChangeText(string itemName)
    {
        uiText.text = itemName;
    }
}

