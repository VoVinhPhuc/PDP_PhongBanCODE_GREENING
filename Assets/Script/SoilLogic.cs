// using UnityEngine;

// public class SoilLogic : MonoBehaviour
// {
//     [Header("Settings")]
//     public GameObject treePrefab; // Kéo Prefab cây (Sapling) vào đây
//     public bool isEmpty = true;   
    
//     public void PlantTree()
//     {
//         if (!isEmpty) return;

//         // SỬA LỖI: Cộng thêm Vector3.up * 0.5f để cây trồi lên mặt đất, không bị chìm
//         Vector3 plantPos = transform.position + new Vector3(0, 0.8f, 0); 
        
//         GameObject newTree = Instantiate(treePrefab, plantPos, Quaternion.identity);

//         isEmpty = false;
//     }
// }

using UnityEngine;

public class SoilLogic : MonoBehaviour
{
    [Header("Settings")]
    public GameObject treePrefab; // Kéo Prefab cây (Sapling) vào đây
    public bool isEmpty = true;   

    // Hàm này tự động chạy khi Nhân vật đi vào vùng của ô đất
    private void OnTriggerEnter(Collider other)
    {
        // 1. Chỉ xử lý nếu đất đang trống
        if (!isEmpty) return;

        // 2. Kiểm tra xem người chạm vào có phải Player không
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            // 3. Nếu là Player và Player dùng được hạt (còn hạt)
            if (player != null && player.TryUseSeed())
            {
                PlantTree(); // Trồng cây ngay lập tức
                
                // (Tùy chọn) Chạy animation trồng cây cho nhân vật nếu muốn
                if(player.animator) player.animator.SetTrigger("Interact");
                
                Debug.Log("Đã tự động trồng cây!");
            }
            else
            {
                Debug.Log("Đi vào đất nhưng hết hạt giống!");
            }
        }
    }

    private void PlantTree()
    {
        // Tính vị trí trồng (cao hơn đất một chút để không bị chìm)
        Vector3 plantPos = transform.position + new Vector3(0, 0.5f, 0); 
        
        // Sinh ra cây
        Instantiate(treePrefab, plantPos, Quaternion.identity);

        // Đánh dấu đất đã có cây
        isEmpty = false;
        
        // Play Sound trồng cây (nếu có)
        if (GameManager.Instance != null)
             GameManager.Instance.PlayButtonSound();
    }
}