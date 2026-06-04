using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;           // Kecepatan peluru terbang
    public int damage = 1;              // Damage peluru (buat nanti)
    public float lifeTime = 3f;         // Peluru hancur otomatis setelah 3 detik biar nggak nyampah

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError("Prefab peluru butuh component Rigidbody2D!");
            return;
        }

        // Trik Ajaib: Karena FirePoint di-spawn ngikutin arah flip Player (localScale X), 
        // kita tinggal suruh peluru terbang lurus ke "kanan" relatif dari rotasi FirePoint-nya.
        // Kalau Player hadap kiri, FirePoint rotasinya 180 derajat, jadi peluru terbang ke kiri.
        rb.linearVelocity = transform.right * speed;

        // Peluru hancur otomatis setelah lifeTime
        Destroy(gameObject, lifeTime);
    }

    // Mekanik kalau kena musuh (buat nanti)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek kalau kena objek dengan tag "Enemy" (bakteri/virus)[cite: 2]
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Peluru kena musuh!");
            
            // TODO: Tambah logic ngurangin health musuh di sini
            // misal: collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);

            // Hancurkan peluru setelah kena hit
            Destroy(gameObject);
        }
        
        // Opsional: Hancur kalau kena tembok/tanah biar nggak nembus
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}