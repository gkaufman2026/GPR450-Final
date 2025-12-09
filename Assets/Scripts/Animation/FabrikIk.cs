using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

//https://www.youtube.com/watch?v=UNoX65PRehA
//https://www.youtube.com/watch?v=qqOAzn05fvk
//FABRIK!!! saves TONS of work using vectors
/*
 * Forwards
 * And
 * Backward
 * Reaching
 * Inverse
 * Kinematics
 */
//https://docs.unity3d.com/Packages/com.unity.2d.ik@1.3/manual/index.html
//https://www.youtube.com/watch?v=Ihp6tOCYHug

public class FabrikIK : MonoBehaviour
{
    // Target the chain should bent to
    public Transform target;
    public Transform effector;

    public bool showGizmo = true;

    // Chain length of bones
    [Range(0, 10)]
    public int chainLength = 1;

    // Solver iterations per update
    [Range(0, 20)]
    public int iterations = 10;

    // Strength of going back to the start position.
    [Range(0, 1)]
    public float weight = 1f;

    // Distance when the solver stops
    public float dT = 0.001f;

    private float[] arrBoneLength;
    private float totalLength;
    private Transform[] arrBones;
    private Vector3[] arrPositions;
    private Vector3[] arrStartDirPrev;
    private Quaternion[] arrInitialRotationBone;
    private Quaternion initialRotTarget;
    private Transform tRoot;


    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    void Init()
    {
        arrBones = new Transform[chainLength + 1];
        arrPositions = new Vector3[chainLength + 1];
        arrBoneLength = new float[chainLength];
        arrStartDirPrev = new Vector3[chainLength + 1];
        arrInitialRotationBone = new Quaternion[chainLength + 1];

        //find root
        tRoot = transform;
        for (var i = 0; i <= chainLength; i++)
        {
            if (tRoot == null)
                throw new UnityException("Not Parented!");
            tRoot = tRoot.parent;
        }

        //target
        if (target == null)
        {
            target = new GameObject(gameObject.name + " target").transform;
            SetPositionRootSpace(target, GetPositionRootSpace(transform));
        }
        initialRotTarget = GetRotationRootSpace(target);

        var current = transform;
        totalLength = 0;
        for (var i = arrBones.Length - 1; i >= 0; i--)
        {
            arrBones[i] = current;
            arrInitialRotationBone[i] = GetRotationRootSpace(current);

            if (i == arrBones.Length - 1) // LEAF bone
            {
                arrStartDirPrev[i] = GetPositionRootSpace(target) - GetPositionRootSpace(current);
            }
            else  // MIDDLE bones
            {

                arrStartDirPrev[i] = GetPositionRootSpace(arrBones[i + 1]) - GetPositionRootSpace(current);
                //dist btwn two tranforms
                arrBoneLength[i] = arrStartDirPrev[i].magnitude;
                totalLength += arrBoneLength[i];
            }
            current = current.parent;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if (target == null)
            return;

        if (arrBoneLength.Length != chainLength)
        {
            Init();
        }

        //get position
        for (int i = 0; i < arrBones.Length; i++)
        {
            arrPositions[i] = GetPositionRootSpace(arrBones[i]);
        }

        var targetPosition = GetPositionRootSpace(target);
        var rootRot = (arrBones[0].parent !=null) ? arrBones[0].parent.rotation : Quaternion.identity;
        var rootRotDif = Quaternion.Inverse(rootRot) * tRoot.rotation;
        
        //cant reach effector(target)?
        if ((targetPosition - GetPositionRootSpace(arrBones[0])).sqrMagnitude >= totalLength * totalLength)
        {
            var direction = (targetPosition - arrPositions[0]).normalized;
            //fixes positions inverse order
            for (int i = 1; i < arrPositions.Length; i++)
                arrPositions[i] = arrPositions[i - 1] + direction * arrBoneLength[i - 1];
        }
        else
        {
            for (int i = 0; i < arrPositions.Length - 1; i++)
            {
                arrPositions[i + 1] = Vector3.Lerp(arrPositions[i + 1], arrPositions[i] + arrStartDirPrev[i], weight);
            }
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                //Ik
                for (int i = arrPositions.Length - 1; i > 0; i--)
                {
                    if (i == arrPositions.Length - 1)
                        arrPositions[i] = targetPosition;
                    else
                    {
                        arrPositions[i] = arrPositions[i + 1] + (arrPositions[i] - arrPositions[i + 1]).normalized * arrBoneLength[i];
                    }
                }

                //Fk
                for (int i = 1; i < arrPositions.Length; i++)
                {
                    arrPositions[i] = arrPositions[i - 1] + (arrPositions[i] - arrPositions[i - 1]).normalized * arrBoneLength[i - 1];
                }

                //checks to see if pos is at target, if so, stop iterating
                if ((arrPositions[arrPositions.Length - 1] - targetPosition).sqrMagnitude < dT * dT)
                    break;
            }
        }

        //move towards effector
        if (effector != null)
        {
            //functions like a shadow
            for (int i = 1; i < arrPositions.Length-1; i++)
            {
                var plane = new Plane(arrPositions[i+1] - arrPositions[i-1], arrPositions[i-1]);
                var projectedPole = plane.ClosestPointOnPlane(effector.position);
                var projectedBone = plane.ClosestPointOnPlane(arrPositions[i]);
                var angle = Vector3.SignedAngle(projectedBone - arrPositions[i-1], projectedPole - arrPositions[i-1], plane.normal);
                arrPositions[i] = Quaternion.AngleAxis(angle, plane.normal) * (arrPositions[i] - arrPositions[i-1]) + arrPositions[i-1];
            }
        }

        //set position & rotation
        for (int i = 0; i < arrPositions.Length; i++)
        {
            if (i == arrPositions.Length - 1)
            {
                SetRotationRootSpace(arrBones[i], Quaternion.Inverse(rootRotDif) * initialRotTarget * Quaternion.Inverse(arrInitialRotationBone[i]));
            } 
            else
            {
                SetRotationRootSpace(arrBones[i], Quaternion.FromToRotation(arrStartDirPrev[i], arrPositions[i + 1] - arrPositions[i])
                    * Quaternion.Inverse(arrInitialRotationBone[i]));
            }
                SetPositionRootSpace(arrBones[i], arrPositions[i]);
        }
    }

    private Vector3 GetPositionRootSpace(Transform current)
    {
        if (tRoot == null)
        {
            return current.position;
        }
        else
        {
            return Quaternion.Inverse(tRoot.rotation) * (current.position - tRoot.position);
        }
    }
    private void SetPositionRootSpace(Transform current, Vector3 position)
    {
        if (tRoot == null)
        {
            current.position = position;
        }
        else
        {
            current.position = tRoot.rotation * position + tRoot.position;
        }
    }

    private Quaternion GetRotationRootSpace(Transform current)
    {
        if (tRoot == null)
        {
            return current.rotation;
        }
        else
        {
            return Quaternion.Inverse(current.rotation) * tRoot.rotation;
        }
    }

    private void SetRotationRootSpace(Transform current, Quaternion rotation)
    {
        if (tRoot == null)
        {
            current.rotation = rotation;
        }
        else
        {
            current.rotation = tRoot.rotation * rotation;
        }
    }

    void OnDrawGizmos()
    {
        if (showGizmo) {
            //visualizes FABRIK
            var current = this.transform;
            for (int i = 0; i < chainLength && current != null && current.parent != null; i++)
            {
                var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
                Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
                Handles.color = Color.red;
                Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
                current = current.parent;
            }
        }
    }
}