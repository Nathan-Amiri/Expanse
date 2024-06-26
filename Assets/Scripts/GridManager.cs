using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // STATIC:
    public static Dictionary<Vector2Int, Item> gridIndex = new();
    public static List<Item> chestIndex = new();

    // SCENE REFERENCE:
    [SerializeField] private SaveAndLoad saveAndLoad;
    [SerializeField] private Player player;

    [SerializeField] private GameObject levelSelectScreen;
    [SerializeField] private GameObject returnToMenu;
    [SerializeField] private GameObject infoText;
    [SerializeField] private GameObject confirmationScreen;
    [SerializeField] private GameObject tutorialText;
    [SerializeField] private GameObject finishScreen;
    [SerializeField] private GameObject arrow;

    [SerializeField] private Transform itemParent;

    [SerializeField] private List<Item> itemPrefs = new(); // 0 = block, 1 = bounce pad, 2 = spike, 3 = chest

    [SerializeField] private List<Item> bonusSpikes = new();

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

        Item item = gridIndex[destroyPosition];

        if (item.itemType == 3)
            chestIndex.Remove(item);

        gridIndex.Remove(destroyPosition);
        Destroy(item.gameObject);
    }

    public void SelectLevel(int level)
    {
        saveAndLoad.LoadLayout(level);

        levelSelectScreen.SetActive(false);
        player.gameObject.SetActive(true);

        player.StartLevel(level == 0);

        returnToMenu.SetActive(true);
        infoText.SetActive(true);

        if (level == 0)
            tutorialText.SetActive(true);

        arrow.SetActive(true);

        foreach (Item bonusSpike in bonusSpikes)
            gridIndex.Add(Vector2Int.RoundToInt(bonusSpike.transform.position), bonusSpike);
    }

    public void ReturnToMenu()
    {
        returnToMenu.SetActive(false);
        confirmationScreen.SetActive(true);
    }
    public void ConfirmQuit()
    {
        foreach (KeyValuePair<Vector2Int, Item> gridIndexEntry in gridIndex)
            if (!gridIndexEntry.Value.bonusSpike)
                Destroy(gridIndexEntry.Value.gameObject);

        gridIndex.Clear();
        chestIndex.Clear();


        player.transform.position = Vector2.zero;
        player.gameObject.SetActive(false);

        infoText.SetActive(false);

        confirmationScreen.SetActive(false);
        levelSelectScreen.SetActive(true);

        tutorialText.SetActive(false);

        finishScreen.SetActive(false);

        arrow.SetActive(false);
    }
    public void ResumeGame()
    {
        confirmationScreen.SetActive(false);
        returnToMenu.SetActive(true);
    }
}