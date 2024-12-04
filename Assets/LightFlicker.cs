using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light playerLight;
    public float minIntensity = 1.2f;
    public float maxIntensity = 1.8f;
    public float flickerSpeed = 0.1f;

    void Update()
    {
        if (playerLight != null)
        {
            playerLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(Time.time * flickerSpeed, 1));
        }
    }
}
