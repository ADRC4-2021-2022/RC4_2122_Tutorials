using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid
{
    #region public fields
    public Vector3Int GridDimensions { get; private set; }
    public float VoxelSize { get; private set; }
    public Vector3 Origin { get; private set; }

    public Vector3 Centre => Origin + (Vector3)GridDimensions * VoxelSize / 2;

    #endregion

    #region private fields
    private Voxel[,,] _voxels;
    #endregion

    #region Constructors
    /// <summary>
    /// Create a new voxelgrid
    /// </summary>
    /// <param name="gridDimensions">X,Y,Z dimensions of the grid</param>
    /// <param name="voxelSize">The size of the voxels</param>
    /// <param name="origin">Where the voxelgrid starts</param>
    public VoxelGrid(Vector3Int gridDimensions, float voxelSize, Vector3 origin)
    {
        GridDimensions = gridDimensions;
        VoxelSize = voxelSize;
        Origin = origin;

        MakeVoxels();
    }

    //Copy constructor with different signature. will refer to the original constructor
    /// <summary>
    /// Create a new voxelgrid
    /// </summary>
    /// <param name="x">X dimensions of the grid</param>
    /// <param name="y">Y dimensions of the grid</param>
    /// <param name="z">Z dimensions of the grid</param>
    /// <param name="voxelSize">The size of the voxels</param>
    /// <param name="origin">Where the voxelgrid starts</param>
    public VoxelGrid(int x, int y, int z, float voxelSize, Vector3 origin) : this(new Vector3Int(x, y, z), voxelSize, origin)
    {
    }


    #endregion

    #region private functions
    private void MakeVoxels()
    {
        _voxels = new Voxel[GridDimensions.x, GridDimensions.y, GridDimensions.z];
        for (int x = 0; x < GridDimensions.x; x++)
        {
            for (int y = 0; y < GridDimensions.y; y++)
            {
                for (int z = 0; z < GridDimensions.z; z++)
                {
                    _voxels[x, y, z] = new Voxel(x, y, z, this);
                }
            }
        }
    }

    #endregion

    #region public function
    //Shorthand syntax for a function returning the output of GetVoxelByIndex
    //Two function with the same name, but different parameters ==> different signature
    /// <summary>
    /// Return a voxel at a certain index
    /// </summary>
    /// <param name="x">x index</param>
    /// <param name="y">y index</param>
    /// <param name="z">z index</param>
    /// <returns>Voxel at x,y,z index. null if the voxel doesn't exist or is out of bounds</returns>
    public Voxel GetVoxelByIndex(int x, int y, int z) => GetVoxelByIndex(new Vector3Int(x, y, z));

    /// <summary>
    /// Return a voxel at a certain index
    /// </summary>
    /// <param name="index">x,y,z index</param>
    /// <returns>Voxel at x,y,z index. null if the voxel doesn't exist or is out of bounds</returns>
    public Voxel GetVoxelByIndex(Vector3Int index)
    {
        if (!Util.CheckInBounds(GridDimensions, index) || _voxels[index.x, index.y, index.z] == null)
        {
            Debug.Log($"A Voxel at {index} doesn't exist");
            return null;
        }
        return _voxels[index.x, index.y, index.z];
    }

    /// <summary>
    /// Get all the voxels at a certain XZ layer
    /// </summary>
    /// <param name="yLayer">Y index of the layer</param>
    /// <returns>List of voxels in XZ layer</returns>
    public List<Voxel> GetVoxelsYLayer(int yLayer)
    {
        List<Voxel> yLayerVoxels = new List<Voxel>();

        //Check if the yLayer is within the bounds of the grid
        if (yLayer < 0 || yLayer >= GridDimensions.y)
        {
            Debug.Log($"Requested Y Layer {yLayer} is out of bounds");
            return null;
        }

        for (int x = 0; x < GridDimensions.x; x++)
            for (int z = 0; z < GridDimensions.z; z++)
                yLayerVoxels.Add(_voxels[x, yLayer, z]);

        return yLayerVoxels;
    }

    /// <summary>
    /// Set the entire grid 'Alive' to a certain state
    /// </summary>
    /// <param name="state">the state to set</param>
    public void SetGridState(bool state)
    {
        foreach (var voxel in _voxels)
        {
            voxel.Alive = state;
        }
    }
    /// <summary>
    /// Copy all the layers one layer up, starting from the top layer going down.
    /// The bottom layer will remain and the top layer will dissapear.
    /// </summary>
    public void CopyLayersUp()
    {
        //Check the signature of the for loop. Starting at the top layer and going down
        for (int y = GridDimensions.y - 1; y > 0; y--)
        {
            for (int x = 0; x < GridDimensions.x; x++)
            {
                for (int z = 0; z < GridDimensions.z; z++)
                {
                    _voxels[x, y, z].Alive = _voxels[x, y - 1, z].Alive;
                }
            }
        }
    }

    public int GetNumberOfAlliveNeighbours(Voxel voxel)
    {
        int nrOfAliveNeighbours = 0;
        foreach (var vox in voxel.GetFaceNeighbourList())
        {
            if (vox.Alive) nrOfAliveNeighbours++;
        }

        return nrOfAliveNeighbours;
    }
    #endregion
}
