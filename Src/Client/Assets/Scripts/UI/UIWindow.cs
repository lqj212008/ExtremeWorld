using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIWindow : MonoBehaviour,IDragHandler
{
	public enum WindowResult { None = 0, Yes, No, }
	public delegate void CloseHandler(UIWindow sender, WindowResult result);
	public event CloseHandler OnClose;
	private RectTransform m_rectTransform;
	public GameObject Root;
	public RectTransform m_RectTransform
	{
		get
		{
			Transform panel = this.transform.Find("Bg");
			if (m_rectTransform == null)
				m_rectTransform = panel.gameObject.GetComponent<RectTransform>();
            return m_rectTransform;
		}
		set
		{
            m_rectTransform = value;

        }
	}

    public virtual System.Type Type { get { return this.GetType(); } }

	public void Close(WindowResult result = WindowResult.None)
	{
		UIManager.Instance.Close(this.Type);
		if(this.OnClose != null)
			this.OnClose(this, result);
		this.OnClose = null;
	}

	public virtual void OnCloseClick()
	{
		this.Close();
	}

	public virtual void OnYesClick()
	{
		this.Close(WindowResult.Yes);
	}

	public virtual void OnNoClick()
	{
		this.Close(WindowResult.No);
	}

	void OnMouseDown()
	{
		Debug.LogFormat(this.name + " Clicked");
	}

    public void OnDrag(PointerEventData eventData)
    {
		if(eventData.button != PointerEventData.InputButton.Left)
			return;
		m_RectTransform.anchoredPosition += eventData.delta;
    }
}
