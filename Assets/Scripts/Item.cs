using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // DYNAMIC:
    [NonSerialized] public int itemType; // 0 = block, 1 = spring, 2 = spike, 3 = chest

    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}