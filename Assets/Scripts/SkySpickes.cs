using UnityEngine;
using System.Collections;

/// <summary>
/// 天空陷阱生成
/// </summary>
public class SkySpickes : MonoBehaviour {

    Transform m_Transform;
    Transform m_Son_Transform;

    Vector3 normal;
    Vector3 target;

    void Start()
    {
        m_Transform = transform.GetComponent<Transform>();
        m_Son_Transform = m_Transform.FindChild("smashing_spikes_b").GetComponent<Transform>();

        normal = m_Son_Transform.position;
        target = m_Son_Transform.position + new Vector3(0, 0.6f, 0);
        StartCoroutine("UpAndDown");
    }
    private IEnumerator UpAndDown()
    {
        while (true)
        {
            StopCoroutine("Down");
            StartCoroutine("Up");
            yield return new WaitForSeconds(2.0f);
            StopCoroutine("Up");
            StartCoroutine("Down");
            yield return new WaitForSeconds(2.0f);
        }
    }
    private IEnumerator Up()
    {
        while (true)
        {
            m_Son_Transform.position = Vector3.Lerp(m_Son_Transform.position, target, Time.deltaTime * 25);

            yield return null;
        }
    }
    private IEnumerator Down()
    {
        while (true)
        {
            m_Son_Transform.position = Vector3.Lerp(m_Son_Transform.position, normal, Time.deltaTime * 25);
            yield return null;
        }
    }
}
