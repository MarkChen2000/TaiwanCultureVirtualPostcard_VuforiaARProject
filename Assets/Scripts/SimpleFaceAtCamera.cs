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

    void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (direction.sqrMagnitude > 0.001f) { //�i�H�קK���H�s�����p�C
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            // �u���� y �b
            Vector3 euler = targetRotation.eulerAngles;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(0, euler.y, 0), // Quaternion.Euler(0, euler.y, 0) �u�O�d y �b����C
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
