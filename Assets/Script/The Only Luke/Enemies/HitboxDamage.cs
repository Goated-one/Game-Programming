using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    public int damageAmount = 20; // Jumlah damage yang dikasih (bisa diatur dari Inspector)

    // Fungsi ini otomatis jalan kalau hitbox nyentuh objek lain
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek apakah objek yang ditabrak punya Tag "Enemy"
        if (collision.CompareTag("Enemy"))
        {
            // 1. Cek musuh lama (Jamur)
            Enemy enemyScript = collision.GetComponent<Enemy>();
            
            // Kalau script-nya ada, kasih damage!
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damageAmount);
            }

            // 2. --- TAMBAHAN BARU: Cek musuh penembak ---
            RangedEnemy rangedEnemyScript = collision.GetComponent<RangedEnemy>();
            
            // Kalau script-nya ada, kasih damage juga!
            if (rangedEnemyScript != null)
            {
                rangedEnemyScript.TakeDamage(damageAmount);
            }
        }
    }
}