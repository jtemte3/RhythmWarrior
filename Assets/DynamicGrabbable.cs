using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DynamicGrabbable : XRGrabInteractable
{

    private Vector3 initialAttachLocalPos;
    private Quaternion initialAttachLocalRot;
    
    // Start is called before the first frame update
    void Start()
    {
        //Create Attach Point
        if (!attachTransform)
        {
            GameObject grab = new GameObject("Grab Pivot");
            grab.transform.SetParent(transform, false);
            attachTransform = grab.transform;
        }

        initialAttachLocalPos = attachTransform.localPosition;
        initialAttachLocalRot = attachTransform.localRotation;
    }

    protected override void OnSelectEntering(SelectEnterEventArgs arg)
    {
        
        if (arg.interactorObject is XRDirectInteractor)
        {
            attachTransform.position = arg.interactorObject.transform.position;
            attachTransform.rotation = arg.interactorObject.transform.rotation;
        }
        else
        {
            attachTransform.localPosition = initialAttachLocalPos;
            attachTransform.localRotation = initialAttachLocalRot;
        }

        base.OnSelectEntering(arg);
    }
}
