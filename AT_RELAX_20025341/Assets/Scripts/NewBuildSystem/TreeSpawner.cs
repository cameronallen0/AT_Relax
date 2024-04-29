using UnityEngine;
using System.Collections.Generic;

public class TreeSpawner : MonoBehaviour
{
    public GameObject[] prefabs; // Array of prefabs to spawn
    public int numberOfPrefabs = 5; // Number of prefabs to spawn on the top layer
    public float minDistanceBetweenTrees = 2f; // Minimum distance between trees

    private List<Vector3> spawnedTreePositions = new List<Vector3>(); // List to store spawned tree positions

    void Start()
    {
        SpawnPrefabsOnTopLayer();
    }

    void SpawnPrefabsOnTopLayer()
    {
        // Create parent object for trees to spawn under
        GameObject parentObject = new GameObject("SpawnedTrees");

        // Find the WorldSpawner script in the scene
        WorldSpawner worldSpawner = FindObjectOfType<WorldSpawner>();
        if (worldSpawner == null)
        {
            Debug.LogError("No WorldSpawner script found in the scene!");
            return;
        }

        // Determine the top layer index
        int topLayerIndex = worldSpawner.layerPrefabs.Length - 1;

        // Get the bounds of the top layer
        Bounds topLayerBounds = GetLayerBounds(worldSpawner, topLayerIndex);

        // Spawn prefabs on the top layer
        for (int i = 0; i < numberOfPrefabs; i++)
        {
            // Randomly select a prefab from the array
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

            // Generate a random position within the bounds of the top layer
            Vector3 spawnPosition = GetValidSpawnPosition(topLayerBounds, worldSpawner.spacing);

            // Instantiate the selected prefab at the position
            GameObject tree = Instantiate(prefab, spawnPosition, Quaternion.identity);
            tree.transform.parent = parentObject.transform;

            // Add the spawned position to the list
            spawnedTreePositions.Add(spawnPosition);
        }
    }

    // Helper function to get a valid spawn position that aligns with the world grid
    Vector3 GetValidSpawnPosition(Bounds topLayerBounds, float spacing)
    {
        Vector3 spawnPosition;
        bool validPositionFound = false;

        // Keep generating random positions until a valid one is found
        do
        {
            spawnPosition = new Vector3(
                Random.Range(topLayerBounds.min.x, topLayerBounds.max.x),
                0f, // Adjust Y coordinate to place on top of the layer
                Random.Range(topLayerBounds.min.z, topLayerBounds.max.z)
            );

            // Align the spawn position with the world grid
            spawnPosition.x = Mathf.RoundToInt(spawnPosition.x / spacing) * spacing;
            spawnPosition.z = Mathf.RoundToInt(spawnPosition.z / spacing) * spacing;

            // Check if the spawn position is too close to existing trees
            validPositionFound = true;
            foreach (Vector3 existingPosition in spawnedTreePositions)
            {
                if (Vector3.Distance(spawnPosition, existingPosition) < minDistanceBetweenTrees)
                {
                    validPositionFound = false;
                    break;
                }
            }
        } while (!validPositionFound);

        return spawnPosition;
    }


    // Helper function to get the bounds of a layer from the WorldSpawner script
    Bounds GetLayerBounds(WorldSpawner worldSpawner, int layerIndex)
    {
        Bounds bounds = new Bounds(worldSpawner.centerObject.position, Vector3.zero);

        // Calculate bounds based on the size of the grid and spacing
        bounds.size = new Vector3(
            worldSpawner.gridWidth * worldSpawner.spacing,
            worldSpawner.spacing,
            worldSpawner.gridDepth * worldSpawner.spacing
        );

        // Adjust bounds to the layer's position
        bounds.center += new Vector3(
            0f,
            layerIndex * worldSpawner.spacing,
            0f
        );

        return bounds;
    }
}
