using Assets.Scripts.Managers;
using Entities;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoSingleton<GameObjectManager>
{

    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();

    // 单例类中不能使用原本的start函数
    protected override void OnStart()
    {
        Debug.LogFormat("GameObjectManager的OnStart被调用了");
        StartCoroutine(InitGameObjects());
        // 监听角色管理器，当角色进入，创建实体
        CharacterManager.Instance.OnCharacterEnter = OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave = OnCharacterLeave;
    }

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave = OnCharacterLeave;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCharacterEnter(Character cha)
    {
        CreateCharacterObject(cha);
    }

    private void OnCharacterLeave(Character cha)
    {
        if (!Characters.ContainsKey(cha.entityId))
        {
            return;
        }
        if (Characters[cha.entityId] != null)
        {
            // 删除游戏物体
            Destroy(Characters[cha.entityId]);
            // 删除管理器中的物体
            this.Characters.Remove(cha.entityId);
        }
    }

    IEnumerator InitGameObjects()
    {
        Debug.LogFormat("IEnumerator InitGameObjects被调用了");
        foreach (var cha in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacterObject(cha);
            yield return null;
        }
    }


    /**
    * 创建角色游戏物体，并进行初始化
    */
    private void CreateCharacterObject(Character character)
    {
        // 是否已经存在，避免重复创建
        Debug.LogFormat("CreatCharacterObject ,ID:{0},Name:{1}", character.entityId, character.Name);
        Debug.LogFormat("GameObjectManager开始创建角色物体，目前已经有{0}个角色/怪物", Characters.Count);
        if (!Characters.ContainsKey(character.entityId) || Characters[character.entityId] == null)
        {
            Object obj = Resloader.Load<Object>(character.Define.Resource);
            if (obj == null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID, character.Define.Resource);
                return;
            }
            GameObject go = (GameObject)Instantiate(obj, this.transform);
            go.name = "Character_" + character.Id + "_" + character.Name;
            Characters[character.entityId] = go;

            // 添加玩家头顶UI
            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, character);
        }
        this.InitGameObject(Characters[character.entityId], character);
        Debug.LogFormat("GameObjectManager创建角色物体完毕，目前共有{0}", Characters.Count);
    }

    private void InitGameObject(GameObject go, Character character)
    {
        // 初始化位置
        go.transform.position = GameObjectTool.LogicToWorld(character.position);
        go.transform.forward = GameObjectTool.LogicToWorld(character.direction);

        // 绑定控制器
        EntityController ec = go.GetComponent<EntityController>();
        if (ec != null)
        {
            ec.entity = character;
            ec.isPlayer = character.IsCurrentPlayer;
        }

        PlayerInputController pc = go.GetComponent<PlayerInputController>();
        if (pc != null)
        {
            // 为玩家控制角色绑定玩家控制器和摄像机
            if (character.IsCurrentPlayer)
            {
                User.Instance.CurrentCharacterObject = go;
                MainPlayerCamera.Instance.player = go;
                pc.enabled = true;
                pc.character = character;
                pc.entityController = ec;
            }
            else
            {
                pc.enabled = false;
            }
        }

    }

}

