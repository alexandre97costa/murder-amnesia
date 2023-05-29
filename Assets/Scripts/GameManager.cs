using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool IsPaused;

    public GameObject PauseMenu;

    public PlayerCamera playerCamera;

    private void Start()
    {
        PauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1;
        playerCamera.enabled = true;
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

    public void PauseGame()
    {
        Time.timeScale = 0;
        PauseMenu.gameObject.SetActive(true);
        playerCamera.enabled = false;
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        PauseMenu.gameObject.SetActive(false);
        playerCamera.enabled = true;
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    public void GoMenu()
    {
         SceneManager.LoadScene("MenuStart");
    }

    public bool GetIsPaused()
    {
        return IsPaused;
    }
}
