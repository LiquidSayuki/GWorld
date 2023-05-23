using Assets.Scripts.Managers;
using Entities;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerInputController : MonoBehaviour
{

    public Rigidbody rb;
    SkillBridge.Message.CharacterState state;
    public Creature character;
    public float rotateSpeed = 2.0f;
    public float turnAngle = 10;
    public int speed;

    public EntityController entityController;

    public bool lockMode; // 玩家索敌视角开关
    public Transform targetLocked; // 玩家面朝敌人

    public bool onAir = false;

    private MainPlayerCamera mainPlayerCamera;

    private NavMeshAgent navMeshAgent;
    private bool autoNav = false;

    // Use this for initialization
    void Start()
    {
        state = SkillBridge.Message.CharacterState.Idle;
        if (this.character == null)
        {
            DataManager.Instance.Load();
            NCharacterInfo cinfo = new NCharacterInfo();
            cinfo.Id = 1;
            cinfo.Name = "Test";
            cinfo.ConfigId = 1;
            cinfo.Entity = new NEntity();
            cinfo.Entity.Position = new NVector3();
            cinfo.Entity.Direction = new NVector3();
            cinfo.Entity.Direction.X = 0;
            cinfo.Entity.Direction.Y = 100;
            cinfo.Entity.Direction.Z = 0;
            this.character = new Creature(cinfo);

            if (entityController != null) entityController.entity = this.character;

            this.mainPlayerCamera = GameObject.FindObjectOfType<MainPlayerCamera>();

            //自动寻路
            if(navMeshAgent == null)
            {
                navMeshAgent = this.gameObject.AddComponent<NavMeshAgent>();
                //提前停止防止目标为npc导致重合穿模
                navMeshAgent.stoppingDistance = 0.3f;
            }
        }
    }


    void FixedUpdate()
    {
        //无角色，或正在打字，不控制角色
        if (character == null || InputManager.Instance.IsInputMode)
            return;

        //input轴向
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        //设置角色动作
        if (v > 0.01 || h > 0.01 || h < -0.01)
        {
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                state = SkillBridge.Message.CharacterState.Move;
                this.character.MoveForward();
                this.SendEntityEvent(EntityEvent.MoveFwd);
            }
            // this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else if (v < -0.01)
        {
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                state = SkillBridge.Message.CharacterState.Move;
                this.character.MoveForward();
                this.SendEntityEvent(EntityEvent.MoveBack);
            }
            //this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else
        {
            if (state != SkillBridge.Message.CharacterState.Idle)
            {
                state = SkillBridge.Message.CharacterState.Idle;
                this.rb.velocity = Vector3.zero;
                this.character.Stop();
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }
       

        // 实现相对相机方向而非角色方向的移动
        Vector3 targetDirection = new Vector3(h, 0, v);
        // 摄像机看向玩家，就会有相对世界的旋转角度 y 
        float y = Camera.main.transform.rotation.eulerAngles.y;
        // 让目标方向绕y轴（世界垂直）旋转y（相机旋转角度）度
        targetDirection = Quaternion.Euler(0, y, 0) * targetDirection;
        targetDirection = targetDirection.normalized;

        // 这段可以使角色持续面朝摄像机反方向
        // this.rb.transform.rotation = Quaternion.Slerp(this.rb.transform.rotation, Quaternion.Euler(0,y,0), 5.0f * Time.deltaTime);

        if (lockMode)
        {
            // 使角色面朝目标方向
            Vector3 targetDir = targetLocked.transform.position - this.rb.transform.position;
            targetDir = Vector3.ProjectOnPlane(targetDir, Vector3.up);
            this.rb.transform.rotation = Quaternion.Slerp(this.rb.transform.rotation, Quaternion.LookRotation(targetDir), 9.0f * Time.deltaTime);
        }
        else
        {
            if (h > 0.01 || h < -0.01 || v > 0.01 || v < -0.01)
            {
                // 角色面朝自身前进方向
                this.rb.transform.rotation = Quaternion.Slerp(this.rb.transform.rotation, Quaternion.LookRotation(targetDirection), 8.0f * Time.deltaTime);
                //TODO: 向服务器同步角色朝向

            }
        }

        // 执行移动
        this.rb.transform.Translate(targetDirection * Time.deltaTime * this.character.speed / 30f, Space.World);

        if (Input.GetButtonDown("Jump"))
        {
            this.SendEntityEvent(EntityEvent.Jump);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!this.lockMode)
            {
                var targets = GetTargets();
                if (targets.Count > 0)
                {
                    this.targetLocked = targets[0];
                    this.mainPlayerCamera.targetLocked = targets[0];
                    this.lockMode = true;
                    this.mainPlayerCamera.lockMode = true;
                }
            }
            else
            {
                this.lockMode = false;
                this.mainPlayerCamera.lockMode = false;
            }
        }



        /*        if (h<-0.1 || h>0.1)
                {
                    this.transform.Rotate(0, h * rotateSpeed, 0);
                    Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
                    Quaternion rot = new Quaternion();
                    rot.SetFromToRotation(dir, this.transform.forward);

                    if(rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
                    {
                        character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                        rb.transform.forward = this.transform.forward;
                        this.SendEntityEvent(EntityEvent.None);
                    }

                }*/
        //Debug.LogFormat("velocity {0}", this.rb.velocity.magnitude);
    }

    private List<Transform> GetTargets()
    {
        List<Transform> listTargets = new List<Transform>();
        RaycastHit[] raycastHits = Physics.SphereCastAll(this.rb.position, 200f, this.rb.transform.forward);

        foreach (var target in raycastHits)
        {
            if (target.transform.CompareTag("Enemy"))
            {
                listTargets.Add(target.transform);
            }
        }
        return listTargets;
    }


    Vector3 lastPos;
    float lastSync = 0;
    private void LateUpdate()
    {
        if (this.character == null) return;

        Vector3 offset = this.rb.transform.position - lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        //Debug.LogFormat("LateUpdate velocity {0} : {1}", this.rb.velocity.magnitude, this.speed);
        this.lastPos = this.rb.transform.position;

        // 当角色位置和服务器位置偏移过大，进行重新定位
        if ((GameObjectTool.WorldToLogic(this.rb.transform.position) - this.character.position).magnitude > 50)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }
        this.transform.position = this.rb.transform.position;
    }

    //自动寻路
    public void StartNav(Vector3 target)
    {
        StartCoroutine(BeginNav(target));
    }

    public void StopNav()
    {
        autoNav = false;
        navMeshAgent.ResetPath();
        if(state  != SkillBridge.Message.CharacterState.Idle)
        {
            state = SkillBridge.Message.CharacterState.Idle;
            this.rb.velocity= Vector3.zero;
            this.character.Stop();
            this.SendEntityEvent(EntityEvent.Idle);
        }
    }
    public void NavMove()
    {
        if (navMeshAgent.pathPending) return;
    }

    IEnumerator BeginNav(Vector3 target)
    {
        navMeshAgent.SetDestination(target);
        yield return null;
        autoNav = true;
        if(state != SkillBridge.Message.CharacterState.Move)
        {
            state = SkillBridge.Message.CharacterState.Move;
            this.character.MoveForward();
            this.SendEntityEvent(EntityEvent.MoveFwd);
            navMeshAgent.speed = this.character.speed / 50f;
        }
    }

    void SendEntityEvent(EntityEvent entityEvent)
    {
        if (entityController != null)
            entityController.OnEntityEvent(entityEvent);
        MapService.Instance.SendMapEntitySync(entityEvent, this.character.EntityData);
    }
}
