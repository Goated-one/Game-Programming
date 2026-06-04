using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool canMove = true; 
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
       
        if (!canMove) return;

        
        float dirX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);

       
        anim.SetFloat("Speed", Mathf.Abs(dirX));

      
        if (dirX > 0f)
        {
            
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (dirX < 0f)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            anim.SetBool("isJumping", true);
        }
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }
    }
}