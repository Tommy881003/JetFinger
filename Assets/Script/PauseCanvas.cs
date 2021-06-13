using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseCanvas : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup group;
    private bool paused = false;

    private void Awake()
    {
        group.alpha = 0;
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            UpdateCanvas();
    }

    private void UpdateCanvas()
    {
        paused = !paused;
        group.alpha = (paused) ? 1 : 0;
        Time.timeScale = (paused)? 0 : 1;
        AudioListener.pause = paused;
    }
}
