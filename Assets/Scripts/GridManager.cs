using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // STATIC:
    public static Dictionary<Vector2Int, Item> gridIndex = new();

    // SCENE REFERENCE:
    [SerializeField] private SaveAndLoad saveAndLoad;
    [SerializeField] private Player player;

    [SerializeField] private GameObject levelSelectScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject confirmationScreen;

    [SerializeField] private Transform itemParent;

    [SerializeField] private List<Item> itemPrefs = new(); // 0 = block, 1 = spring, 2 = spike, 3 = chest

    public Item SpawnItem(int itemType, Vector2Int itemPosition, Quaternion itemRotation)
    {
        Item item = Instantiate(itemPrefs[itemType], (Vector2)itemPosition, itemRotation, itemParent);

        gridIndex.Add(itemPosition, item);

        return item;
    }
    public void DestroyItem(Vector2Int destroyPosition)
    {
        if (!gridIndex.ContainsKey(destroyPosition))
            Debug.LogError("Attempted to destroy item at empty position");

        gridIndex[destroyPosition].DestroyItem();
        gridIndex.Remove(destroyPosition);
    }

    public void ClearGrid()
    {
        foreach (KeyValuePair<Vector2Int, Item> gridIndexEntry in gridIndex)
            gridIndexEntry.Value.DestroyItem();

        gridIndex.Clear();
    }

    private void Update()
    {
        if (!Application.isEditor)
            return;

        // Developer only
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ClearGrid();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            saveAndLoad.SaveLayout();
        if (Input.GetKeyDown(KeyCode.Alpha3))
            player.Die();
    }

    public void SelectLevel(int level)
    {
        saveAndLoad.LoadLayout(level);

        levelSelectScreen.SetActive(false);
        player.gameObject.SetActive(true);

        mainMenu.SetActive(true);
    }

    public void ReturnToMenu()
    {
        mainMenu.SetActive(false);
        confirmationScreen.SetActive(true);
    }
    public void ConfirmQuit()
    {
        ClearGrid();

        player.gameObject.transform.position = Vector2.zero;
        player.gameObject.SetActive(false);

        confirmationScreen.SetActive(false);
        levelSelectScreen.SetActive(true);
    }
    public void ResumeGame()
    {
        confirmationScreen.SetActive(false);
        mainMenu.SetActive(true);
    }
}