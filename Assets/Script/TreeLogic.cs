using System.Collections;
using UnityEngine;
using TMPro; 

public class TreeLogic : MonoBehaviour
{
    [Header("Settings")]
    public float growTime = 10f; 

    [Header("New Logic")]
    // Kéo Prefab cây lớn (BigTree_Prefab) vào đây, KHÔNG kéo object con
    public GameObject matureTreePrefab;
    public ParticleSystem growEffect; 
    public TMP_Text countdownText;

    void Start()
    {
        if (countdownText == null) 
            countdownText = GetComponentInChildren<TMP_Text>();
        StartCoroutine(AutoGrow());
    }

    IEnumerator AutoGrow()
    {
        float timer = growTime;

        while (timer > 0)
        {
            // Cập nhật text hiển thị số giây
            if (countdownText != null)
                countdownText.text = Mathf.Ceil(timer).ToString(); // Làm tròn số (vd: 9.0 -> "9")

            yield return new WaitForSeconds(1f); // Chờ 1 giây thật
            timer--; // Giảm 1 giây
        }

        // Hết giờ -> Xóa text đếm ngược cho đẹp
        if (countdownText != null) countdownText.text = "";

        // --- Logic lớn lên (như cũ) ---
        if (matureTreePrefab != null)
        {
            Instantiate(matureTreePrefab, transform.position, transform.rotation);
        }

        if(growEffect) 
        {
            growEffect.transform.SetParent(null);
            growEffect.Play();
            Destroy(growEffect.gameObject, 2f);
        }

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore();

        Destroy(gameObject);
    }
}