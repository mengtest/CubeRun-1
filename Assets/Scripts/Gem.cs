using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour {

    Transform m_Transform;
    Transform gem;

	// Use this for initialization
	void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        gem = m_Transform.FindChild("gem 3").GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        gem.Rotate(Vector3.up);
	}
}
