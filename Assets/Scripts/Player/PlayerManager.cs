using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, ITeleportable
{
    public FloatVariable speed;
    public UintVariable ghostsEaten;
    public GameObject startObject;
    public GameObject tailSocketObject;
    public GameObject headSocketObject;
    public float secondsToTeleportedReset = 0.3f;
    public float secondsToSlowDownReset = 1f;
    public float slowDownScale = 0.9f;
    public float secondsToEnergizedEnd = 7f;
    public float secondsToEnergizedNearEnd = 5f;
    public float speedBuffAmountOnEnergized = 2f;
    public GameEvent onEnergizedNearEnd;
    public GameEvent onEnergizedEnd;
    public GameEvent onEnergized;

    private Rigidbody playerRigidbody;
    private Vector3 movement;
    private Vector3 startPostion;
    private Vector3 tailSocketPosition;
    private Vector3 headSocketPosition;

    private float horizontalInput;
    private float verticalInput;
    private float newHorizontalInput;
    private float newVerticalInput;
    private float nextHorizontalInput;
    private float nextVerticalInput;

    private bool isTeleported;
    private bool isSlowedDown;
    private bool isEnergized;
    private WaitForSeconds waitToTeleportedReset;
    private WaitForSeconds waitToSlowDownReset;
    private WaitForSeconds waitToEnergizedEnd;
    private WaitForSeconds waitToEnergizedNearEnd;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        startPostion = startObject.transform.position; 
        isTeleported = false;
        isSlowedDown = false;
        isEnergized = false;
        waitToTeleportedReset = new WaitForSeconds(secondsToTeleportedReset);
        waitToSlowDownReset = new WaitForSeconds(secondsToSlowDownReset);
        waitToEnergizedEnd = new WaitForSeconds(secondsToEnergizedEnd - secondsToEnergizedNearEnd);
        waitToEnergizedNearEnd = new WaitForSeconds(secondsToEnergizedNearEnd);
    }

    // Use this for initialization
    void Start ()
    {        
		
	}
	
    public void ResetPosition()
    {
        playerRigidbody.MovePosition(startPostion);
        playerRigidbody.MoveRotation(startObject.transform.rotation);     
        horizontalInput = 0f;
        verticalInput = 0f;
        newHorizontalInput = 0f;
        newVerticalInput = 0f;
        nextHorizontalInput = 0f;
        nextVerticalInput = 0f;
    }


    private void Update()
    {
        RaycastHit hit;
        tailSocketPosition = tailSocketObject.transform.position;
        headSocketPosition = headSocketObject.transform.position;

        newHorizontalInput = Input.GetAxisRaw("Horizontal");
        newVerticalInput = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(newHorizontalInput) > 0)
        {
            nextHorizontalInput = newHorizontalInput;
            nextVerticalInput = 0;
        }
        if (Mathf.Abs(newVerticalInput) > 0)
        {
            nextVerticalInput = newVerticalInput;
            nextHorizontalInput = 0;
        }


        if (Mathf.Abs(nextHorizontalInput) > 0 || Mathf.Abs(nextVerticalInput) > 0)
        {
            Vector3 hitDirection = new Vector3(nextHorizontalInput, 0f, nextVerticalInput);
            Debug.DrawRay(tailSocketPosition, hitDirection * 1f, Color.red);
            if (Physics.Raycast(tailSocketPosition, hitDirection, out hit, 1f))
            {
                if (hit.collider.tag == "Blocking" || hit.collider.tag == "Enemy")
                {
                    return;
                }
            }             
            Debug.DrawRay(headSocketPosition, hitDirection * 1f, Color.red);
            if (Physics.Raycast(headSocketPosition, hitDirection, out hit, 1f))
            {
                if (hit.collider.tag == "Blocking" || hit.collider.tag == "Enemy")
                {
                    return;
                }
            }

            horizontalInput = nextHorizontalInput;
            verticalInput = nextVerticalInput;
        }


    }
    void FixedUpdate ()
    { 
        Move();
    }

    void Move()
    {
        movement.Set(horizontalInput, 0f, verticalInput);    
        movement = movement.normalized * speed.runtimeValue * Time.deltaTime;
        playerRigidbody.MovePosition(transform.position + movement);
        
        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput);
        if (direction.magnitude > 0)
        {        
            playerRigidbody.MoveRotation(Quaternion.LookRotation(direction,playerRigidbody.transform.up));
        }
       
    }

    public void Teleport(Vector3 position)
    {
        if (!isTeleported)
        {         
            playerRigidbody.isKinematic = true;            
            playerRigidbody.transform.position = position;   
            playerRigidbody.isKinematic = false;
            isTeleported = true;
            StartCoroutine(ResetTeleportedState());
        }

    }

    private IEnumerator ResetTeleportedState()
    {
        yield return waitToTeleportedReset;
        isTeleported = false;
    }

    private IEnumerator ResetSlowDownedState()
    {
        yield return waitToSlowDownReset;
        speed.runtimeValue = speed.initialValue;
        isSlowedDown = false;
    }

    public void SlowDown()
    { 
        if (isEnergized)
        {       
            return;
        }    
        if (isSlowedDown)
        {       
            StopCoroutine("ResetSlowDownedState");
            StartCoroutine("ResetSlowDownedState");
        }
        else
        {
            speed.runtimeValue *= slowDownScale;
            isSlowedDown = true;
            StartCoroutine("ResetSlowDownedState");
        }
    }


    private IEnumerator EnergizedResetCoroutine()
    {
        yield return waitToEnergizedNearEnd;
        onEnergizedNearEnd.Rise();
        yield return waitToEnergizedEnd;   
        EnergizedReset();        
    }

    private void EnergizedReset()
    {    
        speed.runtimeValue = speed.initialValue;
        isEnergized = false;
        onEnergizedEnd.Rise();
    }

    private void Energize()
    {     
        speed.runtimeValue = speed.initialValue + speedBuffAmountOnEnergized;
        isEnergized = true;
        onEnergized.Rise();
    }

    public void OnEnergized()
    {
        if (isEnergized)
        {         
            StopCoroutine("EnergizedResetCoroutine");
            EnergizedReset();
            Energize();
            StartCoroutine("EnergizedResetCoroutine");
            return;
        }
        if (isSlowedDown)
        {          
            StopCoroutine("ResetSlowDownedState");
            speed.runtimeValue = speed.initialValue;
            isSlowedDown = false;
        }
        Energize();
        StartCoroutine("EnergizedResetCoroutine");       
    }

}
