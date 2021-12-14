using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//static classes can be accessed in the entire solution without creating an object of the class
public static class Util
{
    /// <summary>
    /// Extension method to Unities Vector3Int class. Now you can use a Vector3 variable and use the .ToVector3InRound to get the vector rounded to its integer values
    /// </summary>
    /// <param name="v">the Vector3 variable this method is applied to</param>
    /// <returns>the rounded Vector3Int value of the given Vector3</returns>
    public static Vector3Int ToVector3IntRound(this Vector3 v) => new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
    public static bool TryOrientIndex(Vector3Int localIndex, Vector3Int anchor, Quaternion rotation, Vector3Int gridDimensions, out Vector3Int worldIndex)
    {
        var rotated = rotation * localIndex;
        worldIndex = anchor + rotated.ToVector3IntRound();
        return CheckInBounds(gridDimensions, worldIndex);
    }

    public static List<Vector3Int> Directions = new List<Vector3Int>
    {
        new Vector3Int(-1,0,0),// min x
        new Vector3Int(1,0,0),// plus x
        new Vector3Int(0,-1,0),// min y
        new Vector3Int(0,1,0),// plus y
        new Vector3Int(0,0,-1),// min z
        new Vector3Int(0,0,1)// plus z
    };
    public static bool CheckInBounds(Vector3Int gridDimensions, Vector3Int index)
    {
        if (index.x<0 || index.x >= gridDimensions.x) return false;
        if (index.y<0 || index.y >= gridDimensions.y) return false;
        if (index.z<0 || index.z >= gridDimensions.z) return false;

        return true;
    }
}
