using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lanternLife : MonoBehaviour
{
    public Light lanternLight;

    private float initialIntensity;
    private float targetIntensity = 0.0f;
    [SerializeField] private float durationWithoutCollision = 90.0f; 
    [SerializeField] private float durationWithCollision = 2.0f;    

    private float elapsedTime = 0.0f;
    private bool collisionOccurred = false;

    void Start()
    {
        initialIntensity = lanternLight.intensity;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t;

        if (collisionOccurred)
        {
            t = elapsedTime / durationWithCollision; //충돌이 있을 때 증가
            lanternLight.intensity = Mathf.Lerp(initialIntensity, 1.0f, t);
        }
        else
        {
            t = elapsedTime / durationWithoutCollision;
            lanternLight.intensity = Mathf.Lerp(initialIntensity, targetIntensity, t);
        }

        if (t >= 1.0f)
            lanternLight.intensity = collisionOccurred ? 1.0f : targetIntensity;
            enabled = false;

        if(lanternLight.intensity <=0f)
        {
            Debug.Log("Game Over");
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Candle")
        {
            targetIntensity = 1.0f;
            collisionOccurred = true;
            elapsedTime = 0.0f;
        }
    }
}
