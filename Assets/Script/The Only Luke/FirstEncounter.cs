using UnityEngine;
using System.Collections; 
using TMPro; 

public class FirstEncounter : MonoBehaviour
{
    [Header("Pengaturan Cinemachine")]
    public GameObject vcamMusuh; 
    public float durasiTransisi = 1.5f; 

    [Header("Pengaturan UI Pop-up")]
    public GameObject panelPenjelasan;
    public TextMeshProUGUI teksJudulPopup;  
    public TextMeshProUGUI teksInfoPopup;   

    [Header("Pengaturan Journal")]
    [Tooltip("Ketik nama bakteri persis seperti yang didaftarkan di JournalManager")]
    public string namaBakteriUntukJournal; 

    private bool sudahDipicu = false;

    void Start()
    {
        if (panelPenjelasan != null) panelPenjelasan.SetActive(false);
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

        // Buka gembok data musuh di Journal tepat saat encounter dimulai
        if (JournalManager.instance != null && !string.IsNullOrEmpty(namaBakteriUntukJournal))
        {
            JournalManager.instance.UnlockDataMusuh(namaBakteriUntukJournal);

            // PROSES PENGAMBILAN DATA OTOMATIS
            foreach (var musuh in JournalManager.instance.daftarMusuh)
            {
                if (musuh.namaBakteri == namaBakteriUntukJournal)
                {
                    if (teksJudulPopup != null) teksJudulPopup.text = musuh.namaBakteri;
                    if (teksInfoPopup != null) teksInfoPopup.text = musuh.infoPenyakit;
                    break; 
                }
            }
        }

        // 1. BEKUKAN WAKTU GAME
        Time.timeScale = 0f;

        // 2. NYALAKAN KAMERA MUSUH 
        if (vcamMusuh != null) vcamMusuh.SetActive(true);

        // 3. TUNGGU KAMERA SELESAI GESER 
        yield return new WaitForSecondsRealtime(durasiTransisi);

        // 4. MUNCULIN TEKS
        if (panelPenjelasan != null) panelPenjelasan.SetActive(true);

        // 5. TUNGGU PLAYER MENCET SPASI (Atau klik kiri)
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));

        // 6. TUTUP TEKS & MATIKAN KAMERA MUSUH
        if (panelPenjelasan != null) panelPenjelasan.SetActive(false);
        if (vcamMusuh != null) vcamMusuh.SetActive(false);

        // 7. TUNGGU KAMERA BALIK KE PLAYER
        yield return new WaitForSecondsRealtime(durasiTransisi);

        // 8. LANJUTKAN GAME
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}