using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public LineRenderer leftControllerTrail; 
    public LineRenderer rightControllerTrail;

    public Transform leftControllerTransform; 
    public Transform rightControllerTransform;
    public Material customMaterial; 
    public float trailWidth = 0.1f; 

    void Start()
    {
        
        leftControllerTrail = leftControllerTransform.gameObject.AddComponent<LineRenderer>();
        leftControllerTrail.material = customMaterial;
        customMaterial.color = Color.yellow;
        leftControllerTrail.startWidth = trailWidth;
        leftControllerTrail.endWidth = 0;

        rightControllerTrail = rightControllerTransform.gameObject.AddComponent<LineRenderer>();
        rightControllerTrail.material = customMaterial;
        
        rightControllerTrail.startWidth = trailWidth;
        rightControllerTrail.endWidth = 0;
    }

    void Update()
    {
        Show();
    }

    void Show()
    {       
        leftControllerTrail.SetPosition(0, leftControllerTransform.position);
        Vector3 leftPreviousPosition = leftControllerTransform.position - leftControllerTransform.forward * 0.1f;
        leftControllerTrail.SetPosition(1, leftPreviousPosition);
  
        rightControllerTrail.SetPosition(0, rightControllerTransform.position);
        Vector3 rightPreviousPosition = rightControllerTransform.position - rightControllerTransform.forward * 0.1f;
        rightControllerTrail.SetPosition(1, rightPreviousPosition);
    }
}
