using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSpike : MonoBehaviour
{
    public static int PositionInSequence;

    [SerializeField] private int mySequence;
    [SerializeField] private bool left;
    [SerializeField] private Player player;

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (player.celesteBonusChestReceived)
            return;

        if (!col.CompareTag("Player"))
            return;

        // Left occurs twice in the sequence, at 1 and 4
        if (mySequence == PositionInSequence || (left && PositionInSequence == 4))
            PositionInSequence += 1;
        else
            PositionInSequence = 0;

        if (PositionInSequence == 6)
        {
            player.BonusChest(new(57, 60));

            player.celesteBonusChestReceived = true;
        }
    }
}