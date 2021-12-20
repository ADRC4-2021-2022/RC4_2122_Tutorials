
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MeshFunctions : MonoBehaviour
{
    #region Private fields
    private  Collider _collider;
    #endregion

    #region Public fields
    public Bounds MeshBounds;
    #endregion

    #region Unity functions
    public void Awake()
    {
        //Get the collider and find the bounding box of this collider
        _collider = this.GetComponent<Collider>();
        MeshBounds = _collider.bounds;

        //Draw a cube on the bouding mesh for debugging purposes
        /*
        var center = MeshBounds.center;
        var scale = MeshBounds.extents * 2;
                
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = scale;
        cube.transform.localPosition = center;
        */
    }

    #endregion

    #region public functions
    

    /// <summary>
    /// Get the origin of the grid
    /// </summary>
    /// <param name="voxelOffset">amount of voxels to be added around the bounding meshes</param>
    /// <param name="voxelSize">The size of the voxels</param>
    /// <returns></returns>
    public Vector3 GetOrigin(int voxelOffset, float voxelSize) =>
        MeshBounds.center - (Vector3)GetGridDimensions(voxelOffset,voxelSize) * voxelSize/2;

    /// <summary>
    /// Calculate the dimensions of the grid based on the collider, the voxelsize and a certain voxel offset
    /// </summary>
    /// <param name="voxelOffset">number of layers of voxel existing around the bounding box of the collider</param>
    /// <param name="voxelSize">size of the voxels</param>
    /// <returns>The dimensions for a voxelgrid encapsulating the collider</returns>
    public Vector3Int GetGridDimensions(int voxelOffset, float voxelSize) =>
        (MeshBounds.size / voxelSize).ToVector3IntRound() + Vector3Int.one * voxelOffset * 2;


    /// <summary>
    /// Check if a voxel is inside the mesh, using the Voxel centre
    /// </summary>
    /// <param name="voxel">voxel to check</param>
    /// <returns>true if inside the mesh</returns>
    public bool IsInsideCentre(Voxel voxel)
    {
        var point = voxel.Centre;
        return Util.PointInsideCollider(point, _collider);
    }
    #endregion
}


