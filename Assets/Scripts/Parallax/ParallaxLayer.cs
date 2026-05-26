using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    private float imageWidth;
    [SerializeField] private Transform background;
    [SerializeField] private float parallaxMultiplier;
    [SerializeField] private float imageWidthOffset = 10;
    public void Move(float distanceToMove)
    {
        background.position += Vector3.right * (distanceToMove * parallaxMultiplier);
    }
    public void CalculateImageWidth()
    {
        imageWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
    }
    public void LoopBackGround(float cameraRightEdge, float cameraLeftEdge)
    {
        float imageRightEdge = (background.position.x + imageWidth / 2) - imageWidthOffset;
        float imageLeftEdge = (background.position.x - imageWidth / 2) + imageWidthOffset;
        if (imageRightEdge < cameraLeftEdge)
            background.position += Vector3.right * imageWidth;
        else if (imageLeftEdge > cameraRightEdge)
            background.position += Vector3.right * -imageWidth;
    }

}
