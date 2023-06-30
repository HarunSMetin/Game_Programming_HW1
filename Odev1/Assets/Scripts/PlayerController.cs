using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlayerController : MonoBehaviour
{

    public float mouseSensitivity = 6f;
    public float moveSpeed = 2.0f;
    public float jumpPower = 0;

    //  public Rigidbody RigidBody;
    public GameObject playerCamera;
    public CharacterController characterController;
    public Animator animator;

    private float defaultJumpPower = 3f;

    private float defaultMoveSpeed;
    private float dashSpeed;

    public float UpAngle = -50f;
    public float DownAngle = 50f;
    [Header("------------------------------------")]

    public int collectedGems = 0;
    public GameObject Interractables;
    public Slider slider;
    public gameMission gameMission;
    public GameObject FinishBoards;

    private float gravity = 9.8f;
    private float GroundPull = 0;

    private PlayerControl playerControl;
    private Vector2 movementInput;
    private Vector2 mouseInput;
    private bool isJumping = false;
    private bool isDoubleJumping = false;
    private bool isDashing = false;
    private bool isPicking = false;
    private bool isFocusing = false;
    private bool isThrowing = false;

    private void Awake()
    {
        playerControl = new PlayerControl();
    }
    private void OnEnable()
    {
        playerControl.Enable();
    }
    private void OnDisable()
    {
        playerControl.Disable();
    }
    private void Start()
    {
        FinishBoards.SetActive(false);
        defaultMoveSpeed = moveSpeed;
        dashSpeed = moveSpeed * 4;
        Cursor.lockState = CursorLockMode.Locked;

    }
    private void Update()
    {
        if (gameMission.isCompleted)
        {
            Cursor.lockState = CursorLockMode.Confined;
            FinishBoards.SetActive(true);
        }
        else
        {
            Move();
            Look();


            animator.SetBool("isWalking", movementInput.x != 0f || movementInput.y != 0f);
            animator.SetBool("isJumping", isJumping);
            animator.SetBool("isDashing", isDashing);
            animator.SetBool("isCollecting", isPicking);

            if (isJumping)
            {
                jumpPower = defaultJumpPower;
                Debug.Log("isJumping");
            }
            else if (isDoubleJumping)
            {
                jumpPower = defaultJumpPower * 2f;
                Debug.Log("isDoubleJumping");
            }
            else
            {
                jumpPower = 0;
            }

            if (isDashing)
                moveSpeed = dashSpeed;
            else
                moveSpeed = defaultMoveSpeed;



            if (isFocusing)
            {
                GrabObject();
                if (isGrabbed && isThrowing)
                {
                    ThrowObject();
                }
            }
            else
            {
                RelaseObject();
            }
        }

    }
    bool isGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 0.1f))
        {
            return true;
        }
        return false;
    }
    private void Move()
    {

        if (isGrounded())//flag && hits.Length != 0
        {
            GroundPull = 0;
            isJumping = false;
            isDoubleJumping = false;
        }
        else
        {
            GroundPull -= gravity * Time.deltaTime;
            if (playerControl.Player.Jump.ReadValue<float>() > 0)
            {
                isJumping = false;
                isDoubleJumping = true;
            }
        }

        movementInput = playerControl.Player.Move.ReadValue<Vector2>();
        isJumping = playerControl.Player.Jump.ReadValue<float>() > 0;
        isDashing = playerControl.Player.Dash.ReadValue<float>() > 0;
        isPicking = playerControl.Player.Interract.ReadValue<float>() > 0;
        isFocusing = playerControl.Player.Focus.ReadValue<float>() > 0;
        isThrowing = playerControl.Player.Click.ReadValue<float>() > 0;

    }
    private void FixedUpdate()
    {

        characterController.Move(Time.deltaTime * ((transform.right * moveSpeed * movementInput.x) + (transform.forward * moveSpeed * movementInput.y) + (transform.up * (GroundPull + jumpPower))));

    }
    private void Look()
    {

        mouseInput = playerControl.Player.Look.ReadValue<Vector2>();
        //using playerControl.Player.look vector2 input of mouse and rotate the CharacterController 
        transform.Rotate(0, mouseInput.x * mouseSensitivity * Time.deltaTime, 0);
        playerCamera.transform.Rotate(-mouseInput.y * mouseSensitivity * Time.deltaTime, 0, 0);

        Vector3 currentRotation = playerCamera.transform.localEulerAngles;
        if (currentRotation.x > 150) currentRotation.x = 0;
        else
        {
            currentRotation.x = Mathf.Clamp(currentRotation.x, UpAngle, DownAngle);
        }
        playerCamera.transform.localRotation = Quaternion.Euler(currentRotation);

    }
    GameObject grabbedObject = null;
    bool isGrabbed = false;


    private float timer = 0f;
    private float delay = 1f; // Bekleme süresi
    void GrabObject()
    {
        if (!isGrabbed && collectedGems > 1)
        {

            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 0.9f, 0), playerCamera.transform.forward, out hit, 3f))
            {
                grabbedObject = hit.collider.gameObject;
                if (grabbedObject.tag == "Interractable")
                {
                    collectedGems -= 1;
                    grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                    grabbedObject.transform.parent = playerCamera.transform;
                    isGrabbed = true;
                }
            }
        }
        else if (isGrabbed && collectedGems > 1)
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                timer = 0f;
                collectedGems -= 1;

            }
        }
        else
        {
            RelaseObject();
        }
        UpdateSlider();
    }
    void RelaseObject()
    {
        if (isGrabbed)
        {
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObject.transform.parent = Interractables.transform;
            isGrabbed = false;
        }
    }

    void ThrowObject()
    {
        if (isGrabbed && collectedGems > 2)
        {
            collectedGems -= 2;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObject.transform.parent = Interractables.transform;
            grabbedObject.GetComponent<Rigidbody>().AddForce(transform.forward * 2500);
            isGrabbed = false; 
        }

        UpdateSlider();
    }
    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Collectable")
        {

            if (isPicking)
            {
                collider.gameObject.SetActive(false);
                collectedGems += 3;
                UpdateSlider();
            }

        }

    }
    void UpdateSlider()
    {
        slider.value = collectedGems;
    }
}
