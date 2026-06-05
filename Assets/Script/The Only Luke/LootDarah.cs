using UnityEngine;

public class LootDarah : MonoBehaviour
{
    [Header("Pengaturan Item")]
    public int jumlahHeal = 1; // Nambah berapa nyawa? (1 hati)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kalau yang nyentuh adalah Player
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                // Coba heal player-nya. Fungsi TambahNyawa bakal nge-return 'true' kalau berhasil (darah belum penuh)
                bool berhasilHeal = playerHealth.TambahNyawa(jumlahHeal);

                if (berhasilHeal)
                {
                    // Hancurkan item darah ini dari layar
                    Destroy(gameObject);
                }
            }
        }
    }
}