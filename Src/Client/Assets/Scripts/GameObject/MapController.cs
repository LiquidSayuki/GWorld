using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

public class MapController : MonoBehaviour {

	public BoxCollider minimapBoundingBox;

	void Start () {
		MinimapManager.Instance.UpdateMinimap(minimapBoundingBox);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
