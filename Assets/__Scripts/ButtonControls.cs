using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonControls : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1;
        Main.S.startButton.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("_Scene_0");
    }
}
