using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    private float elapsedTime;
    //public bool stopTimer;

    void Update()
    {
        //Timer to show how long a player stayed on a level
        elapsedTime += Time.deltaTime;
        timerText.text = elapsedTime.ToString("00:00.00");
    }
}
