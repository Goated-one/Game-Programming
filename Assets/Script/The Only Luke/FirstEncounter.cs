using UnityEngine;
using System.Collections; 

public class FirstEncounter : MonoBehaviour
{
    [Header("Pengaturan Cinemachine")]
    public GameObject vcamMusuh; // Masukin Virtual Camera khusus musuh ke sini
    public float durasiTransisi = 1.5f; // Cocokin sama pengaturan Default Blend di Cinemachine

    [Header("Pengaturan UI")]
    public GameObject panelPenjelasan;

    private bool sudahDipicu = false;

    void Start()
    {
        if (panelPenjelasan != null) panelPenjelasan.SetActive(false);
        
        // Pastikan VCam musuh mati pas game baru mulai
        if (vcamMusuh != null) vcamMusuh.SetActive(false); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !sudahDipicu)
        {
            StartCoroutine(MulaiEncounter());
        }
    }

    IEnumerator MulaiEncounter()
    {
        sudahDipicu = true;

        // 1. BEKUKAN WAKTU GAME
        Time.timeScale = 0f;

        // 2. NYALAKAN KAMERA MUSUH 
        // (Cinemachine bakal otomatis nge-pan & zoom ke sana)
        if (vcamMusuh != null) vcamMusuh.SetActive(true);

        // 3. TUNGGU KAMERA SELESAI GESER 
        // (Pakai WaitForSecondsRealtime karena Time.timeScale lagi 0)
        yield return new WaitForSecondsRealtime(durasiTransisi);

        // 4. MUNCULIN TEKS
        if (panelPenjelasan != null) panelPenjelasan.SetActive(true);

        // 5. TUNGGU PLAYER MENCET SPASI (Atau klik kiri)
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));

        // 6. TUTUP TEKS & MATIKAN KAMERA MUSUH
        // (Otomatis kamera utamanya balik ke Player)
        if (panelPenjelasan != null) panelPenjelasan.SetActive(false);
        if (vcamMusuh != null) vcamMusuh.SetActive(false);

        // 7. TUNGGU KAMERA BALIK KE PLAYER
        yield return new WaitForSecondsRealtime(durasiTransisi);

        // 8. LANJUTKAN GAME
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}