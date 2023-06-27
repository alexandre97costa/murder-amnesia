using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ChangeScene : MonoBehaviour
{
    [Header("Choose Scene")]
    public ListScene sceneSelect;
    private List<string> listScene = new List<string> {
        "Cha1_Lvl2", "Cha1_Lvl3", "Cha2_Lvl1", "Cha2_Lvl2", "End", "DemoTutorial", "TesteCutScene"
    };

    [Space(10)]
    [Header("CutScene")]
    public VideoPlayer cutScene;

    public enum ListScene
    {
        Cha1_Lvl2 = 0,
        Cha1_Lvl3 = 1,
        Cha2_Lvl1 = 2,
        Cha2_Lvl2 = 3,
        End = 4,
        DemoTutorial = 5,
        TesteCutScene = 6,
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
            ChangeSceneManager();
    }

    void ChangeSceneManager()
    {
        SceneManager.LoadScene(listScene[(int)sceneSelect]);
    }

    private void Start()
    {
        cutScene.loopPointReached += VideoPlayer_LoopPointReached;
    }

    private void VideoPlayer_LoopPointReached(VideoPlayer source)
    {
        ChangeSceneManager();
    }
}
