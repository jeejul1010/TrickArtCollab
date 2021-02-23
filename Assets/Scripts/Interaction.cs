using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(AudioSource))]
public class Interaction : MonoBehaviour
{
    //public float scrollX = 0.5f;
    //public float scrollY = 0.5f;
    Vector2 offset = Vector2.zero;
    public Vector2 flowRate = new Vector2(0f, 0.5f); //물 흐르기

    RectTransform foot;
    GameObject footUI; //발 아이콘

    public GameObject river; //바닥에 깐 작품. 물바닥
    public GameObject particle;
    public GameObject redfish;
    public GameObject frog;

    Animator redfish_anim; //물고기 애니메이션
    Animator frog_anim; //개구리 애니메이션
    private Rigidbody rigid;

    [SerializeField] private float jumpForce;

    AudioSource riverSound;

    bool waterflow; //물 흐르기 여부
    bool artworkHit; //작품에 발을 들였는지 여부
    public bool rise; //물 속에서 나오기 시작여부 결정
    private bool isJump;
    private bool isDie;

    private void Start()
    {
        footUI = FindInActiveObjectByName("FootUI");
        foot = footUI.GetComponent<RectTransform>();
        //foot = GameObject.FindGameObjectWithTag("footUI").GetComponent<RectTransform>();

        redfish_anim = GameObject.FindGameObjectWithTag("redfish").GetComponent<Animator>();
        redfish_anim.enabled = false;

        frog_anim = GameObject.FindGameObjectWithTag("frog").GetComponent<Animator>();
        frog_anim.enabled = false;
        rigid = frog.GetComponent<Rigidbody>();

        riverSound = GetComponent<AudioSource>();
        artworkHit = false;

        waterflow = false;

        rise = false;

        isJump = false;
        isDie = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (footUI.activeInHierarchy) //발 아이콘이 나타났는지
        {
            if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began) && artworkHit) //터치가 있었는지 발 아이콘이 작품에 들어가 있고
            {
                Vector2 touch = Input.GetTouch(0).position;
                Vector3 touchPos = new Vector3(touch.x, touch.y, 0);
                Ray t_ray = Camera.main.ScreenPointToRay(touchPos);
                RaycastHit t_hit;

                if (Physics.Raycast(t_ray, out t_hit))
                {
                    if (t_hit.collider != null)
                    {
                        if (t_hit.collider.gameObject.tag == "redfish") //터치가 어떤 오브젝트와 충돌했는지
                        {
                            Instantiate(particle, t_hit.point, Quaternion.LookRotation(t_hit.normal));
                            Destroy(redfish);
                        }
                        else if(t_hit.collider.gameObject.tag == "rise")
                        {
                            rise = true; //물 속에 있다가 수면 위로!
                        }
                        else if(t_hit.collider.gameObject.tag == "froggy")
                        {
                            isDie = false;
                            rigid.AddForce(Vector3.up * jumpForce);
                            frog_anim.SetTrigger("Jump");
                        }
                    }
                }
            }
            //if((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
            //{
            //Vector2 touch = Input.GetTouch(0).position;
            //Vector3 touchPos = new Vector3(touch.x, touch.y, 0);
            Ray ray = Camera.main.ScreenPointToRay(foot.position); //touchPos
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.tag == "artwork") //발 아이콘이 어떤 오브젝트와 충돌했는지. 어떤 오브젝트 위에 와 있는지
                    {
                        
                        if (!artworkHit) //발 아이콘이 작품에 들어갔을 때 일어나는 일들
                        {
                            if (redfish_anim != null)
                            {
                                redfish_anim.enabled = true;
                            }

                            frog_anim.enabled = true;

                            riverSound.Play();
                            artworkHit = true;

                            waterflow = true;
                        }
            
                    }
                    else if(hit.collider.gameObject.tag == "froggy" && !isDie)
                    {
                        isDie = true;
                        frog_anim.SetTrigger("Die");
                    }

                }
                
                //float offsetX = Time.time * scrollX;
                //float offsetY = Time.time * scrollY;
                //GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
                /*if (raycastHit.collider.gameObject.tag == "artwork")
                {
                    waterflow = true;
                }*/
            }
            else
            {
                if(artworkHit) //발 아이콘이 작품에서 빠져나왔을 때 일어나는 일들
                {
                    if (redfish_anim != null)
                    {
                        redfish_anim.enabled = false;
                    }

                    frog_anim.enabled = false;

                    riverSound.Stop();
                    artworkHit = false;

                    waterflow = false;
                } 
            }
            //}
        }

        if (waterflow)
        {
            offset += (flowRate * Time.deltaTime);

            river.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
        }
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
}
