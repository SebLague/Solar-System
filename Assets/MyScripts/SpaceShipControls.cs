using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipControls : MonoBehaviour
{
    [Header ("Engage Parameters")]
    public float forwardSpeed = 2f;
    public float forwardAcceleration = 2f;
    [Header("Strafe Parameters")]
    public float strafeSpeed = 1f;
    public float strafeAcceleration = 2f;
    [Header("Hover Parameters")]
    public float hoverSpeed = 1f;
    public float hoverAcceleration = 2f;
    [Header("Roll Parameters")]
    public float rollSpeed = 1f;
    public float rollAcceleration = 2f;
    [Header("Mouse Look Speed")]
    public float lookRotateSpeed = 90f;

    [Header("Gravity Stuff")]
    public GameObject Planet;
    public float gravity = 10;
    public bool OnGround = false;
    float distanceToGround;
    Vector3 Groundnormal;

    private Rigidbody rb;

    private float currentForwardSpeed;
    private float currentStrafeSpeed;
    private float currentHoverSpeed;

    private Vector2 lookInput, screenCenter, mouseDistance;
    private float rollInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;

        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;
        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);

        rollInput = Mathf.Lerp(rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.deltaTime); 

        transform.Rotate(-mouseDistance.y * lookRotateSpeed * Time.deltaTime, 
                          mouseDistance.x * lookRotateSpeed * Time.deltaTime, 
                          rollInput * rollSpeed * Time.deltaTime, 
                          Space.Self);

        currentForwardSpeed = Mathf.Lerp(currentForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed , forwardAcceleration * Time.deltaTime);
        currentStrafeSpeed = Mathf.Lerp(currentStrafeSpeed, Input.GetAxisRaw("Horizontal") * strafeSpeed , strafeAcceleration * Time.deltaTime);
        currentHoverSpeed = Mathf.Lerp(currentHoverSpeed, Input.GetAxisRaw("Hover") * hoverSpeed , hoverAcceleration * Time.deltaTime);

        transform.position += transform.forward * currentForwardSpeed * Time.deltaTime;
        transform.position += transform.right * currentStrafeSpeed * Time.deltaTime;
        transform.position += transform.up * currentHoverSpeed * Time.deltaTime;

       
    }

    //CHANGE PLANET

    private void OnTriggerEnter(Collider collision)
    {
        //if (Planet = null)
        //{
        //    Planet = collision.gameObject;
        //}
        if (Planet == null || collision.transform != Planet.transform)
        {

            Planet = collision.transform.gameObject;

            // TODO:
            // try to level the aircraft to the planets surface upon entering the athmosphere

            //Vector3 targetRotation = (transform.position - Planet.transform.position);
            //Quaternion rotation = Quaternion.LookRotation(targetRotation);
            //transform.rotation = Quaternion.Lerp(rotation, targetRotation, 0.5f);

            //Vector3 gravDirection = (transform.position - Planet.transform.position).normalized;

            //Quaternion toRotation = Quaternion.Lerp(transform.rotation, gravDirection, 1f) * transform.rotation;
            //transform.rotation = toRotation;

            //rb.velocity = Vector3.zero;
            //rb.AddForce(gravDirection * gravity);


            NewPlanet(Planet);

        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    //GroundControl
    //    RaycastHit hit = new RaycastHit();
    //    if (Physics.Raycast(transform.position, -transform.up, out hit, 10))
    //    {
    //        distanceToGround = hit.distance;
    //        Groundnormal = hit.normal;

    //        if (distanceToGround <= 0.2f)
    //        {
    //            OnGround = true;
    //        }
    //        else
    //        {
    //            OnGround = false;
    //        }
    //    }

    //    //GRAVITY and ROTATION
    //    Vector3 gravDirection = (transform.position - Planet.transform.position).normalized;
    //    if (OnGround == false)
    //    {
    //        //rb.AddForce(gravDirection * -gravity);
    //    }
    //    //
    //    Quaternion toRotation = Quaternion.FromToRotation(transform.up, Groundnormal) * transform.rotation;
    //    transform.rotation = toRotation;
    //}

    private void OnTriggerExit(Collider other)
    {
        Planet = null;
    }

    public void NewPlanet(GameObject newPlanet)
    {

        Planet = newPlanet;
    }
}
