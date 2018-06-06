using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostManager : MonoBehaviour
{    
    public GameObject targetObject;
    public GameObject ghostCageObject;
    public int ghostID;
    public UintVariable ghostsActivated;
    public GameObject startObject;
    public GameEvent onPlayerKilled;
    public GameEvent onGhostEaten;
    public Mesh scaredMesh;
    public Mesh eatenMesh;
    public Material[] materials;
    public float choosePointIntervalWhenScared = 3f;
    public float blinkIntervalWhenEnergizedNearEnd = 0.1f;   

    private NavMeshAgent navMeshAgent;
    private Vector3 startPosition;
    private Vector3 ghostCagePosition;
    private Quaternion startRotation;
    private bool isActive;
    private bool isScared;
    private bool isEaten;
    private Mesh defaultMesh;
    private Renderer objectRenderer;
    private int currentMaterialIndex;
    
    private NavMeshTriangulation navMeshTriangulation;
    private Vector3[] navMeshVertices;
    private WaitForSeconds waitToChoosePoint;
    private WaitForSeconds waitToBlinkWhenEnergizedNearEnd;

    public 

    // Use this for initialization
    void Awake ()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (startObject != null)
        {
            startPosition = startObject.transform.position;
            startRotation = startObject.transform.rotation;
        }
        if (ghostCageObject != null)
        {
            ghostCagePosition = ghostCageObject.transform.position;     
        }

        isActive = false;
        isScared = false;
        isEaten = false;
        MeshFilter meshFilter = GetComponent<MeshFilter>();       
        if (meshFilter != null)
        {
            defaultMesh = meshFilter.mesh;    
        }
        objectRenderer = GetComponent<Renderer>();
        navMeshTriangulation = NavMesh.CalculateTriangulation();
        navMeshVertices = navMeshTriangulation.vertices;
        waitToChoosePoint = new WaitForSeconds(choosePointIntervalWhenScared);
        waitToBlinkWhenEnergizedNearEnd = new WaitForSeconds(blinkIntervalWhenEnergizedNearEnd);
        currentMaterialIndex = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (navMeshAgent != null && isActive && !isScared && !isEaten)
        {
            navMeshAgent.SetDestination(targetObject.transform.position);
        }
    }
  

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GhostCage")
        {
            if (isEaten)
            {               
                ResetScared();
                ResetEaten();
            }           
        }

        if (other.gameObject.tag == "Player")
        {
            if (isEaten)
            {             
                return;
            }
            if (isScared)
            {
                Eaten();
            }
            else
            {
                onPlayerKilled.Rise();
            }

        }
    }

    public void OnGhostActivation()
    {
        if (ghostsActivated.runtimeValue == ghostID)
        {         
            Activate();
        }
    }

    public void OnGhostDeactivation()
    {
        Deactivate();
    }

    public void Activate()
    {
        navMeshAgent.enabled = true;
        isActive = true;  
    }

    public void Deactivate()
    {
        isActive = false;
        navMeshAgent.enabled = false;
        ResetEaten();
        ResetScared();
        ResetTransform();
    }

    public void ResetTransform()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    public void Scared()
    {
        if (!isEaten)
        {
            isScared = true;
            ChangeMesh(scaredMesh);
            StartCoroutine("ChooseRandomPointOnNavMesh");
        }   

    }

    public void ResetScared()
    {
        isScared = false;
        if (!isEaten)
        {
            ChangeMesh(defaultMesh);
        }     
        ChangeMaterial(materials[0]);
        StopCoroutine("ChooseRandomPointOnNavMesh");
        StopCoroutine("BlinkMaterial");
    }

    private IEnumerator ChooseRandomPointOnNavMesh()
    {
        while (isScared)
        {
            if (isActive)
            {
                int randomVertice = Random.Range(0, navMeshVertices.Length - 1);
                if (navMeshAgent.enabled)
                {
                    navMeshAgent.SetDestination(navMeshVertices[randomVertice]);
                }
                
            }
            yield return waitToChoosePoint;
        }
    }

    private void Eaten()
    {        
        onGhostEaten.Rise();
        isEaten = true;
        ResetScared();
        ChangeMesh(eatenMesh);
        MoveToStartPosition();
    }

    public void ResetEaten()
    {  
        ChangeMesh(defaultMesh);
        isEaten = false;
    }


    private void ChangeMesh(Mesh newMesh)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = newMesh;
        }
    }

    private void ChangeMaterial(Material newMaterial)
    {
        if (objectRenderer != null)
        {
            objectRenderer.sharedMaterial = newMaterial;
        }
 
    }


    public void OnEnergizedNearEnd()
    {
        if (isScared)
        {
            ChangeMesh(scaredMesh);
            StartCoroutine("BlinkMaterial");
        }
    }
 

    private IEnumerator BlinkMaterial()
    {
        while (isScared)
        {
            currentMaterialIndex = currentMaterialIndex == 0 ? 1 : 0;          
            ChangeMaterial(materials[currentMaterialIndex]);
            yield return waitToBlinkWhenEnergizedNearEnd;
        }
    }

    private void MoveToStartPosition()
    {
        navMeshAgent.SetDestination(ghostCagePosition);
    }

}

