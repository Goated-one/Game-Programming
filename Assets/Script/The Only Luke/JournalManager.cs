using UnityEngine;
using UnityEngine.UI; // WAJIB: Diaktifkan biar Unity bisa ngebaca komponen UI Image
using TMPro;

[System.Serializable]
public class EnemyData
{
    public string namaBakteri;
    [TextArea(3, 10)]
    public string infoPenyakit;
    
    // --- TAMBAHAN BARU: Slot buat nyimpen file gambar/sprite masing-masing musuh ---
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
    
    // --- TAMBAHAN BARU: Slot buat naruh komponen UI Image yang ada di Canvas ---
    public Image komponenGambarUI; 
    
    public GameObject journalPanel; 

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
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
                
                // --- TAMBAHAN BARU: Menampilkan gambar dan memasukkan fotonya ---
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
        
        // --- TAMBAHAN BARU: Sembunyiin kotak gambar pas baru buka Journal (biar gk kosong melompong) ---
        if (komponenGambarUI != null)
        {
            komponenGambarUI.gameObject.SetActive(false);
        }
    }
}