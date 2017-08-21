using System.Collections.Generic;
using System.Linq;
using JSONApi.Extensions;
using Polycular.Clustar.Behaviours;
using UnityEngine;

namespace Polycular.Clustar
{
	public class GraphController : MonoBehaviour
	{
		public delegate void BehaviourTaskFinishedHandler(IBehaviour behaviour);

		public string BehaviourSuffix;

		Queue<List<BehaviourTask>> taskQueue;
		CampaignGraph campaignGraph;
		IGraphRoot campaign;


		void Start()
		{
			RegisterEventHandlers();
		}

		public void Init(IGraphRoot campaign)
		{
			if (campaign.First == null)
			{
				Debug.LogError("ERR: Campaign 'First' is null");
				return;
			}
			if (campaign.Current == null)
			{
				campaign.Current = campaign.First;
			}

			this.campaign = campaign;
			campaignGraph = new CampaignGraph(campaign.First, campaign.Nodes);

			ResetController();
			StartController();
		}

		void ResetController()
		{
			if (taskQueue != null && taskQueue.Count > 0)
			{
				// TODO disable all behaviours and clear the queue
				List<BehaviourTask> currentTasks = taskQueue.Peek();

				foreach (BehaviourTask task in currentTasks)
				{
					(task.Behaviour as MonoBehaviour).enabled = false;
				}

				taskQueue.Clear();
			}
		}

		void RegisterEventHandlers()
		{
			var behaviours = GetComponents<IBehaviour>();
			foreach (var behaviour in behaviours)
			{
				behaviour.OnBehaviourTaskFinished -= OnBehaviourTaskFinishedHandler;
				behaviour.OnBehaviourTaskFinished += OnBehaviourTaskFinishedHandler;
			}
		}

		void StartController()
		{
			UpdateQueue(campaign.Current);

			// start iteration through the current node's component behaviours
			Step();
		}

		/// <summary>
		/// Wraps following functions: 'GetBehaviours, GetBehaviourQueue
		/// </summary>
		void UpdateQueue(IGraphNode node)
		{
			List<BehaviourTask> tasks = CreateTasks(node.Components);
			taskQueue = CreateBehaviourQueue(tasks);
		}

		List<BehaviourTask> CreateTasks(List<IGraphComponent> components)
		{
			List<BehaviourTask> tasks = new List<BehaviourTask>();
			int currentOrderId = campaign.Current.CurrentOrderId;

			foreach (IGraphComponent component in components)
			{
				// check if the component was already processed
				if (component.OrderId < currentOrderId)
					continue;

				string componentType = component.Type.ToPascalCase('-');
				string behaviourName = componentType + BehaviourSuffix;

				// Check if there is a matching MonoBehaviour attached
				var monoBehaviour = GetComponent(behaviourName);
				if (monoBehaviour != null)
				{
					IBehaviour iBehaviour = monoBehaviour as IBehaviour;
					iBehaviour.NodeName = campaign.Current.Name;

					tasks.Add(new BehaviourTask(iBehaviour, component));
				}
				else
				{
					// graceful degradation if there are new / disabled (or removed) component types
					Debug.LogWarning(string.Format("{0}: ComponentType={1} -> Behaviour isn't attached!", GetType().Name, componentType));
				}
			}

			return tasks;
		}

		Queue<List<BehaviourTask>> CreateBehaviourQueue(List<BehaviourTask> tasks)
		{
			Queue<List<BehaviourTask>> queue = new Queue<List<BehaviourTask>>();

			// sort tasks by OrderId
			tasks.Sort((a, b) => (a.Component.OrderId).CompareTo(b.Component.OrderId));

			List<BehaviourTask> currentList = new List<BehaviourTask>();
			int currentOrderId = int.MinValue;

			foreach (BehaviourTask task in tasks)
			{
				if (currentList.Count == 0)
				{
					currentOrderId = task.Component.OrderId;
					currentList.Add(task);
					continue;
				}

				if (task.Component.OrderId == currentOrderId)
				{
					// Add to list if it does have the same OrderId
					currentList.Add(task);
				}
				else
				{
					// Enqueue the tmpList, create a new one because of the new OrderId and add the task
					queue.Enqueue(currentList);
					currentList = new List<BehaviourTask>();
					currentOrderId = task.Component.OrderId;
					currentList.Add(task);
				}
			}

			// enqueue current tmpList if not empty
			if (currentList.Count > 0)
				queue.Enqueue(currentList);

			return queue;
		}

		IGraphNode GetCurrentNode()
		{
			if (campaign.Current == null)
			{
				campaign.Current = campaign.First;
			}

			return campaign.Current;
		}

		IGraphNode TransitToNextNode()
		{
			var nextNode = campaignGraph.GetNextNode(campaign.Current).FirstOrDefault();
			if (nextNode != null)
			{
				campaign.Current = nextNode;
			}

			return nextNode;
		}

		/// <summary>
		/// Steps through each node's component behaviours
		/// </summary>
		void Step()
		{
			if (taskQueue.Count > 0)
			{
				List<BehaviourTask> currentTasks = taskQueue.Peek();

				// assign the currently active components to the respective behaviours
				foreach (BehaviourTask task in currentTasks)
				{
					task.Behaviour.Component = task.Component;

					// select all behaviour for the current task and assign them to the active behaviour list in 'BehaviourBase'
					(task.Behaviour as BehaviourBase).ActiveBehaviours = currentTasks.Select(tsk => tsk.Behaviour).ToList();
					(task.Behaviour as MonoBehaviour).enabled = true;
				}
			}
			else if (taskQueue.Count == 0)
			{
				Debug.Log("queue is empty / node had no components ...");

				IGraphNode nextNode = TransitToNextNode();
				if (nextNode != null)
				{
					UpdateQueue(nextNode);
					Step();
				}
				else
				{
					SetStateEnd();
				}
			}
		}

		void SetStateEnd()
		{
			// TODO: add event

			Debug.Log("=== END ===");
		}

		void OnBehaviourTaskFinishedHandler(IBehaviour behaviour)
		{
			// disable current behaviours after one behaviour is finished!
			// 'behaviours' which only hold data don't have a specific state; only logic behaviours finish!
			List<BehaviourTask> currentTasks = taskQueue.Dequeue();
			List<IBehaviour> currentlyActiveBehaviours = currentTasks.Select(task => task.Behaviour).ToList();
			currentlyActiveBehaviours.ForEach(beh =>
			{
				(beh as MonoBehaviour).enabled = false;
			});

			Eventbus.Instance.FireEvent<ComponentCompletedEvent>(new ComponentCompletedEvent(behaviour.Component));
			Step();
		}
	}
}