using UnityEngine;
using TMPro;
using System.Collections; // Wajib buat bikin efek jeda & transisi waktu

public class PintuPenghalang : MonoBehaviour
{
    [Header("Pengaturan Pintu")]
    public int syaratSkor = 50; 

    [Header("Pengaturan UI Peringatan")]
    public TextMeshProUGUI teksPeringatan;
    public float durasiFade = 0.5f; // Kecepatan efek transisi transparan (detik)
    public float durasiTampil = 1.5f; // Berapa lama teksnya nampang sebelum ngilang (detik)

    private bool sudahTerbuka = false; 
    private bool sedangMunculinTeks = false; // Biar efek fadenya nggak tumpang tindih kalau player nabrak berkali-kali

    void Start()
    {
        // Pas game mulai, bikin teks peringatannya jadi 100% transparan (nggak kelihatan)
        if (teksPeringatan != null)
        {
            Color warnaAwal = teksPeringatan.color;
            warnaAwal.a = 0f; // a itu Alpha (0 = hilang, 1 = solid)
            teksPeringatan.color = warnaAwal;
        }
    }

    void Update()
    {
        if (sudahTerbuka) return;

        // Cek terus apakah skor udah cukup buat ngancurin pintu
        if (ScoreManager.instance != null && ScoreManager.instance.skorSaatIni >= syaratSkor)
        {
            BukaPintu();
        }
    }

    // Fungsi ini kepanggil otomatis pas badan THE ONLY LUKE nabrak (nyentuh fisik) pintu ini
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!sudahTerbuka && collision.gameObject.CompareTag("Player"))
        {
            // Kalau skor kurang dan teksnya lagi nggak diputar, jalanin efek fade-nya
            if (ScoreManager.instance.skorSaatIni < syaratSkor)
            {
                if (!sedangMunculinTeks && teksPeringatan != null)
                {
                    StartCoroutine(ProsesFadeTeks());
                }
            }
        }
    }

    void BukaPintu()
    {
        sudahTerbuka = true;
        gameObject.SetActive(false); // Pintunya lenyap
    }

    // --- COROUTINE BUAT NGATUR ANIMASI FADE ---
    IEnumerator ProsesFadeTeks()
    {
        sedangMunculinTeks = true;
        
        // Update isi teksnya
        teksPeringatan.text = "Gerbang Terkunci!\nButuh " + syaratSkor + " Skor.";

        // 1. FADE IN (Muncul perlahan dari Alpha 0 ke 1)
        yield return StartCoroutine(UbahTransparansi(0f, 1f, durasiFade));

        // 2. TAHAN (Biar playernya sempet baca)
        yield return new WaitForSeconds(durasiTampil);

        // 3. FADE OUT (Ngilang perlahan dari Alpha 1 ke 0)
        yield return StartCoroutine(UbahTransparansi(1f, 0f, durasiFade));

        sedangMunculinTeks = false; // Buka kunci biar bisa dipanggil lagi nanti
    }

    // Fungsi mesin matematika buat ngubah warna secara mulus
    IEnumerator UbahTransparansi(float alphaAwal, float alphaAkhir, float durasi)
    {
        float waktuBerjalan = 0f;
        Color warna = teksPeringatan.color;

        while (waktuBerjalan < durasi)
        {
            waktuBerjalan += Time.deltaTime;
            // Mathf.Lerp itu rumus buat nyari nilai tengah secara mulus
            warna.a = Mathf.Lerp(alphaAwal, alphaAkhir, waktuBerjalan / durasi);
            teksPeringatan.color = warna;
            yield return null; // Tunggu ke frame layar selanjutnya
        }

        // Pastikan angka akhirnya pas persis di tujuan
        warna.a = alphaAkhir;
        teksPeringatan.color = warna;
    }
}