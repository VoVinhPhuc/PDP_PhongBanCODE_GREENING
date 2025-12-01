using UnityEngine;

public class FloodController : MonoBehaviour
{
    public float rainStartTime = 120f; // Thời gian bắt đầu mưa
    public float riseSpeed = 0.5f;     // Tốc độ nước dâng
    public GameObject rainParticle;    // Hệ thống hạt mưa
    public GameManager gameManager;

    void Update()
    {
        // Đã bỏ comment để code chạy
        if (gameManager.elapsedTime >= rainStartTime && !gameManager.isGameOver)
        {
            // 1. Bật mưa nếu chưa bật
            if (rainParticle != null && !rainParticle.activeSelf) 
                rainParticle.SetActive(true);

            // 2. Dâng nước lên theo trục Y
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Logic thua game khi nước chạm đầu nhân vật
        if (other.CompareTag("PlayerHead")) 
        {
             gameManager.TriggerGameOver(false, "Bạn đã bị lũ cuốn trôi!");
        }
    }
}