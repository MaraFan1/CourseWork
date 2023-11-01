using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private PlayerMovement playerMovement;
    private float currentPositionX;
    private float currentPositionY;
    private Vector3 velocity = Vector3.zero;
    

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPositionX, currentPositionY, transform.position.z), ref velocity, speed);
    }
    public void MoveToNewRoom(Transform _newRoom)
    {
        currentPositionX = _newRoom.position.x;
        currentPositionY = _newRoom.position.y;
    }
}
