using UnityEngine;

public class SoilLogic : MonoBehaviour
{
    [Header("Settings")]
    public GameObject treePrefab; // Kéo Prefab cây (Sapling) vào đây
    public bool isEmpty = true;   
    
    public void PlantTree()
    {
        if (!isEmpty) return;

        // Vị trí trồng = Tâm của ô đất (transform.position) + Chỉnh cao độ Y (để ko bị chìm)
        Vector3 centerPosition = transform.position; 
        
        // Lưu ý: Nếu ô đất của bạn là Plane phẳng lì, Y=0. Nếu cây bị chìm thì tăng Y lên
        Vector3 plantPos = new Vector3(centerPosition.x, centerPosition.y + 0.5f, centerPosition.z);
        
        // Quaternion.identity = Không xoay (Cây đứng thẳng)
        GameObject newTree = Instantiate(treePrefab, plantPos, Quaternion.identity);

        // Đánh dấu đất đã có chủ
        isEmpty = false;
    }
}