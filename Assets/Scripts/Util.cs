using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//static classes can be accessed in the entire solution without creating an object of the class
public static class Util
{
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
