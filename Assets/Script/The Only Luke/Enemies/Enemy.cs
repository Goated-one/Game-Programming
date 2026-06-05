using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Knockback")]
    public float knockbackForce = 4f; 
    public float knockbackUpward = 2f; 
    
    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Movement & Jump")]
    public float moveSpeed = 2f;
    public float jumpForce = 6f; 
    public float tinggiTembokBatas = 1f; 
    public bool movingRight = false;
    public Transform edgeCheck; 
    public LayerMask groundLayer; 

    [Header("Attack Settings")]
    public float attackRange = 1.5f; 
    public float aggroRange = 5f;
    public int attackDamage = 20;
    public float attackCooldown = 2f; 
    private float cooldownTimer = 0f;
    public LayerMask playerLayer; 
    public float lungeSpeed = 5f; 
    public float lungeTime = 0.2f; 

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;
    private bool isDead = false;
    private bool isStunned = false; 
    private bool isAttacking = false;
    private Transform playerTransform;
    private PlayerHealth targetHealth; 

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            targetHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

   void Update()
    {
        if (playerTransform == null) return; 

        cooldownTimer += Time.deltaTime; 

        if (isDead || isStunned || isAttacking)
        {
            if (!isAttacking && !isStunned) 
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            anim.SetFloat("Speed", 0);
            return;
        }

        if (targetHealth != null && targetHealth.isDead)
        {
            Patrol(); 
            return;   
        }
        
        float distanceX = Mathf.Abs(transform.position.x - playerTransform.position.x);
        float distanceY = Mathf.Abs(transform.position.y - playerTransform.position.y);
        bool isSameFloor = distanceY < 3f; 

        if (distanceX <= attackRange && isSameFloor)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetFloat("Speed", 0);

            FacePlayer(); 

            if (cooldownTimer >= attackCooldown)
            {
                AttackPlayer();
            }
        }
        else if (distanceX <= aggroRange && isSameFloor)
        {
            FacePlayer();

            RaycastHit2D groundInfo = Physics2D.Raycast(edgeCheck.position, Vector2.down, 1f, groundLayer);
            Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
            RaycastHit2D wallInfo = Physics2D.Raycast(edgeCheck.position, rayDirection, 0.5f, groundLayer);
            
            Vector2 posisiKepala = new Vector2(edgeCheck.position.x, edgeCheck.position.y + tinggiTembokBatas);
            RaycastHit2D headInfo = Physics2D.Raycast(posisiKepala, rayDirection, 0.5f, groundLayer);

            bool isEdge = groundInfo.collider == null;
            bool isWall = wallInfo.collider != null;
            bool isTallWall = headInfo.collider != null;

            // CARA BARU YANG LEBIH AKURAT: Cek murni dari kecepatannya (bukan dari panjang laser)
            // Kalau kecepatan Y (atas/bawah) lebih dari 0.1, artinya dia lagi di udara
            bool lagiDiUdara = Mathf.Abs(rb.linearVelocity.y) > 0.1f;

            if (!lagiDiUdara)
            {
                if (isEdge)
                {
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                    anim.SetFloat("Speed", 0);
                }
                else if (isWall)
                {
                    if (!isTallWall)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    }
                    else
                    {
                        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                        anim.SetFloat("Speed", 0);
                    }
                }
                else
                {
                    float velocityX = movingRight ? moveSpeed : -moveSpeed;
                    rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
                    anim.SetFloat("Speed", Mathf.Abs(velocityX));
                }
            }
            else
            {
                // KALAU LAGI DI UDARA, TETEP MAJU AJA, ABAIKAN TEMBOK & JURANG!
                float velocityX = movingRight ? moveSpeed : -moveSpeed;
                rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
                anim.SetFloat("Speed", Mathf.Abs(velocityX));
            }
        }
        else
        {
            Patrol(); 
        }
    }

    void FacePlayer()
    {
        if (playerTransform.position.x > transform.position.x && !movingRight)
        {
            movingRight = true;
            transform.localRotation = Quaternion.Euler(0, 180, 0); 
        }
        else if (playerTransform.position.x < transform.position.x && movingRight)
        {
            movingRight = false;
            transform.localRotation = Quaternion.Euler(0, 0, 0); 
        }
    }

    void AttackPlayer()
    {
        cooldownTimer = 0f; 
        anim.SetTrigger("Attack"); 
        StartCoroutine(AttackLunge());
    }

    IEnumerator AttackLunge()
    {
        isAttacking = true;
        float direction = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * lungeSpeed, rb.linearVelocity.y);
        yield return new WaitForSeconds(lungeTime);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        yield return new WaitForSeconds(0.5f);
        isAttacking = false; 
    }

    public void DealDamage()
    {
        Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, attackRange, playerLayer);
        
        if (hit.collider != null)
        {
            PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage, transform);
            }
        }
    }

    void Patrol()
    {
        float velocityX = movingRight ? moveSpeed : -moveSpeed;
        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
        anim.SetFloat("Speed", Mathf.Abs(velocityX));

        if (movingRight) transform.localRotation = Quaternion.Euler(0, 180, 0); 
        else transform.localRotation = Quaternion.Euler(0, 0, 0); 

        RaycastHit2D groundInfo = Physics2D.Raycast(edgeCheck.position, Vector2.down, 1f, groundLayer);
        Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallInfo = Physics2D.Raycast(edgeCheck.position, rayDirection, 0.5f, groundLayer);
        
        Vector2 posisiKepala = new Vector2(edgeCheck.position.x, edgeCheck.position.y + tinggiTembokBatas);
        RaycastHit2D headInfo = Physics2D.Raycast(posisiKepala, rayDirection, 0.5f, groundLayer);

        bool isEdge = groundInfo.collider == null; 
        bool isWall = wallInfo.collider != null;
        bool isTallWall = headInfo.collider != null;
        
        bool lagiDiUdara = Mathf.Abs(rb.linearVelocity.y) > 0.1f;

        if (!lagiDiUdara)
        {
            if (isEdge)
            {
                movingRight = !movingRight; 
            }
            else if (isWall)
            {
                if (!isTallWall)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                }
                else
                {
                    movingRight = !movingRight;
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
        else
        {
            anim.SetTrigger("Hit");
            float knockbackDir = transform.position.x < playerTransform.position.x ? -1f : 1f;
            StartCoroutine(HitStun(knockbackDir));
        }
    }

    IEnumerator HitStun(float knockbackDir)
    {
        isStunned = true;
        isAttacking = false; 
        rb.linearVelocity = new Vector2(knockbackDir * knockbackForce, knockbackUpward);
        yield return new WaitForSeconds(0.5f); 
        isStunned = false;
    }

    void Die()
    {
        isDead = true;
        anim.SetBool("IsDead", true);
        GetComponent<Collider2D>().enabled = false;
        rb.gravityScale = 0; 
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 2f);
    }
}