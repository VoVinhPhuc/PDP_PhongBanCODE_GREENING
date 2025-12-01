using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public Joystick joystick; 

    [Header("Interaction Settings")]
    public float interactRadius = 3.5f; 
    public LayerMask interactLayer;   // QUAN TRỌNG: Phải chọn đúng Layer của Hạt và Đất
    public Transform handPosition;
    
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
        // 1. Lấy tất cả vật thể trong phạm vi
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRadius, interactLayer);

        // 2. Ưu tiên Nhặt Hạt trước (nếu đứng trúng hạt)
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Seed"))
            {
                // Gọi logic nhặt hạt (hoặc dùng SeedPickup.cs riêng thì bỏ qua đoạn này)
                return; 
            }
        }

        // 3. Tìm ô đất GẦN NHẤT để trồng
        Collider nearestSoil = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Soil"))
            {
                SoilLogic soil = hit.GetComponent<SoilLogic>();
                // Chỉ quan tâm đất trống
                if (soil != null && soil.isEmpty)
                {
                    float dist = Vector3.Distance(transform.position, hit.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        nearestSoil = hit;
                    }
                }
            }
        }

        // 4. Nếu tìm thấy đất phù hợp và có hạt -> TRỒNG
        if (nearestSoil != null && seedCount > 0)
        {
            SoilLogic targetSoil = nearestSoil.GetComponent<SoilLogic>();
            targetSoil.PlantTree(); // Hàm này sẽ tự lo việc căn giữa
            
            seedCount--; // Trừ hạt
            
            // Cập nhật UI
            if (GameManager.Instance != null)
                GameManager.Instance.UpdateSeedUI(seedCount);

            // Play Sound
            if (GameManager.Instance != null)
                GameManager.Instance.PlayButtonSound();

            if(animator) animator.SetTrigger("Interact");
            
            Debug.Log("Đã trồng vào ô đất gần nhất!");
        }
        else if (seedCount <= 0)
        {
            Debug.Log("Hết hạt giống rồi!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}