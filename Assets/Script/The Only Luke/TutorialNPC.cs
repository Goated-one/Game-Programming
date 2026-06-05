using UnityEngine;
using TMPro; 

public class TutorialNPC : MonoBehaviour
{
    [Header("Pengaturan Jarak")]
    public Transform player;
    public float jarakDeteksi = 3f; 

    [Header("Pengaturan UI")]
    public GameObject panelTutorial; 
    public TextMeshProUGUI teksTutorialUI; 

    [Header("Pesan Dialog (Bisa Banyak Halaman)")]
    [TextArea(3, 5)] 
    // UBAHAN PENTING: Sekarang pakai kurung siku [] biar jadi Array (kumpulan kalimat)
    public string[] pesanDialog; 
    
    // Variabel buat ngelacak kita lagi di halaman ke berapa
    private int indeksDialog = 0; 
    private bool isPanelAktif = false;

    void Start()
    {
        if (panelTutorial != null)
        {
            panelTutorial.SetActive(false);
        }
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        float jarakKePlayer = Vector2.Distance(transform.position, player.position);

        if (jarakKePlayer <= jarakDeteksi)
        {
            // Kalau baru masuk area, munculkan dari halaman pertama
            if (!isPanelAktif)
            {
                MunculkanTutorial();
            }
            // Kalau panel udah nyala, tunggu THE ONLY LUKE mencet tombol
            else
            {
                // DETEKSI TOMBOL: Bisa klik kiri mouse (0) ATAU tekan tombol 'E' di keyboard
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
                {
                    LanjutDialog();
                }
            }
        }
        else
        {
            // Kalau menjauh, langsung tutup dan reset
            if (isPanelAktif)
            {
                TutupTutorial();
            }
        }
    }

    void MunculkanTutorial()
    {
        isPanelAktif = true;
        indeksDialog = 0; // Mulai selalu dari halaman 0 (pertama)
        
        if (panelTutorial != null) panelTutorial.SetActive(true);
        
        TampilkanTeksSaatIni();
    }

    void LanjutDialog()
    {
        indeksDialog++; // Tambah 1 ke halaman berikutnya

        // Cek apakah halamannya masih ada
        if (indeksDialog < pesanDialog.Length)
        {
            TampilkanTeksSaatIni(); // Tunjukin kalimat selanjutnya
        }
        else
        {
            // Kalau indeksnya udah lebih dari jumlah kalimat, berarti dialog habis -> Tutup!
            TutupTutorial();
        }
    }

    void TampilkanTeksSaatIni()
    {
        if (teksTutorialUI != null && pesanDialog.Length > 0)
        {
            teksTutorialUI.text = pesanDialog[indeksDialog];
        }
    }

    void TutupTutorial()
    {
        isPanelAktif = false;
        indeksDialog = 0; // Reset memori halamannya
        if (panelTutorial != null) panelTutorial.SetActive(false);
    }
}