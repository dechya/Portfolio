using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.SocialPlatforms;

public class GameManager : MonoBehaviour
{           
    public static GameManager Instance;
        
    public enum Weather
    {
        Sunny,
        Rain,
        Drought,
        Snow
    }
    public Weather weather;

    [Header("재화시스템")]
    public Text moneyText;
    public int money;
    [Header("시간제한")]   
    public Slider timeSlider;
    int maxvalue = 30;
    [HideInInspector] public float timeSliderval;
    public bool TimerOn = false;
    [HideInInspector] public float time;
    [HideInInspector] public int curtime;
    [HideInInspector] public int maxtime = 30;
    [HideInInspector] public int limitMonth;
    [HideInInspector] public bool coroutineRunning = false;
    [HideInInspector] public int curMonth = 1;    
    [HideInInspector] public bool Dis;
    public int DisCount;
    public bool isTime;
    int dismoney;

    [Header("플레이어 적재량")]
    public PlayerProperties playerProperties;
    public GameObject RankingChang; 

    [Header("배속 시스템")]
    public Image[] speedSp;
    [HideInInspector] public float b;
    int a;
    
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main; 
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        RankingChang.SetActive(false);
        PlayerPrefs.DeleteAll();
        money = PlayerPrefs.GetInt("money", 800000);
        Show(0);
        b = Time.timeScale = 1f;
        isTime = false;
        DisCount = 0;
    }

    void Update()
    {
        PlayerPrefs.SetInt("자금(임시)", money);
        moneyText.text = money.ToString("N0");
        
        mainCamera.transform.position = new Vector3((Input.mousePosition.x - 1920f / 2) / (1920f / 2) * 0.05f, 
                                                    (Input.mousePosition.y - 1080f / 2) / (1080f / 2) * 0.05f, 
                                                     -10);
    }

    public void TimeTest()
    {
        if (TimerOn == true && !coroutineRunning && isTime == true)
       {
           
           if (timeSliderval < maxvalue)
           {
                CancelInvoke("DisMoney");
                if(playerProperties.player == Player.Truck)
                {
                    timeSliderval += ((playerProperties.currentPlayerSpeed / playerProperties.currentPlayerSpeed) * (Time.deltaTime * 0.4f));
                    timeSlider.value = timeSliderval;
                }
                if (playerProperties.player == Player.Ship)
                {
                    timeSliderval += ((playerProperties.currentPlayerSpeed / playerProperties.currentPlayerSpeed) * (Time.deltaTime * 0.5f));
                    timeSlider.value = timeSliderval;
                }
                if (playerProperties.player == Player.Airplane)
                {
                    timeSliderval += ((playerProperties.currentPlayerSpeed / playerProperties.currentPlayerSpeed) * (Time.deltaTime * 1f));
                    timeSlider.value = timeSliderval;
                }
                DisCount = 0;
                dismoney = 2000;
            }
           else
           {
                coroutineRunning = true;
                Dis = true;
                InvokeRepeating("DisMoney", 0f, 3f);
                StartCoroutine(Wait());
           }
       }
    }

    public void DisMoney()
    {        
        if (Dis == true)
        {
            if (money > 0)
            {        
                money -= dismoney;
                dismoney += 1000;
                MoneyChange();
                DisCount += 1;
            }
        }
    }

    public void MoneyChange()
    {
        var animator = moneyText.GetComponent<Animator>();
        animator.Play("재화", 0, 0);
        SoundManager.Instance.PlaySFX("돈소리");
    }

    IEnumerator Wait()
    {                                 
        yield return null;
        curMonth += 1;
        timeSliderval = 0;
        timeSlider.value = timeSliderval;                                                    
    }

    public void TimeVal()
    {
        timeSliderval = 0;
        timeSlider.value = timeSliderval;
    }

    public void Ranking()
    {
        if(curMonth == 13)
        {
            Debug.Log("Ranking");
            RoadMap.Instance.chang[0].gameObject.SetActive(false);
            RoadMap.Instance.chang[1].gameObject.SetActive(false);
            Time.timeScale = 0f;
            RankingChang.SetActive(true);
            RandomImage.Instance.RanCreate();
            curMonth = 1;        
        }
    }
    
    public void RankingExit()
    {
        RankingChang.SetActive(false);
        RandomImage.Instance.count = 0;
    }

    public void MainScnene()
    {
#if UNITY_EDITOR
        SceneManager.LoadScene(0);
#else
        SceneManager.LoadScene(0);
#endif
    }

    public void SpeedCtrl()
    {
        SoundManager.Instance.PlaySFX("메뉴클릭");
        a++;        
        if(a == 1)
        {
            Show(1);
            b = Time.timeScale = 2f;
        }
        if(a == 2)
        {
            Show(2);
            b = Time.timeScale = 4f;
        }
        if(a > 2)
        {
            a = 0;
            Show(0);
            b = Time.timeScale = 1f;
        }
    }

    void Show(int Index)
    {
        for (int limitMonth = 0; limitMonth < speedSp.Length; limitMonth++)
        {
            speedSp[limitMonth].gameObject.SetActive(limitMonth == Index);
        }
    }

    IEnumerator TimeEx()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            curtime++;
        }        
    }
}