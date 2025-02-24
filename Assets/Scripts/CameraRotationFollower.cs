using UnityEngine;

public class CameraRotationFollower : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    void Update()
    {
        Vector3 cameraRotation = cameraTransform.eulerAngles;

        transform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
    }
}
