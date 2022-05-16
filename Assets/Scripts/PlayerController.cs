using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [SerializeField] [Range(0, 50)] 
    private float movementSpeed = 25;

    [SerializeField] [Range(0, 50)] 
    private float accelerationSpeed = 25;

    [SerializeField] [Range(0, 50)]
    private float rotationSpeed = 25;

    private float rollInput, activeForwardSpeed, activeStrafeSpeed, activeHoverSpeed;
    private Vector2 lookInput, screenCenter, mouseDistance;

    private float multiplier = .5f;

    private PhotonView view;
    private Camera _camera;
    
    private void Start()
    {
        screenCenter.x = Screen.width / 2f;
        screenCenter.y = Screen.height / 2f;
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            _camera = Camera.main;
        }
    }

    private void Update()
    {
        if (view.IsMine)
        {
            multiplier = Input.GetKey("z") ? 1f : .5f;
        
            rollInput = Mathf.Lerp(rollInput, Input.GetAxis("Roll"), 
                accelerationSpeed * Time.deltaTime * multiplier);
        
            transform.Rotate(0, 0, 
                rollInput * rotationSpeed * Time.deltaTime * 
                multiplier, Space.Self);
        
            if (Input.GetMouseButton(1))
            {
                lookInput = Input.mousePosition;
                mouseDistance = (lookInput - screenCenter) 
                                / Mathf.Min(screenCenter.x, screenCenter.y);
                mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);
        
                transform.Rotate(
                    -mouseDistance.y * rotationSpeed * Time.deltaTime * multiplier,
                    mouseDistance.x * rotationSpeed * Time.deltaTime * multiplier,
                    0, Space.Self);
            }

            activeForwardSpeed = Mathf.Lerp(
                activeForwardSpeed, Input.GetAxis("Vertical") * movementSpeed,
                accelerationSpeed * Time.deltaTime * multiplier);
            
            activeStrafeSpeed = Mathf.Lerp(
                activeStrafeSpeed, Input.GetAxis("Horizontal") * movementSpeed,
                accelerationSpeed * Time.deltaTime * multiplier);
            
            activeHoverSpeed = Mathf.Lerp(
                activeHoverSpeed, Input.GetAxis("Hover") * movementSpeed,
                accelerationSpeed * Time.deltaTime * multiplier);

            var playerTransform = transform;
            playerTransform.position += multiplier * Time.deltaTime *
                                   (playerTransform.forward * activeForwardSpeed + 
                                    playerTransform.right * activeStrafeSpeed + 
                                    playerTransform.up * activeHoverSpeed);

            var cameraTransform = _camera.transform;
            cameraTransform.position = playerTransform.position;
            cameraTransform.rotation = playerTransform.rotation;
        }
    }
}
