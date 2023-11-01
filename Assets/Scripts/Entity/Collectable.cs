using UnityEngine;

public class Collectable : MonoBehaviour
{

    [SerializeField] private AudioClip pickupSound;
    protected PlayerMovement player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<PlayerMovement>();
            OnPickupAction();
            SoundManager.Instance.PlaySound(pickupSound);
            Destroy(gameObject);
        }
    }
    protected virtual void OnPickupAction() { }
}
