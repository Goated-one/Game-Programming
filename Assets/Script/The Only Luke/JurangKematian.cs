using UnityEngine;

public class JurangKematian : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ngecek apakah yang jatuh ke jurang itu THE ONLY LUKE
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                // Panggil fungsi mati instan tanpa ampun
                playerHealth.MatiInstan();
            }
        }
        
        // (Opsional) Kalau lu mau musuh juga mati kalau jatuh ke jurang ini
        else if (collision.CompareTag("Enemy"))
        {
            // Langsung hancurkan objek musuhnya
            Destroy(collision.gameObject);
        }
    }
}