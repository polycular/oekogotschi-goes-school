using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Flurl;
using JSONApi.Attributes;
using JSONApi.ContractResolvers;
using JSONApi.Exceptions;
using JSONApi.Extensions;
using JSONApi.JSON;
using JSONApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polycular.Collections;
using Polycular.Persistance;
using Polycular.ResourceProvider;
using RSG;
using UnityEngine;

namespace JSONApi.Services
{
	public class Store : IPersistant
	{
		Tuple<string, string> m_rootObjectInfo;
		/// <summary>
		/// First = root object type 
		/// Second = root object ID
		/// </summary>
		Tuple<string, string> RootObjInfo
		{
			get { return m_rootObjectInfo; }
			set
			{
				if (m_rootObjectInfo == null)
					m_rootObjectInfo = value;
			}
		}

		const string savePathConfigKey = "lastStoreSavePath";
		string savePath;
		public string SavePath
		{
			get
			{
				if (savePath == null)
					savePath = Storage.GetConfigValue(savePathConfigKey);

				return savePath;
			}
			set { savePath = value; }
		}

		Dictionary<string, IModel> _modelCache = new Dictionary<string, IModel>();
		Dictionary<string, Resource> _resourceCache = new Dictionary<string, Resource>();

		Dictionary<string, Type> _modelTypeCache = new Dictionary<string, Type>();

		HttpResourceProvider httpResourceProvider = new HttpResourceProvider();
		DiskResourceProvider diskResourceProvider = new DiskResourceProvider();

		public delegate void LogoutHandler();
		public event LogoutHandler UserLoggedOut;


		public Store(string modelNamespace)
		{
			Assembly asm = typeof(IModel).Assembly;
			var listOfBs = (from assemblyType in asm.GetTypes()
							where typeof(JSONApi.Models.IModel).IsAssignableFrom(assemblyType) && assemblyType.Namespace.Contains(modelNamespace)
							select assemblyType).ToArray();

			foreach (var type in listOfBs)
			{
				_modelTypeCache.Add(type.Name.ToKebabCase(), type);
			}
		}

		/// <summary>Automatically logs in user after successful register.</summary>
		public Promise RegisterUser(string email, string password)
		{
			Logout();
			return httpResourceProvider.RegisterUser(email, password);
		}

		public Promise LoginUser(string email, string password)
		{
			Logout();
			return httpResourceProvider.LoginUser(email, password);
		}

		public Promise LoginUserFacebook()
		{
			return httpResourceProvider.LoginUserFacebook();
		}

		public void Logout()
		{
			foreach (var kvp in _modelTypeCache)
			{
				diskResourceProvider.Delete(kvp.Key + '/');
			}

			httpResourceProvider.Reset();
			Reset();

			if (UserLoggedOut != null)
				UserLoggedOut();
		}

		public bool Empty()
		{
			return _resourceCache.Count == 0;
		}

		public Promise<List<IModel>> LoadFromJSON(string text)
		{
			var promise = new Promise<List<IModel>>();
			IModel model;

			var jsonapiObject = Assets.JSONApi.JSONApi.Decode(text);
			FillResourceCache(jsonapiObject);

			if (jsonapiObject.Data is Resource)
			{
				model = CreateOrUpdateModel(jsonapiObject.Data as Resource);

				if (model != null)
				{
					List<IModel> loadedModels = new List<IModel>
					{
						model
					};

					promise.Resolve(loadedModels);
				}
				else
				{
					promise.Reject(new ArgumentException("Model was null. Your JSON is probably malformed."));
				}
			}
			else if (jsonapiObject.Data is ResourceList)
			{
				var models = CreateOrUpdateModel(jsonapiObject.Data as ResourceList);

				if (models != null)
				{
					promise.Resolve(models);
				}
				else
				{
					promise.Reject(new ArgumentException("Models were null. Your JSON is probably malformed."));
				}
			}
			else
			{
				promise.Reject(new ArgumentException("JSONApi object data field doesn't match the required type."));
				return promise;
			}

			return promise;
		}

		/// <summary>
		/// Query a logical collection of resources
		/// </summary>
		public Promise<List<IModel>> Find(string type, Dictionary<string, string> queryParams, bool authorized = true, bool useQueue = true)
		{
			var promise = new Promise<List<IModel>>();
			Url url = BuildUrl(type, null, queryParams);

			Find(url, authorized, useQueue)
				.Then(jsonapiObject =>
				{
					FillResourceCache(jsonapiObject);

					var model = CreateOrUpdateModel(jsonapiObject.Data as ResourceList);
					Storage.Save(this);

					promise.Resolve(model);
				})
				.Catch(ex =>
				{
					Debug.LogWarning(ex);
					promise.Reject(ex);
				});

			return promise;
		}

		/// <summary>
		/// Query a single resource object
		/// </summary>
		public Promise<IModel> Find(string type, string id, Dictionary<string, string> queryParams, bool authorized = true, bool useQueue = true)
		{
			var promise = new Promise<IModel>();
			Url url = BuildUrl(type, id, queryParams);

			Find(url, authorized, useQueue)
				.Then(jsonapiObject =>
				{
					FillResourceCache(jsonapiObject);

					var model = CreateOrUpdateModel(jsonapiObject.Data as Resource);
					Storage.Save(this);

					promise.Resolve(model);
				})
				.Catch(ex =>
				{
					Debug.LogWarning(ex);
					promise.Reject(ex);
				});

			return promise;
		}

		public IModel FindLocal(string id)
		{
			if (_modelCache.ContainsKey(id))
				return _modelCache[id];

			return null;
		}

		Url BuildUrl(string type, string id, Dictionary<string, string> queryParams)
		{
			Url url = new Url(httpResourceProvider.ServerConfig.UrlBase);
			url.AppendPathSegment(type.ToPlural());

			if (!string.IsNullOrEmpty(id))
			{
				url.AppendPathSegment(id);
			}

			RootObjInfo = new Tuple<string, string>(type, id);

			if (queryParams != null)
				url.SetQueryParams(queryParams);

			return url;
		}

		Promise<JSON.Object> Find(Url url, bool authorized, bool useQueue)
		{
			Promise<JSON.Object> promise = new Promise<JSON.Object>();

			httpResourceProvider.Get(url, ContentType.JSON, authorized, useQueue)
				.Then(response =>
				{
					var jsonapiObject = Assets.JSONApi.JSONApi.Decode(response.Text);
					promise.Resolve(jsonapiObject);
				})
				.Catch(ex =>
				{
					Debug.LogWarning(ex);
					promise.Reject(ex);
				});

			return promise;
		}

		public Promise<IModel> Save(IModel model)
		{
			if (httpResourceProvider.User.IsTester)
			{
				var promise = new Promise<IModel>();
				promise.Reject(new UserIsTesterException("Rejecting patch because current user is a tester"));
				return promise;
			}

			UpdateResource(model);

			Resource resource = CreateResource(model);

			JSON.Object jsonapiObject = new JSON.Object();
			jsonapiObject.Data = resource;

			JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
			{
				ContractResolver = new ToCamelCaseContractResolver(),
				NullValueHandling = NullValueHandling.Ignore
			};

			string json = JsonConvert.SerializeObject(jsonapiObject, Formatting.None, jsonSettings);

			Url url = new Url(httpResourceProvider.ServerConfig.UrlBase);
			url.AppendPathSegment(resource.Type.ToPlural());
			url.AppendPathSegment(model.Id);

			Promise<IModel> prm = httpResourceProvider.Patch(url, json, ContentType.JSON)
				.Then(response =>
				{
					JSON.Object jsonObject = Assets.JSONApi.JSONApi.Decode(response.Text);
					FillResourceCache(jsonapiObject);
					var updatedModel = UpdateModel(model, jsonObject.Data as Resource);
					Storage.Save(this);
					return Promise<IModel>.Resolved(updatedModel);
				}) as Promise<IModel>;

			return prm;
		}

		public void Reset()
		{
			_modelCache.Clear();
			_resourceCache.Clear();
			// reset the root object information
			m_rootObjectInfo = null;
		}

		public string Serialize()
		{
			StringBuilder sb = new StringBuilder();

			using (StringWriter sw = new StringWriter(sb))
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.NullValueHandling = NullValueHandling.Ignore;
				serializer.ContractResolver = new ToCamelCaseContractResolver();

				serializer.Serialize(sw, _resourceCache);
			}

			if (RootObjInfo == null)
			{
				SavePath = "store.json";
			}
			else if (RootObjInfo.Second != null)
			{
				SavePath = Path.Combine(RootObjInfo.First, RootObjInfo.Second) + ".json";
			}
			else
			{
				SavePath = Path.Combine(RootObjInfo.First, RootObjInfo.First) + ".json";
			}

			Storage.SetConfigValue(savePathConfigKey, SavePath);

			return sb.ToString();
		}

		public void Deserialize(string serialObject)
		{
			var loadedResourceCache = JsonConvert.DeserializeObject<Dictionary<string, Resource>>(serialObject);

			if (loadedResourceCache != null && loadedResourceCache.Count > 0)
			{
				_resourceCache = loadedResourceCache;

				foreach (var kvp in _resourceCache)
				{
					if (!_modelCache.ContainsKey(kvp.Key))
						CreateOrUpdateModel(kvp.Value);
				}
			}
		}

		public Resource UpdateResource(IModel model)
		{
			string id = model.Id;

			// check if resource is present in the resource cache
			if (_resourceCache.ContainsKey(id))
			{
				Resource updatedResource = CreateResource(model);
				_resourceCache[id] = updatedResource;
				return updatedResource;
			}
			else
			{
				Debug.LogWarning("resource with id " + model.Id + " couldnt be updated because it wasnt present in the RD");
				return null;
			}
		}

		Resource CreateResource(IModel model)
		{
			Type type = model.GetType();
			Resource resource = new Resource();

			var attributes = GetPropertiesByAttribute(type, typeof(AttributeAttribute));
			var relationships = GetPropertiesByAttribute(type, typeof(RelationshipAttribute));

			resource.Id = model.Id;
			resource.Type = model.GetType().Name.ToKebabCase();

			if (resource.Attributes == null && attributes.Count > 0)
			{
				resource.Attributes = new Dictionary<string, JToken>();
			}

			if (resource.Relationships == null && relationships.Count > 0)
			{
				resource.Relationships = new Dictionary<string, Relationship>();
			}

			foreach (KeyValuePair<string, PropertyInfo> pair in attributes)
			{
				var attributeName = pair.Key;
				PropertyInfo attribute = pair.Value;
				var value = attribute.GetValue(model, null);
				// need not to be an object - number, string, array etc possible

				if (value == null)
					continue;

				if (attribute.PropertyType == typeof(string))
				{
					resource.Attributes.Add(attributeName, value.ToString());
					continue;
				}

				string json = JsonConvert.SerializeObject(value);
				resource.Attributes.Add(attributeName, json);
			}

			foreach (KeyValuePair<string, PropertyInfo> pair in relationships)
			{
				var relationshipName = pair.Key;
				PropertyInfo relationshipProperty = pair.Value;
				var propertyType = relationshipProperty.PropertyType;

				if (propertyType.GetInterfaces().Contains((typeof(JSONApi.Models.IModel))))
				{
					var relationship = new Relationship();
					var resourceIdentifier = new ResourceIdentifier();

					var value = relationshipProperty.GetValue(model, null) as IModel;

					if (value == null)
						continue;

					resourceIdentifier.Id = value.Id;
					resourceIdentifier.Type = relationshipName.ToKebabCase();

					relationship.Data = resourceIdentifier;
					resource.Relationships.Add(relationshipName, relationship);
				}
				else
				{
					var list = (IList)relationshipProperty.GetValue(model, null);
					if (list == null || list.Count == 0)
						continue;

					var relationship = new Relationship();
					ResourceIdentifierList data = new ResourceIdentifierList();

					foreach (IModel el in list)
					{
						var resourceIdentifier = new ResourceIdentifier();
						resourceIdentifier.Id = el.Id;
						resourceIdentifier.Type = el.GetType().Name.ToKebabCase();
						data.Add(resourceIdentifier);
					}

					relationship.Data = data;
					resource.Relationships.Add(relationshipName, relationship);
				}
			}

			return resource;
		}


		#region Private methods
		void FillResourceCache(JSONApi.JSON.Object obj)
		{
			if (obj.Data is ResourceList)
			{
				(obj.Data as ResourceList).ForEach(res =>
				{
					_resourceCache[res.Id] = res;
				});
			}
			else if (obj.Data is Resource)
			{
				Resource res = obj.Data as Resource;
				_resourceCache[res.Id] = res;
			}

			obj.Included.ForEach(res =>
			{
				_resourceCache[res.Id] = res;
			});
		}

		List<IModel> CreateOrUpdateModel(ResourceList resourceList)
		{
			List<IModel> models = new List<IModel>();

			foreach (var resource in resourceList)
			{
				var model = CreateOrUpdateModel(resource);

				// TODO: check if model is null or invalid
				models.Add(model);
			}

			return models;
		}

		IModel CreateOrUpdateModel(Resource resource)
		{
			if (_modelCache.ContainsKey(resource.Id))
			{
				UpdateModel(_modelCache[resource.Id], resource);
				return _modelCache[resource.Id];
			}

			return CreateModel(resource);
		}

		IModel CreateModel(Resource resource)
		{
			var type = GetModelByType(resource.Type);

			IModel model = (IModel)type.GetConstructor(new Type[] { }).Invoke(new object[] { });
			model.Id = resource.Id;

			_modelCache[model.Id] = model;

			SetAttributes(model, resource);
			SetRelationships(model, resource);

			return model;
		}

		IModel UpdateModel(IModel model, Resource resource)
		{
			SetAttributes(model, resource);
			SetRelationships(model, resource);

			return model;
		}

		Type GetModelByType(string x)
		{
			return _modelTypeCache[x];
		}

		Dictionary<string, PropertyInfo> GetPropertiesByAttribute(Type type, Type attributeType)
		{
			//PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			return type.GetProperties().Where(prop => prop.IsDefined(attributeType, false)).ToDictionary(p => p.Name, p => p);
		}

		Dictionary<string, PropertyInfo> GetProperties(Type type)
		{
			//PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

			return type.GetProperties().Where(prop => prop.IsDefined(typeof(AttributeAttribute), false) || prop.IsDefined(typeof(RelationshipAttribute), false)).ToDictionary(p => p.Name, p => p);
		}

		void SetAttributes(IModel model, Resource resource)
		{
			var type = GetModelByType(resource.Type);
			var properties = GetProperties(type);

			if (resource.Attributes != null)
			{
				foreach (var attr in resource.Attributes)
				{
					var key = attr.Key.ToPascalCase();
					PropertyInfo propertyInfo = properties.ContainsKey(key) ? properties[key] : null;

					if (propertyInfo != null)
					{
						if (attr.Value == null)
							continue;

						var propType = propertyInfo.PropertyType;

						if (propType == typeof(bool))
						{
							type.GetProperty(propertyInfo.Name).SetValue(model, Boolean.Parse(attr.Value.ToString()), null);
						}
						else if (propType == typeof(string))
						{
							type.GetProperty(propertyInfo.Name).SetValue(model, attr.Value.ToString(), null);
						}
						else
						{
							object val = JsonConvert.DeserializeObject(attr.Value.ToString(), propType);
							type.GetProperty(propertyInfo.Name).SetValue(model, val, null);
						}
					}
				}
			}
		}

		void SetRelationships(IModel model, Resource resource)
		{
			var type = GetModelByType(resource.Type);
			var properties = GetProperties(type);

			if (resource.Relationships != null)
			{
				foreach (var rel in resource.Relationships)
				{
					var key = rel.Key.ToPascalCase();
					PropertyInfo propertyInfo = properties.ContainsKey(key) ? properties[key] : null;

					if (propertyInfo != null)
					{
						var attr = propertyInfo.GetCustomAttributes(typeof(RelationshipAttribute), true).FirstOrDefault() as RelationshipAttribute;

						if (attr.Type == RelationshipType.BELONGS_TO)
						{
							var relResourceIdentifier = rel.Value.Data as ResourceIdentifier;

							// check if the resource is present in the resource cache
							if (!(_resourceCache.ContainsKey(relResourceIdentifier.Id)))
							{
								continue;
							}

							Resource relResource = _resourceCache[relResourceIdentifier.Id];
							IModel relModel = CreateOrUpdateModel(relResource);

							type.GetProperty(propertyInfo.Name).SetValue(model, relModel, null);
						}
						else if (attr.Type == RelationshipType.HAS_MANY)
						{
							var list = (IList)propertyInfo.PropertyType.GetConstructor(new Type[] { }).Invoke(new object[] { });

							var ril = (rel.Value.Data as ResourceIdentifierList);
							foreach (var entry in ril)
							{
								// check if the resource is present in the resource cache
								if (!(_resourceCache.ContainsKey(entry.Id)))
								{
									continue;
								}

								Resource relResource = _resourceCache[entry.Id];
								IModel relModel = CreateOrUpdateModel(relResource);

								list.Add(relModel);
							}

							type.GetProperty(propertyInfo.Name).SetValue(model, list, null);
						}
					}
				}
			}
		}

		#endregion
	}
}
