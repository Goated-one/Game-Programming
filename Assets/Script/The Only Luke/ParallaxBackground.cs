using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    
    [Header("Pengaturan Parallax")]
    public float efekParallax; 

    [Header("Sensor Jarak (Biar Gak Kabur)")]
    [Tooltip("Isi 9999 buat langit atas. Isi 30 buat underground.")]
    public float jarakAktif = 9999f; 

    private float posisiAsliX;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        
        // Simpan posisi awal gambar pas game baru mulai
        posisiAsliX = transform.position.x;
    }

    void LateUpdate()
    {
        Vector3 pergerakanKamera = cameraTransform.position - lastCameraPosition;
        
        // Cek apakah kamera udah masuk ke radius area gambar ini
        if (Mathf.Abs(cameraTransform.position.x - posisiAsliX) < jarakAktif)
        {
            // Kalau kamera udah deket, baru efek parallax-nya nyala
            transform.position += new Vector3(pergerakanKamera.x * efekParallax, 0, 0);
        }
        
        // Posisi terakhir kamera wajib di-update terus walaupun gambarnya lagi diam
        lastCameraPosition = cameraTransform.position;
    }
}