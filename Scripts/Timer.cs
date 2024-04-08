using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public int hours = 12;
    public float minutes = 0;
    public Text timerText;
    private string s;

    // Start is called before the first frame update
    void Start()
    {
        s = hours.ToString() + ":" + minutes.ToString();
        timerText.text = s;
    }

    // Update is called once per frame
    void Update()
    {
        minutes += Time.deltaTime;
        if (minutes >= 60)
        {
            minutes = 0;
            hours++;
            if (hours > 23) { hours = 0;}
        }
        if (minutes > 9)
        s = hours.ToString() + ":" + Mathf.Round(minutes).ToString();
        else s = hours.ToString() + ":0" + Mathf.Round(minutes).ToString();
        timerText.text = s;
    }
}
