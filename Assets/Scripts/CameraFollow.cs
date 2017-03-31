using UnityEngine;
using System.Collections;

/// <summary>
/// 摄像机跟随角色移动
/// </summary>
public class CameraFollow : MonoBehaviour {

    private Transform m_Transform;
    private Transform m_Player;

    public bool startFollow = false; //判断摄像机是否跟随角色移动

	// Use this for initialization
	void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Player = GameObject.Find("cube_books").GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        CameraMove();
	}
    void CameraMove()
    {
        if (startFollow)
        {
            Vector3 newPos = new Vector3(m_Transform.position.x, m_Player.position.y + 1.8f, m_Player.position.z);
            //m_Transform.position = newPos;
            m_Transform.position = Vector3.Lerp(m_Transform.position, newPos, Time.deltaTime);

        }
    }
}
