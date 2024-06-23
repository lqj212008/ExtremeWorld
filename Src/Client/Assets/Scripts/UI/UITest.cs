using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : UIWindow {

	public Text titleBar;


	public void SetTitle(string title)
	{
		this.titleBar.text = title;
	}

}
