using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PickUpSet pickUpSet;
    public UintVariable level;
    public UintVariable playerLifes;
    public FloatVariable playerScores;
    public UintVariable ghostActivated;
    public UintVariable ghostsEaten;
    public UintVariable bonusLifesGet;
    public BoolVariable isGameOvered;

    public float secondsToLevelStart = 3f;
    public float secondsToLevelFinished = 3f;
    public float scoreToGetBonusLife = 10000f;
    public float baseGhostValue = 200f;
    public StringVariable messageText;

    public GameEvent onStartLevel;
    public GameEvent onLevelInProgress;
    public GameEvent onLevelFinished;
    public GameEvent onBonusPickUpActivated;
    public GameEvent onGhostActivation;
    public GameEvent onGameOvered;

    private WaitForSeconds waitToLevelStart;
    private WaitForSeconds waitToLevelFinished;
    private int basePickUpsCollected;
    private int bonusActivations;

    void Start()
    {
        waitToLevelStart = new WaitForSeconds(secondsToLevelStart);
        waitToLevelFinished = new WaitForSeconds(secondsToLevelFinished);       
        RestartGame();
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    private IEnumerator StartLevel()
    {
        onStartLevel.Rise();
        ReactivatePickUps();
        basePickUpsCollected = 0;
        bonusActivations = 0;
        ghostsEaten.runtimeValue = ghostsEaten.initialValue;
        isGameOvered.runtimeValue = isGameOvered.initialValue;
        ghostActivated.runtimeValue = ghostActivated.initialValue;
        messageText.runtimeValue = "READY!";
        yield return waitToLevelStart;
        LevelInProgress();
        
    }

    private void LevelInProgress()
    {
        onLevelInProgress.Rise();      
        messageText.runtimeValue = "";
        ActivateGhosts();
    }

    private IEnumerator LevelFinished()
    {
        onLevelFinished.Rise();
        level.runtimeValue++;
        messageText.runtimeValue = "FINISHED!";
        yield return waitToLevelFinished; 
        StartCoroutine(StartLevel());
    }

    private IEnumerator ResumeLevelAfterPlayerDeath()
    {
        onStartLevel.Rise();
        ghostsEaten.runtimeValue = ghostsEaten.initialValue;
        messageText.runtimeValue = "READY!";
        yield return waitToLevelStart;
        LevelInProgress();
    }

    public void RestartGame()
    {
        level.runtimeValue = level.initialValue;
        playerLifes.runtimeValue = playerLifes.initialValue;
        playerScores.runtimeValue = playerScores.initialValue;
        bonusLifesGet.runtimeValue = bonusLifesGet.initialValue;
        isGameOvered.runtimeValue = false;
        StartCoroutine(StartLevel());
    }    


    public void BasePickUpCollected()
    {
        basePickUpsCollected++;
        CheckBonusActivation();
        CheckGhostActivation();
        CheckForBonusLife();
        CheckLevelFinished();
    }

    public void OnPlayerKilled()
    {
        if (playerLifes.runtimeValue == 0)
        {
            isGameOvered.runtimeValue = true;
            onGameOvered.Rise(); 
        }
        else
        {
            playerLifes.runtimeValue--;
            StartCoroutine(ResumeLevelAfterPlayerDeath());
        }
    }

    public void OnGhostEaten()
    {
        ghostsEaten.runtimeValue++;
        playerScores.runtimeValue += baseGhostValue * Mathf.Pow(2,ghostsEaten.runtimeValue - 1);
        CheckForBonusLife();
    }

    public void OnEnergizerEnd()
    {
        ghostsEaten.runtimeValue = ghostsEaten.initialValue;
    }

    private void ReactivatePickUps()
    {
        for (int i = 0; i < pickUpSet.items.Count; i++)
        {
            pickUpSet.items[i].gameObject.SetActive(true);
        }
    }

    private void CheckLevelFinished()
    {     
        bool isAllPickupsInActive = true;
        for (int i = 0; i < pickUpSet.items.Count; i++)
        {
            if (pickUpSet.items[i].gameObject.activeSelf == true)
            {
                isAllPickupsInActive = false;
                break;
            }
        }

        if (isAllPickupsInActive)
        {
            StartCoroutine(LevelFinished());         
        }
    }

    private void CheckBonusActivation()
    {
        if (bonusActivations > 1)
        {
            return;
        }

        switch(bonusActivations)
        {
            case 0:
                if (basePickUpsCollected >= 50)
                {
                    onBonusPickUpActivated.Rise();
                    bonusActivations++;
                }
                break;
            case 1:
                if (basePickUpsCollected >= 100)
                {
                    onBonusPickUpActivated.Rise();
                    bonusActivations++;
                }
                break;
        } 

    }

    public void CheckForBonusLife()
    {     
        while ((playerScores.runtimeValue - scoreToGetBonusLife*(bonusLifesGet.runtimeValue + 1)) >= 0)
        {          
            playerLifes.runtimeValue++;
            bonusLifesGet.runtimeValue++;
        }
    }

    private void CheckGhostActivation()
    {
        if (ghostActivated.runtimeValue > 3)
        {
            return;
        }

        switch (ghostActivated.runtimeValue)
        {
            case 0:
                ghostActivated.runtimeValue++;
                onGhostActivation.Rise();                
                break;
            case 1:
                if (basePickUpsCollected >= 20)
                {
                    ghostActivated.runtimeValue++;
                    onGhostActivation.Rise();                    
                }
                break;
            case 2:
                if (basePickUpsCollected >= 40)
                {
                    ghostActivated.runtimeValue++;
                    onGhostActivation.Rise();                    
                }
                break;
            case 3:
                if (basePickUpsCollected >= 70)
                {
                    ghostActivated.runtimeValue++;
                    onGhostActivation.Rise();                 
                }
                break;
        }

    }

    private void ActivateGhosts()
    {
        if (ghostActivated.runtimeValue == 0)
        {
            ghostActivated.runtimeValue++;
            onGhostActivation.Rise();
        }
        else
        {
            uint ghostActivatedTemp = ghostActivated.runtimeValue;
            ghostActivated.runtimeValue = ghostActivated.initialValue;
            for (int i = 0; i < ghostActivatedTemp; i++)
            {
                ghostActivated.runtimeValue++;
                onGhostActivation.Rise();
            }
        }
    }
    

}
