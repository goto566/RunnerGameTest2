using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentAI : MonoBehaviour
{
    public CharacterControls charControl;

    public GameObject currentWaypoint;
    float waypointResetTimer;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.m_gameStarted)
            return;
        Vector3 targetPos = Vector3.zero;
        waypointResetTimer -= Time.deltaTime;
        if (waypointResetTimer <= 0)
            currentWaypoint = null;
        if (currentWaypoint == null)
        {
            currentWaypoint = GetWaypoint();
            if (currentWaypoint == null)
            {
                targetPos = charControl.endPos.transform.position;
            }
            else
            {
                targetPos = currentWaypoint.transform.position;
                waypointResetTimer = 2;
            }

        }
        else
        {
            float dist = Vector3.Distance(transform.position, currentWaypoint.transform.position);

            if (dist <= 2)
            {
                currentWaypoint = null;
                Debug.Log("Currentwaypoint null");
                return;
            }
            else
                targetPos = currentWaypoint.transform.position;
        }

        Vector3 dir = (targetPos - transform.position).normalized;
        dir.y = 0;
        charControl.moveDir = transform.forward;
        Quaternion rotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * charControl.rotateSpeed);
        charControl.anim.SetBool("isRunning", true);

    }
    public float lineLenght = 10;
    public int LineCount = 10;
    public LayerMask waypointLayer;

    public GameObject GetWaypoint()
    {
        for (int i = -LineCount; i < LineCount; i++)
        {
            Ray ray = new Ray(transform.position, new Vector3(i, 2, 1 * lineLenght).normalized);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, lineLenght, waypointLayer))
            {
                if (Vector3.Distance(transform.position, hit.collider.gameObject.transform.position) < 2)
                {
                    Debug.Log("Currentwaypoint < 2 ");
                    continue;
                }
                return hit.collider.gameObject;
            }
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        bool first = false;
        for (int i = -LineCount; i < LineCount; i++)
        {
            Ray ray = new Ray(transform.position, new Vector3(i, 2, 1 * lineLenght).normalized);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, lineLenght, waypointLayer))
            {
                if (!first)
                {
                    first = true;
                    Gizmos.color = Color.yellow;
                }
                else
                    Gizmos.color = Color.green;
            }
            else Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(i, 2, 1 * lineLenght));
        }
    }

}
