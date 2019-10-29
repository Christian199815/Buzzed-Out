using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private TextMeshProUGUI m_timerTextobject;
    [SerializeField] private GameObject m_antsHaveWonCanvas;
    [SerializeField] private GameObject m_SwatterHasWonCanvas;
    [Header("Game Settings")]
    [SerializeField] private int m_minutesToPlay;
    [SerializeField] private int m_secondsToPlay;

    private List<Ant> ants = new List<Ant>();

    private int m_secondsLeft = 0;
    private bool m_gamePaused = false;

    private void Start()
    {
        if (m_secondsToPlay >= 60)
        {
            Debug.LogError("GameManager: secondsToPlay >= 60, the seconds were were converted to minutes");

            int secondsToPlay = m_secondsToPlay;
            while (secondsToPlay >= 60)
            {
                secondsToPlay -= 60;
                m_minutesToPlay += 1;
            }
            m_secondsLeft = (m_minutesToPlay * 60) + secondsToPlay;
        }
        else
        {
            m_secondsLeft = (m_minutesToPlay * 60) + m_secondsToPlay;
        }

        StartCoroutine(CountDownTimer());
    }

    private IEnumerator CountDownTimer()
    {
        float previousSecond = 0;

        while(m_secondsLeft > 0)
        {
            if (!m_gamePaused)
            {
                if (Mathf.RoundToInt(Time.time) - 1 >= previousSecond)
                {
                    RetractSecondFromTimer();
                    previousSecond = Mathf.RoundToInt(Time.time);
                }
            }
            yield return null;
        }
    }

    private void RetractSecondFromTimer()
    {
        m_secondsLeft -= 1;
        UpdateTimerText();

        if(m_secondsLeft <= 0)
        {
            TimerReachedZero();
        }
    }

    private void UpdateTimerText()
    {
        string textToInput = string.Empty; // make the string for the timer text that will be edited

        textToInput += Mathf.RoundToInt(m_secondsLeft / 60); // add the minutes to the timer text

        textToInput += ":"; // add the ":" to the timer text

        int secondsLeft = m_secondsLeft; 
        while(secondsLeft >= 60) // calculate the seconds left after minutes have been removed
        {
            secondsLeft -= 60;
        }
        textToInput += secondsLeft; // add the seconds to the timer text
        m_timerTextobject.text = textToInput; // set the text
    }

    private void TimerReachedZero()
    {
        AntsHaveWon();
        print("ants win, timer <= 0");
    }

    private void AntsHaveWon()
    {
        m_antsHaveWonCanvas.SetActive(true);
        PauseGame();
    }
    private void SwatterHasWon()
    {
        PauseGame();
        m_SwatterHasWonCanvas.SetActive(true);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        m_gamePaused = true;
    }
    private void UnPauseGame()
    {
        Time.timeScale = 1;
        m_gamePaused = false;
    }

    public bool GetGamePaused()
    {
        return m_gamePaused;
    }

    public void AddAntToList(Ant _ant)
    {
        ants.Add(_ant);
    }

    public void AntDied(Ant _ant)
    {
        ants.Remove(_ant);

        if(ants.Count <= 0)
        {
            print("swatter won");
            SwatterHasWon();
        }
    }
}