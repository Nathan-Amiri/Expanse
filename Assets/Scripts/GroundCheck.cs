using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private Player player;
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Terrain"))
        {
            Vector2Int groundedPosition = Vector2Int.RoundToInt(col.transform.position + new Vector3(0, 1));

            if (GridManager.gridIndex.ContainsKey(groundedPosition))
                return;

            player.isGrounded = true;
            player.lastGroundedPosition = Vector2Int.RoundToInt(col.transform.position + new Vector3(0, 1));
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Terrain"))
            player.isGrounded = false;
    }
}