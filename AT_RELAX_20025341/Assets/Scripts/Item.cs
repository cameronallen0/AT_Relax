using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
[CreateAssetMenu(fileName ="New Item",menuName ="Item/New Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite icon;
    public GameObject itemObject;
    public AudioClip placeClip;
    public AudioClip destroyClip;
    public float pitch;
    public float volume;
    public ItemType itemType;

    public enum ItemType
    {
        Blocks
    }
}