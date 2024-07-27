using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;
using Managers;



public class LoadingManager : MonoBehaviour {
	public GameObject UITip;
	public GameObject UILoading;
	public GameObject UILogin;

	public Slider progressBar;
	public Text progressText;
	public Text progressNumber;

	// Use this for initialization
	IEnumerator Start () {
		log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
		UnityLogger.Init();
		Common.Log.Init("Unity");
		Common.Log.Info("LoadingManager start");

		UITip.SetActive(true);
		UILoading.SetActive(false);
		UILogin.SetActive(false);
		yield return new WaitForSeconds(2f);
		UILoading.SetActive(true);
		yield return new WaitForSeconds(1f);
		UITip.SetActive(false);

		yield return DataManager.Instance.LoadData();

		//Init basic services
		MapService.Instance.Init();
		UserService.Instance.Init();
		StatusService.Instance.Init();
		FriendService.Instance.Init();
		TeamService.Instance.Init();

		ShopManager.Instance.Init();
		ChatService.Instance.Init();

		//Fake Loading Simulate
		for(float i = 50;i<100;)
		{
			i += Random.Range(0.1f, 0.5f);
			progressBar.value = i;
			yield return new WaitForEndOfFrame();
		}

		UILoading.SetActive(false);
		UILogin.SetActive(true);
        UILogin.transform.Find("LoginPanel").gameObject.SetActive(true);
        UILogin.transform.Find("RegisterPanel").gameObject.SetActive(false);
		yield return null;

	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
