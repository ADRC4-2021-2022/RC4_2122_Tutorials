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
    

    /// <summary>
    /// List of the Carthesian directions (along the x, y, z axis)
    /// </summary>
    public static List<Vector3Int> Directions = new List<Vector3Int>
    {
        new Vector3Int(-1,0,0),// min x
        new Vector3Int(1,0,0),// plus x
        new Vector3Int(0,-1,0),// min y
        new Vector3Int(0,1,0),// plus y
        new Vector3Int(0,0,-1),// min z
        new Vector3Int(0,0,1)// plus z
    };

    /// <summary>
    /// Generate a random color
    /// </summary>
    public static Color RandomColor
    {
        get
        {
            float r = Random.Range(0, 255) / 255f;
            float g = Random.Range(0, 255) / 255f;
            float b = Random.Range(0, 255) / 255f;
            return new Color(r, g, b);
        }
    }

    public static bool TryOrientIndex(Vector3Int localIndex, Vector3Int anchor, Quaternion rotation, Vector3Int gridDimensions, out Vector3Int worldIndex)
    {
        Vector3 rotated = rotation * localIndex;
        worldIndex = anchor + rotated.ToVector3IntRound();
        return CheckInBounds(gridDimensions, worldIndex);
    }

    /// <summary>
    /// Check if an index is inside a given bounds.
    /// </summary>
    /// <param name="gridDimensions">Dimensions of the grid</param>
    /// <param name="index">index to check</param>
    /// <returns>true if the point is inside the bounds.</returns>
    public static bool CheckInBounds(Vector3Int gridDimensions, Vector3Int index)
    {
        if (index.x < 0 || index.x >= gridDimensions.x) return false;
        if (index.y < 0 || index.y >= gridDimensions.y) return false;
        if (index.z < 0 || index.z >= gridDimensions.z) return false;

        return true;
    }

    /// <summary>
    /// Check if a point is inside a collider. The collider needs to be watertight!
    /// </summary>
    /// <param name="point">point to check</param>
    /// <param name="collider">collider to check</param>
    /// <returns>true if inside the collider</returns>
    public static bool PointInsideCollider(Vector3 point, Collider collider)
    {
        Physics.queriesHitBackfaces = true;

        int hitCounter = 0;

        //Shoot a ray from the point in a direction and check how many times the ray hits the mesh collider
        while (Physics.Raycast(new Ray(point, Vector3.forward), out RaycastHit hit))
        {
            //Check if the hit collider is the mesh you're currently checking
            if (hit.collider == collider)
                hitCounter++;

            //A ray will stop when it hits something. We need to continue the ray, so we offset the startingpoint by a
            //minimal distanse in the diretion of the ray and continue castin the ray
            point = hit.point + Vector3.forward * 0.00001f;
        }

        //If the mesh is hit an odd number of times, this means the point is inside the collider
        bool isInside = hitCounter % 2 != 0;
        return isInside;
    }

    /// <summary>
    /// Generate a random index within voxelgrid dimensions
    /// </summary>
    /// <returns>A random index</returns>
    public static Vector3Int RandomIndex(Vector3Int gridDimensions)
    {
        int x = Random.Range(0, gridDimensions.x);
        int y = Random.Range(0, gridDimensions.y);
        int z = Random.Range(0, gridDimensions.z);
        return new Vector3Int(x, y, z);
    }

    /// <summary>
    /// Get a random rotation alligned with the x,y or z axis
    /// </summary>
    /// <returns>A random rotation</returns>
    public static Quaternion RandomCarthesianRotation()
    {
        int x = Random.Range(0, 4) * 90;
        int y = Random.Range(0, 4) * 90;
        int z = Random.Range(0, 4) * 90;
        return Quaternion.Euler(x, y, z);
    }

  
}
