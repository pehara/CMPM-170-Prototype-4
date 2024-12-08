using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private float timerValue = 0;

    // Update is called once per frame
    void Update()
    {
        countUpTimer();
    }

    private void countUpTimer() {
        timerValue += Time.deltaTime;
        timerText.text = "Timer: " + Convert.ToInt32(timerValue);
    }
}
