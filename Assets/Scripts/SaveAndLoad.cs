using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveAndLoad : MonoBehaviour
{
    // SCENE REFERENCE:
    [SerializeField] private GridManager gridManager;

    [SerializeField] private List<TextAsset> levelGridFiles = new();


    public void LoadLayout(int newLayoutNumber)
    {
        gridManager.ClearGrid();

        string fileContents = levelGridFiles[newLayoutNumber].ToString();
        LayoutData layoutData = JsonUtility.FromJson<LayoutData>(fileContents);

        foreach (ItemData itemData in layoutData.itemsInLayout)
        {
            Quaternion cellRotation = Quaternion.Euler(0, 0, itemData.itemRotation);

            gridManager.SpawnItem(itemData.itemType, itemData.itemPosition, cellRotation);
        }
    }

    // Only used in editor to create levels. Comment method when shipping build
    public void SaveLayout()
    {
        LayoutData layoutData = new();

        foreach (KeyValuePair<Vector2Int, Item> gridIndexEntry in GridManager.gridIndex)
        {
            Item item = gridIndexEntry.Value;
            ItemData itemData = new()
            {
                itemType = item.itemType,
                itemRotation = Mathf.RoundToInt(item.transform.rotation.eulerAngles.z),
                itemPosition = gridIndexEntry.Key
            };

            layoutData.itemsInLayout.Add(itemData);
        }

        string jsonString = JsonUtility.ToJson(layoutData, true);

        File.WriteAllText(Application.persistentDataPath + "/DeveloperGrid.json", jsonString);
    }
}

[System.Serializable]
public class LayoutData
{
    public List<ItemData> itemsInLayout = new();
}

[System.Serializable]
public class ItemData
{
    public int itemType; // 0 = block, 1 = spring, 2 = spike, 3 = chest
    public int itemRotation;
    public Vector2Int itemPosition;
}