using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BoxCalc : MonoBehaviour
{
    [Serializable]
    public class Harch
    {
        public Transform target;
        public GameObject tracker;
    }

    [SerializeField] private List<Harch> arrTargets;
    [Range(0, 1)]
    [SerializeField] public float snapDistance = 0.57f;
    [Range(0, 1)]
    [SerializeField] public float legSmoothing = 0.4f;

    private bool alternateLegCall = false;
    private Vector3 lastBodyUp;

    private void Start()
    {
        lastBodyUp = transform.up;
    }

    private void Update()
    {
        CalcDistance();

        if (alternateLegCall)
        {
            handleOddLeggs();
        }

        //rotateBody();
    }

    private void CalcDistance()
    {
        for (int i = 0; i < arrTargets.Count; i++)
        {
            Vector3 dif = arrTargets[i].target.position - arrTargets[i].tracker.transform.position;

            //if distance is too far, snap
            if (MathF.Abs(dif.x) > snapDistance ||
                MathF.Abs(dif.y) > snapDistance ||
                MathF.Abs(dif.z) > snapDistance) //   :(
            {

                if (i == 0 || i == 1)
                {
                    //calls two pairs of leggs
                    StartCoroutine(LerpLeg(arrTargets[i].target.position, arrTargets[i].tracker.transform.position, i, true));
                }

            }

            RaycastHit hit;

            if (Physics.Raycast(arrTargets[i].tracker.transform.position, Vector3.down, out hit, 10))
            {
                Debug.DrawRay(arrTargets[i].tracker.transform.position, Vector3.down * hit.distance, Color.green);
            }
        }
    }

    private void handleOddLeggs()
    {
        for (int i = 2; i < 4; i++)
        {
            //handles movement for offlegs simulating spider movement
            StartCoroutine(LerpLeg(arrTargets[2].target.position, arrTargets[2].tracker.transform.position, 2, true));
            StartCoroutine(LerpLeg(arrTargets[3].target.position, arrTargets[3].tracker.transform.position, 3, false));
            alternateLegCall = false;
        }

    }

    //smooths the leg movement to the location
    IEnumerator LerpLeg(Vector3 tar, Vector3 tracker, int index, bool callLeg)
    {

        float totalTime = 0;

        while (totalTime < legSmoothing)
        {
            float t = totalTime / legSmoothing;

            arrTargets[index].target.transform.position = BlendNodes.LerpVec3(tar, tracker + new Vector3(0, MathF.Sin(t * MathF.PI) * 0.2f, 0), t);

            totalTime += Time.deltaTime;

            yield return null;
        }


        arrTargets[index].target.transform.position = tracker;
        alternateLegCall = callLeg;

    }

    void rotateBody()
    {

        Vector3 v1 = arrTargets[0].target.position - arrTargets[1].target.position;
        Vector3 v2 = arrTargets[2].target.position - arrTargets[3].target.position;
        Vector3 normal = Vector3.Cross(v1, v2).normalized;
        Vector3 up = Vector3.Lerp(lastBodyUp, normal, 1f / (float)(8));
        transform.up = up;
        transform.rotation = Quaternion.LookRotation(transform.parent.forward, up);
        lastBodyUp = transform.up;
    }
}
