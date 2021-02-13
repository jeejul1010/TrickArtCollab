using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    [Header("Unity Setup")]
    public float time;

    /*public bool shakeCamera;
    [Range(0f, 1f)]
    public float duration;
    [Range(0f, 1f)]
    public float magnitude;*/

    void Start()
    {
        /*if(shakeCamera)
        {
            StartCoroutine(FindObjectOfType<CameraShake>().Shake(duration, magnitude));
        }*/
        Destroy(gameObject, time);
    }

}
