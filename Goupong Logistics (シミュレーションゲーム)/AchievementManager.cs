using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SocialPlatforms;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;
    public GameObject[] achievementImage;
    public Text[] achievementtext; 

    public GameObject[] popupAchievement; 
    public GameObject popupWindow; 
    public Animator anim; 

    public GameObject Lisence;
    public float lisenceCount = 0;
    public float tradeCount = 0;
    public float clickCount = 0;

    public float achievement0;
    public float achievement1;
    public float achievement2;
    public float achievement3;
    public float achievement4;
    public float achievement5;

    bool achievement2Active = false;

    void Start()
    {
        lisenceCount = 0;
        tradeCount = 0;
        popupWindow.SetActive(false);

        AchievementSystem achievementSystem = FindObjectOfType<AchievementSystem>();
        if (achievementSystem != null)
        {
            float tempAchievement0 = 0;
            float tempAchievement1 = 0;
            float tempAchievement2 = 0;
            float tempAchievement3 = 0;
            float tempAchievement4 = 0;
            float tempAchievement5 = 0;
            achievementSystem.achievementList.ReturnEntry(ref tempAchievement0, ref tempAchievement1, ref tempAchievement2, ref tempAchievement3, ref tempAchievement4, ref tempAchievement5);
            
            achievement0 = tempAchievement0;
            achievement1 = tempAchievement1;
            achievement2 = tempAchievement2;
            achievement3 = tempAchievement3;
            achievement4 = tempAchievement4;
            achievement5 = tempAchievement5;
        }
    }

    private void Update()
    {
        Dojun();
        UnDojun();
    }

    public void lisenceCountplus()
    {
        lisenceCount++;
    }
    public void tradeCountplus()
    {
        tradeCount++;
    }
    public void clickCountplus()
    {
        clickCount++;
    }

    void Dojun()
    {
        if (tradeCount == 100 && achievement0 == 0)
        {
            achievement0 = 1;
            ActiveDojun(0);
            ReturnImage(0);
            StartCoroutine(Setting());
            AchievementSystem.Instance.achievementList.AddEntry(achievement0, achievement1, achievement2, achievement3, achievement4, achievement5);
            AchievementSystem.Instance.achievementList.SaveAchievements();
        }
        if (lisenceCount == 5 && achievement1 == 0)
        {
            achievement1 = 1;
            ActiveDojun(1);
            ReturnImage(1);            
            StartCoroutine(Setting());
            AchievementSystem.Instance.achievementList.AddEntry(achievement0, achievement1, achievement2, achievement3, achievement4, achievement5);
            AchievementSystem.Instance.achievementList.SaveAchievements();
        }
        if (achievement2 == 1 && achievement2Active == false)
        {
            achievement2Active = true;
            AchievementSystem.Instance.achievementList.AddEntry(achievement0, achievement1, achievement2, achievement3, achievement4, achievement5);
            AchievementSystem.Instance.achievementList.SaveAchievements();
        }
        if (GameManager.Instance.curMonth < 9 && Unlock.instance.AirBT.gameObject.activeSelf && achievement3 == 0)
        {
            achievement3 = 1;
            ActiveDojun(3);
            ReturnImage(3);
            StartCoroutine(Setting());
            AchievementSystem.Instance.achievementList.AddEntry(achievement0, achievement1, achievement2, achievement3, achievement4, achievement5);
            AchievementSystem.Instance.achievementList.SaveAchievements();
        }
        if (clickCount == 12 && achievement4 == 0)
        {
            achievement4 = 1;
            ActiveDojun(4);
            ReturnImage(4);
            StartCoroutine(Setting());
            AchievementSystem.Instance.achievementList.AddEntry(achievement0, achievement1, achievement2, achievement3, achievement4, achievement5);
            AchievementSystem.Instance.achievementList.SaveAchievements();
        }
        if (GameManager.Instance.DisCount == 10 && achievement5 == 0)
        {
            achievement5 = 1;
            ActiveDojun(5);
            ReturnImage(5);
            StartCoroutine(Setting());
            AchievementSystem.Instance.achievementList.AddEntry(achievement0, achievement1, achievement2, achievement3, achievement4, achievement5);
            AchievementSystem.Instance.achievementList.SaveAchievements();
        }
    }

    void UnDojun()
    {
        //for (int i = 0; i < popupAchievement.Length; i++)
        //{
        //    popupAchievement[i].SetActive(false);
        //}        
        if (achievement0 == 1)
        {
            achievementImage[0].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            achievementtext[0].gameObject.SetActive(true);
        }
        if (achievement1 == 1)
        {
            achievementImage[1].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            achievementtext[1].gameObject.SetActive(true);            
        }
        if (achievement2 == 1)
        {
            achievementImage[2].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            achievementtext[2].gameObject.SetActive(true);
        }
        if (achievement3 == 1)
        {
            achievementImage[3].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            achievementtext[3].gameObject.SetActive(true);
        }
        if (achievement4 == 1)
        {
            achievementImage[4].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            achievementtext[4].gameObject.SetActive(true);
        }
        if (achievement5 == 1)
        {
            achievementImage[5].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            achievementtext[5].gameObject.SetActive(true);
        }
    }

    public void ReturnImage(int index)
    {        
        achievementImage[index].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        achievementtext[index].gameObject.SetActive(true);
    }

    public void ActiveDojun(int index)
    {
        popupWindow.SetActive(true);
        anim.SetBool("5Secend", false);
        popupAchievement[index].SetActive(true);
    }

    public IEnumerator Setting()
    {
        yield return new WaitForSecondsRealtime(6f);
        anim.SetBool("5Secend", true);
        yield return new WaitForSecondsRealtime(1f);
        popupWindow.SetActive(false);
        for (int i = 0; i < popupAchievement.Length; i++)
        {
            popupAchievement[i].SetActive(false);
        }
    }
}
