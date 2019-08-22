using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct DateTime
{
    public int minutes, hours, day, month, year, season;
}


public class WorldInfo : MonoBehaviour
{
    enum seasons { winter, spring, summer, autumn};
    public float sunMultiplier = 1f, mineralMultiplier = 1f;
    private float slSunMult = 1f, slMinMult = 1f, tSunMult = 1f, tMinMult = 1f, sSunMult = 1f, sMinMult = 1f;
    private int currentStep;
    private DateTime time;
    public void SliderSunChange(float newValue)
    {
        slSunMult = newValue;
    }
    public void SliderMineralChange(float newValue)
    {
        slMinMult = newValue;
    }


    void Start()
    {
        Screen.fullScreen = false;
        currentStep = 0;
        time.minutes = 0;
        time.hours = 6;
        time.day = 1;
        time.month = 6;
        time.season = 2;
        time.year = 0;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene("test");
    }

    int getSeason(int month)
    {
        if (month == 12 || month <= 2) return (int)seasons.winter;
        if (month >= 3 || month <= 5) return (int)seasons.spring;
        if (month >= 6 || month <= 8) return (int)seasons.summer;
        if (month >= 9 || month <= 11) return (int)seasons.autumn;
        return 0;
    }

    void FixedUpdate()
    {
        currentStep++;
        time.minutes++;
        if (time.minutes >= 60)
        {
            time.minutes = 0;
            time.hours++;
        }
        if (time.hours >= 24)
        {
            time.hours = 0;
            time.day++;
        }
        if (time.day >= 31)
        {
            time.day = 1;
            time.month++;
            time.season = getSeason(time.month);
        }
        if (time.month >= 13)
        {
            time.month = 1;
            time.season = getSeason(time.month);
            time.year++;
        }


        if (time.hours >= 6 && time.hours <= 17)
        {
            tSunMult = 1f;
            tMinMult = 1f;
        }
        else if (time.hours >= 4 && time.hours < 6 || time.hours >= 18 && time.hours < 21)
        {
            tSunMult = 0.5f;
            tMinMult = 1f;
        }
        else
        {
            tSunMult = 0.25f;
            tMinMult = 1f;
        }

        if (time.season == (int)seasons.summer)
        {
            sSunMult = 1.3f;
            sMinMult = 1f;
        }
        else if (time.season == (int)seasons.spring)
        {
            sSunMult = 1f;
            sMinMult = 1f;
        }
        else if (time.season == (int)seasons.autumn)
        {
            sSunMult = 0.8f;
            sMinMult = 1.3f;
        }
        else
        {
            sSunMult = 0.5f;
            sMinMult = 1f;
        }

        sunMultiplier = tSunMult * sSunMult * slSunMult;
        mineralMultiplier = tMinMult * sMinMult * slMinMult;
    }

    public int getInfo(string type)
    {
        switch (type)
        {
            case "minutes": return time.minutes;
            case "hours": return time.hours;
            case "day": return time.day;
            case "month": return time.month;
            case "year": return time.year;
            case "season": return time.season;
            case "currentStep": return currentStep;
            default: return 0;
        }
    }
}
