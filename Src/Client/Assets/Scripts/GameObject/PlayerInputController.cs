using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities;
using SkillBridge.Message;
public class PlayerInputController : MonoBehaviour {

	public Rigidbody rb;

	CharacterState state;

	public Character character;

	public float rotateSpeed = 2.0f;

	public float turnAngle = 10;

	public int speed;

	public EntityController entityController;

	public bool onAir = false;

	// Use this for initialization
	void Start () {
		state  = CharacterState.Idle;
		if(this.character == null)
		{
			DataManager.Instance.Load();
			NCharacterInfo cInfo  = new NCharacterInfo();
			cInfo.Id = 1;
			cInfo.Name = "Test";
			cInfo.Tid = 1;
			cInfo.Entity = new NEntity();
			cInfo.Entity.Position = new NVector3();
			cInfo.Entity.Direction = new NVector3();
			cInfo.Entity.Direction.X = 0;
			cInfo.Entity.Direction.Y = 100;
			cInfo.Entity.Direction.Z = 0;
			this.character = new Character(cInfo);

			if(entityController != null) entityController.entity = this.character;
		}
	}
	
	void FixedUpdate()
	{
		if(character == null)
			return;

		float v = Input.GetAxis("Vertical");
		if(v > 0.1)
		{
			if (state != CharacterState.Move)
			{
				state = CharacterState.Move;
				this.character.MoveForward();
				this.SendEntityEvent(EntityEvent.MoveFwd);
			}
			this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
		}
		else if( v < -0.01)
		{
			if(state != CharacterState.Move)
			{
				state = CharacterState.Move;
				this.character.MoveBack();
				this.SendEntityEvent(EntityEvent.MoveBack);
			}
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
		else
		{
			if(state != CharacterState.Idle)
			{
				state = CharacterState.Idle;
				this.rb.velocity = Vector3.zero;
				this.character.Stop();
				this.SendEntityEvent(EntityEvent.Idle);
			}
		}

		if (Input.GetButtonDown("Jump"))
		{
			this.SendEntityEvent(EntityEvent.Jump);
		}

		float h = Input.GetAxis("Horizontal");
		if (h < -0.1 || h > 0.1)
		{
			this.transform.Rotate(0, h * rotateSpeed, 0);
			Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
			Quaternion rot = new Quaternion();
			rot.SetFromToRotation(dir, this.transform.forward);

			if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
			{
				character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
				rb.transform.forward = this.transform.forward;
				this.SendEntityEvent(EntityEvent.None);
			}
		}
	}

	Vector3 lastPos;
	float lastSync = 0;

	void LateUpdate()
	{
		Vector3 offset = this.rb.transform.position - lastPos;
		this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
		//Debug.LogFormat("LateUpdate velocity {0}: {1}",this,rb.velocity.magnitude,this.speed);
		this.lastPos = this.rb.transform.position;

		if ((GameObjectTool.WorldToLogic(this.rb.transform.position) - this.character.position).magnitude > 80)
		{
			this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
			this.SendEntityEvent(EntityEvent.None);
		}
		this.transform.position = this.rb.transform.position;
	}
	
	void SendEntityEvent(EntityEvent entityEvent)
	{
		if(entityController != null)
		{
			entityController.OnEntityEvent(entityEvent);
		}
	}

}

