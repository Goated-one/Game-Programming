using UnityEngine;
using System; // Wajib ditambahin biar bisa nyari nama sound

// Ini "cetakan" biar lu bisa ngatur volume masing-masing suara langsung di Inspector
[Serializable]
public class Sound
{
    public string namaEfek; // Contoh: "Lompat", "Nembak", "Melee"
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Mesin Pemutar Suara")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Background Music")]
    public AudioClip laguBGM;

    [Header("Daftar Sound Effect (SFX)")]
    public Sound[] daftarSFX;

    void Awake()
    {
        // Sistem Singleton: Biar bisa dipanggil dari script manapun
        if (instance == null) 
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // Hapus tanda '//' di depan kalau mau BGM nggak mati pas pindah scene
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Otomatis muter BGM pas game mulai
        if (laguBGM != null)
        {
            bgmSource.clip = laguBGM;
            bgmSource.loop = true; // Lagunya muter terus (looping)
            bgmSource.Play();
        }
    }

    // Fungsi sakti buat muter SFX
    public void PlaySFX(string nama)
    {
        Sound s = Array.Find(daftarSFX, x => x.namaEfek == nama);
        
        if (s != null)
        {
            sfxSource.PlayOneShot(s.clip, s.volume);
        }
        else
        {
            Debug.LogWarning("Waduh, sound '" + nama + "' nggak ketemu! Cek lagi ketikannya bro.");
        }
    }
}