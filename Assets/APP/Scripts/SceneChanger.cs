using UnityEngine;
using System.Collections;

public class SceneChanger : MonoBehaviour {

    public GameObject Stations;

  
	// Use this for initialization
	void Start () {

       
	}
    int delay = 120;
    int detectedSceneID = -1;
    private bool isalreadyloading = false;
	// Update is called once per frame
	void Update () {

        detectedSceneID = findstation();

        if(detectedSceneID != -1 && !isalreadyloading && delay <= 0)
        {
            SceneLoader.Instance.ChangeScene(detectedSceneID);
           

        }


        delay--;
       
    }

    

    private int findstation()
    {
        for (int i = 0; i < Stations.transform.childCount; i++)
        {
            if(Stations.transform.GetChild(i).GetComponent<ImageTargetTracker>().IsTracked)
            {
                return i;

            }


        }

        return -1;
    }



}
