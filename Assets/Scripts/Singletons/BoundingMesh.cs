using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;


/// <summary>
/// Singelton class for gathering all bounding mesh colliders and voxelising them
/// </summary>
public class BoundingMesh
{
    #region Private fields
    private static IEnumerable<Collider> _colliders;
    #endregion

    #region Public fields
    /// <summary>
    /// The singleton instance of the bounding mesh generator
    /// </summary>
    public static BoundingMesh Instance { get; } = new BoundingMesh();
    public static Bounds MeshBounds;

    #endregion

    #region Constructors
    /// <summary>
    /// Create a bounding mesh object that gathers all the GameObjects with a "BoundingMesh" tag. This will be executed the first time the BoundingMesh Instance object gets called.
    /// </summary>
    public BoundingMesh()
    {
        GameObject[] boundingMeshes = GameObject.FindGameObjectsWithTag("BoundingMesh");
        _colliders = boundingMeshes.Select(g => g.GetComponent<Collider>());

        Bounds meshBounds = new Bounds();

        foreach (var collider in _colliders)
        {
            meshBounds.Encapsulate(collider.bounds);
        }
        MeshBounds = meshBounds;
    }
    #endregion

    #region Private functions

    #endregion

    #region Public functions
    public  static VoxelGrid GetVoxelGrid(int voxelOffset, float voxelSize)
    {
        VoxelGrid voxelGrid = new VoxelGrid(GetGridDimensions(voxelOffset, voxelSize), voxelSize, GetOrigin(voxelOffset, voxelSize));
        return voxelGrid;
    }
       


    /// <summary>
    /// Get the origin of the grid
    /// </summary>
    /// <param name="voxelOffset">amount of voxels to be added around the bounding meshes</param>
    /// <param name="voxelSize">The size of the voxels</param>
    /// <returns></returns>
    public static Vector3 GetOrigin(int voxelOffset, float voxelSize) =>
        MeshBounds.center - (Vector3)GetGridDimensions(voxelOffset, voxelSize) * voxelSize / 2;

    /// <summary>
    /// Calculate the dimensions of the grid based on the collider, the voxelsize and a certain voxel offset
    /// </summary>
    /// <param name="voxelOffset">number of layers of voxel existing around the bounding box of the collider</param>
    /// <param name="voxelSize">size of the voxels</param>
    /// <returns>The dimensions for a voxelgrid encapsulating the collider</returns>
    public static Vector3Int GetGridDimensions(int voxelOffset, float voxelSize) =>
        (MeshBounds.size / voxelSize).ToVector3IntRound() + Vector3Int.one * voxelOffset * 2;

    /// <summary>
    /// Check if a voxel is inside the bounding meshes, using the Voxel centre
    /// </summary>
    /// <param name="voxel">voxel to check</param>
    /// <returns>true if the voxelcentre is inside the collider</returns>
    public static bool IsInsideCentre(Voxel voxel)
    {
        Physics.queriesHitBackfaces = true;

        var point = voxel.Centre;

        //Add a collection to store the number of hits per collider
        var sortedHits = new Dictionary<Collider, int>();
        foreach (var collider in _colliders)
            sortedHits.Add(collider, 0);

        //Shoot a ray from the point in a direction and check how many times the ray hits any mesh collider
        while (Physics.Raycast(new Ray(point, Vector3.forward), out RaycastHit hit))
        {
            var collider = hit.collider;

            //Check if the hit collider is one of the bounding mesh colliders
            //add to the colliders count
            if (sortedHits.ContainsKey(collider))
                sortedHits[collider]++;

            //A ray will stop when it hits something. We need to continue the ray, so we offset the startingpoint by a
            //minimal distanse in the diretion of the ray and continue castin the ray
            point = hit.point + Vector3.forward * 0.00001f;
        }

        //If any of the bounding mesh colliders is hit an odd number of times, this means the point is inside the bounding colliders
        bool isInside = sortedHits.Any(kv => kv.Value % 2 != 0);
        return isInside;
    }

    public void ChangeColliderMaterials(Material material)
    {
        foreach (var collider in _colliders)
        {
            MeshRenderer renderer = collider.gameObject.GetComponent<MeshRenderer>();
            renderer.material = material;
        }
    }


    #endregion

}
