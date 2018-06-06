using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusPickUpManager : MonoBehaviour
{
    public UintVariable level;
    public FloatVariable score;
    public MeshSet meshSet;
    public float baseScoreValue = 100f;
    public float secondsToDeactivation = 5f;
    public GameEvent onBonusPickUpTaken;

    private MeshFilter meshFilter;
    private WaitForSeconds waitToDeactivation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            score.runtimeValue += baseScoreValue + 200 * (level.runtimeValue - 1);
            onBonusPickUpTaken.Rise();
            gameObject.SetActive(false);
        }     
    } 

    public void OnLevelChanged()
    {        
        Mesh mesh;
        if (level.runtimeValue > 3)
        {
            mesh = meshSet.items[meshSet.items.Count - 1];
        }
        else
        {
            mesh = meshSet.items[(int)level.runtimeValue - 1];
        }     
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();    
        }
  
        meshFilter.mesh = mesh;

    }

    public void OnBonusActivation()
    {
        if (waitToDeactivation == null)
        {
            waitToDeactivation = new WaitForSeconds(secondsToDeactivation);      
        }
  
        StartCoroutine(Deactivate());
    }

    private IEnumerator Deactivate()
    {
        yield return waitToDeactivation;
        gameObject.SetActive(false);
    }
}
