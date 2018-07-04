﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class Hand : MonoBehaviour {

    IGrabbable hoverObject;
    Grabbable holdObject;

    SteamVR_TrackedObject controllerObject;

    void Awake() {
        controllerObject = GetComponent<SteamVR_TrackedObject>();
    }

    void Update() {
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip) && hoverObject != null && holdObject == null) {
            hoverObject.TryGrab(this);
        } else if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip) && holdObject != null) {
            holdObject.TryRelease(this);
        }
    }

    public void PlaceInHand(Grabbable obj) {
        if (holdObject != null) {
            Debug.LogError("Tried to pick up an object while holding one!");
        }
        holdObject = obj;

        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        joint.breakForce = 20000;
        joint.breakTorque = 20000;
        joint.connectedBody = obj.GetComponent<Rigidbody>();
        
    }
    public void Release() {
        holdObject = null;
        Destroy(this.GetComponent<FixedJoint>());
    }

    void FixedUpdate() {
        hoverObject = null;
    }

    void OnTriggerStay(Collider other) {
        if (holdObject != null)
            return;

        IGrabbable otherGrabbable = other.GetComponent<IGrabbable>();
        if (otherGrabbable == null)
            return;

        hoverObject = otherGrabbable;
    }

    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)controllerObject.index); }
    }
}
