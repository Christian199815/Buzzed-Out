using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private int SceneNumber;
   public void OnButtonClick()
    {
        SceneManager.LoadScene(SceneNumber);
    }
   
}
