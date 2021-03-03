using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRise : MonoBehaviour
{
    public GameObject artworkHolder;
    bool rise;
    bool artworkHit;
    public float riseSpeed;
    // Start is called before the first frame update
    void Start()
    {
        riseSpeed = 0.7f;
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void OnTriggerStay(Collider other)
    {
        rise = artworkHolder.GetComponent<Interaction>().rise;
        artworkHit = artworkHolder.GetComponent<Interaction>().artworkHit;

        if (rise && other.gameObject.CompareTag("rise") && artworkHit)
        {
            other.gameObject.transform.Translate(Vector3.up * Time.deltaTime);
        }
    }
}
