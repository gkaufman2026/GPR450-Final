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
    [Range(0, 10)]
    [SerializeField] public float legJitter = 1f;

    private void Update()
    {
        CalcDistance();
    }

    private void FixedUpdate()
    {
        
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
                StartCoroutine(LerpLeg(arrTargets[i].target.position, arrTargets[i].tracker.transform.position, i));

            }
        }
    }

    IEnumerator LerpLeg(Vector3 tar, Vector3 tracker, int index)
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
    }
}
