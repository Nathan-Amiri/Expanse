using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // PREFAB REFERENCE:
    public int itemType; // 0 = block, 1 = spring, 2 = spike, 3 = chest

        // Spring only
    [SerializeField] private SpriteRenderer springSR;
    [SerializeField] private CircleCollider2D springCol;
    public float springTransparency;

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player"))
            return;

        Player player = col.GetComponent<Player>();

        if (itemType == 1) // Spring
            player.Bounce(transform.up);
        else if (itemType == 2) // Spike
            player.Die();
        else if (itemType == 3)
            player.OpenChest(Vector2Int.RoundToInt(transform.position));
    }

    public void ToggleSpringReady(bool ready)
    {
        springSR.color = ready ? Color.white : new Color(1, 1, 1, springTransparency);
        springCol.enabled = ready;
    }

    public void SetSpringRotation(Vector2 mousePosition) // Run in Player's Update
    {
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;
        direction = Vector2Int.RoundToInt(direction);

        transform.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg);
    }
}