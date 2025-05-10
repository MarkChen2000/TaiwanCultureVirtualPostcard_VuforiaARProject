using UnityEngine;

public class SimpleFaceAtCamera : MonoBehaviour
{
    Transform target;

    [SerializeField]
    float rotationSpeed = 5f; // ����t�סA�i�H�b Inspector �վ�

    void Start()
    {
        target = Camera.main.transform; // ����D��v���� Transform
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
