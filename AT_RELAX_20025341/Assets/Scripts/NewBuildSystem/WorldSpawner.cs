using UnityEngine;

public class WorldSpawner : MonoBehaviour
{
    // Define a class to hold information about prefabs for each layer
    [System.Serializable]
    public class LayerPrefabInfo
    {
        public GameObject[] prefabs;
    }

    public LayerPrefabInfo[] layerPrefabs; // Array of LayerPrefabInfo for each layer
    public int gridWidth = 5;
    public int gridHeight = 5;
    public int gridDepth = 5;
    public float spacing = 2f;
    public Transform centerObject;

    void Start()
    {
        SpawnCubesInGrid();
    }

    void SpawnCubesInGrid()
    {
        GameObject parentObject = new GameObject("SpawnedCubes");

        Vector3 centerOffset = new Vector3(
            -((gridWidth - 1) * spacing) / 2f,
            -((gridHeight - 1) * spacing) / 2f,
            -((gridDepth - 1) * spacing) / 2f
        );

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                for (int z = 0; z < gridDepth; z++)
                {
                    // Determine the layer index based on y value
                    int layerIndex = layerPrefabs.Length - 1 - (y % layerPrefabs.Length);

                    // Get a random prefab from the chosen layer
                    GameObject prefab = layerPrefabs[layerIndex].prefabs[Random.Range(0, layerPrefabs[layerIndex].prefabs.Length)];

                    Vector3 spawnPosition = new Vector3(
                        x * spacing,
                        y * spacing,
                        z * spacing
                    ) + centerObject.position + centerOffset;

                    GameObject cube = Instantiate(prefab, spawnPosition, Quaternion.identity);
                    cube.transform.parent = parentObject.transform;
                }
            }
        }
    }
}