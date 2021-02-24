using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementIndicator : MonoBehaviour
{
    private ARRaycastManager rayManager;
    private GameObject visual; //보여지는 placement indicator
    bool isObjectSpawned;
    public GameObject arSession;
    public GameObject loadingIcon;
    GameObject spawnedObject;

    bool startIndicator;
    void Start()
    {
        rayManager = FindObjectOfType<ARRaycastManager>();
        visual = transform.GetChild(0).gameObject;

        visual.SetActive(false); //hide the placement visual
        loadingIcon.SetActive(false);

        startIndicator = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(startIndicator)
        {
            isObjectSpawned = arSession.GetComponent<TapToPlace>().isObjectSpawned;

            if (isObjectSpawned)
            {
                visual.SetActive(false);
            }
            else
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                rayManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon);

                if (hits.Count > 0) //if we hit something
                {
                    transform.position = hits[0].pose.position;
                    transform.rotation = hits[0].pose.rotation;

                    if (!visual.activeInHierarchy)
                    {
                        loadingIcon.SetActive(false);
                        visual.SetActive(true);
                    }
                        
                }
            }
        }
   
    }

    public void showIndicator(string TorF)
    {
        if(TorF.Equals("true"))
        {
            startIndicator = true;
        }
        else
        {
            startIndicator = false;
        }
    }

    public void showLoading(string TorF)
    {
        spawnedObject = arSession.GetComponent<TapToPlace>().spawnedObject;

        if (TorF.Equals("true") && spawnedObject==null && !visual.activeInHierarchy)
        {
            loadingIcon.SetActive(true);
        }
        else if(TorF.Equals("false"))
        {
            loadingIcon.SetActive(false);
        }
        
    }
}
