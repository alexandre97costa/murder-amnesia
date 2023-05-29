using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool IsPaused;

    public GameObject PauseMenu;

    private void Start()
    {
        PauseMenu.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
            if (IsPaused) { PauseGame(); }
            else { ResumeGame(); }
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        PauseMenu.gameObject.SetActive(true);
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        PauseMenu.gameObject.SetActive(false);
    }

    public bool GetIsPaused()
    {
        return IsPaused;
    }
}
