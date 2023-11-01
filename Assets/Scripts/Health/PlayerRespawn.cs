using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip respawnSound;
    private Transform spawnPoint;
    private Health playerHealth;
    private UIManager uiManager;
    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        uiManager = FindObjectOfType<UIManager>();
    }
    public void CheckRespawn()
    {
        if (spawnPoint == null)
        {
            uiManager.GameOver();
            return;
        }

        transform.position = spawnPoint.position;
        playerHealth.Respawn();

        var room = spawnPoint.parent.GetComponent<Room>().CameraPoint;
        Camera.main.GetComponent<CameraController>().MoveToNewRoom(room);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            spawnPoint = collision.transform;
            SoundManager.Instance.PlaySound(respawnSound);
            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>().SetTrigger("appear");
        }
    }
}
