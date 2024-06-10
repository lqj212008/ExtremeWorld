using Entities;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameObjectManager : MonoBehaviour {

	Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();

	// Use this for initialization
	void Start () {
		StartCoroutine(InitGameObject());
		CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
	}

	void OnDestroy()
	{
		CharacterManager.Instance.OnCharacterEnter = null;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCharacterEnter(Character character)
	{
		CreateCharacterObject(character);
	}

	IEnumerator InitGameObject()
	{
		foreach (var character in CharacterManager.Instance.Characters.Values) 
		{
			CreateCharacterObject (character);
			yield return null;
		}
	}

	private void CreateCharacterObject(Character character)
	{
		if(!Characters.ContainsKey(character.Info.Id) || Characters[character.Info.Id] == null)
		{
			Object obj = Resloader.Load<Object>(character.Define.Resource);
			if(obj == null)
			{
				Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID,character.Define.Resource);
				return;
			}
			GameObject go = (GameObject)Instantiate(obj);
			go.name = "Character_"+character.Info.Id+"_"+character.Info.Name;
			go.transform.position = GameObjectTool.LogicToWorld(character.position);
			go.transform.forward = GameObjectTool.LogicToWorld(character.direction);
			Characters[character.Info.Id] = go;

			EntityController ec = go.GetComponent<EntityController>();
			if(ec != null)
			{
				ec.entity = character;
				ec.isPlayer = character.IsPlayer;
			}

			PlayerInputController pc = go.GetComponent<PlayerInputController>();
			if(pc != null)
			{
				if(character.Info.Id == Models.User.Instance.CurrentCharacter.Id)
				{
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

			UIWorldElementManager.Instance.AddCharacterNameBar(go.transform,character);
		}
	}
}
