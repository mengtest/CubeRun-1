using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 地图管理器
/// </summary>
public class MapManager : MonoBehaviour {

    private GameObject prefabs_tile;
    private GameObject prefabs_wall;
    private GameObject prefabs_spikes;
    private GameObject prefabs_sky_spikes;
    private GameObject prefabs_gem;

    private Transform m_transform;
    private PlayerController m_PlayerController;

    //陷阱概率
    private int pr_hole = 0;
    private int pr_spikes = 0;
    private int pr_sky_spikes = 0;

    private int pr_gem = 2;

    //地图数据存储
    public List<GameObject[]> mapList = new List<GameObject[]>();

    public float bottomLength = Mathf.Sqrt(2) * 0.254f;
    private int index = 0;
    

    private Color colorWall = new Color(87 / 255f, 93 / 255f, 169 / 255f);
    private Color colorOne = new Color(124 / 255f, 155 / 255f, 230 / 255f);
    private Color colorTwo = new Color(125 / 255f, 169 / 255f, 233 / 255f);

	// Use this for initialization
	void Start () {
        prefabs_tile = Resources.Load("tile_white") as GameObject;
        prefabs_wall = Resources.Load("wall2") as GameObject;
        prefabs_spikes = Resources.Load("moving_spikes") as GameObject;
        prefabs_sky_spikes = Resources.Load("smashing_spikes") as GameObject;
        prefabs_gem = Resources.Load("gem 2") as GameObject;

        m_transform = gameObject.GetComponent<Transform>();
        m_PlayerController = GameObject.Find("cube_books").GetComponent<PlayerController>();
        float offsetZ = 0;
        createMapItem(offsetZ);
	}
    /// <summary>
    /// 创建地图元素（段）
    /// </summary>
    public void createMapItem(float z)
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject[] item = new GameObject[6];
            
            for (int j = 0; j < 6; j++)
            {
                Vector3 pos = new Vector3(j * bottomLength, 0, z + i * bottomLength);
                Vector3 rot = new Vector3(-90, 45, 0);
                GameObject tile_item = null;
                if (j == 0 || j == 5)
                {
                    tile_item = GameObject.Instantiate(prefabs_wall, pos, Quaternion.Euler(rot)) as GameObject;
                    tile_item.GetComponent<MeshRenderer>().material.color = colorWall;
                }
                else
                {
                    //
                    int pr = CalPr();
                    if (pr == 0)
                    {
                        tile_item = GameObject.Instantiate(prefabs_tile, pos, Quaternion.Euler(rot)) as GameObject;
                        tile_item.GetComponent<Transform>().Find("normal_a2").GetComponent<MeshRenderer>().material.color = colorOne;
                        tile_item.GetComponent<MeshRenderer>().material.color = colorOne;
                        int gemPr = CalcGemPr();
                        if (gemPr == 1)
                        {
                            GameObject gem = GameObject.Instantiate(prefabs_gem, tile_item.GetComponent<Transform>().position + new Vector3(0, 0.06f, 0), Quaternion.identity) as GameObject;
                            gem.GetComponent<Transform>().parent = tile_item.GetComponent<Transform>();
                        }

                    }
                    else if (pr == 1)
                    {
                        tile_item = new GameObject();
                        tile_item.GetComponent<Transform>().position = pos;
                        tile_item.GetComponent<Transform>().rotation = Quaternion.Euler(rot);
                        int gemPr = CalcGemPr();
                        if (gemPr == 1)
                        {
                            GameObject.Instantiate(prefabs_gem, tile_item.GetComponent<Transform>().position + new Vector3(0, 0.06f, 0), Quaternion.identity);
                        }
                    }
                    else if (pr == 2)
                    {
                        tile_item = GameObject.Instantiate(prefabs_spikes, pos, Quaternion.Euler(rot)) as GameObject;
                    }
                    else if (pr == 3)
                    {
                        tile_item = GameObject.Instantiate(prefabs_sky_spikes, pos, Quaternion.Euler(rot)) as GameObject;
                    }
                }
                tile_item.GetComponent<Transform>().SetParent(m_transform);
                item[j] = tile_item;
            }
            mapList.Add(item);

            GameObject[] item2 = new GameObject[5];
            for (int j = 0; j < 5; j++)
            {
                Vector3 pos = new Vector3(j * bottomLength + bottomLength / 2, 0, z + i * bottomLength + bottomLength / 2);
                Vector3 rot = new Vector3(-90, 45, 0);
                GameObject tile_item = null;
                int pr = CalPr();
                if (pr == 0)
                {
                    tile_item = GameObject.Instantiate(prefabs_tile, pos, Quaternion.Euler(rot)) as GameObject;
                    tile_item.GetComponent<Transform>().Find("normal_a2").GetComponent<MeshRenderer>().material.color = colorTwo;
                    tile_item.GetComponent<MeshRenderer>().material.color = colorTwo;

                }
                else if (pr == 1)
                {
                    tile_item = new GameObject();
                    tile_item.GetComponent<Transform>().position = pos;
                    tile_item.GetComponent<Transform>().rotation = Quaternion.Euler(rot);
                }
                else if (pr == 2)
                {
                    tile_item = GameObject.Instantiate(prefabs_spikes, pos, Quaternion.Euler(rot)) as GameObject;
                }
                else if (pr == 3)
                {
                    tile_item = GameObject.Instantiate(prefabs_sky_spikes, pos, Quaternion.Euler(rot)) as GameObject;
                } 
                tile_item.GetComponent<Transform>().SetParent(m_transform);
                item2[j] = tile_item;
            }
            mapList.Add(item2);
        }
        
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown( KeyCode.Space))
        {
            
            string str = "";
            for (int i = 0; i < mapList.Count; i++)
            {
                for(int j = 0; j < mapList[i].Length; j++){
                    str += mapList[i][j].name;
                    mapList[i][j].name = i + "--" + j;
                }
                str += "\n";
            }
            Debug.Log(str);
        }
	}
    //开始地面塌陷
    public void StartTileDown()
    {
        StartCoroutine("TileDown");
    }
    //停止地面塌陷
    public void StopTileDown()
    {
        StopCoroutine("TileDown");
    }
    //地面塌陷
    private IEnumerator TileDown()
    {
        
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            for (var i = 0; i < mapList[index].Length; i++)
            {
                Rigidbody re= mapList[index][i].AddComponent<Rigidbody>();
                //掉落的地板呈现多样性
                re.angularVelocity = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)) * Random.Range(1, 10);
                GameObject.Destroy(mapList[index][i], 1.0f);

            }
            if (m_PlayerController.z == index)
            {
                StopTileDown();
                m_PlayerController.gameObject.AddComponent<Rigidbody>();
                m_PlayerController.StartCoroutine("GameOver", true);
            }
            index++;
            
        }
    }

    /// <summary>
    /// 计算概率，返回：
    /// 0: 瓷砖
    /// 1: 坑洞
    /// 2：地面陷阱
    /// 3：天空陷阱
    /// </summary>
    /// <returns></returns>
    private int CalPr()
    {
        int pr = Random.Range(1,100);
        if(pr <= pr_hole && pr < 30){
            return 1;
        }else if(pr > 31 && pr <= pr_spikes + 30){
            return 2;
        }
        else if (pr > 61 && pr <= pr_sky_spikes + 60)
        {
            return 3;
        }
        return 0;
    }
    /// <summary>
    /// 宝石生成概率
    /// </summary>
    /// <returns></returns>
    private int CalcGemPr()
    {
        int pr = Random.Range(1, 100);
        if (pr <= pr_gem)
        {
            return 1;
        }
        return 0;
    }
    public void AddPr()
    {
        pr_hole += 2;
        pr_spikes += 2;
        pr_sky_spikes += 2;
        
    }
}
