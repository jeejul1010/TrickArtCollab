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
    public GameObject leftSpinLily;
    public GameObject rightSpinLily;
    public GameObject standingLotus;

    public GameObject updownLotus;
    public GameObject farleftLily;
    public GameObject middleLily;
    public GameObject farrightLily;

    Animator redfish_anim; //물고기 애니메이션
    Animator frog_anim; //개구리 애니메이션
    Animator log1_anim;
    Animator leftSpin_anim;
    Animator rightSpin_anim;
    Animator updown_anim;
    Animator farleft_anim;
    Animator middle_anim;
    Animator farright_anim;

    private Rigidbody rigid;

    [SerializeField] private float jumpForce;

    AudioSource riverSound;

    bool waterflow; //물 흐르기 여부
    public bool artworkHit; //작품에 발을 들였는지 여부
    public bool rise;
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

        leftSpin_anim = leftSpinLily.GetComponent<Animator>();
        rightSpin_anim = rightSpinLily.GetComponent<Animator>();
        leftSpin_anim.enabled = false;
        rightSpin_anim.enabled = false;

        updown_anim = updownLotus.GetComponent<Animator>();
        updown_anim.enabled = false;
        rise = false;

        farleft_anim = farleftLily.GetComponent<Animator>();
        farright_anim = farrightLily.GetComponent<Animator>();
        middle_anim = middleLily.GetComponent<Animator>();
        farleft_anim.enabled = false;
        farright_anim.enabled = false;
        middle_anim.enabled = false;

        log1_anim = GameObject.FindGameObjectWithTag("log1").GetComponent<Animator>();
        log1_anim.enabled = false;

        riverSound = GetComponent<AudioSource>();
        artworkHit = false;

        waterflow = false;

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
                        else if(t_hit.collider.gameObject == updownLotus)
                        {
                            if(updown_anim.enabled == false)
                            {
                                updown_anim.enabled = true;
                            }
                            

                            if(rise==false && !isPlaying(updown_anim, "lotusflowerdown"))
                            {
                                updown_anim.SetBool("up", true);
                                rise = true;
                            }
                            else if(rise==true && !isPlaying(updown_anim, "lotusflower"))
                            {
                                updown_anim.SetBool("up", false);
                                rise = false;
                            }
                            //rise = true; //물 속에 있다가 수면 위로!
                        }
                        else if(t_hit.collider.gameObject.tag == "froggy")
                        {
                            isDie = false;
                            rigid.AddForce(Vector3.up * jumpForce);
                            frog_anim.SetTrigger("Jump");
                        }
                        else if(t_hit.collider.gameObject == standingLotus)
                        {
                            leftSpin_anim.enabled = true;
                            rightSpin_anim.enabled = true;

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
                    if(!updown_anim.enabled)
                    {
                        farleft_anim.enabled = true;
                        farright_anim.enabled = true;
                        middle_anim.enabled = true;
                    }
                    else
                    {
                        if (isPlaying(updown_anim, "lotusflower") || isPlaying(updown_anim, "lotusflowerdown"))
                        {
                            farleft_anim.enabled = false;
                            farright_anim.enabled = false;
                            middle_anim.enabled = false;
                        }
                        else
                        {
                            farleft_anim.enabled = true;
                            farright_anim.enabled = true;
                            middle_anim.enabled = true;
                        }
                    }
                  
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
                    else if(hit.collider.gameObject.tag == "log1")
                    {
                        log1_anim.enabled = true;
                    }

                    if(hit.collider.gameObject.tag != "log1")
                    {
                        log1_anim.enabled = false;
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

                    farleft_anim.enabled = false;
                    farright_anim.enabled = false;
                    middle_anim.enabled = false;

                    rightSpin_anim.enabled = false;
                    leftSpin_anim.enabled = false;

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

    bool isPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }
}
