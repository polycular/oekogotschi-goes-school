using System.Collections.Generic;
using UnityEngine;

namespace SharedAssets.GuiMain
{
    public class DialogueManagerBehaviour : MonoBehaviour
    {
		public GameObject DialoguesObject;
		public GameObject TopBarsObject;
        public Dictionary<string, DialogueBehaviour> KeywordDialogueDictionary;
        public Dictionary<string, TopBarBehaviour> KeywordTopBarDictionary;

		void Awake()
		{
			KeywordDialogueDictionary = new Dictionary<string, DialogueBehaviour>();
			KeywordTopBarDictionary = new Dictionary<string, TopBarBehaviour>();

			foreach (var dialogue in DialoguesObject.GetComponentsInChildren<DialogueBehaviour>())
			{
				KeywordDialogueDictionary.Add(dialogue.gameObject.name, dialogue);
			}

			foreach (var topbar in TopBarsObject.GetComponentsInChildren<TopBarBehaviour>())
			{
				KeywordTopBarDictionary.Add(topbar.gameObject.name, topbar);
			}
		}

        public DialogueBehaviour getDialogue(string key)
        {
            if (KeywordDialogueDictionary.ContainsKey(key))
            {
                return KeywordDialogueDictionary[key];
            }
            else
            {
                Debug.LogWarning(string.Format("KeywordDialogueDictionary doesn't contain a Dialogue with key: {0} {1}", key, KeywordDialogueDictionary.Keys.Count));
                return null;
            }
        }

        public TopBarBehaviour getTopBarController(string key)
        {
            if (KeywordTopBarDictionary.ContainsKey(key))
            {
                return KeywordTopBarDictionary[key];
            }
            else
            {
                Debug.LogWarning(string.Format("KeywordTopBarDictionary doesn't contain a TopBarController with key: {0}", key));
                return null;
            }
        }
    }
}
