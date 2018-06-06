using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameEvent onGameRestart;
    public BoolVariable isGamePaused;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnGameOvered()
    {
        isGamePaused.runtimeValue = true;
        Time.timeScale = 0f;        
    }

    public void Restart()
    {
        isGamePaused.runtimeValue = false;
        Time.timeScale = 1f;
        onGameRestart.Rise();    
    }

    public void LoadMenu()
    {
        isGamePaused.runtimeValue = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

}
