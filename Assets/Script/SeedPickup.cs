using UnityEngine;

public class SeedPickup : MonoBehaviour
{
    // Hàm này tự động chạy khi có vật thể (có Collider) chui vào vùng của Hạt giống
    private void OnTriggerEnter(Collider other)
    {
        // 1. Kiểm tra xem cái lao vào có phải là Player không?
        // (Bạn nhớ gán Tag "Player" cho nhân vật nhé)
        if (other.CompareTag("Player"))
        {
            // 2. Tìm script PlayerController trên người nhân vật
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                // 3. Cộng số lượng hạt
                player.seedCount++;
                
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.UpdateSeedUI(player.seedCount);
                }

                // 4. Biến mất (Hủy object hạt giống này)
                Destroy(gameObject);
            }
        }
    }
}