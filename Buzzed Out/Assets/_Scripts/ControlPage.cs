using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPage : MonoBehaviour
{
    [SerializeField] private string ToControl;
    [SerializeField] private GameObject ControlsPage;
    [SerializeField] private GameObject HomePage;

    public void ControlClick()
    {
        if(ToControl == "Yes")
        {
            Debug.Log("Check");
            HomePage.SetActive(false);
            ControlsPage.SetActive(true);
        }
        else if(ToControl == "No")
        {
            Debug.Log("Checked");
            ControlsPage.SetActive(false);
            HomePage.SetActive(true);
        }
        else
        {

        }
    }
}
