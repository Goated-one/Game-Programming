using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Knockback")]
    public float knockbackForce = 4f; // Jauhnya pentalan ke belakang
    public float knockbackUpward = 2f; // Efek sedikit terangkat ke udara
    
    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public bool movingRight = false;
    public Transform edgeCheck; 

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
    private PlayerHealth targetHealth; // <--- TAMBAHIN INI

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // PENGAMAN: Cari Player dengan aman biar gak crash
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            targetHealth = playerObj.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogError("WOY! Objek karakter utama lu belum dipasangin Tag 'Player' di Inspector tuh!");
        }
    }

   void Update()
    {
        // Kalau player gak ketemu (crash), stop semuanya biar gak error
        if (playerTransform == null) return; 

        cooldownTimer += Time.deltaTime; 

        if (isDead || isStunned || isAttacking)
        {
            // UBAHAN PENTING: Hanya paksa berhenti (0) kalau lagi GAK STUN dan GAK NYERANG
            if (!isAttacking && !isStunned) 
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            anim.SetFloat("Speed", 0);
            return;
        }

        // --- TAMBAHAN BARU: CEK APAKAH PLAYER UDAH MATI ---
        if (targetHealth != null && targetHealth.isDead)
        {
            Patrol(); // Kalau Luke mati, musuhnya santai lanjut jalan-jalan
            return;   // Stop eksekusi kode ke bawah biar dia gak ngejar/nyerang lagi
        }
        
        // CARA BARU: Ngukur jarak khusus sumbu X (kiri-kanan) biar lebih akurat buat 2D
        float distanceToPlayer = Mathf.Abs(transform.position.x - playerTransform.position.x);

        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetFloat("Speed", 0);

            FacePlayer(); 

            if (cooldownTimer >= attackCooldown)
            {
                AttackPlayer();
            }
        }
        else if (distanceToPlayer <= aggroRange)
        {
            FacePlayer();
            Patrol(); 
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
        }
        else if (playerTransform.position.x < transform.position.x && movingRight)
        {
            movingRight = false;
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

    // --- POSISI DEAL DAMAGE UDAH DIBENERIN JADI DI TENGAH BADAN ---
    public void DealDamage()
    {
        Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
        
        // Sekarang pakai transform.position (titik tengah), BUKAN edgeCheck
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, attackRange, playerLayer);
        
        // Garis merah buat ngebantu lu nge-debug di scene
        Debug.DrawRay(transform.position, rayDirection * attackRange, Color.red, 1f);

        if (hit.collider != null)
        {
            PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // TAMBAHKAN "transform" DI SINI BIAR PLAYER TAHU DIA DIPUKUL SAMA SIAPA
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

        RaycastHit2D groundInfo = Physics2D.Raycast(edgeCheck.position, Vector2.down, 1f);
        Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallInfo = Physics2D.Raycast(edgeCheck.position, rayDirection, 0.2f);

        bool isEdge = groundInfo.collider == null; 
        
        // CARA BARU: Cek tembok, TAPI abaikan objek yang punya Tag Player
        bool isWall = wallInfo.collider != null && wallInfo.collider.gameObject != gameObject && !wallInfo.collider.CompareTag("Player");

        if (isEdge || isWall)
        {
            movingRight = !movingRight; 
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
            
            // Logika arah pantulan: Kalau musuh di kiri Player, pentalin ke kiri (-1). Kalau di kanan, ke kanan (1).
            float knockbackDir = transform.position.x < playerTransform.position.x ? -1f : 1f;
            
            // Kirim arahnya ke Coroutine Stun
            StartCoroutine(HitStun(knockbackDir));
        }
    }

    IEnumerator HitStun(float knockbackDir)
    {
        isStunned = true;
        isAttacking = false; 
        
        // KASIH DORONGAN KNOCKBACK!
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