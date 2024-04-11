using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    // PREFAB REFERENCE:
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CircleCollider2D col;

    // SCENE REFERENCE:
    [SerializeField] private GridManager gridManager;
    [SerializeField] private AudioManager audioManager;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject destroyMeter;
    [SerializeField] private Transform destroyBarScaler;

    // CONSTANT:
    private readonly float gravityScale = 3.5f;
    private readonly float moveSpeed = 8;
    private readonly float jumpForce = 15;
    private readonly float fallMultiplier = 3; // Fastfall
    private readonly float lowJumpMultiplier = 10; // Dynamic jump
    private readonly float jumpBuffer = .08f;

    private readonly float bounceForce = 35;
    private readonly float horizontalBounceIncrease = 4;

    private readonly float deathWarpDuration = .15f;

    private readonly int materialPerChest = 100;

    private readonly float destroyDuration = 1;

    // DYNAMIC:
    private Vector2 mousePosition;

        // Set by GridManager
    [NonSerialized] public bool isStunned;

    private float moveInput;
    private bool hasJump;
    private bool jumpInputDown;
    private bool jumpInput;

    private Item springToPlace;

    public int currentMaterial;

        // Set by GroundCheck:
    [NonSerialized] public bool isGrounded;
    [NonSerialized] public Vector2 lastGroundedPosition;

    private Vector2Int currentSpawnPosition;

    private float destroyAmount;
    private Coroutine destroyRoutine;
    private bool justDestroyed;

    private void Update()
    {
        Vector3 tempMousePosition = Input.mousePosition;
        tempMousePosition.z = -mainCamera.transform.position.z;
        mousePosition = mainCamera.ScreenToWorldPoint(tempMousePosition);

        rb.gravityScale = isStunned ? 0 : gravityScale;

        if (springToPlace != null)
            springToPlace.SetSpringRotation(mousePosition);

        if (isStunned)
            return;

        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            jumpInputDown = true;

        jumpInput = Input.GetButton("Jump");

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
            DestroyItem();
        if (Input.GetMouseButtonUp(0))
        {
            justDestroyed = false;

            if (destroyRoutine != null)
            {
                StopCoroutine(destroyRoutine);
                destroyRoutine = null;

                destroyMeter.SetActive(false);
            }

        }

        if (Input.GetMouseButton(0))
            SpawnItem(true);

        if (Input.GetMouseButtonDown(1))
        {
            if (springToPlace == null)
                SpawnItem(false);
            else
                PlaceSpring();
        }
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

                StartCoroutine(audioManager.PlayClip(1));
            }
        }
    }

    private IEnumerator JumpBuffer()
    {
        yield return new WaitForSeconds(jumpBuffer);
        jumpInputDown = false;
    }

    private void SpawnItem(bool block)
    {
        Vector2Int spawnPosition = Vector2Int.RoundToInt(mousePosition);

        // Check if the mouse has moved before searching the grid index
        if (spawnPosition == currentSpawnPosition)
            return;

        if (justDestroyed)
            return;

        if (spawnPosition == Vector2Int.RoundToInt((Vector2)transform.position))
            return;

        if (MathF.Abs(spawnPosition.x) > 112 || MathF.Abs(spawnPosition.y) > 112)
            return;

        if (GridManager.gridIndex.ContainsKey(spawnPosition))
            return;

        currentSpawnPosition = spawnPosition;

        if (block)
            gridManager.SpawnItem(0, spawnPosition, Quaternion.identity);
        else // Spring
        {
            springToPlace = gridManager.SpawnItem(1, spawnPosition, Quaternion.identity);

            springToPlace.ToggleSpringReady(false);
        }

        StartCoroutine(audioManager.PlayClip(2));
    }

    private void PlaceSpring()
    {
        springToPlace.ToggleSpringReady(true);

        springToPlace = null;

        StartCoroutine(audioManager.PlayClip(2));
    }

    private void DestroyItem()
    {
        Vector2Int destroyPosition = Vector2Int.RoundToInt(mousePosition);

        if (!GridManager.gridIndex.ContainsKey(destroyPosition))
            return;

        destroyRoutine = StartCoroutine(DestroyMeter(destroyPosition));
    }
    private IEnumerator DestroyMeter(Vector2Int destroyPosition)
    {
        destroyMeter.SetActive(true);
        destroyMeter.transform.position = destroyPosition + new Vector2(0, 1);
        destroyAmount = 0;
        
        while (destroyAmount < destroyDuration)
        {
            destroyAmount += Time.deltaTime;
            destroyBarScaler.localScale = new Vector2(destroyAmount / destroyDuration, destroyBarScaler.localScale.y);
            yield return null;
        }

        destroyMeter.SetActive(false);

        gridManager.DestroyItem(destroyPosition);

        justDestroyed = true;
    }


    public void Die()
    {
        col.enabled = false;

        StartCoroutine(DeathWarp(deathWarpDuration));

        isStunned = true;

        float warpSpeed = Vector2.Distance(lastGroundedPosition, transform.position) / deathWarpDuration;
        rb.velocity = warpSpeed * ((Vector3)lastGroundedPosition - transform.position).normalized;

        StartCoroutine(audioManager.PlayClip(4));
    }
    private IEnumerator DeathWarp(float duration)
    {
        yield return new WaitForSeconds(duration);

        rb.velocity = Vector3.zero;
        transform.position = (Vector3)lastGroundedPosition;

        isStunned = false;

        col.enabled = true;
    }

    public void Bounce(Vector2 bounceDirection)
    {
        rb.velocity = Vector2.zero;

        Vector2 bounceVelocity = bounceForce * bounceDirection;
        bounceVelocity.x *= horizontalBounceIncrease;
        rb.AddForce(bounceVelocity, ForceMode2D.Impulse);
        hasJump = true;

        StartCoroutine(audioManager.PlayClip(3));
    }

    public void OpenChest()
    {
        currentMaterial += materialPerChest;
        //.chest points? update score?

        StartCoroutine(audioManager.PlayClip(5));
    }
}