using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool IsPaused;

    public GameObject PauseMenu;

    public PlayerCamera playerCamera;

    private void Start()
    {
        PauseMenu.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
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
        playerCamera.enabled = false;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        PauseMenu.gameObject.SetActive(false);
        playerCamera.enabled = true;
    }

    public bool GetIsPaused()
    {
        return IsPaused;
    }
}
