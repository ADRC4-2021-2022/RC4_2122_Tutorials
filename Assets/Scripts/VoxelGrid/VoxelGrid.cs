using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid
{
    #region public fields
    public Vector3Int GridDimensions { get; private set; }


    #endregion

    #region private fields
    private Voxel[,,] _voxels;
    #endregion

    #region Constructors
    public VoxelGrid(Vector3Int gridDimensions)
    {
        GridDimensions = gridDimensions;

        MakeVoxels();
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
    public Voxel GetVoxelByIndex(Vector3Int index)
    {
        if(!Util.CheckInBounds(GridDimensions,index)||_voxels[index.x, index.y, index.z]==null)
        {
            Debug.Log($"A Voxel at {index} doesn't exist");
            return null;
        }
        return _voxels[index.x, index.y, index.z];
    }
    #endregion
}
