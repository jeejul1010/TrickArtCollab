using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class TapToPlace : MonoBehaviour
{
    public GameObject[] gameObjectToInstantiate;
    string nowSelected; //지금 선택된 작품

    private GameObject selected;
    private GameObject spawnedObject;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPosition;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public Canvas canvas;
    public GameObject button;
    GameObject footIcon;

    public bool isObjectSpawned;
    bool isFixed;

    //private Collider2D button;

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        footIcon = FindInActiveObjectByName("FootUI");
        isObjectSpawned = false;
        isFixed = false;
        //button = GameObject.Find("Button").GetComponent<Collider2D>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if(Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    public void ButtonOnClick()
    {
        LeanTwistRotateAxis rotate = spawnedObject.GetComponentInChildren<LeanTwistRotateAxis>();
        LeanPinchScale scale = spawnedObject.GetComponentInChildren<LeanPinchScale>();
        rotate.enabled = false;
        scale.enabled = false;
        spawnedObject.AddComponent<ARAnchor>();
        isFixed = true;
        button.SetActive(false);
        footIcon.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(nowSelected!=null)
        {
            if (!TryGetTouchPosition(out Vector2 touchPosition))
            {
                return;
            }

            if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;

                if (spawnedObject == null)
                {
                    isObjectSpawned = true;
                    spawnedObject = Instantiate(selected, hitPose.position, hitPose.rotation);

                }
                else if (isFixed == true)
                {
                    return;
                }
                else if (!IsPointerOverUIObject(canvas, touchPosition)) //button != Physics2D.OverlapPoint(touchPosition)
                {
                    spawnedObject.transform.position = hitPose.position;
                }
            }
        }
        
        
    }

    private bool IsPointerOverUIObject(Canvas canvas, Vector2 screenPosition)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = screenPosition;

        GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    GameObject FindInActiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None)
            {
                if (objs[i].name == name)
                {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }

    public void ChooseArtwork(string chosen)
    {
        if(chosen.Equals(nowSelected)) //이미 선택된 애 선택했을 때
        {
            return;
        }
        else if(selected!=null && !chosen.Equals(nowSelected)) //이미 선택된 애랑 다른 애 선택했을 때
        {
            if(spawnedObject!=null)
            {
                Destroy(spawnedObject);
            }
        }
        if(chosen.Equals("Nature"))
        {
            selected = gameObjectToInstantiate[0];
            nowSelected = chosen;
        }
        else if(chosen.Equals("Fantasy"))
        {
            selected = gameObjectToInstantiate[1];
            nowSelected = chosen;
        }
        

    }
}
