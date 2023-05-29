using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnd : MonoBehaviour
{
    public GameObject GameEndUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collisionInfo) {
        if (collisionInfo.collider.CompareTag("Player")) {
            GameEndUI.SetActive(true);

            Invoke(nameof(GoToMenu), 3);
        }
    }

    void GoToMenu() {
        SceneManager.LoadScene("MenuStart");
    }
}
