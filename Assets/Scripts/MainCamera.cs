using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Transform playerTr;

    private void LateUpdate()
    {
        transform.position = new(playerTr.position.x, playerTr.transform.position.y, -10);
    }
}