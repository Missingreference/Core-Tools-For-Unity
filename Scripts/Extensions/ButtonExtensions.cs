using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Elanetic.Tools
{
	static public class ButtonExtensions
	{
		/// <summary>
		/// This will add an EventTrigger component if one does not already exist. Destroying that component will make you lose your listener.
		/// </summary>
		/// <param name="button"></param>
		/// <param name="action"></param>
		/// <param name="triggerType"></param>
		static public void AddListener(this Button button, UnityAction action, EventTriggerType triggerType)
		{
			EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
			if(eventTrigger == null) eventTrigger = button.gameObject.AddComponent<EventTrigger>();

			// Create a nee TriggerEvent and add a listener
			EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
			trigger.AddListener((eventData) => action()); // you can capture and pass the event data to the listener

			// Create and initialise EventTrigger.Entry using the created TriggerEvent
			EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };

			// Add the EventTrigger.Entry to delegates list on the EventTrigger
			eventTrigger.triggers.Add(entry);
		}

		/// <summary>
		/// This will add an EventTrigger component if one does not already exist. Destroying that component will make you lose your listener.
		/// </summary>
		/// <param name="button"></param>
		/// <param name="action"></param>
		/// <param name="triggerType"></param>
		static public void AddListener(this Button button, UnityAction<BaseEventData> action, EventTriggerType triggerType)
		{
			EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
			if(eventTrigger == null) eventTrigger = button.gameObject.AddComponent<EventTrigger>();

			// Create a nee TriggerEvent and add a listener
			EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
			trigger.AddListener((eventData) => action(eventData)); // you can capture and pass the event data to the listener

			// Create and initialise EventTrigger.Entry using the created TriggerEvent
			EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };

			// Add the EventTrigger.Entry to delegates list on the EventTrigger
			eventTrigger.triggers.Add(entry);
		}
	}
}