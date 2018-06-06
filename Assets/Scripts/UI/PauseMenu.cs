using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public BoolVariable isGamePaused;
    public BoolVariable isGameOvered;
    public GameEvent onGamePaused;
    public GameEvent onGameResumed;

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isGameOvered.runtimeValue)
            {
                if (isGamePaused.runtimeValue)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }  
        }
		
	}

    public void Resume()
    {
        isGamePaused.runtimeValue = false;
        Time.timeScale = 1f;
        onGameResumed.Rise();
    }

    public void Pause()
    {
        isGamePaused.runtimeValue = true;
        Time.timeScale = 0f;
        onGamePaused.Rise();
    }

    public void LoadMenu()
    {      
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

}
