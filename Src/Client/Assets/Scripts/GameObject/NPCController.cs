using Common.Data;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

public class NPCController : MonoBehaviour {
	public int npcID;
	SkinnedMeshRenderer renderer;
	Animator anim;
	NPCDefine npc;
	Color orignColor;
	private bool inInteractive = false;

	// Use this for initialization
	void Start () {
		renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		anim = this.gameObject.GetComponentInChildren<Animator>();
		npc = NPCManager.Instance.GetNPCDefine(npcID);
		orignColor = renderer.materials[0].color;
		this.StartCoroutine(Actions());
	}
	
	IEnumerator Actions()
	{
		while (true)
		{
			if (inInteractive)
				yield return new WaitForSeconds(2f);
			else
				yield return new WaitForSeconds(Random.Range(5f, 10f));
			this.Relax();
		}
	}

	void Relax()
	{
		anim.SetTrigger("Relax");
	}

	void Interactive()
	{
		if (!inInteractive)
		{
			inInteractive = true;
			StartCoroutine(DoInteractive());
		}
	}

	IEnumerator DoInteractive()
	{
		yield return FaceToPlay();
		if (NPCManager.Instance.Interactive(npc))
		{
			anim.SetTrigger("Talk");
		}
		yield return new WaitForSeconds(3f);
		inInteractive = false;
        anim.SetTrigger("Idle");
    }

	IEnumerator FaceToPlay()
	{
		Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
		while(Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward,faceTo)) > 5)
		{
			this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
			yield return null;
		}
	}

	void OnMouseDown() 
	{
		Interactive();
	}

	void OnMouseOver()
	{
		Highlight(true);
	}

	void OnMouseEnter()
	{
		Highlight(true);
	}

	void OnMouseExit()
	{
		Highlight(false);
	}

	void Highlight(bool highlight)
	{
        if (highlight)
        {
            if (renderer.materials[0].color != Color.white)
				renderer.materials[0].color = Color.white;
        }
		else
		{
			if(renderer.materials[0].color != Color.black)
				renderer.materials[0].color = Color.black;
		}
    }
}
