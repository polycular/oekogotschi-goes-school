using System.Collections.Generic;
using JSONApi.Models;
using JSONApi.Services;
using RSG;
using UnityEngine;

namespace Polycular.Clustar
{
	public enum ModelType
	{
		CLIENT,
		PROJECT,
		CAMPAIGN,
		NODE,
		COMPONENT,
		TRANSITION
	}

	public enum AuthAction
	{
		REGISTER,
		LOGIN,
		FACEBOOK
	}

	class FetchParams
	{
		public Dictionary<string, string> Includes { get; set; }
		public bool Authorize { get; set; }
		public string ModelName { get; set; }
	}

	public class DataController : MonoBehaviour
	{
		public string ModelID;
		public ModelType ModelType;

		Store store;
		Dictionary<ModelType, FetchParams> modelTypeToParams;

		void Start()
		{
			store = new Store(GetType().Namespace);

			modelTypeToParams = new Dictionary<ModelType, FetchParams>
			{
				{ ModelType.CLIENT, new FetchParams()
					{
						 Includes = new Dictionary<string,string>{ { "include", "projects,projects.campaigns" } },
						 Authorize = false,
						 ModelName = ModelType.CLIENT.ToString().ToLower()
					}
				},
				{ ModelType.PROJECT, new FetchParams()
					{
						Includes = new Dictionary<string,string>{ { "include", "campaigns" } },
						Authorize = true,
						ModelName = ModelType.PROJECT.ToString().ToLower()
					}
				},
				{ ModelType.CAMPAIGN, new FetchParams()
					{
						Includes = new Dictionary<string,string>{ { "include", "nodes,nodes.components,nodes.transitions" } },
						Authorize = true,
						ModelName = ModelType.CAMPAIGN.ToString().ToLower()
					}
				},
				{ ModelType.NODE, new FetchParams()
					{
						Includes = new Dictionary<string,string>{ { "include", "components,transitions" } },
						Authorize = true,
						ModelName = ModelType.NODE.ToString().ToLower()
					}
				},
				{ ModelType.COMPONENT, new FetchParams()
					{
						Includes = new Dictionary<string,string>{ { "", "" } },
						Authorize = true,
						ModelName = ModelType.COMPONENT.ToString().ToLower()
					}
				},
				{ ModelType.TRANSITION, new FetchParams()
					{
						Includes = new Dictionary<string,string>{ { "", "" } },
						Authorize = true,
						ModelName = ModelType.TRANSITION.ToString().ToLower()
					}
				}
			};
		}

		string ModelTypeToString(ModelType type)
		{
			return type.ToString().ToLower();
		}

		Promise<IModel> Fetch()
		{
			var stype = ModelTypeToString(ModelType);
			var param = modelTypeToParams[ModelType];
			return store.Find(stype, ModelID, param.Includes, param.Authorize);
		}

		public Promise<IModel> Fetch(ModelType type, string id)
		{
			var stype = ModelTypeToString(type);
			var param = modelTypeToParams[type];
			return store.Find(stype, id, param.Includes, param.Authorize);
		}

		public void Save(IModel model)
		{
			store.Save(model);
		}

		public Promise Authenticate(string email, string password, AuthAction action)
		{
			if (action == AuthAction.REGISTER)
			{
				return store.RegisterUser(email, password);
			}
			else if (action == AuthAction.LOGIN)
			{
				return store.LoginUser(email, password);
			}
			else 
			{
				return store.LoginUserFacebook();
			}
		}
	}
}
