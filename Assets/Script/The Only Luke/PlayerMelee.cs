using UnityEngine;
using System.Collections; // WAJIB DITAMBAHIN BUAT BIKIN TIMER COROUTINE

public class PlayerMelee : MonoBehaviour
{
    public float dropForce = 15f;      // Kecepatan meluncur ke bawah saat Drop Attack
    public GameObject meleeHitbox;     // Drag objek MeleeHitbox ke sini
    public GameObject dropHitbox;      // Drag objek DropHitbox ke sini
    
    // Waktu jeda (stun) setelah mendarat dari drop attack
    public float recoveryTime = 0.5f;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerController playerCtrl; // Buat ngecek isGrounded
    
    public Animator meleeSlashAnim; // Drag objek MeleeHitbox ke sini nanti
    public Animator dropSlashAnim;  // Drag objek DropHitbox ke sini nanti

    // Gembok biar drop attack nggak bisa di-spam
    private bool hasDropAttacked = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerCtrl = GetComponent<PlayerController>(); 
    }

    void Update()
    {
        // 1. Cek momen pas mendarat HANYA SETELAH melakukan drop attack
        if (playerCtrl.isGrounded && hasDropAttacked)
        {
            hasDropAttacked = false; // Langsung reset biar gk panggil timer berkali-kali
            StartCoroutine(DropAttackRecovery()); // Jalankan timer jeda
        }

        // 2. Cek input melee (Klik Kanan atau K) + Cek canMove biar gk bisa mukul pas lg stun
        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.K)) && playerCtrl.canMove)
        {
            if (playerCtrl.isGrounded)
            {
                // Kalau lagi di tanah -> Serangan Biasa
                anim.SetTrigger("Melee");
            }
            else if (!hasDropAttacked) // Cek apakah gembok drop attack masih kebuka
            {
                hasDropAttacked = true; // Langsung kunci gemboknya
                
                // Kalau lagi di udara -> Drop Attack
                anim.SetTrigger("DropAttack");
                
                // Paksa karakter langsung meluncur cepat ke bawah
                rb.linearVelocity = new Vector2(0, -dropForce);
            }
        }
    }

    // --- FUNGSI COROUTINE BUAT JEDA RECOVERY ---
    IEnumerator DropAttackRecovery()
    {
        // Kunci pergerakan (player gk bisa jalan/lompat)
        playerCtrl.canMove = false; 
        
        // Pastikan karakternya beneran ngerem berhenti pas nabrak tanah
        rb.linearVelocity = Vector2.zero; 

        // Tunggu selama waktu recovery (0.5 detik)
        yield return new WaitForSeconds(recoveryTime); 

        // Buka gemboknya lagi, player bisa main normal
        playerCtrl.canMove = true; 
    }

    // --- FUNGSI-FUNGSI INI DIPANGGIL LEWAT ANIMATION EVENT ---

    public void EnableMeleeHitbox()
    {
        meleeHitbox.SetActive(true);
        // Mainkan animasi slash dari frame 0
        if (meleeSlashAnim != null) 
        {
            meleeSlashAnim.Play("Slash_Anim", -1, 0f); 
        }
    }

    public void DisableMeleeHitbox()
    {
        meleeHitbox.SetActive(false);
    }

    public void EnableDropHitbox()
    {
        dropHitbox.SetActive(true);
        // Mainkan animasi drop slash dari frame 0
        if (dropSlashAnim != null) 
        {
            dropSlashAnim.Play("DropSlash_Anim", -1, 0f); 
        }
    }

    public void DisableDropHitbox()
    {
        dropHitbox.SetActive(false);
    }
}