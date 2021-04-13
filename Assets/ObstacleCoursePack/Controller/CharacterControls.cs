using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class CharacterControls : MonoBehaviour
{

    public string strCharName = "";
    public GameObject endPos;


    public float speed = 10.0f;
    public float airVelocity = 8f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public float jumpHeight = 2.0f;
    public float maxFallSpeed = 20.0f;
    public float rotateSpeed = 25f; //Speed the player rotate
    public Vector3 moveDir;

    public Animator anim;
    public Rigidbody rb;

    private float distToGround;

    private bool canMove = true; //If player is not hitted
    private bool isStuned = false;
    private bool wasStuned = false; //If player was stunned before get stunned another time
    private float pushForce;
    private Vector3 pushDir;
    public Vector3 checkPoint;
    public Vector3 rotatorOffset = Vector3.zero;
    private bool slide = false;

    public bool isMainPlayeer = false;

    public sbyte finishedRank = 100;

    void Start()
    {
        // get the distance to ground
        distToGround = GetComponent<Collider>().bounds.extents.y;
        finishedRank = 100;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }


    public LayerMask layerRotator;
    bool IsOnRotator()
    {
        Ray ray = new Ray(transform.position, -Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distToGround + 0.2f, layerRotator))
        {
            Rotator rotator = hit.collider.gameObject.GetComponent<Rotator>();
            if (rotator != null)
            {
                rotatorOffset = rotator.hitDir;
                return true;
            }
        }
        return false;
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;

        checkPoint = transform.position;

    }

    void FixedUpdate()
    {

        if (canMove)
        {
            if (moveDir.x != 0 || moveDir.z != 0)
            {
                //Vector3 targetDir = moveDir; //Direction of the character

                //targetDir.y = 0;
                //if (targetDir == Vector3.zero)
                //	targetDir = transform.forward;
                //Quaternion tr = Quaternion.LookRotation(targetDir); //Rotation of the character to where it moves
                //Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * rotateSpeed); //Rotate the character little by little
                //transform.rotation = targetRotation;
            }

            if (IsGrounded())
            {
                // Calculate how fast we should be moving
                Vector3 targetVelocity = moveDir;
                targetVelocity *= speed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rb.velocity;
                if (targetVelocity.magnitude < velocity.magnitude) //If I'm slowing down the character
                {
                    targetVelocity = velocity;
                    rb.velocity /= 1.1f;
                }
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;
                if (!slide)
                {
                    if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
                        rb.AddForce(velocityChange, ForceMode.VelocityChange);
                }
                else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
                {
                    rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
                    //Debug.Log(rb.velocity.magnitude);
                }

                //// Jump
                //if (IsGrounded() && Input.GetButton("Jump"))
                //{
                //    rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                //}
            }
            else
            {
                if (!slide)
                {
                    Vector3 targetVelocity = new Vector3(moveDir.x * airVelocity, rb.velocity.y, moveDir.z * airVelocity);
                    Vector3 velocity = rb.velocity;
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    rb.AddForce(velocityChange, ForceMode.VelocityChange);
                    if (velocity.y < -maxFallSpeed)
                        rb.velocity = new Vector3(velocity.x, -maxFallSpeed, velocity.z);
                }
                else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
                {
                    rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
                }
            }
        }
        else
        {
            rb.velocity = pushDir * pushForce;
        }
        // We apply gravity manually for more tuning control
        if (IsOnRotator()) { rb.AddForce(rotatorOffset, ForceMode.VelocityChange); }
        else rotatorOffset = Vector3.zero;

        rb.AddForce(new Vector3(0, -gravity * rb.mass, 0) + rotatorOffset);

    }
    Vector3 firstPosition;
    public Vector3 dir;
    void CheckInput()
    {

        if (Input.GetMouseButtonDown(0))
            firstPosition = Input.mousePosition;
        if (Input.GetMouseButton(0))
        {
            if (Vector3.Distance(Input.mousePosition, firstPosition) > 10)
                dir = (Input.mousePosition - firstPosition).normalized;
        }

        if (Input.GetMouseButtonUp(0))
            dir = Vector3.zero;
    }
    private void Update()
    {


        if (isMainPlayeer)
        {
            CheckInput();
            if (!GameManager.instance.m_gameStarted)
                return;
            if (dir.x != 0 || dir.y != 0)
            {
                moveDir = transform.forward;
                var input = new Vector3(dir.x, 0, dir.y);
                Quaternion rotation = Quaternion.LookRotation(input); ;
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotateSpeed);
                anim.SetBool("isRunning", true);
            }
            else
            {
                anim.SetBool("isRunning", false);
                moveDir = Vector3.zero;
            }

        }




        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f))
        {
            if (hit.transform.tag == "Slide")
            {
                slide = true;
            }
            else
            {
                slide = false;
            }
        }
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    public void HitPlayer(Vector3 velocityF, float time)
    {
        rb.velocity = velocityF;

        pushForce = velocityF.magnitude;
        pushDir = Vector3.Normalize(velocityF);
        StartCoroutine(Decrease(velocityF.magnitude, time));
    }

    public void LoadCheckPoint()
    {
        transform.position = checkPoint;
    }

    private IEnumerator Decrease(float value, float duration)
    {
        if (isStuned)
            wasStuned = true;
        isStuned = true;
        canMove = false;

        float delta = 0;
        delta = value / duration;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            yield return null;
            if (!slide) //Reduce the force if the ground isnt slide
            {
                pushForce = pushForce - Time.deltaTime * delta;
                pushForce = pushForce < 0 ? 0 : pushForce;
                //Debug.Log(pushForce);
            }
            rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0)); //Add gravity
        }

        if (wasStuned)
        {
            wasStuned = false;
        }
        else
        {
            isStuned = false;
            canMove = true;
        }
    }

    public float GetDistance(Vector3 pos)
    {
        return Vector3.Distance(transform.position, pos);
    }
}
