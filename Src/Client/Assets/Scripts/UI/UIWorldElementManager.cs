using Assets.Scripts.Managers;
using Entities;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager>
{
    public GameObject nameBarPrefab;
    public GameObject npcQuestStatusPrefab;

    private Dictionary<Transform, GameObject> playerNameBar = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, GameObject> npcQuestStatus = new Dictionary<Transform, GameObject>();

    protected override void OnStart()
    {
        nameBarPrefab.SetActive(false);
    }

    void Update()
    {

    }


    public void AddCharacterNameBar(Transform owner, Creature character)
    {
        GameObject goNameBar = Instantiate(nameBarPrefab, this.transform);
        goNameBar.name = "NameBar" + character.entityId;
        goNameBar.GetComponent<UIWorldElement>().owner = owner;
        goNameBar.GetComponent<UINameBar>().character = character;
        goNameBar.SetActive(true);
        this.playerNameBar[owner] = goNameBar;
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.playerNameBar.ContainsKey(owner))
        {
            Destroy(this.playerNameBar[owner]);
            this.playerNameBar.Remove(owner);
        }
    }

    public void AddNpcQuestStatus(Transform owner, NpcQuestStatus status)
    {
        if (this.npcQuestStatus.ContainsKey(owner))
        {
            npcQuestStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        }
        else
        {
            GameObject go = Instantiate(npcQuestStatusPrefab, this.transform);
            go.name = "NpqQuestStatus" + owner.name;
            go.GetComponent<UIWorldElement>().owner = owner;
            go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            go.SetActive(true);
            this.npcQuestStatus[owner] = go;
        }

    }

    public void RemoveNpcQuestStatus(Transform owner)
    {
        if (this.npcQuestStatus.ContainsKey(owner))
        {
            Destroy(this.npcQuestStatus[owner]);
            this.npcQuestStatus.Remove(owner);
        }
    }
}
