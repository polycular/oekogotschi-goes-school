using System;
using System.Collections.Generic;
using System.Linq;
using Http.Exceptions;
using Newtonsoft.Json;
using Polycular.Persistance;
using Polycular.ResourceProvider;
using RSG;
using UnityEngine;

namespace Http
{
	public class RequestQueue : IDisposable, IPersistant
	{
		// Configuration fields within unity
		public float retryInterval;

		HttpResourceProvider httpProvider;

		// HEAD, GET requests (non-persistant)
		Queue<KeyValuePair<Promise<Response>, QueuedRequest>> outgoingQueue;

		// POST, PATCH, DELETE requests
		Queue<KeyValuePair<Promise<Response>, QueuedRequest>> incomingQueue;

		System.Timers.Timer timer = new System.Timers.Timer();
		bool isProcessing = false;

		string savePath = "rq/outgoing.json";
		public string SavePath
		{
			get { return savePath; }
		}


		public RequestQueue(HttpResourceProvider provider, float retryInterval)
		{
			httpProvider = provider;
			this.retryInterval = retryInterval;
			incomingQueue = new Queue<KeyValuePair<Promise<Response>, QueuedRequest>>();
			outgoingQueue = new Queue<KeyValuePair<Promise<Response>, QueuedRequest>>();

            Storage.Load(this);
		}

		public string Serialize()
		{
			var tmp = outgoingQueue.ToList();
			return JsonConvert.SerializeObject(tmp);
		}

		public void Deserialize(string serialObject)
		{
            var tmp = JsonConvert.DeserializeObject<List<KeyValuePair<Promise<Response>, QueuedRequest>>>(serialObject);

            if (tmp == null)
                return;

            outgoingQueue = new Queue<KeyValuePair<Promise<Response>, QueuedRequest>>(tmp);
		}

		public Promise<Response> Enqueue(QueuedRequest qrequest)
		{
			var promise = new Promise<Response>();

			// if request method is get, add to get queue - this queue isnt persistant
			if (qrequest.Request.Method.Equals(Verbs.GET))
				incomingQueue.Enqueue(new KeyValuePair<Promise<Response>, QueuedRequest>(promise, qrequest));
			else
			{
				// check if the queue contains patches on the same id already
				List<KeyValuePair<Promise<Response>, QueuedRequest>> queuedRequests = outgoingQueue.Where(qreq => qreq.Value.Request.Url.Equals(qrequest.Request.Url)).ToList();
				queuedRequests.ForEach(qreq => qreq.Value.IsOutdated = true);

				outgoingQueue.Enqueue(new KeyValuePair<Promise<Response>, QueuedRequest>(promise, qrequest));

				// Persist patch queue after each enqueue'd or dequeue'd request
				Storage.Save(this);
			}

			// if no processing is currently done, start immediatelly
			if (!isProcessing)
				Cycle();

			return promise;
		}

		void Timeout()
		{
			isProcessing = false;
			timer.Start();
		}

		void OnTimerElapsedHandler(object sender, System.Timers.ElapsedEventArgs e)
		{
            Cycle();
		}

        void Cycle()
        {
            isProcessing = true;
            Process();
        }

		void Process()
		{
			// process patch queue before get queue; peek an entry, send the request and dequeue on success
			if (outgoingQueue.Count > 0)
			{
				ProcessEntry(outgoingQueue);
			}
			else if (incomingQueue.Count > 0)
			{
				ProcessEntry(incomingQueue);
			}
			else
			{
				isProcessing = false;
			}
		}

		void ProcessEntry(Queue<KeyValuePair<Promise<Response>, QueuedRequest>> queue)
		{
			KeyValuePair<Promise<Response>, QueuedRequest> next = queue.Peek();

            Promise<Response> promise = next.Key;
			QueuedRequest queuedRequest = next.Value;

			// check if the request is outdated
			if (queuedRequest.IsOutdated)
			{
				queue.Dequeue();
				Process();
			}
			else
			{
				ProcessRequest(promise, queuedRequest);
			}
		}

		void ProcessRequest(Promise<Response> promise, QueuedRequest qreq)
		{
			httpProvider.Fetch(qreq.Request)
				.Then(response =>
				{
					promise.Resolve(response);
					Dequeue(qreq.Direction);

					// continue with the next request dependent on online status
					Cycle();
				})
				.Catch(ex =>
				{
					// dequeue on status code >= 400 && < 500
					var exception = ex as HttpException;
					if (exception != null)
					{
                        if (exception.StatusCode >= 400 && exception.StatusCode < 500)
                        {
                            Debug.LogWarning("Dequeuing with Status " +  exception.StatusCode + "\n\n" + qreq.Request.Url + "\n\n" + ex);
                            promise.Reject(ex);
                            Dequeue(qreq.Direction);
                        }
					}

					Timeout();
				});
		}

		void Dequeue(QueuedRequest.NetworkDirection ndir)
		{
			if (ndir == QueuedRequest.NetworkDirection.INCOMING)
			{
				incomingQueue.Dequeue();
			}
			else
			{
				outgoingQueue.Dequeue();
				// Only outgoingQueue is persistant!
				Storage.Save(this);
			}
		}

		public void Reset()
		{
			foreach (var request in outgoingQueue)
				request.Key.Reject(new UserLoggedOutException("The user was logged out before the request could be fulfilled."));

			foreach (var request in incomingQueue)
				request.Key.Reject(new UserLoggedOutException("The user was logged out before the request could be fulfilled."));

			outgoingQueue.Clear();
			incomingQueue.Clear();
			timer.Stop();
		}

		public void Dispose()
        {
            timer.Stop();            
        }
    }
}
