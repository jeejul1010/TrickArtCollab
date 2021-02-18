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
    //public GameObject button;
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
        if(spawnedObject!=null)
        {
            LeanTwistRotateAxis rotate = spawnedObject.GetComponentInChildren<LeanTwistRotateAxis>();
            LeanPinchScale scale = spawnedObject.GetComponentInChildren<LeanPinchScale>();
            rotate.enabled = false;
            scale.enabled = false;
            spawnedObject.AddComponent<ARAnchor>();
            isFixed = true;
            footIcon.SetActive(true);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(selected!=null)
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
                    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    //activity.CallStatic("showCheckButton"); //안드로이드 쪽으로 체크버튼 표시하게
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(() => { activity.Call("showCheckButton"); }));

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
        /*if(chosen.Equals("Else"))
        {
            isObjectSpawned = false;
            nowSelected = null;
            if(selected!=null)
            {
                Destroy(selected);
            }
            if(spawnedObject!=null)
            {
                Destroy(spawnedObject);
            }
            return;
        }
        else if(chosen.Equals(nowSelected)) //이미 선택된 애 선택했을 때
        {
            return;
        }
        else if(selected!=null && !chosen.Equals(nowSelected)) //이미 선택된 애랑 다른 애 선택했을 때
        {
            if(spawnedObject!=null)
            {
                Destroy(spawnedObject);
                isObjectSpawned = false;
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
        }*/
        if(chosen.Equals("Nature"))
        {
            if(selected==null) //아무 작품도 안 연결
            {
                isObjectSpawned = false;
                selected = gameObjectToInstantiate[0];
                nowSelected = chosen;
            }
            else if(chosen.Equals(nowSelected)) //같은 작품 선택
            {
                return;
            }
            else //다른 작품 선택
            {
                if(spawnedObject!=null) //다른 작품 이미 생성된 경우
                {
                    Destroy(spawnedObject);
                    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(() => { activity.Call("hideCheckButton"); }));
                    isObjectSpawned = false;
                }
                selected = gameObjectToInstantiate[0];
                nowSelected = chosen;
            }

        }
        else if(chosen.Equals("Fantasy"))
        {
            if (selected == null) //아무 작품도 안 연결
            {
                isObjectSpawned = false;
                selected = gameObjectToInstantiate[1];
                nowSelected = chosen;
            }
            else if (chosen.Equals(nowSelected)) //같은 작품 선택
            {
                return;
            }
            else //다른 작품 선택
            {
                if (spawnedObject != null) //다른 작품 이미 생성된 경우
                {
                    Destroy(spawnedObject);
                    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(() => { activity.Call("hideCheckButton"); }));
                    isObjectSpawned = false;
                }
                selected = gameObjectToInstantiate[1];
                nowSelected = chosen;
            }

        }
        else
        {
            isObjectSpawned = true;
            if(selected == null) //아무 작품도 안 연결
            {
                return;
            }
            else //뭐라도 연결돼있다면
            {
                selected = null;
                nowSelected = null;
                if(spawnedObject!=null)
                {
                    Destroy(spawnedObject);
                    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(() => { activity.Call("hideCheckButton"); }));
                }

            }

        }
        

    }
}
