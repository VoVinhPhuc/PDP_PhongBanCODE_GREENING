using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public Joystick joystick; 

    [Header("Interaction Settings")]
    public float interactRadius = 2f; 
    public LayerMask interactLayer;   // QUAN TRỌNG: Phải chọn đúng Layer của Hạt và Đất
    
    [Header("Inventory")]
    public int seedCount = 0;
    
    [Header("References")]
    public Animator animator;         

    private CharacterController characterController;
    private float gravity = -9.81f;
    private Vector3 velocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (GameManager.Instance != null) 
            GameManager.Instance.UpdateSeedUI(seedCount);
    }

    void Update()
    {
        HandleMovement();
        ApplyGravity();
    }

    void HandleMovement()
    {
        float hor = joystick.Horizontal;
        float ver = joystick.Vertical;
        Vector3 direction = new Vector3(hor, 0f, ver).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, targetAngle, 0f), rotationSpeed * Time.deltaTime);
            characterController.Move(direction * moveSpeed * Time.deltaTime);
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }
    }

    void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    // HÀM NÀY GẮN VÀO NÚT BẤM
    public void OnActionButtonPressed()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRadius, interactLayer);

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Soil") && seedCount > 0)
            {
                SoilLogic soil = hit.GetComponent<SoilLogic>();
                if (soil != null && soil.isEmpty)
                {
                    soil.PlantTree(); 
                    
                    seedCount--; // Trừ hạt
                    
                    // THÊM DÒNG NÀY: Cập nhật UI sau khi trừ
                    if (GameManager.Instance != null)
                        GameManager.Instance.UpdateSeedUI(seedCount);

                    if(animator) animator.SetTrigger("Interact");
                    return;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}