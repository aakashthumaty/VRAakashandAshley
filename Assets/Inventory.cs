using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SerializedInventory : SerializableDictionary<CollectibleTreasure, int>{}

public class Inventory : MonoBehaviour
{
    public int[] treasureInventory = new int[3];
    public List<CollectibleTreasure> colTres;
    // Start is called before the first frame update

    [SerializeField]
    public SerializedInventory dict = new SerializedInventory();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
