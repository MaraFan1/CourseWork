using UnityEngine;

public class RoomCollider : MonoBehaviour
{
    static private BoxCollider2D boxCollider;
    static public float Height
    {
        get { return boxCollider.size.y; }
        private set { Height = value; }
    }
    static public float Width
    {
        get { return boxCollider.size.x; }
        private set { Width = value; }
    }
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
}
