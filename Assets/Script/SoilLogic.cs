using UnityEngine;

public class SoilLogic : MonoBehaviour
{
    [Header("Settings")]
    public GameObject treePrefab; // Kéo Prefab cây (Sapling) vào đây
    public bool isEmpty = true;   
    
    public void PlantTree()
    {
        if (!isEmpty) return;

        // SỬA LỖI: Cộng thêm Vector3.up * 0.5f để cây trồi lên mặt đất, không bị chìm
        Vector3 plantPos = transform.position + new Vector3(0, 0.8f, 0); 
        
        GameObject newTree = Instantiate(treePrefab, plantPos, Quaternion.identity);

        isEmpty = false;
    }
}