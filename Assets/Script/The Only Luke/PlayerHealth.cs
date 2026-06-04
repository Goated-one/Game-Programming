using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private Animator anim;
    private PlayerController playerCtrl;
    private PlayerMelee playerMelee; 
    
    public bool isDead = false;
    private bool isInvincible = false; // Bikin player kebal sesaat abis kena hit

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        playerCtrl = GetComponent<PlayerController>();
        playerMelee = GetComponent<PlayerMelee>();
    }

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackUpward = 3f;

    // UBAHAN: Kita minta info "Transform attacker" biar tahu siapa yang mukul
    public void TakeDamage(int damage, Transform attacker)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("Hit"); 
            
            // Cari tahu harus kepental ke mana
            float knockbackDir = transform.position.x < attacker.position.x ? -1f : 1f;
            
            StartCoroutine(InvincibilityFrames(knockbackDir));
        }
    }

    IEnumerator InvincibilityFrames(float knockbackDir)
    {
        isInvincible = true;
        
        // Kunci pergerakan dari PlayerController
        if (playerCtrl != null) playerCtrl.canMove = false;
        
        // KASIH DORONGAN KNOCKBACK KE LUKE
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(knockbackDir * knockbackForce, knockbackUpward);
        
        yield return new WaitForSeconds(0.3f); 
        
        // Buka lagi kunci pergerakannya setelah selesai kepental
        if (playerCtrl != null) playerCtrl.canMove = true;

        yield return new WaitForSeconds(0.7f); 
        isInvincible = false;
    }

    void Die()
    {
        isDead = true;
        anim.SetBool("isDead", true);

     // 1. Kunci semua pergerakan dan serangan
        if (playerCtrl != null) playerCtrl.enabled = false;
        if (playerMelee != null) playerMelee.enabled = false;

        // 2. Stop momentum dan MATIKAN GRAVITASI biar mayat gak tembus ke bawah lantai
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0; 
        
        // 3. MATIKAN COLLIDER biar musuh bisa lewat nembus mayatnya
        GetComponent<Collider2D>().enabled = false;

        // 4. (OPSIONAL) Bikin mayat Luke ada di "depan" musuh secara visual
        // Pastikan Luke punya komponen SpriteRenderer ya
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Naikin angkanya biar dia digambar paling akhir (paling depan) di layar
            sr.sortingOrder = 10; 
        }
    }
}