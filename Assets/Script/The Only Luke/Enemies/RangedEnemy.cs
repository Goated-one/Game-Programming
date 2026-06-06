using UnityEngine;
using System.Collections;

public class RangedEnemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Loot Drop")]
    public GameObject itemDarahPrefab;
    [Range(0f, 100f)]
    public float peluangDrop = 30f; // 30% kemungkinan musuh ini nge-drop darah

    [Header("Movement")]
    public float moveSpeed = 2f;
    public bool movingRight = false;
    public Transform edgeCheck;
    public LayerMask groundLayer; // KUNCI ANTI GETER: Deteksi khusus buat lantai

    [Header("AI Ranges")]
    public float fleeRange = 3f;      
    public float attackRange = 6f;    
    public float aggroRange = 9f;     

    [Header("Shooting")]
    public float attackCooldown = 3f; 
    private float cooldownTimer = 0f;
    public GameObject projectilePrefab; 
    public Transform firePoint;         
    public GameObject chargeEffect;     
    public float chargeTime = 0.5f;       

    [Header("Knockback")]
    public float knockbackForce = 4f; 
    public float knockbackUpward = 2f; 

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;
    private Transform playerTransform;
    private PlayerHealth targetHealth;
    
    private bool isDead = false;
    private bool isStunned = false;
    private bool isAttacking = false;

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
        else
        {
            Debug.LogError("Tag 'Player' belum dipasang!");
        }
        
        if(chargeEffect != null) chargeEffect.SetActive(false);
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
            if (!isAttacking) anim.SetFloat("Speed", 0);
            return;
        }

        if (targetHealth != null && targetHealth.isDead)
        {
            Patrol(); 
            return;   
        }

        float distanceToPlayer = Mathf.Abs(transform.position.x - playerTransform.position.x);
        
        if (distanceToPlayer <= fleeRange)
        {
            bool isCornered = Flee();
            if (isCornered)
            {
                PrepareToShoot();
            }
        }
        else if (distanceToPlayer <= attackRange)
        {
            PrepareToShoot();
        }
        else if (distanceToPlayer <= aggroRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol(); 
        }
    }

    void PrepareToShoot()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetFloat("Speed", 0);
        FacePlayer(); 

        if (cooldownTimer >= attackCooldown)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    bool Flee()
    {
        bool shouldRunRight = transform.position.x > playerTransform.position.x;
        
        movingRight = shouldRunRight;
        if (movingRight) transform.localRotation = Quaternion.Euler(0, 180, 0); 
        else transform.localRotation = Quaternion.Euler(0, 0, 0); 

        // DETEKSI YANG UDAH DIPERBAIKI PAKE groundLayer
       // Deteksi jurang tetep dari kaki (edgeCheck)
        RaycastHit2D groundInfo = Physics2D.Raycast(edgeCheck.position, Vector2.down, 1f, groundLayer);
        Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
        
        // --- PERBAIKAN ---
        // Tembak laser tembok dari perut (transform.position) dan panjangin jadi 1f
        RaycastHit2D wallInfo = Physics2D.Raycast(transform.position, rayDirection, 1f, groundLayer);
        
        bool isEdge = groundInfo.collider == null; 
        bool isWall = wallInfo.collider != null;

        if (isEdge || isWall)
        {
            return true; 
        }

        float velocityX = movingRight ? moveSpeed : -moveSpeed;
        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
        anim.SetFloat("Speed", Mathf.Abs(velocityX));
        
        return false;
    }

    void ChasePlayer()
    {
        FacePlayer();
        
        // DETEKSI YANG UDAH DIPERBAIKI PAKE groundLayer
        RaycastHit2D groundInfo = Physics2D.Raycast(edgeCheck.position, Vector2.down, 1f, groundLayer);
        Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallInfo = Physics2D.Raycast(edgeCheck.position, rayDirection, 0.2f, groundLayer);
        
        bool isEdge = groundInfo.collider == null; 
        bool isWall = wallInfo.collider != null;

        if (isEdge || isWall)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetFloat("Speed", 0);
        }
        else
        {
            float velocityX = movingRight ? moveSpeed : -moveSpeed;
            rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
            anim.SetFloat("Speed", Mathf.Abs(velocityX));
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

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        cooldownTimer = 0f;

        anim.SetTrigger("Attack"); 
        
        // Tambahan Audio Nembak
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Suara Enemy");

        if (chargeEffect != null) chargeEffect.SetActive(true); 

        yield return new WaitForSeconds(chargeTime);

        if (chargeEffect != null) chargeEffect.SetActive(false); 
        
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }

    void Patrol()
    {
        float velocityX = movingRight ? moveSpeed : -moveSpeed;
        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
        anim.SetFloat("Speed", Mathf.Abs(velocityX));

        if (movingRight) transform.localRotation = Quaternion.Euler(0, 180, 0); 
        else transform.localRotation = Quaternion.Euler(0, 0, 0); 

        // DETEKSI YANG UDAH DIPERBAIKI PAKE groundLayer
        RaycastHit2D groundInfo = Physics2D.Raycast(edgeCheck.position, Vector2.down, 1f, groundLayer);
        Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallInfo = Physics2D.Raycast(edgeCheck.position, rayDirection, 0.2f, groundLayer);

        bool isEdge = groundInfo.collider == null; 
        bool isWall = wallInfo.collider != null;

        if (isEdge || isWall)
        {
            movingRight = !movingRight; 
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        // Samain persis kayak enemy Melee: ada layar geter dan suara kena pukul
        if (ScreenShakeManager.instance != null) ScreenShakeManager.instance.ShakeCamera(0.5f); 
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Hit Enemy");

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
        
        if (chargeEffect != null) chargeEffect.SetActive(false); 
        
        rb.linearVelocity = new Vector2(knockbackDir * knockbackForce, knockbackUpward);
        
        yield return new WaitForSeconds(0.5f); 
        
        isStunned = false;
    }

    void Die()
    {
        isDead = true;

        // Tambahan Audio Mati
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Suara Enemy");
        
        // Sistem Skor
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.TambahSkor(10);
        }

        // Sistem Loot Drop (Gacha Item Darah)
        if (itemDarahPrefab != null)
        {
            float gacha = Random.Range(0f, 100f);
            if (gacha <= peluangDrop)
            {
                Vector2 posisiSpawn = new Vector2(transform.position.x, transform.position.y + 0.5f);
                Instantiate(itemDarahPrefab, posisiSpawn, Quaternion.identity);
            }
        }

        anim.SetBool("IsDead", true); 
        GetComponent<Collider2D>().enabled = false;
        rb.gravityScale = 0; 
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 2f);
    }
}