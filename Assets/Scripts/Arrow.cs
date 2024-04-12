using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Transform playerTR;
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private Camera mainCamera;

    private void Update()
    {
        if (GridManager.chestIndex.Count == 0)
            return;

        //.if chest on screen return

        Vector2 closestChestPosition = Vector2.zero;
        float closestChestDistance = 0;
        foreach (Item item in GridManager.chestIndex)
        {
            float distance = Vector2.Distance(playerTR.position, item.transform.position);
            if (closestChestDistance == 0 || distance < closestChestDistance)
            {
                closestChestPosition = item.transform.position;
                closestChestDistance = distance;
            }
        }

        Vector2 direction = (closestChestPosition - (Vector2)playerTR.position).normalized;
        transform.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg);

        transform.localPosition = PointOnRectangle(920, 500, direction);

        distanceText.text = Mathf.RoundToInt(closestChestDistance).ToString();

        Quaternion rotation = transform.rotation;
        rotation.z *= -1;
        distanceText.transform.localRotation = rotation;
    }

    private Vector2 PointOnRectangle(float halfWidth, float halfHeight, Vector2 normalizedDirection)
    {
        float angle = Angle360(Vector2.right, normalizedDirection);

        // Get y coordinate of the intersection with the right edge of the rectangle
        float yCoord = TanInDegrees(angle) * halfWidth;

        // If yCoord is within rectangle
        if (Mathf.Abs(yCoord) <= halfHeight)
        {
            if (normalizedDirection.x > 0)
                // Choose the right edge
                return new(halfWidth, yCoord);
            else
                // Choose the left edge
                return new(-halfWidth, -yCoord);
        }

        // Get x coordinate of the intersection with the bottom edge of the rectangle
        float xCoord = halfHeight / TanInDegrees(angle);

        if (normalizedDirection.y < 0)
            // Choose the bottom edge
            return new(-xCoord, -halfHeight);
        else
            // Choose the top edge
            return new(xCoord, halfHeight);
    }

    private float TanInDegrees(float value)
    {
        value = Mathf.Deg2Rad * value;
        return Mathf.Tan(value);
    }

    private float Angle360(Vector2 p1, Vector2 p2)
    {
        float angle = Vector2.Angle(p1, p2);
        return Mathf.Sign(Vector3.Cross(p1, p2).z) < 0 ? (360 - angle) % 360 : angle;
    }
}