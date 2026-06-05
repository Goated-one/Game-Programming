using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;
using System.Collections; // Wajib ditambahin buat bikin jeda waktu (Coroutine)

public class TriggerJurang : MonoBehaviour
{
    [Header("Pengaturan Jurang")]
    public int syaratSkor = 50; 
    public string namaSceneTujuan = "P14_Level2"; 

    [Header("Kalau Gagal (Skor Kurang)")]
    public Transform titikKembali; // Posisi THE ONLY LUKE dilempar balik
    public TextMeshProUGUI teksPeringatan; 

    void Start()
    {
        if (teksPeringatan != null) teksPeringatan.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (ScoreManager.instance.skorSaatIni >= syaratSkor)
            {
                // SKOR CUKUP: Pindah level!
                SceneManager.LoadScene(namaSceneTujuan);
            }
            else
            {
                // SKOR KURANG: Teleport player balik ke atas
                if (titikKembali != null)
                {
                    collision.transform.position = titikKembali.position;
                }

                // Munculin teks peringatan
                if (teksPeringatan != null)
                {
                    StopAllCoroutines(); // Reset timer kalau dia lompat berkali-kali
                    StartCoroutine(MunculkanPeringatan());
                }
            }
        }
    }

    // Fungsi khusus buat ngatur waktu teks hilang otomatis
    IEnumerator MunculkanPeringatan()
    {
        teksPeringatan.text = "Area Terkunci!\nButuh " + syaratSkor + " Poin untuk turun ke sini.";
        teksPeringatan.gameObject.SetActive(true);
        
        // Tunggu 3 detik
        yield return new WaitForSeconds(3f); 
        
        // Matikan teksnya lagi
        teksPeringatan.gameObject.SetActive(false);
    }
}