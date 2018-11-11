using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define the base state
public class FSMState : MonoBehaviour {
    protected FSMSystem system;

    // Use this for initialization
    virtual protected void Start() {
        system = GetComponent<FSMSystem>();
    }

    // When active
    virtual public void Active () { }
}
