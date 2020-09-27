using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleUIPanel : AbstractUIPanel
{
    public Button lowPriorityButton;
    public Button mediumPriorityButton;
    public Button webLinkButton;
    public Button errorButton;

    [SerializeField]
    private string webLinkURL = "https://sujaygchand.github.io/";

    [SerializeField]
    private PopUpManager popUpManager;

    public PopUpManager PopUpManager
    {
        get {
            if (popUpManager == null)
                popUpManager = FindObjectOfType<PopUpManager>();

            return popUpManager;
        }
    }

    private void Awake()
    {
        lowPriorityButton?.onClick.AddListener(OpenLowPriorityPopUp);
        mediumPriorityButton?.onClick.AddListener(OpenMediumPriorityPopUp);
        webLinkButton?.onClick.AddListener(OpenWebLinkPopUp);
        errorButton?.onClick.AddListener(TriggerError);
	}

    private void OnDestroy()
    {
		lowPriorityButton?.onClick.RemoveListener(OpenLowPriorityPopUp);
		mediumPriorityButton?.onClick.RemoveListener(OpenMediumPriorityPopUp);
		webLinkButton?.onClick.RemoveListener(OpenWebLinkPopUp);
		errorButton?.onClick.RemoveListener(TriggerError);
	}

    private void OpenLowPriorityPopUp()
    {
        PopUpManager?.DispatchPopUp<LowPriorityUpPanel>("Low Priority", "This will be ignored if any other pop up is triggered", PopUpManager.PopUpPriority.LowPriority);
    }

	private void OpenMediumPriorityPopUp()
	{
		PopUpManager?.DispatchPopUp<BasePopUpPanel>("Medium Priority", "This will be stacked if a high priority pop up is triggered");
	}

	private void OpenWebLinkPopUp()
	{
		PopUpManager?.DispatchPopUp<WebLinkPopUpPanel>("Web Link", "This will open a link to my website", PopUpManager.PopUpPriority.MediumPriority, null, null, null, new object[] { webLinkURL });
	}

    // Used to trigger an error to show on log received in action
	private void TriggerError()
	{
        AbstractPopUpPanel abstractPopUpPanel = null;
        AbstractPopUpPanel.PopUpPanelData popUpPanelData = null;

        abstractPopUpPanel.DisplayPopUpContent(popUpPanelData);

    }
}
