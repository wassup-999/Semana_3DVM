using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    public InputSystem_Actions inputs;
    [SerializeField] private Vector2 moveInputs;
    private CharacterController controller;
    //movimiento
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    //Salto y sistema gravedad
    public float gravity = -9.8f;
    public float verticalVelocity = 0f;
    public float jumpForce = 10f;
    //empuje con objeto
    public float pushForce = 4f;
    //Dash
    private bool IsDashing;
    public float dashForce;

    public float dashDuration;
    private float dashTimer;
    private void Awake()
    {
        inputs = new();
        controller = GetComponent<CharacterController>();
    }
    private void OnEnable()
    {
        inputs.Enable();

        inputs.Player.Move.performed += ctx => moveInputs = ctx.ReadValue<Vector2>();//-> lamba para comprimir el metodo move
        inputs.Player.Move.canceled += ctx => moveInputs = Vector2.zero;

        inputs.Player.Jump.performed += OnJump;

        inputs.Player.Sprint.performed += OnDash;
    }

    

    void Start()
    {
        
    }

    void Update()
    {
        OnMove();
        //OnSimplemove();
    }
    public void OnMove() // para que rote y se mueva ademas de rotar y la gravedad
    {
        transform.Rotate(Vector3.up * moveInputs.x * rotationSpeed * Time.deltaTime); //Rotate = Vector3(direccion que quiero) * mvIpts * rtspedd * tmdtim
        Vector3 moveDir = transform.forward * moveSpeed * moveInputs.y ;
        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f; // recomendado poner -2f cuando aplicas gravedad

        moveDir.y = verticalVelocity;

        if (IsDashing)
        {
            moveDir = transform.forward * dashForce * (dashTimer/dashDuration);            
            dashTimer -= Time.deltaTime;
            if(dashTimer <= 0)
            {
                IsDashing = false;
            }
        }
        controller.Move(moveDir * Time.deltaTime);
    }
    
    /*public void OnSimplemove() // ignora cualquier variable vertical , bueno para hacer que los personajes no puedan saltar
    {
        transform.Rotate(Vector3.up * moveInputs.x * rotationSpeed * Time.deltaTime); //Rotate = Vector3(direccion que quiero) * mvIpts * rtspedd * tmdtim
        Vector3 moveDir = transform.forward * moveSpeed * moveInputs.y;
        controller.SimpleMove(moveDir);
    }*/


    /*private void MoveAction(InputAction.CallbackContext context)
    {
        moveInputs = context.ReadValue<Vector2>();
    }*/

    
    private void OnJump(InputAction.CallbackContext context)
    {
        if (!controller.isGrounded) return;
        verticalVelocity = jumpForce;
    }   
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
        Vector3 PushDir = (hit.transform.position - transform.position).normalized;
        if (hit.rigidbody != null && hit.rigidbody.linearVelocity == Vector3.zero) 
        {
            hit.rigidbody.AddForce(PushDir * pushForce, ForceMode.Impulse);
            print(hit.gameObject.name);
        }
    }
    private void OnDash(InputAction.CallbackContext context)
    {
        IsDashing = true;
        dashTimer = dashDuration;
    }
    
}
