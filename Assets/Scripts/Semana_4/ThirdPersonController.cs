using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    public InputSystem_Actions inputs;
    [SerializeField] private Vector2 moveInputs;
    private CharacterController controller;
    public CinemachineCamera characterCamera;
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

        //Con respecto al cursor diferentes metodos para poder controllar el cursor del mouse
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnEnable()
    {
        inputs.Enable();

        inputs.Player.Move.performed += ctx => moveInputs = ctx.ReadValue<Vector2>();//-> lamba para comprimir el metodo move
        inputs.Player.Move.canceled += ctx => moveInputs = Vector2.zero;

        inputs.Player.Jump.performed += OnJump;        
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
        //Se mueve en direccion a la camara 
        Vector3 cameraForwardDir = characterCamera.transform.forward;
        cameraForwardDir.y = 0f;
        cameraForwardDir.Normalize();
        if (moveInputs != Vector2.zero) 
        { 
            //Rota el transform del personaje en base a quaternion
            Quaternion targetQuaternion = Quaternion.LookRotation(cameraForwardDir);
            //transform.rotation = targetQuaternion;
            transform.rotation = Quaternion.Slerp(transform.rotation,targetQuaternion, rotationSpeed * Time.deltaTime);

            //transform.Rotate(Vector3.up * moveInputs.x * rotationSpeed * Time.deltaTime); //Rotate = Vector3(direccion que quiero) * mvIpts * rtspedd * tmdtim
            Vector3 moveDir = (cameraForwardDir * moveInputs.y + transform.right * moveInputs.x) * moveSpeed;

            controller.Move(moveDir * Time.deltaTime);
        }      
    }    
    private void OnJump(InputAction.CallbackContext context)
    {
        if (!controller.isGrounded) return;
        verticalVelocity = jumpForce;
    }    
}
