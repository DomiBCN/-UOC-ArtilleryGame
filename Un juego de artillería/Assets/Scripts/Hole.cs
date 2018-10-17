using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour {

    public Transform parent;

	// Use this for initialization
	void Start () {
        //parent = transform.parent;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        parent.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        parent.GetComponent<BoxCollider2D>().enabled = true;
    }
}
