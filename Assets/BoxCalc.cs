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
    [Range(0,1)]
    [SerializeField] public float snapDistance = 0.57f;
    [Range(0, 1)]
    [SerializeField] public float legSmoothing = 0.4f;

    private bool alternateLegCall = false;

    private void Update()
    {
        CalcDistance();

        if(alternateLegCall)
        {
            handleOddLeggs();
        }
    }

    private void CalcDistance()
    {
        for(int i = 0; i < arrTargets.Count; i++)
        {
            Vector3 dif = arrTargets[i].target.position - arrTargets[i].tracker.transform.position;
            if (MathF.Abs(dif.x) > snapDistance || 
                MathF.Abs(dif.y) > snapDistance || 
                MathF.Abs(dif.z) > snapDistance) //   :(
            {

                int otherLegIndex = i + 2;
                if (otherLegIndex > 3)
                {
                    otherLegIndex = otherLegIndex - 4;
                }
                if(i == 0 || i == 1)
                {
                    StartCoroutine(LerpLeg(arrTargets[i].target.position, arrTargets[i].tracker.transform.position, i, true));
                }

                //StartCoroutine(LerpLeg(arrTargets[otherLegIndex].target.position, arrTargets[otherLegIndex].tracker.transform.position, otherLegIndex));
            }
        }
    }

    private void handleOddLeggs()
    {
        StartCoroutine(LerpLeg(arrTargets[2].target.position, arrTargets[2].tracker.transform.position, 2, false));
        StartCoroutine(LerpLeg(arrTargets[3].target.position, arrTargets[3].tracker.transform.position, 3, false));

    }

    IEnumerator LerpLeg(Vector3 tar, Vector3 tracker, int index, bool callLeg)
    {

        float totalTime = 0;

        while(totalTime < legSmoothing)
        {
            float t = totalTime / legSmoothing;
            
            arrTargets[index].target.transform.position = BlendNodes.LerpVec3(tar, tracker + new Vector3 (0,MathF.Sin(t *MathF.PI) * 0.2f, 0), t);

            totalTime += Time.deltaTime;

            yield return null;
        }

        
        arrTargets[index].target.transform.position = tracker;
        alternateLegCall = callLeg;
        
    }
}
