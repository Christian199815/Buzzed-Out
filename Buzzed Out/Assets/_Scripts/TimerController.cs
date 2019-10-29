using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class TimerController : MonoBehaviour
{
    float timer = 0.0f;
    int seconds;
    public Text txt;
    public int NumTime;
    public int PlayTime = 20;
    public string LetTime;
    public AudioClip beepsound;
    public AudioSource Beep;

    void Update()
    {
        timer += Time.deltaTime;
        seconds = (int)(timer % 60);

        AbstractTime();
        DisplayTime();
        LastTenSeconds();
    }

    void AbstractTime()
    {
        NumTime = (PlayTime - seconds);
        LetTime = NumTime.ToString();
    }

    //this Displays the time in the game
    void DisplayTime()
    {
        txt.text = (LetTime);
    }

    //If the timer hit its last 10 seconds a sound is heared
    void LastTenSeconds()
    {
        if(NumTime <= 10)
        {
           
            
        }
    }
}
