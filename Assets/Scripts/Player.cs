using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    // PREFAB REFERENCE:
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PolygonCollider2D col;

    // SCENE REFERENCE:
    [SerializeField] private GridManager gridManager;

    [SerializeField] private Camera mainCamera;

    // CONSTANT:
    private readonly float gravityScale = 3.5f;
    private readonly float moveSpeed = 8;
    private readonly float jumpForce = 15;
    private readonly float fallMultiplier = 3; // Fastfall
    private readonly float lowJumpMultiplier = 10; // Dynamic jump
    private readonly float jumpBuffer = .08f;

    private readonly float deathWarpDuration = .15f;

    // DYNAMIC:
    private bool isStunned;

    private float moveInput;
    private bool hasJump;
    private bool jumpInputDown;
    private bool jumpInput;

        // Set by GroundCheck:
    [NonSerialized] public bool isGrounded;
    [NonSerialized] public Vector2 lastGroundedPosition;

    private Vector2Int currentSpawnPosition;

    private void Update()
    {
        rb.gravityScale = isStunned ? 0 : gravityScale;

        Debug.Log(lastGroundedPosition);

        if (isStunned)
            return;

        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            jumpInputDown = true;

        jumpInput = Input.GetButton("Jump");

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            SpawnBlock();

        if (Input.GetKeyDown(KeyCode.P))
            Die();
    }

    private void FixedUpdate()
    {
        if (isStunned)
            return;

        // Snappy horizontal movement:
        // (This movement method will prevent the player from slowing completely in a frictionless environment. To prevent this,
        // ensure that either at least a tiny bit of friction is present or the player's velocity is rounded to 0 when low enough)
        float desiredVelocity = moveInput * moveSpeed;
        float velocityChange = desiredVelocity - rb.velocity.x;
        float acceleration = velocityChange / .05f;
        float force = rb.mass * acceleration;
        rb.AddForce(new(force, 0));

        // Fastfall
        if (rb.velocity.y < 0)
            // Subtract fall and lowjump multipliers by 1 to more accurately represent the multiplier (fallmultiplier = 2 means fastfall will be x2)
            rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        // Dynamic jump
        else if (rb.velocity.y > 0 && !jumpInput)
            rb.velocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;

        if (isGrounded)
            hasJump = true;

        if (jumpInputDown)
        {
            if (!hasJump)
                StartCoroutine(JumpBuffer());
            else
            {
                jumpInputDown = false;
                hasJump = false;

                rb.velocity = new(rb.velocity.x, jumpForce);
            }
        }
    }

    private IEnumerator JumpBuffer()
    {
        yield return new WaitForSeconds(jumpBuffer);
        jumpInputDown = false;
    }

    private void SpawnBlock()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        Vector3 mousePositionOnGrid = mainCamera.ScreenToWorldPoint(mousePos);

        Vector2Int spawnPosition = Vector2Int.RoundToInt(mousePositionOnGrid);

        // Check if the mouse has moved before searching the grid index
        if (spawnPosition == currentSpawnPosition)
            return;
        currentSpawnPosition = spawnPosition;

        if (MathF.Abs(spawnPosition.x) > 112 || MathF.Abs(spawnPosition.y) > 112)
            return;

        if (GridManager.gridIndex.ContainsKey(spawnPosition))
            return;


        gridManager.SpawnItem(0, spawnPosition, Quaternion.identity, false);
    }

    public void Die()
    {
        Debug.Log("Die");

        col.enabled = false;

        StartCoroutine(DeathWarp(deathWarpDuration));

        isStunned = true;

        float warpSpeed = Vector2.Distance(lastGroundedPosition, transform.position) / deathWarpDuration;
        rb.velocity = warpSpeed * ((Vector3)lastGroundedPosition - transform.position).normalized;
    }
    private IEnumerator DeathWarp(float duration)
    {
        yield return new WaitForSeconds(duration);

        rb.velocity = Vector3.zero;
        transform.position = (Vector3)lastGroundedPosition;

        isStunned = false;

        col.enabled = true;
    }
}