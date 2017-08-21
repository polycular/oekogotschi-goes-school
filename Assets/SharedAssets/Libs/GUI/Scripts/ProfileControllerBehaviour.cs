using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TrialCity;


namespace SharedAssets.GuiMain
{
    public class ProfileControllerBehaviour : MonoBehaviour
    {
        public GameObject ProfilePanel;
        public List<GameObject> TableRows;

        void Awake()
        {
            ProfilePanel.GetComponent<UIPanel>().alpha = 0;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void updateProfile(Dictionary<TrialCityUtilBehaviour.Type, float> allScores)
        {
            foreach (KeyValuePair<TrialCityUtilBehaviour.Type, float> score in allScores)
            { 
                //score.
            }
        }
    }
}
