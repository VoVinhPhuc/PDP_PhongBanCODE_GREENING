using UnityEngine;

public class WaterTextureScroll : MonoBehaviour
{
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.1f;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // Làm trôi vân bề mặt (Texture Offset)
        float offsetX = Time.time * scrollSpeedX;
        float offsetY = Time.time * scrollSpeedY;
        rend.material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}