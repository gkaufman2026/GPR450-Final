using UnityEngine;

public class BlendNodes : MonoBehaviour
{
    //Near
    //Lerp

    public Vector3 NearestVec3(Vector3 input)
    {
        Vector3 nearest = new Vector3(Mathf.Round(input.x), Mathf.Round(input.y), Mathf.Round(input.z));
        return nearest;
    }

    public Vector3 LerpVec3(Vector3 start, Vector3 end, float time)
    {
        return new Vector3(start.x + (end.x - start.x) * time, start.y + (end.y - start.y) * time, start.z + (end.z - start.z) * time);
    }
}
