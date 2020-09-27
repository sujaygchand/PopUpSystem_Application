using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public abstract class AbstractUIPanel : MonoBehaviour
{
	public delegate void OnDisplayPanel(AbstractUIPanel uiPanel);
	public OnDisplayPanel onShowPanel;
	public OnDisplayPanel onHidePanel;

	public RectTransform RectTransform
	{
		get
		{
			return transform as RectTransform;
		}
	}

	public CanvasGroup CanvasGroup
	{
		get
		{
			canvasGroup = canvasGroup ?? gameObject.GetComponent<CanvasGroup>();
			return canvasGroup;
		}

	}

	protected CanvasGroup canvasGroup;
	private bool isInitialised = false;

	public void Initialise()
	{
		if (isInitialised)
			return;

		OnInitialised();
		isInitialised = true;
	}

	protected virtual void OnInitialised() { }

	public void Show()
	{
		gameObject.SetActive(true);
		onShowPanel?.Invoke(this);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		onHidePanel?.Invoke(this);
	}

}
