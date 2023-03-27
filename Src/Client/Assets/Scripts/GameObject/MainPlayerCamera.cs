using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera camera;
    public GameObject player;

    Vector3 mDefaultDir; // 默认朝向
    Transform mPlayerTransfrom; // 玩家位置
    Vector3 mRotateValue; // 玩家旋转值
    Vector3 mPicthRotateAxis; //俯仰旋转轴
    Vector3 mYaoRotateAxis; // 左右旋转轴

    public bool invertPicth = true; //反转方向
    public Vector2 pitchLimit = new Vector2(-40f, 70f); //控制摄像机纵轴角度 

    public bool lockMode; // 玩家索敌视角开关
    public Transform targetLocked;

    public float distance = 4f; // 相机观测距离
    public float speed = 120f; // 相机旋转速度
    public Vector3 offset = new Vector3(0f, 1f, 0f); //相对观测目标的偏移值
    /*
     * Mono Singleton中start会导致重载父类函数，导致问题
     * void Start () {

        }
    */

    void OnEnable()
    {
        const string PLAYER = "Player";
        var upAxis = -Physics.gravity.normalized; // 世界垂直y

        mPlayerTransfrom = this.player.transform;
        // 这是摄像机相对于玩家角色的方向，在世界平面的投影点
        mDefaultDir = Vector3.ProjectOnPlane((this.transform.position - mPlayerTransfrom.position), upAxis).normalized;
        mYaoRotateAxis = upAxis;
        // 得到摄像机正面朝向在地面投影的垂线的作为轴向
        mPicthRotateAxis = Vector3.Cross(upAxis, Vector3.ProjectOnPlane(this.transform.forward, upAxis));
    }

    void LateUpdate () {
        // 由于项目的角色与相机加载顺序问题，需要在此进行重新绑定
        if (this.mPlayerTransfrom == null)
        {
            var upAxis = -Physics.gravity.normalized;
            mPlayerTransfrom = this.player.transform;
            mDefaultDir = Vector3.ProjectOnPlane((this.transform.position - mPlayerTransfrom.position), upAxis).normalized;
            mYaoRotateAxis = upAxis;
            mPicthRotateAxis = Vector3.Cross(upAxis, Vector3.ProjectOnPlane(this.transform.forward, upAxis));
        }


        // 锁定敌人旋转 (固定面相玩家背部)
        if (lockMode)
        {
            // 计算偏移玩家位置
            var from = mPlayerTransfrom.localToWorldMatrix.MultiplyPoint3x4(offset);
            // 计算相机位置
            var to = from - (player.transform.forward)* distance;

            transform.position = to;
            transform.LookAt(from);
        }
        // 鼠标自由视角
        else
        {
            // 更新相机位置和相机旋转
            var inputDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            // 更新横向旋转
            mRotateValue.x += inputDelta.x * speed * Time.smoothDeltaTime;
            mRotateValue.x = AngleCorrection(mRotateValue.x);
            // 更新纵向旋转
            mRotateValue.y += inputDelta.y * speed * (invertPicth ? -1f : 1f) * Time.smoothDeltaTime;
            mRotateValue.y = AngleCorrection(mRotateValue.y);
            mRotateValue.y = Mathf.Clamp(mRotateValue.y, pitchLimit.x, pitchLimit.y);
            // 构建角轴四元数
            var horizontalQuat = Quaternion.AngleAxis(mRotateValue.x, mYaoRotateAxis);
            var vecticalQuat = Quaternion.AngleAxis(mRotateValue.y, mPicthRotateAxis);
            var finalDir = horizontalQuat * vecticalQuat * mDefaultDir;

            // 计算偏移玩家位置
            var from = mPlayerTransfrom.localToWorldMatrix.MultiplyPoint3x4(offset);
            var to = from + finalDir * distance;

            // 缓动跟随
            // 会抖动
/*            float xVelocity = 0;
            float yVelocity = 0;
            float zVelocity = 0;
            float newPositionX = Mathf.SmoothDamp(transform.position.x, to.x, ref xVelocity, 0.01f);
            float newPositionY = Mathf.SmoothDamp(transform.position.y, to.y, ref yVelocity, 0.01f);
            float newPositionZ = Mathf.SmoothDamp(transform.position.z, to.z, ref zVelocity, 0.01f);
            transform.position = new Vector3(newPositionX, newPositionY, newPositionZ);*/

            // 无缓动请注释上方缓动模式
            transform.position = to;
            transform.LookAt(from);
        }
    }

    /// <summary>
    ///  防止旋转度数无限变大
    /// </summary>
    /// <param name="value"> 当前旋转度数</param>
    /// <returns>修正后旋转度数</returns>
    float AngleCorrection(float value)
    {
        if (value > 180f) return mRotateValue.x - 360f;
        else if (value < -180f) return mRotateValue.x + 360f;
        return value;
    }
}
