using UnityEngine;
using UnityEngine.UI; 
using TMPro;

[System.Serializable]
public class EnemyData
{
    public string namaBakteri;
    [TextArea(3, 10)]
    public string infoPenyakit;
    public Sprite fotoBakteri; 
    public bool sudahKetemu = false; 
    public GameObject tombolUI; 
}

public class JournalManager : MonoBehaviour
{
    public static JournalManager instance;

    [Header("Data Musuh")]
    public EnemyData[] daftarMusuh;

    [Header("UI Tampilan")]
    public TextMeshProUGUI teksJudul;
    public TextMeshProUGUI teksInfo;
    public Image komponenGambarUI; 
    public GameObject journalPanel; 

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // --- FITUR BARU: BACA CATATAN INGATAN (LOAD DATA) ---
        foreach (var musuh in daftarMusuh)
        {
            // Cek apakah di memori komputer udah ada data bakteri ini yang disimpen?
            // Angka 1 artinya udah ketemu, angka 0 artinya belum.
            if (PlayerPrefs.GetInt("Jurnal_" + musuh.namaBakteri, 0) == 1)
            {
                musuh.sudahKetemu = true;
            }
        }

        RefreshTampilanTombol();
        KosongkanTeks();
    }

    public void RefreshTampilanTombol()
    {
        foreach (var musuh in daftarMusuh)
        {
            if (musuh.tombolUI != null)
            {
                musuh.tombolUI.SetActive(musuh.sudahKetemu);
            }
        }
    }

    public void UnlockDataMusuh(string nama)
    {
        foreach (var musuh in daftarMusuh)
        {
            if (musuh.namaBakteri == nama && !musuh.sudahKetemu)
            {
                musuh.sudahKetemu = true;

                // --- FITUR BARU: TULIS KE BUKU CATATAN (SAVE DATA) ---
                // Menyimpan angka 1 ke memori komputer dengan nama "Jurnal_NamaBakteri"
                PlayerPrefs.SetInt("Jurnal_" + nama, 1);
                PlayerPrefs.Save(); 

                RefreshTampilanTombol();
                return;
            }
        }
    }

    public void TampilkanInfo(string nama)
    {
        foreach (var musuh in daftarMusuh)
        {
            if (musuh.namaBakteri == nama && musuh.sudahKetemu)
            {
                teksJudul.text = musuh.namaBakteri;
                teksInfo.text = musuh.infoPenyakit;
                
                if (komponenGambarUI != null && musuh.fotoBakteri != null)
                {
                    komponenGambarUI.gameObject.SetActive(true);
                    komponenGambarUI.sprite = musuh.fotoBakteri;
                }
                return;
            }
        }
    }

    public void KosongkanTeks()
    {
        teksJudul.text = "Pilih Data";
        teksInfo.text = "Pilih nama virus/bakteri di samping untuk membaca wawasan penyakitnya.";
        
        if (komponenGambarUI != null)
        {
            komponenGambarUI.gameObject.SetActive(false);
        }
    }

    // --- FITUR BONUS: Tombol Darurat Buat Reset Data Pas Lagi Ngetes Game ---
    [ContextMenu("Reset Semua Data Jurnal")]
    public void ResetJurnal()
    {
        foreach (var musuh in daftarMusuh)
        {
            musuh.sudahKetemu = false;
            PlayerPrefs.DeleteKey("Jurnal_" + musuh.namaBakteri);
        }
        PlayerPrefs.Save();
        RefreshTampilanTombol();
        KosongkanTeks();
        Debug.Log("Semua data Jurnal berhasil di-reset!");
    }
}