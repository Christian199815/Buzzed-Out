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
    [SerializeField] private ItemPool m_pool;
    [SerializeField] private BoxCollider SpawnFieldForPickups;
    [Header("PoolitemObjects")]
    [SerializeField] private GameObject m_healthBean;
    [SerializeField] private GameObject m_moveSpeedBean;

    [Header("Game Settings")]
    [SerializeField] private int m_minutesUntilSwatterWins;
    [SerializeField] private int m_secondsUntilSwatterWins;

    private List<Ant> ants = new List<Ant>();

    private int m_timeLeftInSeconds = 0;
    private bool m_gamePaused = false;

    private void Start()
    {
        if (m_secondsUntilSwatterWins >= 60)
        {
            Debug.LogError("GameManager: secondsToPlay >= 60, the seconds were were converted to minutes");

            int secondsToPlay = m_secondsUntilSwatterWins;
            while (secondsToPlay >= 60)
            {
                secondsToPlay -= 60;
                m_minutesUntilSwatterWins += 1;
            }
            m_timeLeftInSeconds = (m_minutesUntilSwatterWins * 60) + secondsToPlay;
        }
        else
        {
            m_timeLeftInSeconds = (m_minutesUntilSwatterWins * 60) + m_secondsUntilSwatterWins;
        }
        UnPauseGame();
        StartCoroutine(CountDownTimer());

        SpawnBean(m_healthBean.GetComponent<PoolItem>());
        SpawnBean(m_healthBean.GetComponent<PoolItem>());
        SpawnBean(m_moveSpeedBean.GetComponent<PoolItem>());
        SpawnBean(m_moveSpeedBean.GetComponent<PoolItem>());
        SpawnBean(m_moveSpeedBean.GetComponent<PoolItem>());
    }

    private IEnumerator CountDownTimer()
    {
        float previousSecond = 0;

        while(m_timeLeftInSeconds > 0)
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
        m_timeLeftInSeconds -= 1;
        UpdateTimerText();

        if(m_timeLeftInSeconds <= 0)
        {
            TimerReachedZero();
        }
    }

    private void UpdateTimerText()
    {
        string textToInput = string.Empty; // make the string for the timer text that will be edited

        textToInput += Mathf.RoundToInt(m_timeLeftInSeconds / 60); // add the minutes to the timer text

        textToInput += ":"; // add the ":" to the timer text

        int secondsLeft = m_timeLeftInSeconds; 
        while(secondsLeft >= 60) // calculate the seconds left after minutes have been removed
        {
            secondsLeft -= 60;
        }

        if(secondsLeft < 10)
        {
            textToInput += "0";
        }

        textToInput += secondsLeft; // add the seconds to the timer text
        m_timerTextobject.text = textToInput; // set the text

        if(secondsLeft == 0)
        {
            print("call");
            StartCoroutine(BounceTimerScale(3, 1, 1.3f));
        }

        if(m_timeLeftInSeconds == 10)
        {
            m_timerTextobject.CrossFadeColor(new Color(220, 20, 60, 1), 1, false, false);
            StartCoroutine(BounceTimerScale(10, 1, 1.5f));
        }
    }

    /// <param name="_amountOfTimes">how many times will the text bounce</param>
    /// <param name="_timePerBounce">in how much time will the text grow bigger and smaller (bigger AND smalled in "_timePerBounce")</param>
    /// <param name="_increaseFactor">how much bigger should the text become</param>
    /// <returns></returns>
    private IEnumerator BounceTimerScale(int _amountOfTimes, float _timePerBounce, float _increaseFactor = 1.3f)
    {
        for (int i = 0; i < _amountOfTimes; i++)
        {
            Vector3 originalScale = m_timerTextobject.gameObject.transform.localScale;
            Vector3 newScale = Vector3.zero;
            int phase = 0;
            float timer = 0;
            bool completedBounce = false;

            while (!completedBounce)
            {
                switch (phase)
                {
                    case 0:
                        m_timerTextobject.gameObject.transform.localScale = Vector3.Lerp(originalScale, originalScale * _increaseFactor, timer * 2f); // multiply by 2, timer goes to 0.5 because it goes to "_timePerBounce" halved
                        break;
                    case 1:
                        m_timerTextobject.gameObject.transform.localScale = Vector3.Lerp(newScale, originalScale, timer * 2); // multiply by 2, timer goes to 0.5 because it goes to "_timePerBounce" halved
                        break;
                    case 2:
                        completedBounce = true;
                        m_timerTextobject.gameObject.transform.localScale = originalScale;
                        break;
                }
                timer += Time.deltaTime;
                if (timer >= ((float)_timePerBounce / 2f))
                {
                    timer = 0;
                    phase += 1;
                    newScale = m_timerTextobject.gameObject.transform.localScale;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void SpawnBean(PoolItem beanToSpawn)
    {
        Vector3 pos = CreatePositionForBean();

        m_pool.ItemInstatiate(beanToSpawn, pos, Quaternion.identity);
    }

    private Vector3 CreatePositionForBean()
    {
        return new Vector3(
            Random.Range(SpawnFieldForPickups.bounds.min.x, SpawnFieldForPickups.bounds.max.x),     // x
            15,                                                                                     // y
               Random.Range(SpawnFieldForPickups.bounds.min.z, SpawnFieldForPickups.bounds.max.z)   // z                                                                     // z
        );
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