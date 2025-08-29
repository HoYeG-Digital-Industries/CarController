using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarController : MonoBehaviour
{

    private Rigidbody theRB; // this is the RigidBody that control everything. Do not touch.

    [Header("Car Attributes")]
    [Tooltip("This is the speed at which you accelerate forward")]
    [Range(0,20f)] public float forwardAccel = 8f; 
    [Tooltip("This is the speed at which you accelerate backwards")]
    [Range(0,20f)] public float reverseAccel = 4f;
    [Tooltip("This is the max speed you can reach")]
    [Range(0,100f)] public float maxSpeed = 50f;
    [Tooltip("This is how much you can turn, higher values will give quicker turning")]
    [Range(0,360f)] public float turnStrength = 180f;
    [Tooltip("This is how speed effects your turn strength. \n\nTime represents speed \nValue represents turning strength")]
    [SerializeField] private AnimationCurve turnStrengthSpeedCurve;
    [Tooltip("This is gravity. Affects when you jump and when you fall off things")]
    [Range(0,20f)] public float gravityForce = 10f;
    [Tooltip("This is makes you stickier when you are on the ground. Less drag means more slippy")]
    [Range(2,5f)] public float dragOnGround = 3f;
    private float speedInput, turnInput;

    [Header("Jumping")]
    [Tooltip("Turns jumping on and off")]
    public bool canJump;
    [Tooltip("This is how high you can jump")]
    [Range(0,60f)] public float jumpHeight = 30f;
    [Tooltip("This is how many times you can jump before landing")]
    [Range(1,3)] public int maxJumpCount = 1;
    private int jumpsSinceGrounded = 0;
    [Tooltip("Can you steer while the vehicle isn't grounded")]
    public bool canAirSteer;

    [Header("Ground Check")]
    [Tooltip("This is the layer mask for teh ground. Remember to add this to your floor objects")]
    public LayerMask whatisGround;
    private bool grounded;
    [Tooltip("This is the length of the ground check. Only change this if you can the ray point position")]
    public float groundRayLength = 0.5f;
    private Transform groundRayPoint;

    [Header("Wheels")]
    [Tooltip("This is how much you can turn the wheels")]
    public float maxWheelTurn = 25f;
    [Tooltip("These are for the wheel game objects. Make sure these are separate when you make the car")]
    public Transform leftFrontWheel, rightFrontWheel;
    

    [Header("Particles")]
    [Tooltip("Turns particles on or off")]
    public bool useParticles;
    private ParticleSystem[] dustTrail;
    [Tooltip("This is how quickly your particles emit")]
    [Range(0,50f)]public float maxEmissionValue = 25f;
    private float emissionRate;
    private GameObject particleHolder;

    [SerializeField]
    public enum UIVariables
    {
        Speed = 0,
        MaxAcceleration,
    }

    [Serializable] public struct UIElement
    {
        public TextMeshProUGUI text;
        public string textPrefix, textSuffix;
        public UIVariables uiType;
    }

    [Header("UI")]
    public UIElement[] UIElementArray;


    void Start()
    {
        theRB = gameObject.GetComponentInChildren<Rigidbody>(); //grabs the Rigidbody in the Sphere that is a child of the main gameObject.
        theRB.transform.parent = null; //moves the sphere out of the parent into the root. 
        groundRayPoint = GameObject.Find("ray point").transform; // finds the raycast point for grounded.
        particleHolder = GameObject.Find("Particle Holder"); // grabs the holder of the particles

        dustTrail = GetComponentsInChildren<ParticleSystem>(); //populated the dust trail array with all the aprticle system that are children of the Particle Holder

        if (!useParticles) //turn off the particles if not being used.
        {
            particleHolder.SetActive(false);
        }

        jumpsSinceGrounded = 0;
    }

    void Update() //update that runs based on frame rate of machine
    {
        UpdateUI();

        //Handle forwards
        speedInput = 0f;
        if (Input.GetAxis("Vertical") > 0) 
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel * 1000f;
        }
        //handles backwards
        else if (Input.GetAxis("Vertical") < 0) 
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel * 1000f;
        }

        //handles jump
        if(canJump)
        {
            if (Input.GetKeyDown("space") && (grounded || jumpsSinceGrounded <= maxJumpCount - 2)) // -2 is to account for the fact that it will remain grounded for a few frames and the counter is 1-indexed not 0
            {
                theRB.AddForce(Vector3.up * jumpHeight * 1000f);

                jumpsSinceGrounded++;
            }
        }

        //handles turning
        turnInput = Input.GetAxis("Horizontal");
        if (grounded || canAirSteer)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(theRB.velocity); // rigidbody velocity in local space

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * (turnStrength * turnStrengthSpeedCurve.Evaluate(theRB.velocity.magnitude)) * (localVelocity.z * 0.1f) * Time.deltaTime, 0f));
        }

        //turns wheels
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, turnInput * maxWheelTurn, rightFrontWheel.localRotation.eulerAngles.z);

        //ultimately moves the car.
        transform.position = theRB.transform.position;
    }

    private void FixedUpdate() //update that runs at frame rate decided by Unity
    {

        //ground check
        grounded = false;
        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatisGround))
        {
            grounded = true;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            jumpsSinceGrounded = 0;
        }

        
        emissionRate = 0;//clears the emission rate of particles if not moving


        //moves the sphere that controls everything
        if (grounded){
            theRB.drag = dragOnGround;

            if (Mathf.Abs(speedInput) > 0) 
            {
                theRB.AddForce(transform.forward * speedInput);

                emissionRate = maxEmissionValue;
            }
        }
        else 
        {
            theRB.drag = 0.1f;
            theRB.AddForce(Vector3.up * -gravityForce * 100f);
        }

        //plays the particles if checked to be used
        if(useParticles)
        {
            foreach(ParticleSystem part in dustTrail)
            {
                var emissionModule = part.emission;
                emissionModule.rateOverTime = emissionRate;
            }
        }

    }

    void UpdateUI()
    {
        // Handles the UI Element Array to display chosen value in the designated TMPro box.
        for(int i = 0; i < UIElementArray.Length; i++)
        {
            if(UIElementArray[i].text == null)
                continue;
            
            switch (UIElementArray[i].uiType)
            {
                case UIVariables.Speed:
                    UIElementArray[i].text.text = theRB.velocity.magnitude.ToString("F0");
                    break;
                case UIVariables.MaxAcceleration:
                    UIElementArray[i].text.text = forwardAccel.ToString("F1");
                    break;
                default:
                    continue;
            }
        }
    }

}
