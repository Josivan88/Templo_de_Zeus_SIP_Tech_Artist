using UnityEngine;

[ExecuteAlways]
public class BillboardWithAxisLock : MonoBehaviour
{
    public Camera targetCamera;
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;
    public bool flipDirection = false;

    void Update()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null) return;
        }

        Vector3 direction = transform.position - targetCamera.transform.position;
        if (flipDirection)
            direction = -direction;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 euler = lookRotation.eulerAngles;

        if (lockX) euler.x = transform.rotation.eulerAngles.x;
        if (lockY) euler.y = transform.rotation.eulerAngles.y;
        if (lockZ) euler.z = transform.rotation.eulerAngles.z;

        transform.rotation = Quaternion.Euler(euler);
    }
}
