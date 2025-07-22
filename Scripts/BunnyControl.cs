using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class BunnyController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float tileSize = 1f;
    public float jumpHeight = 1f;
    public float jumpDuration = 0.3f;
    public float rotationSpeed = 10f;

    public int maxJumpTiles = 3;

    // NOI VARIABILE PENTRU COOLDOWN
    public float jumpCooldownTime = 10f; 
    private float lastJumpTime = -Mathf.Infinity; // Ultima dată când s-a sărit, inițial setat la o valoare foarte mică
    private bool isOnCooldown = false; // Flag pentru a ști dacă abilitatea este în cooldown

    private Vector3 direction = Vector3.zero;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isJumping = false;

    private float jumpTimer = 0f;
    private Vector3 jumpStartPos;
    private Vector3 jumpEndPos;

    private Vector3 lastMoveDirection = Vector3.forward;

    public bool IsJumping => isJumping;

    void Start()
    {
        Debug.Log("BunnyController: Start() called.");

        Vector3 initialScenePosition = transform.position;
        Debug.Log("BunnyController: Poziția inițială a iepurașului în scenă (din editor): " + initialScenePosition);

        isMoving = false;
        isJumping = false;
        jumpTimer = 0f;
        direction = Vector3.zero;
        lastMoveDirection = Vector3.forward;

        // Păstrăm această linie conform cerinței tale, pentru a folosi poziția Y din editor
        transform.position = new Vector3(initialScenePosition.x, initialScenePosition.y, initialScenePosition.z);

        targetPosition = transform.position;
        Debug.Log("BunnyController: Poziția Y a iepurașului după Start(): " + transform.position.y);
        Debug.Log("BunnyController: Poziția FINALĂ a iepurașului după Start(): " + transform.position);

        transform.rotation = Quaternion.LookRotation(Vector3.forward);

        // Resetăm cooldown-ul la începutul nivelului
        // Setarea la -jumpCooldownTime asigură că abilitatea este disponibilă imediat
        lastJumpTime = -jumpCooldownTime;
        isOnCooldown = false;
    }


    void Update()
    {
        // Calculăm timpul rămas din cooldown
        float timeSinceLastJump = Time.time - lastJumpTime;
        isOnCooldown = timeSinceLastJump < jumpCooldownTime;

        if (!isMoving && !isJumping)
        {
            Vector3 inputDirection = Vector3.zero;

            if (Input.GetKey(KeyCode.UpArrow))
                inputDirection = new Vector3(-tileSize, 0, 0);
            else if (Input.GetKey(KeyCode.DownArrow))
                inputDirection = new Vector3(tileSize, 0, 0);
            else if (Input.GetKey(KeyCode.LeftArrow))
                inputDirection = new Vector3(0, 0, -tileSize);
            else if (Input.GetKey(KeyCode.RightArrow))
                inputDirection = new Vector3(0, 0, tileSize);

            if (inputDirection != Vector3.zero)
            {
                direction = inputDirection;
                lastMoveDirection = direction.normalized;
            }

            if (direction != Vector3.zero && Input.GetKeyDown(KeyCode.Space) == false)
            {
                
                bool hitWall = Physics.Raycast(transform.position, direction.normalized, tileSize * 0.6f);
                if (!hitWall)
                {
                    targetPosition = transform.position + direction;
                    isMoving = true;
                }
                else
                {
                    Debug.Log("Blocat de perete la mișcare normală!");
                }
                direction = Vector3.zero;
            }

            // VERIFICARE COOLDOWN ÎNAINTE DE A SĂRI
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (lastMoveDirection != Vector3.zero)
                {
                    if (!isOnCooldown) // Permitem săritura DOAR dacă nu este în cooldown
                    {
                        Vector3 tempJumpDirection = lastMoveDirection * tileSize;
                        StartJump(tempJumpDirection);
                    }
                    else
                    {
                        Debug.Log($"Săritura este în cooldown. Timp rămas: {jumpCooldownTime - timeSinceLastJump:F1} secunde.");
                    }
                }
            }
        }

        if (lastMoveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (isJumping)
        {
            jumpTimer += Time.deltaTime;
            float t = jumpTimer / jumpDuration;

            Vector3 pos = Vector3.Lerp(jumpStartPos, jumpEndPos, t);
            pos.y += Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI) * jumpHeight;
            transform.position = pos;

            if (jumpTimer >= jumpDuration)
            {
                transform.position = jumpEndPos;
                isJumping = false;
                jumpTimer = 0f;
                if (lastMoveDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lastMoveDirection);
                }
            }
        }
    }


    void FixedUpdate()
    {
        if (isMoving && !isJumping)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    void StartJump(Vector3 jumpDir)
    {
        if (jumpDir == Vector3.zero)
            return;

        Vector3 directionNormalized = jumpDir.normalized;
        Vector3 boxSize = new Vector3(0.4f, 1f, 0.4f);
        LayerMask wallMask = LayerMask.GetMask("Wall");

        bool blocked = false;

        if (maxJumpTiles < 1) maxJumpTiles = 1;

        for (int i = 1; i <= maxJumpTiles; i++)
        {
            Vector3 checkPosition = transform.position + directionNormalized * tileSize * i;

            if (Physics.CheckBox(checkPosition, boxSize / 2f, Quaternion.identity, wallMask))
            {
                blocked = true;
                Debug.Log($"Nu pot sări – există perete în calea la tile-ul {i} (poziția: {checkPosition}). Salt blocat.");
                break;
            }
        }

        if (!blocked)
        {
            isJumping = true;
            jumpStartPos = transform.position;
            jumpEndPos = transform.position + directionNormalized * tileSize * maxJumpTiles;
            jumpTimer = 0f;
            lastMoveDirection = jumpDir.normalized;

            transform.rotation = Quaternion.LookRotation(lastMoveDirection);

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
                Invoke(nameof(ReenableCollider), jumpDuration);
            }

            lastJumpTime = Time.time;
            isOnCooldown = true;

            // Eliminat: MazeGenerator.Instance.RegenerateMazeRandomPreservingGameplay();
        }
    }


    void ReenableCollider()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;
    }
}
