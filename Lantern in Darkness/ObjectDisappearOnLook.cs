using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObjectDisappearOnLook : MonoBehaviour
{
    private Camera vrCamera; 
    public float maxDistance = 10f;
    private GameObject firsthitObject;

    private void Start()
    {        
        vrCamera = Camera.main;
    }

    private void Update()
    {        
        RaycastHit hit;
        Debug.DrawRay(vrCamera.transform.position, vrCamera.transform.forward, Color.red, maxDistance);
        if (Physics.Raycast(vrCamera.transform.position, vrCamera.transform.forward, out hit, maxDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.name == "StairDoll")
                hitObject.GetComponent<DollRun>().SetDestinationToTarget();

            if(hitObject.name == "SneakDoll")
                hitObject.GetComponent<DOTweenAnimation>().DOPlayBackwards();

            if (hitObject.name == "WindowDollTrigger")
                hitObject.transform.GetChild(0).gameObject.SetActive(true);

            if (hitObject.name == "CreatureTrigger")
                hitObject.transform.GetChild(0).GetComponent<Animator>().enabled = true;
        }
    }
}
