using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] public float smoothSpeed = 5.0f;
    private Vector3 smoothedPosition;
    
    void FixedUpdate()
    {
    	transform.position = transform.position.CameraFollow(target.position, smoothSpeed, Time.deltaTime);
    }
}
