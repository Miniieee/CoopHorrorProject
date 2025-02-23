using UnityEngine;

public class SetObjectPosition : MonoBehaviour
{
    private Transform targetPosition;

    public void SetTargetPosition(Transform target)
    {
        targetPosition = target;
    }

    private void LateUpdate()
    {
        this.gameObject.transform.position = targetPosition.position;
    }
}
