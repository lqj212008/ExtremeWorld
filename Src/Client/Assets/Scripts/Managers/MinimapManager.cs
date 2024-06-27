using Common.Data;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    class MinimapManager : Singleton<MinimapManager>
	{
		public UIMinimap minimap;
		public MapDefine mDefine
		{
			get
			{
				if(User.Instance.CurrentMapData == null)
					return null;
				return User.Instance.CurrentMapData;

            }
		}
		public Transform PlayerTransform
		{
			get
			{
				if(User.Instance.CurrentCharacterObject == null)
					return null;
				return User.Instance.CurrentCharacterObject.transform;
			}
		}
		public Sprite LoadCurrentMinimap()
		{
			return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.Minimap);
		}

		public void UpdateMinimap()
		{
			if(this.minimap != null)
				this.minimap.UpdateMap();
		}

		
	}
}


