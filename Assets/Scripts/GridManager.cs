using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // STATIC:
    public static Dictionary<Vector2Int, Item> gridIndex = new();

    // SCENE REFERENCE:
    [SerializeField] private SaveAndLoad saveAndLoad;

    [SerializeField] private List<Item> itemPrefs = new(); // 0 = block, 1 = spring, 2 = spike, 3 = chest

    // DYNAMIC:
    public int currentLayoutNumber { get; private set; }

    public void SpawnItem(int itemType, Vector2Int itemPosition, Quaternion itemRotation, bool layoutLoading)
    {
        //.if layout loading, don't play itemspawn audio

        Item item = Instantiate(itemPrefs[itemType], (Vector2)itemPosition, itemRotation);

        gridIndex.Add(itemPosition, item);

        if (!layoutLoading)
            saveAndLoad.SaveLayout(currentLayoutNumber);
    }

    public void ClearGrid()
    {
        foreach (KeyValuePair<Vector2Int, Item> gridIndexEntry in gridIndex)
            gridIndexEntry.Value.DestroyItem();

        gridIndex.Clear();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            SpawnItem(0, new(0, 0), Quaternion.identity, false);
        
        if (Input.GetKeyDown(KeyCode.W))
            saveAndLoad.SaveLayout(0);

        if (Input.GetKeyDown(KeyCode.E))
            ClearGrid();

        if (Input.GetKeyDown(KeyCode.R))
            saveAndLoad.LoadLayout(0);
    }
}