using Common.Data;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour {

	public Image minimap;
	public Image arrow;
	public Text mapName;
	public Vector2Int realSize;
	private Transform playerTransform;
	// Use this for initialization
	void Start () {
		this.InitMap();
		
	}

	void InitMap()
	{
		MapDefine mapDefine = User.Instance.CurrentMapData;
        this.mapName.text = mapDefine.Name;
		if(this.minimap.overrideSprite == null)
		{
			this.minimap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap();
		}
		this.minimap.SetNativeSize();
		this.minimap.transform.localPosition = Vector3.zero;
        
        realSize = new Vector2Int(mapDefine.Size[0], mapDefine.Size[1]);
		this.playerTransform  = User.Instance.CurrentCharacterObject.transform;
    }
	
	// Update is called once per frame
	void Update () {
		if(this.playerTransform == null) return;

		float pivotX = playerTransform.position.x / (realSize.x / 100);
		float pivotY = playerTransform.position.z / (realSize.y / 100);

		this.minimap.rectTransform.pivot = new Vector2 (pivotX, pivotY);
		this.minimap.rectTransform.localPosition = Vector2.zero;

		this.arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);
	}
}
