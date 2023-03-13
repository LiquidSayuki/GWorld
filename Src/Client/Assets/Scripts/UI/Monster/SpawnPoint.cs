using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    Mesh mesh = null;
    public int Id;

    void Start ()
    {
        this.mesh = this.GetComponent<MeshFilter>().sharedMesh;
    }

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Vector3 pos = this.transform.position + Vector3.up * this.transform.localScale.y * 0.5f;
        Gizmos.color = Color.yellow;
        if (this.mesh != null)
        {
            Gizmos.DrawWireMesh(this.mesh, pos, this.transform.rotation, this.transform.localScale);
        }

        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.ArrowHandleCap(0, pos, this.transform.rotation, 1f, EventType.Repaint);
        UnityEditor.Handles.Label(pos, "SpawnPoint:" + this.Id);
    }
#endif
}
