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

	private void OpenWebLink(AbstractPopUpPanel self)
	{
		Application.OpenURL(webLink);
	}
}
