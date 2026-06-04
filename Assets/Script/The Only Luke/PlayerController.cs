using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool canMove = true; // Gembok utama pergerakan
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public bool isGrounded;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Kalau canMove false (lagi recovery drop attack), stop baca input gerak!
        if (!canMove) return;

        // Mengatur Pergerakan Kiri & Kanan
        float dirX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);

        // Mengatur Animasi Walk / Idle berdasarkan input
        anim.SetFloat("Speed", Mathf.Abs(dirX));

        // --- CARA BARU FLIP PAKAI ROTASI (Biar arah peluru ikut putar balik) ---
        if (dirX > 0f)
        {
            // Putar rotasi Y jadi 0 (Hadap Kanan)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (dirX < 0f)
        {
            // Putar rotasi Y jadi 180 (Hadap Kiri)
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        // Mengatur Lompat
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            anim.SetBool("isJumping", true);
        }
    }

    // Mengecek apakah karakter menyentuh tanah
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }
    }
}