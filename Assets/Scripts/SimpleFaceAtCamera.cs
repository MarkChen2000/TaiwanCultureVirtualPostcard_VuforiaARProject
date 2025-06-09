using UnityEngine;

public class SimpleFaceAtCamera : MonoBehaviour
{
    Transform target;

    [SerializeField]
    float rotationSpeed = 5f; // 旋轉速度，可以在 Inspector 調整

    void Start()
    {
        target = Camera.main.transform; // 獲取主攝影機的 Transform
    }

    void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (direction.sqrMagnitude > 0.001f) { //可以避免除以零的情況。
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            // 只改變 y 軸
            Vector3 euler = targetRotation.eulerAngles;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(0, euler.y, 0), // Quaternion.Euler(0, euler.y, 0) 只保留 y 軸旋轉。
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
