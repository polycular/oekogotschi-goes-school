using System;
using System.Collections.Generic;

namespace Polycular
{
	public delegate void OnEventRaised(EventBase ev);

	class ListenerData
	{
		public ListenerData(object listener, OnEventRaised callback)
		{
			Listener = listener;
			Callback = callback;
		}

		public object Listener;
		public OnEventRaised Callback;
	}

	class BufferedAdd
	{
		public BufferedAdd(Type typeinfo, ListenerData data)
		{
			Data = data;
			TypeInfo = typeinfo;
		}

		public ListenerData Data;
		public Type TypeInfo;
	}

	class BufferedRemove
	{
		public BufferedRemove(Type typeinfo, object listener)
		{
			Listener = listener;
			TypeInfo = typeinfo;
		}

		public object Listener;
		public Type TypeInfo;
	}

	public class Eventbus
	{
		static Eventbus ms_instance;

		Queue<BufferedAdd> m_toAdd;
		Queue<BufferedRemove> m_toRemove;
		Stack<bool> m_stackFrames;

		public static Eventbus Instance
		{
			get
			{
				if (ms_instance == null)
				{
					ms_instance = new Eventbus();
				}

				return ms_instance;
			}

			private set { }
		}

		// Holds all callbacks to be called when a certain event is raised.
		Dictionary<Type, List<ListenerData>> m_listeners;

		// Holds the IDs of all events that an object is subscribed to.
		Dictionary<object, List<Type>> m_listenerIDs;

		Eventbus()
		{
			m_listeners = new Dictionary<Type, List<ListenerData>>();
			m_listenerIDs = new Dictionary<object, List<Type>>();
			m_toAdd = new Queue<BufferedAdd>();
			m_toRemove = new Queue<BufferedRemove>();
			m_stackFrames = new Stack<bool>();
		}

		public static void ClearBus()
		{
			ms_instance = new Eventbus();
		}

		public void AddListener<EventType>(OnEventRaised callback, object listener)
		{
			var eventType = typeof(EventType);
			var data = new ListenerData(listener, callback);

			if (m_stackFrames.Count != 0)
			{
				m_toAdd.Enqueue(new BufferedAdd(eventType, data));
				return;
			}

			AddListenerImpl(eventType, data);
		}

		void AddListenerImpl(Type typeInfo, ListenerData data)
		{
			if (!typeInfo.IsSubclassOf(typeof(EventBase)))
			{
				throw new IsNotEventTypeException();
			}

			if (!m_listeners.ContainsKey(typeInfo))
			{
				m_listeners.Add(typeInfo, new List<ListenerData>());
			}

			if (!m_listenerIDs.ContainsKey(data.Listener))
			{
				m_listenerIDs.Add(data.Listener, new List<Type>());
			}

			m_listeners[typeInfo].Add(data);
			m_listenerIDs[data.Listener].Add(typeInfo);
		}

		/// <summary>Unsubscribes listener from all events it is subscribed to.</summary>
		public void RemoveListener(object listener)
		{
			if (m_stackFrames.Count != 0)
			{
				m_toRemove.Enqueue(new BufferedRemove(null, listener));
				return;
			}

			RemoveListenerImpl(null, listener);
		}

		/// <summary>Unsubscribes listener from Events of type EventType.</summary>
		/// <exception cref="IsNotEventTypeException"></exception>
		public void RemoveListener<EventType>(object listener)
		{
			var type = typeof(EventType);

			if (m_stackFrames.Count != 0)
			{
				m_toRemove.Enqueue(new BufferedRemove(type, listener));
				return;
			}

			RemoveListenerImpl(type, listener);
		}

		void RemoveListenerImpl(Type typeInfo, object listener)
		{
			if (!m_listenerIDs.ContainsKey(listener))
			{
				return;
			}

			if (typeInfo != null)
			{
				if (!typeInfo.IsSubclassOf(typeof(EventBase)))
				{
					throw new IsNotEventTypeException();
				}

				m_listeners[typeInfo].RemoveAll(data => data.Listener == listener);
			}
			else
			{
				foreach (var eventID in m_listenerIDs[listener])
				{
					m_listeners[eventID].RemoveAll(data => data.Listener == listener);
				}
			}
		}

		void ProcessBuffered()
		{
			while (m_toAdd.Count != 0)
			{
				var buffered = m_toAdd.Dequeue();
				AddListenerImpl(buffered.TypeInfo, buffered.Data);
			}

			while (m_toRemove.Count != 0)
			{
				var buffered = m_toRemove.Dequeue();
				RemoveListenerImpl(buffered.TypeInfo, buffered.Listener);
			}
		}

		public void FireEvent<EventType>(EventBase ev)
		{
			var type = typeof(EventType);

			if (!m_listeners.ContainsKey(type))
			{
				return;
			}

			m_stackFrames.Push(true);

			foreach (var listener in m_listeners[type])
			{
				listener.Callback(ev);
			}

			m_stackFrames.Pop();

			if (m_stackFrames.Count == 0)
				ProcessBuffered();
		}
	}
}