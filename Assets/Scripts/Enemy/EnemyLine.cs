using UnityEngine;

public class EnemyLine : MonoBehaviour
{
    [SerializeField] private float movementDistanceX = 1f;
    private bool movingLeft;
    private float LeftEdge;
    private float RightEdge;

    [SerializeField] private float movementDistanceY = 1f;
    private bool movingDown;
    private float UpEdge;
    private float DownEdge;

    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private bool isHorizontal = true;
    private void Awake()
    {
        LeftEdge = transform.position.x - movementDistanceX;
        RightEdge = transform.position.x + movementDistanceX;
        UpEdge = transform.position.y - movementDistanceY;
        DownEdge = transform.position.y + movementDistanceY;
    }
    private void Update()
    {
        if (isHorizontal)
        {
            if (movingLeft)
            {
                if (transform.position.x > LeftEdge)
                {
                    transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
                }
                else
                {
                    movingLeft = false;
                }
            }
            else // !movingLeft
            {
                if (transform.position.x < RightEdge)
                {
                    transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
                }
                else
                {
                    movingLeft = true;
                }
            }
        }
        else // !isHorizontal
        {
            if (movingDown)
            {
                if (transform.position.y > UpEdge)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - speed * Time.deltaTime, transform.position.z);
                }
                else
                {
                    movingDown = false;
                }
            }
            else // !movingDown
            {
                if (transform.position.y < DownEdge)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);
                }
                else
                {
                    movingDown = true;
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Health>().TakeDamage(damage, this.transform);
        }
    }
}
