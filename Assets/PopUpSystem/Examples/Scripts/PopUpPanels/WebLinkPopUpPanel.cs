using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebLinkPopUpPanel : AbstractPopUpPanel
{
	public string webLink = "https://sujaygchand.github.io/";

	protected override void Awake()
	{
		base.Awake();

		internalConfirmButtonCallback += OpenWebLink;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		internalConfirmButtonCallback -= OpenWebLink;
	}

	public override void DisplayPopUpContent(PopUpPanelData popUpPanelData)
	{
		base.DisplayPopUpContent(popUpPanelData);

		if (popUpPanelData.data.Length < 1)
			return;

		webLink = popUpPanelData.data[0].ToString();
	}

	private void OpenWebLink(AbstractPopUpPanel self)
	{
		Application.OpenURL(webLink);
	}
}
