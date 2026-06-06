using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Sistem Nyawa")]
    public int maxNyawa = 3;
    private int nyawaSaatIni;

    [Header("UI Nyawa")]
    public Image[] arrayHatiUI; 
    public Sprite hatiPenuh;    
    public Sprite hatiKosong;   

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackUpward = 3f;

    private Animator anim;
    private PlayerController playerCtrl;
    private PlayerMelee playerMelee; 
    
    public bool isDead = false;
    private bool isInvincible = false; 

    void Start()
    {
        nyawaSaatIni = maxNyawa;
        anim = GetComponent<Animator>();
        playerCtrl = GetComponent<PlayerController>();
        playerMelee = GetComponent<PlayerMelee>();
        UpdateUINyawa(); 
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        if (isDead || isInvincible) return;

        nyawaSaatIni -= 1; 
        UpdateUINyawa(); 

        if (ScreenShakeManager.instance != null)
        {
            ScreenShakeManager.instance.ShakeCamera(0.5f); 
        }
        
        // --- TAMBAHAN AUDIO ---
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Hit Player");

        if (nyawaSaatIni <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("Hit"); 
            float knockbackDir = transform.position.x < attacker.position.x ? -1f : 1f;
            StartCoroutine(InvincibilityFrames(knockbackDir));
        }
    }

    void UpdateUINyawa()
    {
        for (int i = 0; i < arrayHatiUI.Length; i++)
        {
            if (i < nyawaSaatIni) arrayHatiUI[i].sprite = hatiPenuh; 
            else arrayHatiUI[i].sprite = hatiKosong; 
            
            if (i < maxNyawa) arrayHatiUI[i].enabled = true;
            else arrayHatiUI[i].enabled = false;
        }
    }

    IEnumerator InvincibilityFrames(float knockbackDir)
    {
        isInvincible = true;
        if (playerCtrl != null) playerCtrl.canMove = false;
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(knockbackDir * knockbackForce, knockbackUpward);
        yield return new WaitForSeconds(0.3f); 
        if (playerCtrl != null) playerCtrl.canMove = true;
        yield return new WaitForSeconds(0.7f); 
        isInvincible = false;
    }

    void Die()
    {
        isDead = true;
        anim.SetBool("isDead", true);

        if (playerCtrl != null) playerCtrl.enabled = false;
        if (playerMelee != null) playerMelee.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0; 
        GetComponent<Collider2D>().enabled = false;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = 10; 

        StartCoroutine(TungguLaluGameOver());
    }

    public bool TambahNyawa(int jumlah)
    {
        if (nyawaSaatIni >= maxNyawa || isDead) return false; 
        nyawaSaatIni += jumlah;
        if (nyawaSaatIni > maxNyawa) nyawaSaatIni = maxNyawa;
        UpdateUINyawa(); 
        return true;     
    }

    public void MatiInstan()
    {
        if (isDead) return; 
        nyawaSaatIni = 0;
        UpdateUINyawa(); 
        
        // --- TAMBAHAN AUDIO KALO MATI JATUH ---
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Hit Player");

        Die(); 
    }

    IEnumerator TungguLaluGameOver()
    {
        yield return new WaitForSeconds(2f);
        if (GameOverManager.instance != null) GameOverManager.instance.MunculkanGameOver();
    }
}