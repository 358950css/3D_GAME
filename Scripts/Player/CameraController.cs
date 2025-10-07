using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;   // 角色目標
    [SerializeField] private float distance = 23f; // 攝影機距離角色的距離
    [SerializeField] private float height = 2f;   // 攝影機的高度
    [SerializeField] private float sensitivity = 100f; // 滑鼠靈敏度

    private float yaw = 0f; // 水平旋轉角度

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; // 鎖定滑鼠
    }

    private void LateUpdate()
    {
        Debug.Log("設相機1");
        if (target == null) return;
        Debug.Log("設相機2");

        // 滑鼠輸入
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        // 更新攝影機水平角度
        yaw += mouseX;

        // 計算旋轉後的位置
        float x = Mathf.Sin(yaw * Mathf.Deg2Rad) * distance;
        float z = Mathf.Cos(yaw * Mathf.Deg2Rad) * distance;

        Vector3 newPosition = new Vector3(x, height, z) + target.position;

        // 設置攝影機位置
        transform.position = newPosition;

        // 攝影機看向角色上半身
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}