using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class HotelCine : MonoBehaviour
{
    public Image fade_in;

    //public CinemachineVirtualCamera cinemachineCamera;
    //public Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        
        //cinemachineCamera.gameObject.SetActive(true);
        //mainCamera.gameObject.SetActive(true);

        StartCoroutine(GoToHotel());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GoToHotel()
    {
        yield return new WaitForSeconds(3f);
        float targetAlpha = 1.0f; 
        float duration = 2.0f;
        float elapsedTime = 0.0f;

        
        Color color = fade_in.color;
        color.a = 0.0f;
        fade_in.color = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration); 
            color.a = Mathf.Lerp(0, targetAlpha, t); 
            fade_in.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(1.3f);
        SceneManager.LoadScene("Hotel");    
    }
}

