using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;
using System.Security.AccessControl;
using Models;

public class NpcController : MonoBehaviour {

	public int ID;
	Animator animator;
	SkinnedMeshRenderer renderer;
	Color originColor;

	private bool inInteractive = false;

	NpcDefine npc;

	void Start () {
		renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		animator = this.gameObject.GetComponentInChildren<Animator>();
		originColor = renderer.sharedMaterial.color;
		npc = NpcManager.Instance.GetNpcDefine(ID);
		this.StartCoroutine(Actions());
	}

	IEnumerator Actions()
	{
		while (true)
		{
			if (inInteractive)
			{
				yield return new WaitForSeconds(2);
			}
			else
			{
				yield return new WaitForSeconds(Random.Range(5f,10f));
			}
			this.Relax();
		}
	}

    private void Relax()
    {
		animator.SetTrigger("Relax");
    }

    void OnMouseDown()
	{
		Interactive();
	}

	private void Interactive()
	{
		if(! inInteractive)
		{
			inInteractive= true;
			StartCoroutine(DoInteractive());
		}
	}

	IEnumerator DoInteractive()
	{
		yield return FaceToPlayer();
		if (NpcManager.Instance.Interactive(npc))
		{
			animator.SetTrigger("Talk");
		}
		yield return new WaitForSeconds(3);
		inInteractive = false;
	}

    IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceTo)) > 5)
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }

    private void OnMouseOver()
    {
        Highlight(true);
    }
    private void OnMouseEnter()
    {
        Highlight(true);
    }
    private void OnMouseExit()
    {
        Highlight(false);
    }

    private void Highlight(bool highlight)
	{
		if (highlight)
		{
			if (renderer.sharedMaterial.color != Color.white)
				renderer.sharedMaterial.color = Color.white;
		}
		else
		{
            if (renderer.sharedMaterial.color != originColor)
                renderer.sharedMaterial.color = originColor;
        }
	}


}
