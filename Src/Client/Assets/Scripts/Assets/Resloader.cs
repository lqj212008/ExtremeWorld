using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 资源加载类，用来进行资源自动加载。
 */

class Resloader : MonoBehaviour {

	public static T Load<T>(string path) where T : UnityEngine.Object 
	{
		return Resources.Load<T>(path);
	}

}
