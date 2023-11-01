using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform leftOrUpRoom;
    [SerializeField] private Transform rightOrDownRoom;
    [SerializeField] private CameraController cam;
    [SerializeField] private bool isHorizontal = true;
    [SerializeField] private UnityEvent onEnterEvent;
    private float directionX; // -1 - налево; 1 - направо
    private float directionY; // -1 - наверх; 1 - вниз
    private void HeadBumpCancel(PlayerMovement playerMovement)
    {
        var nextRoomComp = rightOrDownRoom.parent.GetComponent<Room>();
        var prevRoomComp = leftOrUpRoom.parent.GetComponent<Room>();
        playerMovement.MoveToNextRoom();
        cam.MoveToNewRoom(rightOrDownRoom);
        nextRoomComp.ActivateRoom(true);
        prevRoomComp.ActivateRoom(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var nextRoomComp = rightOrDownRoom.parent.GetComponent<Room>();
            var prevRoomComp = leftOrUpRoom.parent.GetComponent<Room>();
            var playerMovement = collision.GetComponent<PlayerMovement>();
            if (isHorizontal)
            {
                if (collision.transform.position.x < transform.position.x)
                {
                    playerMovement.MoveToNextRoom();
                    cam.MoveToNewRoom(rightOrDownRoom);
                    nextRoomComp.ActivateRoom(true);
                    prevRoomComp.ActivateRoom(false);
                }
                else
                {
                    playerMovement.MoveToNextRoom();
                    cam.MoveToNewRoom(leftOrUpRoom);
                    nextRoomComp.ActivateRoom(false);
                    prevRoomComp.ActivateRoom(true);
                }
            }
            else
            {
                if (collision.transform.position.y > transform.position.y)
                {
                    Debug.Log("check");
                    playerMovement.MoveToNextRoom();
                    cam.MoveToNewRoom(rightOrDownRoom);
                    nextRoomComp.ActivateRoom(true);
                    prevRoomComp.ActivateRoom(false);
                }
                else
                {
                    playerMovement.MoveToNextRoom(HeadBumpCancel, true);
                    cam.MoveToNewRoom(leftOrUpRoom);
                    nextRoomComp.ActivateRoom(false);
                    prevRoomComp.ActivateRoom(true);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovement>().isRoomChange = false;
        }
    }
}
