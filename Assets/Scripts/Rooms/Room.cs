using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject[] resetable;
    public Transform CameraPoint
    { get { return this.GetComponentInChildren<RoomCollider>().transform; }
    }
    private Vector3[] initialPosition;
    private void Awake()
    {
        initialPosition = new Vector3[resetable.Length];
        for (int i = 0; i < resetable.Length; i++)
        {
            if (resetable[i] != null)
                initialPosition[i] = resetable[i].transform.position;
        }
    }
    public void ActivateRoom(bool _status)
    {
        for (int i = 0; i < resetable.Length; i++)
        {
            resetable[i].SetActive(_status);
            resetable[i].transform.position = initialPosition[i];
        }
    }
}
