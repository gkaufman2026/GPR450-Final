using System;
using System.Collections.Generic;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        CalcDistance();
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
                arrTargets[i].target.position = Vector3.Lerp(arrTargets[i].target.position, arrTargets[i].tracker.transform.position, 1);
            }
        }
    }
}
