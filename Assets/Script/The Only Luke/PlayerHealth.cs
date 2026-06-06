using UnityEngine;
using UnityEngine.UI; // WAJIB: Biar kita bisa ngendaliin komponen UI Image
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Sistem Nyawa")]
    public int maxNyawa = 3;
    private int nyawaSaatIni;

    [Header("UI Nyawa")]
    public Image[] arrayHatiUI; // Kotak buat masukin objek Hati_1, Hati_2, Hati_3
    public Sprite hatiPenuh;    // Sprite pas nyawa utuh
    public Sprite hatiKosong;   // Sprite pas nyawa hilang

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
        
        // Panggil fungsi ini pas game mulai biar tampilan hatinya sesuai
        UpdateUINyawa(); 
    }

    // Fungsi ini dipanggil sama Enemy / Projectile
    public void TakeDamage(int damage, Transform attacker)
    {
        if (isDead || isInvincible) return;

        // KUNCINYA DI SINI: Kita abaikan (int damage) dari musuh. 
        // Berapapun pukulannya, nyawa Luke cuma ngurang 1.
        nyawaSaatIni -= 1; 
        
        UpdateUINyawa(); // Update gambar hati di layar

        // --- TAMBAHAN SCREEN SHAKE KENCANG ---
        if (ScreenShakeManager.instance != null)
        {
            ScreenShakeManager.instance.ShakeCamera(0.5f); // Getaran full (1f) buat peringatan bahaya
        }
        
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
        // Mesin otomatis buat ngecek satu-satu gambar hati di layar
        for (int i = 0; i < arrayHatiUI.Length; i++)
        {
            // Kalau urutan hatinya masih di bawah jumlah nyawa saat ini, kasih gambar Penuh
            if (i < nyawaSaatIni)
            {
                arrayHatiUI[i].sprite = hatiPenuh; 
            }
            // Kalau udah melebihi, kasih gambar Kosong
            else
            {
                arrayHatiUI[i].sprite = hatiKosong; 
            }
            
            // Pengaman: Tampilkan hati sesuai batas maxNyawa
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
        if (sr != null)
        {
            sr.sortingOrder = 10; 
        }

        // --- TAMBAHAN BARU: PANGGIL PROSES GAME OVER ---
        StartCoroutine(TungguLaluGameOver());
    }

    // FUNGSI BARU BUAT NAMBAH NYAWA DARI LOOT
    public bool TambahNyawa(int jumlah)
    {
        // Kalau nyawa udah penuh atau player udah mati, itemnya dibiarin aja (nggak diambil)
        if (nyawaSaatIni >= maxNyawa || isDead) 
        {
            return false; 
        }

        nyawaSaatIni += jumlah;
        
        // Pengaman biar nyawa nggak bablas nembus batas maksimal
        if (nyawaSaatIni > maxNyawa) 
        {
            nyawaSaatIni = maxNyawa;
        }
        
        UpdateUINyawa(); // Panggil fungsi update gambar hati UI
        return true;     // Kasih tau itemnya kalau nyawa berhasil ditambah!
    }

    // FUNGSI BARU KHUSUS BUAT JURANG KEMATIAN
    public void MatiInstan()
    {
        // Kalau udah mati duluan, nggak usah diulang
        if (isDead) return; 

        // Kuras habis nyawanya jadi 0
        nyawaSaatIni = 0;
        
        // Panggil fungsi update UI biar semua hatinya langsung kosong
        UpdateUINyawa(); 
        
        // Eksekusi fungsi mati
        Die(); 
    }

    IEnumerator TungguLaluGameOver()
    {
        // Tunggu 2 detik biar Luke bisa beres nge-play animasi matinya
        yield return new WaitForSeconds(2f);
        
        // Panggil panel game over-nya
        if (GameOverManager.instance != null)
        {
            GameOverManager.instance.MunculkanGameOver();
        }
    }
}