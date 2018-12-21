using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define the base state
public class IndividualState : MonoBehaviour {
    protected IndividualFSMSystem system;
    public string stateName = "EmptyState";

    public string StateName
    {
        get { return stateName; }
    }

    // Use this for initialization
    virtual protected void Start() {
        system = GetComponent<IndividualFSMSystem>();
    }

    // When active
    virtual public void Active() { }

    virtual public void Enter() { }
}
