using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 8f;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");

        // RUN pakai tombol R
        bool isRunning = Input.GetKey(KeyCode.R);
        float currentSpeed = isRunning ? speed * 2 : speed;

        rb.linearVelocity = new Vector2(move * currentSpeed, rb.linearVelocity.y);

        // LOMPAT
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false; 

            // NYALAKAN animasi lompat!
            anim.SetBool("isJump", true); 
        }

        // FLIP KARAKTER
        Vector3 scale = transform.localScale;
        if (move > 0) scale.x = Mathf.Abs(scale.x);
        else if (move < 0) scale.x = -Mathf.Abs(scale.x);
        transform.localScale = scale;

        // ANIMATOR
        anim.SetFloat("Speed", Mathf.Abs(move));
        
        // HAPUS tanda "//" di awal baris bawah ini KALAU lu udah bikin parameter "isRunning" (Bool) di Animator
        // anim.SetBool("isRunning", isRunning && Mathf.Abs(move) > 0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            
            // MATIKAN animasi lompat pas kaki nyentuh tanah!
            anim.SetBool("isJump", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}