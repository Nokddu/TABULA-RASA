using UnityEngine;
using System.Collections;

public class RainController : MonoBehaviour
{
    [Header("Rain 파티클 시스템")]
    [SerializeField] private ParticleSystem rainParticles;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;

        if (rainParticles != null)
        {
            rainParticles.Stop();
        }
    }

    private void Start()
    {
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.ChangeField += StartRain;
        }
    }

    private void OnDestroy()
    {
        StopRain();

        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.ChangeField -= StartRain;
        }
    }

    public void StartRain()
    {
        if (rainParticles == null)
        {
            return;
        }

        UpdateEmitterWidth();
        rainParticles.Play();

        ObjectPool.Instance.Get_Pool_Ambience("Rain", Ambience.Rain);
    }

    public void StopRain()
    {
        if (rainParticles == null) return;

        rainParticles.Stop();

        if (SoundManager.Instance != null && ObjectPool.Instance != null)
        {
            GameObject ambienceToStop = SoundManager.Instance.Remove_Ambience("Rain");
            if (ambienceToStop != null)
            {
                ObjectPool.Instance.Return_To_Ambience("Rain", ambienceToStop);
            }
        }
    }

    private void UpdateEmitterWidth()
    {
        if (mainCamera == null || rainParticles == null) return;

        if (mainCamera.orthographic)
        {
            float cameraHeight = mainCamera.orthographicSize * 2;
            float cameraWidth = cameraHeight * mainCamera.aspect;
            var shape = rainParticles.shape;
            shape.scale = new Vector3(cameraWidth, shape.scale.y, shape.scale.z);
        }
        else
        {

        }
    }
}