using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildSystem : MonoBehaviour
{
    private static BuildSystem _instance;
    public static BuildSystem Instance { get { return _instance; } }

    public PlayerMovement player;
    public Transform shootingPoint;

    private GameObject currentObject;
    public GameObject inventory;

    public TextMeshProUGUI uiText;

    private GameObject parentObject;
    private AudioSource source;
    private AudioClip currentPlaceClip;
    private float currentPitch;
    private float currentVolume;

    private void Awake()
    {
        source = player.GetComponent<AudioSource>();

        inventory.gameObject.SetActive(false);
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        uiText.text = "";
        parentObject = new GameObject("PlacedBlocks");
    }

    public void SetObject(GameObject itemObject)
    {
        if(currentObject != null)
        {
            currentObject = null;
        }

        currentObject = itemObject;
        
    }

    public void BuildBlock()
    {
        if (currentObject == null)
        {
            return;
        }
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo, player.reach))
        {
            if (hitInfo.transform.tag == "object")
            {
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x + hitInfo.normal.x / 2), Mathf.RoundToInt(hitInfo.point.y + hitInfo.normal.y / 2), Mathf.RoundToInt(hitInfo.point.z + hitInfo.normal.z / 2));
                GameObject cube = Instantiate(currentObject, spawnPosition, Quaternion.identity);
                cube.transform.parent = parentObject.transform;
                source.PlayOneShot(currentPlaceClip);
            }
            else if (hitInfo.transform.tag == "ground")
            {
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x + hitInfo.normal.x / 2), Mathf.RoundToInt(hitInfo.point.y + hitInfo.normal.y / 2), Mathf.RoundToInt(hitInfo.point.z + hitInfo.normal.z / 2));
                GameObject cube = Instantiate(currentObject, spawnPosition, Quaternion.identity);
                cube.transform.parent = parentObject.transform;
                source.PlayOneShot(currentPlaceClip);
            }
            else if (hitInfo.transform.tag == "border")
            {
                return;
            }
            else
            {
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x), Mathf.RoundToInt(hitInfo.point.y), Mathf.RoundToInt(hitInfo.point.z));
                GameObject cube = Instantiate(currentObject, spawnPosition, Quaternion.identity);
                cube.transform.parent = parentObject.transform;
                source.PlayOneShot(currentPlaceClip);
            }
        }
    }

    public void DestroyBlock()
    {
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo, player.reach))
        {
            if (hitInfo.transform.tag == "object")
            {
                Item destroyedItem = hitInfo.transform.GetComponent<InventoryItemController>().item;
                source.volume = destroyedItem.volume;
                source.pitch = destroyedItem.pitch;
                source.PlayOneShot(destroyedItem.destroyClip);
                Destroy(hitInfo.transform.gameObject);
            }
            else
            {
                return;
            }
        }
    }

    public void ChangeText(string itemName)
    {
        uiText.text = itemName;
    }

    public void ChangeAudio(AudioClip placeClip, float pitch, float volume)
    {
        currentPlaceClip = placeClip;
        currentPitch = pitch;
        source.pitch = currentPitch;
        currentVolume = volume;
        source.volume = currentVolume;
    }
}
