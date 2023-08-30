using Cinemachine;
using Fusion;
using UnityEditor.Experimental.GraphView;
using UnityEngine.InputSystem;
using UnityEngine;


public class PlayerMovementController : NetworkBehaviour
{
    
    // This class controls only movement related processes
    
    public static PlayerMovementController Local { get; set; }
    private PlayerStateController _playerStateController;
    
    // camera settings
    public CinemachineVirtualCamera localCamera;
    private Transform cameraMainTransform;
    
    // other components
    private Rigidbody rb;
    private Canvas playerNameCanvas;
    [SerializeField] private LayerMask groundLayer;
    
    //movement
    
    private PlayerControls controls;
    private Vector2 moveInput;
    public Vector3 m_movement;
    //private float playerSpeed = 3f;

    
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float maxSpeed = 4f;
    
    // for collision detection

     void Awake()
    {
        rb = transform.GetChild(0).GetComponent<Rigidbody>();
        localCamera = transform.GetChild(1).GetComponent<CinemachineVirtualCamera>();
        playerNameCanvas = transform.GetChild(2).GetComponent<Canvas>();
        controls = new PlayerControls();
        _playerStateController = rb.GetComponent<PlayerStateController>();


    }

    public override void Spawned()
    {
        // state authority is checked to see who is your local player to assign this script
        // to make maincamera able to render the camera of local player make it free from its parent when it is the local 
        // disable otherwise
        if (Object.HasStateAuthority)
        {
            Local = this;
            localCamera.transform.parent = null;
            controls.Player.Enable();
            cameraMainTransform = Camera.main.transform;
            
          
        }
        else
        {
            localCamera.enabled = false;
        }
    }

    public void FixedUpdate()
    {
        // MOVE WITH MOUSE POSITION RAYCAST
        Vector2 mousePosition = controls.Player.Look.ReadValue<Vector2>(); 
            
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 playerPosition = rb.position;
            Vector3 hitPoint = hit.point;

            // Calculate the direction from sphere to hit point
            m_movement = (hitPoint - playerPosition).normalized;
            
        }
    }

    

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {

            if (m_movement != Vector3.zero)
            {
                rb.AddForce(m_movement * moveSpeed, ForceMode.Acceleration);
                if (rb.velocity.magnitude > maxSpeed)
                {
                    rb.velocity = rb.velocity.normalized * maxSpeed;
                    Debug.Log("from limitng" + rb.velocity);
                }
                Debug.Log("from adding" + rb.velocity);
                playerNameCanvas.GetComponent<RectTransform>().transform.position =  new Vector3(rb.position.x , PlayerStateController.StateInstance.NetworkedSize + 1f, rb.position.z);
                // Limit the maximum speed
            }
            
            /*
            // MOVE WITH MOUSE POSITION RAYCAST
            Vector2 mousePosition = controls.Player.Look.ReadValue<Vector2>(); 
            
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                Vector3 playerPosition = rb.position;
                Vector3 hitPoint = hit.point;

                // Calculate the direction from sphere to hit point
                m_movement = (hitPoint - playerPosition).normalized;

                // Apply force to the sphere in the calculated direction
                rb.AddForce(m_movement * moveSpeed);
                
                playerNameCanvas.GetComponent<RectTransform>().transform.position =  new Vector3(rb.position.x , PlayerStateController.StateInstance.NetworkedSize + 1f, rb.position.z);

                // Limit the maximum speed
                if (rb.velocity.magnitude > maxSpeed)
                {
                    rb.velocity = rb.velocity.normalized * maxSpeed;
                }
            }
            */
            
            /*
           //MOVE WITH JOYSTICK
          
           moveInput = controls.Player.Move.ReadValue<Vector2>();
           m_movement.Set(moveInput.x, 0f,moveInput.y);
           m_movement = Quaternion.AngleAxis(cameraMainTransform.eulerAngles.y, Vector3.up) * m_movement;
           rb.AddForce(m_movement * moveSpeed);
           

           // MOVW WITH MOUSE POSITION AND SCREENTOWORLPOINT
           Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x,0.5f, mousePosition.y));
           targetPosition.y = transform.position.y;
           Vector3 direction = (targetPosition - transform.position).normalized;
           // Apply force 
           rb.AddForce(direction * moveSpeed , ForceMode.Acceleration);

          
           */
            
        }
    }

   




}
