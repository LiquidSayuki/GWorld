using UnityEngine;

public class SingJumpRapBasketball : MonoBehaviour
{

    public LineRenderer lineR;
    public Vector3[] keyPoints = new Vector3[30];
    public float subdivision = 0.05f;
    public Transform playerPos;
    public Transform camPos;
    public float velocity;

    public GameObject basketballPrefab;

    void Start()
    {
        lineR.positionCount = keyPoints.Length;
        lineR.enabled = false;
    }


    void Update()
    {
        if (this.camPos == null)
        {
            this.camPos = Camera.main.transform;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lineR.enabled = true;

        }
        if (Input.GetKey(KeyCode.Space))
        {
            playerPos.rotation = Quaternion.Slerp(playerPos.rotation, Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0), 8f * Time.deltaTime);
            for (int i = 0; i < keyPoints.Length; i++)
            {
                // Physics.gravity*(0.5f * Mathf.Pow(i*subdivision, 2f)) 
                // 1/2 的 x轴数值平方 * 重力 = y轴数值
                keyPoints[i] = playerPos.position + Vector3.up + -(camPos.position + new Vector3(0, -4, 0) - playerPos.position).normalized * velocity * i * subdivision + Physics.gravity * (0.5f * Mathf.Pow(i * subdivision, 2f));
            }
            lineR.SetPositions(keyPoints);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GameObject go = Instantiate(basketballPrefab, (playerPos.position + Vector3.up + playerPos.forward), playerPos.rotation);
            Rigidbody rb = go.GetComponent<Rigidbody>();
            rb.AddForce(-(camPos.position + new Vector3(0, -4, 0) - playerPos.position).normalized * velocity, ForceMode.VelocityChange);
            lineR.enabled = false;
        }
    }
}
