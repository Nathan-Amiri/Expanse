using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveAndLoad : MonoBehaviour
{
    // STATIC:
    private readonly List<string> saveFiles = new();

    // SCENE REFERENCE:
    [SerializeField] private GridManager gridManager;

    [SerializeField] private TextAsset tutorialGridFile;

    private void Awake()
    {
        // Application.persistentDataPath cannot be called in a constructor
        saveFiles.Add(Application.persistentDataPath + "/Grid1.json");
        saveFiles.Add(Application.persistentDataPath + "/Grid2.json");
        saveFiles.Add(Application.persistentDataPath + "/Grid3.json");

        // Create save files
        foreach (string file in saveFiles)
            if (!File.Exists(file))
                File.WriteAllText(file, string.Empty);
    }


    public void LoadLayout(int newLayoutNumber, bool tutorial = false)
    {
        gridManager.ClearGrid();

        LayoutData layoutData;

        if (tutorial)
        {
            string fileContents = tutorialGridFile.ToString();
            layoutData = JsonUtility.FromJson<LayoutData>(fileContents);
        }
        else
        {
            string file = saveFiles[newLayoutNumber];
            string fileContents = File.ReadAllText(file);
            layoutData = JsonUtility.FromJson<LayoutData>(fileContents);
        }

        foreach (ItemData itemData in layoutData.itemsInLayout)
        {
            Quaternion cellRotation = Quaternion.Euler(0, 0, itemData.itemRotation);

            gridManager.SpawnItem(itemData.itemType, itemData.itemPosition, cellRotation, true);
        }
    }

    public void SaveLayout(int currentLayoutNumber) //.no need to ever save when in tutorial!
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

        string saveFile = saveFiles[currentLayoutNumber];
        File.WriteAllText(saveFile, jsonString);
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