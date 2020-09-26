using System;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{

	public enum PopUpPriority
	{
		///<summary>Ignore this, if another pop up is shown</summary>
		LowPriority,
		///<summary>Adds the pop up to a stack to show after the current pop has been closed</summary>
		MediumPriority,
		///<summary>top of the stack</summary>
		HighPriority
	}


	[SerializeField]
	private List<AbstractPopUpPanel> popUpPrefabList = new List<AbstractPopUpPanel>();

	private List<AbstractPopUpPanel> availablePopUpObjects = new List<AbstractPopUpPanel>();

	private AbstractPopUpPanel activePopUp = null;

	[SerializeField]
	private AbstractPopUpPanel defaultPopUpPrefab;

	private LinkedList<AbstractPopUpPanel.PopUpPanelData> popUpDataStack = new LinkedList<AbstractPopUpPanel.PopUpPanelData>();

	public Canvas canvas;

	private void Update()
	{
		availablePopUpObjects.RemoveAll(item => item == null);
	}

	private void ClosePopUpWindow(AbstractPopUpPanel abstractPopUpPanel)
	{
		abstractPopUpPanel.Hide();
		abstractPopUpPanel.Reset();
		IterateThroughStack();
	}

	public void CloseActivePopUp()
	{
		if (activePopUp == null)
			return;

		ClosePopUpWindow(activePopUp);
	}

	public void DispatchPopUp<T>(string title, string message, PopUpPriority popUpPriority = PopUpPriority.MediumPriority, AbstractUIPanel ownerPanel = null, AbstractPopUpPanel.OnUIButtonPressed confirmButtonPressedCallback = null, AbstractPopUpPanel.OnUIButtonPressed cancelButtonPressedCallback = null, params object[] data) where T : AbstractPopUpPanel
	{
		var popUpPanelData = new AbstractPopUpPanel.PopUpPanelData();

		popUpPanelData.title = title;
		popUpPanelData.message = message;
		popUpPanelData.confirmButtonPressedCallback = confirmButtonPressedCallback;
		popUpPanelData.cancelButtonPressedCallback = cancelButtonPressedCallback;
		popUpPanelData.ownerPanel = ownerPanel;
		popUpPanelData.data = data;

		DispatchPopUp<T>(popUpPanelData, popUpPriority);
	}

	public void DispatchPopUp<T>(AbstractPopUpPanel.PopUpPanelData popUpPanelData, PopUpPriority popUpPriority = PopUpPriority.MediumPriority) where T : AbstractPopUpPanel
	{
		var popUpPrefab = popUpPrefabList.Find(item => item.GetType() == typeof(T));

		if (popUpPrefab == null && defaultPopUpPrefab == null)
		{
			Debug.LogError("No defaultPopUpPrefab is set");
			return;
		}

		AbstractPopUpPanel displayPanelPrefab = popUpPrefab ?? defaultPopUpPrefab;

		var scenePanelObject = availablePopUpObjects.Find(item => item.name == displayPanelPrefab.name);

		switch (popUpPriority)
		{
			case PopUpPriority.LowPriority:
				DisplayLowPriorityPopUp(popUpPanelData, displayPanelPrefab, scenePanelObject);
				break;
			case PopUpPriority.MediumPriority:
				DisplayMediumPriorityPopUp(popUpPanelData, displayPanelPrefab, scenePanelObject, popUpPriority);
				break;
			case PopUpPriority.HighPriority:
				DisplayHighPriorityPopUp(popUpPanelData, displayPanelPrefab, scenePanelObject, popUpPriority);
				break;
		}
	}

	private void DisplayLowPriorityPopUp(AbstractPopUpPanel.PopUpPanelData popUpPanelData, AbstractPopUpPanel displayPanelPrefab, AbstractPopUpPanel scenePanelObject)
	{
		if (activePopUp)
			return;

		displayPanelPrefab = InstantiatePopUp(displayPanelPrefab, scenePanelObject);

		DisplayPopup(displayPanelPrefab, popUpPanelData);
	}

	private void DisplayMediumPriorityPopUp(AbstractPopUpPanel.PopUpPanelData popUpPanelData, AbstractPopUpPanel displayPanelPrefab, AbstractPopUpPanel scenePanelObject, PopUpPriority popUpPriority)
	{
		displayPanelPrefab = InstantiatePopUp(displayPanelPrefab, scenePanelObject);

		if (activePopUp)
		{
			AddToDataStack(displayPanelPrefab, popUpPanelData, popUpPriority);
		}
		else
		{
			DisplayPopup(displayPanelPrefab, popUpPanelData);
		}
	}

	private void DisplayHighPriorityPopUp(AbstractPopUpPanel.PopUpPanelData popUpPanelData, AbstractPopUpPanel displayPanelPrefab, AbstractPopUpPanel scenePanelObject, PopUpPriority popUpPriority)
	{
		displayPanelPrefab = InstantiatePopUp(displayPanelPrefab, scenePanelObject);

		if (activePopUp)
		{
			AddToDataStack(activePopUp, activePopUp.popUpPanelData, popUpPriority);
		}

		DisplayPopup(displayPanelPrefab, popUpPanelData);
	}

	private void DisplayPopup(AbstractPopUpPanel displayPanel, AbstractPopUpPanel.PopUpPanelData popUpPanelData)
	{
		if (canvas == null)
			canvas = FindObjectOfType<Canvas>();
		
		displayPanel.transform.parent = canvas.transform;
		displayPanel.transform.localPosition = Vector3.zero;
		displayPanel.transform.localRotation = Quaternion.identity;
		displayPanel.transform.localScale = Vector3.one;

		displayPanel.DisplayPopUpContent(popUpPanelData);

		BindPopUpWindowDelegates(displayPanel, popUpPanelData.confirmButtonPressedCallback, popUpPanelData.cancelButtonPressedCallback);

		if (activePopUp != displayPanel)
		{
			if (activePopUp)
			{
				activePopUp.Hide();
			}

			activePopUp = displayPanel;
		}
	}

	private void AddToDataStack(AbstractPopUpPanel displayPanel, AbstractPopUpPanel.PopUpPanelData popUpPanelData, PopUpPriority popUpPriority)
	{
		popUpPanelData.displayPanel = displayPanel;

		// Add high priority to the front of the stack of incoming priority is high priority
		if (popUpPriority == PopUpPriority.HighPriority)
		{
			popUpDataStack.AddFirst(popUpPanelData);
		}
		else
		{
			popUpDataStack.AddLast(popUpPanelData);
		}

		if (activePopUp.gameObject.activeInHierarchy == false)
		{
			activePopUp = null;
			IterateThroughStack();
		}
	}

	private void BindPopUpWindowDelegates(AbstractPopUpPanel displayPanel, AbstractPopUpPanel.OnUIButtonPressed confirmButtonPressed, AbstractPopUpPanel.OnUIButtonPressed cancelButtonPressed)
	{
		if (confirmButtonPressed != null)
			displayPanel.confirmButtonPressedCallback += (panel) => confirmButtonPressed(panel);

		displayPanel.confirmButtonPressedCallback += ClosePopUpWindow;

		if (cancelButtonPressed != null)
			displayPanel.cancelButtonPressedCallback += (panel) => cancelButtonPressed(panel);

		displayPanel.cancelButtonPressedCallback += ClosePopUpWindow;
	}

	/// <summary>
	/// Instantiate if it does not exist in scene
	/// </summary>
	/// <param name="displayPanelPrefab"></param>
	/// <returns></returns>
	private AbstractPopUpPanel InstantiatePopUp(AbstractPopUpPanel displayPanelPrefab, AbstractPopUpPanel scenePanelObject)
	{
		return (scenePanelObject) ? scenePanelObject : InstantiatePopUp(displayPanelPrefab);
	}

	private AbstractPopUpPanel InstantiatePopUp(AbstractPopUpPanel popUp)
	{
		if(canvas == null)
			canvas = FindObjectOfType<Canvas>();

		var popUpPanel = Instantiate(popUp, canvas.transform);
		popUpPanel.gameObject.name = popUp.gameObject.name;

		availablePopUpObjects.Add(popUpPanel);

		return popUpPanel;
	}

	public void IterateThroughStack()
	{
		if (popUpDataStack.Count == 0)
		{
			activePopUp = null;
			return;
		}

		var popUpDataElement = popUpDataStack.First.Value;
		popUpDataStack.RemoveFirst();

		DisplayPopup(popUpDataElement.displayPanel, popUpDataElement);
		activePopUp = popUpDataElement.displayPanel;

	}
}
