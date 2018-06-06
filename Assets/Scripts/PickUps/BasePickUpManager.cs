using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickUpManager : MonoBehaviour
{

    [HideInInspector] public float scoreValue = 10f;
    [HideInInspector] public Mesh mesh;

    public PickUpSettings pickUpSettings;    
    public FloatVariable score;
    public PickUpSet pickUpSet;
    public GameEvent onPickUp;

	// Use this for initialization
	void Start ()
    {
        scoreValue = pickUpSettings.scoreValue;
        mesh = pickUpSettings.mesh;
        pickUpSet.Add(this);
        ChangeMesh();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PickUp();           
            onPickUp.Rise();
        }
    }

    private void PickUp()
    {
        score.runtimeValue += scoreValue;
        gameObject.SetActive(false);
    }

    private void ChangeMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    private void OnDestroy()
    {
        pickUpSet.Remove(this);
    }   

}
