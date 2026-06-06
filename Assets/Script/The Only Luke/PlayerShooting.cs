using UnityEngine;
using UnityEngine.EventSystems;
public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.3f;
    private float nextFireTime = 0f;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // SATPAM UI Biar gak nembak pas ngeklik tombol Pause/Menu
        if (PauseMenuManager.GameIsPaused || EventSystem.current.IsPointerOverGameObject()) return;

        // Cek input nembak dan cooldown
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J)) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            
            // --- TAMBAHAN AUDIO ---
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Nembak");

            // 1. DI SINI KITA CUMA MANGGIL ANIMASINYA AJA
            if(anim != null) anim.SetTrigger("Shoot");
        }
    }

    // 2. FUNGSI INI AKAN DIPANGGIL OLEH ANIMATION EVENT
    public void SpawnBullet()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}