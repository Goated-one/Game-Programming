using UnityEngine;
using Unity.Cinemachine;

public class ScreenShakeManager : MonoBehaviour
{
    public static ScreenShakeManager instance;
    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void ShakeCamera(float kekuatan = 1f)
    {
        if (impulseSource != null)
        {
            // Ini bakal ngedorong kamera sesuai Default Velocity yang lu atur di Inspector
            impulseSource.GenerateImpulseWithForce(kekuatan);
        }
    }
}