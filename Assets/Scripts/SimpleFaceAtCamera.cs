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

    void Update()
    {
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized * -1f;

        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
