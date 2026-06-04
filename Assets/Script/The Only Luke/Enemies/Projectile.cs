using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f; 
    public int damage = 20;   
    public float lifetime = 3f; 

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // 1. Cari Player saat peluru baru saja di-spawn
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            // 2. Hitung arah pasti dari peluru menuju Player
            Vector2 direction = (player.transform.position - transform.position).normalized;
            
            // 3. Tembak peluru ke arah tersebut
            rb.linearVelocity = direction * speed;
            
            // 4. (Opsional) Putar gambar peluru biar moncongnya ikut mengarah ke Player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Kalau Player mati atau gak ketemu, tembak lurus aja
            rb.linearVelocity = transform.right * speed;
        }
        
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            PlayerHealth player = hitInfo.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage, transform);
            }
            Destroy(gameObject);
        }
        else if (hitInfo.CompareTag("Ground") ) 
        {
            Destroy(gameObject);
        }
    }
}