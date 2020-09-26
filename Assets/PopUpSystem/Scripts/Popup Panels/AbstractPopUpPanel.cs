using System.Collections;
using System.Collections.Generic;
#if TM_PRO
using TMPro;
#endif
using UnityEngine;
using UnityEngine.UI;

public class AbstractPopUpPanel : AbstractUIPanel
{
	[System.Serializable]
	public class PopUpPanelData
	{
		public AbstractUIPanel ownerPanel;
		public AbstractPopUpPanel displayPanel;

		public string title;
		public string message;

		public OnUIButtonPressed confirmButtonPressedCallback;
		public OnUIButtonPressed cancelButtonPressedCallback;

		public object[] data;
	}

	public PopUpPanelData popUpPanelData;

	public delegate void OnUIButtonPressed(AbstractPopUpPanel self);
	public OnUIButtonPressed confirmButtonPressedCallback;
	public OnUIButtonPressed cancelButtonPressedCallback;

#if TM_PRO
	[SerializeField]
	protected TextMeshProUGUI titleText;

	[SerializeField]
	protected TextMeshProUGUI messageText;

#else
	[SerializeField]
	protected Text titleText;

	[SerializeField]
	protected Text messageText;
#endif

	[SerializeField]
	protected Button confirmButton;

	[SerializeField]
	protected Button cancelButton;

	protected virtual void Awake()
	{

		Initialise();

		confirmButton?.onClick.AddListener(OnConfirmButtonPressed);
		cancelButton?.onClick.AddListener(OnCancelButtonPressed);
	}



	protected void OnConfirmButtonPressed()
	{
		Hide();
		confirmButtonPressedCallback?.Invoke(this);
	}

	protected void OnCancelButtonPressed()
	{
		Hide();
		cancelButtonPressedCallback?.Invoke(this);
	}

	public virtual void DisplayPopUpContent(PopUpPanelData popUpPanelData)
	{
		this.popUpPanelData = popUpPanelData;

		if (titleText)
			titleText.text = this.popUpPanelData.title;

		if (messageText)
			messageText.text = this.popUpPanelData.message;

		confirmButtonPressedCallback += popUpPanelData.confirmButtonPressedCallback;
		cancelButtonPressedCallback += popUpPanelData.cancelButtonPressedCallback;

		Show();
	}

	public void Reset()
	{
		popUpPanelData = null;

		if (canvasGroup == null)
			return;

		canvasGroup.alpha = 1;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
	}

}
