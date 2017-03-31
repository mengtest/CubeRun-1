using UnityEngine;
using System.Collections;

/// <summary>
/// 角色控制
/// </summary>
public class PlayerController : MonoBehaviour {

    public int z = 3;
    private int x = 2;
    private Transform  m_Transform;
    private MapManager m_MapManager;
    private CameraFollow m_CameraFollow;

    private bool life = true;
    //得分
    private int gemCount = 0;
    private int score = 0;

    private void AddGemCount()
    {
        gemCount++;
        Debug.Log("宝石数：" + gemCount);
    }
    private void AddScore()
    {
        score++;
        Debug.Log("分数：" + score);
    }

    //蜗牛痕迹的两种颜色
    private Color colorOne = new Color(122 / 255f, 85 / 255f, 179 / 255f);
    private Color colorTwo = new Color(126 / 255f, 93 / 255f, 183 / 255f);

    Vector3 offsetPos = new Vector3(0, 0.254f / 2, 0);

	// Use this for initialization
	void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        m_MapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        m_CameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
    
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetPlayerPos();
            m_CameraFollow.startFollow = true;
            m_MapManager.StartTileDown();
        }
        CalcPosition();
        if (life)
        {
            PlayerMove();
        }
        
        
	}
    //控制角色移动，A向左移动，D向右移动
    private void PlayerMove()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (x != 0)
            {
                z++;
                AddScore();
            }
            if (z % 2 == 1 && x != 0)
            {
                x--;
            }
            Debug.Log("Left: z:" + z + "--" + "x: " + x);
            SetPlayerPos();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!(x == 4 && z % 2 == 1))
            {
                z++;
                AddScore();
            }
            if (z % 2 == 0)
            {
                x++;
            }
            Debug.Log("Right: z:" + z + "--" + "x: " + x);
            SetPlayerPos();
        }
    }
    
    //设置角色的位置，并将角色经过的地板改变颜色（蜗牛痕迹）
    private void SetPlayerPos()
    {
        Transform playerPos = m_MapManager.mapList[z][x].GetComponent<Transform>();
        MeshRenderer normal_a2 = null;
        if (playerPos.tag == "Tile")
        {
            normal_a2 = playerPos.FindChild("normal_a2").GetComponent<MeshRenderer>();
        }
        else if (playerPos.tag == "Spike")
        {
            normal_a2 = playerPos.FindChild("moving_spikes_a2").GetComponent<MeshRenderer>();
        }
        else if (playerPos.tag == "Sky_Spike")
        {
            normal_a2 = playerPos.FindChild("smashing_spikes_a2").GetComponent<MeshRenderer>();
        }

        if (normal_a2 != null)
        {
            if (z % 2 == 0)
            {
                normal_a2.material.color = colorOne;
            }
            else
            {
                normal_a2.material.color = colorTwo;
            }
        }
        else
        {
            m_Transform.gameObject.AddComponent<Rigidbody>();
            StartCoroutine("GameOver", true);
        }
        
        m_Transform.position = playerPos.position + offsetPos;
        m_Transform.rotation = playerPos.rotation;
    }
    
    //计算连续地图的初始位置
    private void CalcPosition()
    {
        if(m_MapManager.mapList.Count - z <= 12)
        {
            m_MapManager.AddPr();
            Debug.Log("MAP");
            float offsetZ = m_MapManager.mapList[m_MapManager.mapList.Count - 1][0].GetComponent<Transform>().position.z + m_MapManager.bottomLength / 2;
            m_MapManager.createMapItem(offsetZ);
        }
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Spike_attack")
        {
            StartCoroutine("GameOver", false);
        }
        else if (coll.tag == "Sky_Spike_attack")
        {
            StartCoroutine("GameOver", false);
        }
        else if (coll.tag == "Gem")
        {
            GameObject.Destroy(coll.gameObject.transform.parent.gameObject);
            AddGemCount();
        }
    }
    public IEnumerator GameOver(bool b)
    {
        if (b)
        {
            yield return new WaitForSeconds(0.5f);
        }
        if (life)
        {
            Debug.Log("游戏结束!");
            m_CameraFollow.startFollow = false;
            life = false;
            //TODO:UI相关的交互
        }
        //Time.timeScale = 0;
    }
}
