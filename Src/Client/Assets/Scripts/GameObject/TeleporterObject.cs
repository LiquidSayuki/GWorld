using Common.Data;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterObject : MonoBehaviour {

	public int ID;
	Mesh mesh = null;

	void Start () {
		this.mesh= this.GetComponent<MeshFilter>().sharedMesh;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		if(this.mesh != null)
		{
			Gizmos.DrawWireMesh(this.mesh, this.transform.position + Vector3.up * this.transform.localScale.y * 0.5f, this.transform.rotation, this.transform.localScale);
		}
		UnityEditor.Handles.color = Color.red;
		UnityEditor.Handles.ArrowHandleCap(0, this.transform.position, this.transform.rotation, 1f, EventType.Repaint);
	}
#endif

	void OnTriggerEnter(Collider other)
	{
		PlayerInputController pc = other.GetComponent<PlayerInputController>();
		if(pc != null && pc.isActiveAndEnabled) 
		{
			TeleporterDefine td = DataManager.Instance.Teleporters[this.ID];
			if(td == null)
			{
				Debug.LogErrorFormat("TeleporterObject ID:{0} - Character Name{1}: ERROR: Teleporter Not Exisit", this.ID,  pc.character.Name);
				return;
			}
			Debug.LogFormat("TeleporterObject ID:{0} - Character Name{1}", this.ID, pc.character.Name);
			if(td.LinkTo > 0)
			{
				if (DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
				{
					MapService.Instance.SendMapTeleport(this.ID);
				}
				else
				{
                    Debug.LogErrorFormat("Teleporter {0} - to -  Teleporter {1}: ERROR: Teleporter Not Exisit", this.ID, td.LinkTo);
                }
			}
		}
	}
}
