using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // Đối tượng mà camera sẽ theo dõi
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10); // Độ lệch của camera so với target
    [SerializeField] private float smoothSpeed = 0.125f; // Tốc độ mượt mà của camera
    [SerializeField] private bool useSmoothDamp = false; // Chọn nội suy mượt mà (SmoothDamp) hoặc tuyến tính (Lerp)

    private Vector3 velocity = Vector3.zero; // Biến dùng cho SmoothDamp

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // Vị trí mong muốn của camera
        Vector3 desiredPosition = target.position + offset;

        // Nội suy vị trí camera
        Vector3 smoothedPosition;
        if (useSmoothDamp)
        {
            // Sử dụng SmoothDamp
            smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        }
        else
        {
            // Sử dụng Lerp
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }

        // Cập nhật vị trí của camera
        transform.position = smoothedPosition;
    }
}
