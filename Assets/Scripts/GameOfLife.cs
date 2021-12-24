using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Add linq to you class to use more enhance list comprehension (eg. .Where, .Count, .Select, .First, .SelectMany, ...)
//https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/
using System.Linq;
public class GameOfLife
{
    #region private fields
    private VoxelGrid _grid;

    private List<Vector3Int> _gameOfLifeNeighbours = new List<Vector3Int>
     {
        new Vector3Int(-1,0,0),// min x
        new Vector3Int(-1,0,1),// min x, plus z
        new Vector3Int(0,0,1),// plus z
        new Vector3Int(1,0,1),// plus x, plus z
        new Vector3Int(1,0,0),// plus x
        new Vector3Int(1,0,-1),// plus x, min z
        new Vector3Int(0,0,-1),// min z
        new Vector3Int(-1,0,-1),// min x, min z
        
    };
    #endregion

    #region constructor
    public GameOfLife(VoxelGrid grid)
    {
        _grid = grid;
    }
    #endregion

    #region public functions
    /// <summary>
    /// Assign a random status to the voxels of a specific XZ layer in the grid
    /// </summary>
    /// <param name="yLayer">Y index of the layer</param>
    /// <param name="chance">ratio of voxels being activated. 0 is 0%, 1 is 100%. Value will be clamped between 0 and 1</param>
    public void SetRandomAlive(int yLayer, float chance)
    {
        //Check if the yLayer is within the bounds of the grid
        if (yLayer < 0 || yLayer >= _grid.GridDimensions.y)
        {
            Debug.Log($"Requested Y Layer {yLayer} is out of bounds");
            return;
        }

        //Make sure the value is between 0 and 1
        chance = Mathf.Clamp(chance, 0, 1);

        foreach (var voxel in _grid.GetVoxelsYLayer(yLayer))
        {
            voxel.Status = Random.value < chance ? VoxelState.Alive : VoxelState.Available;
        }
    }


    /// <summary>
    /// perform Game Of Life on a specific XZ layer in the grid
    /// </summary>
    /// <param name="yLayer">Y index of the layer</param>
    public void GameOfLifeStep(int yLayer)
    {
        //Check if the yLayer is within the bounds of the grid
        if (yLayer < 0 || yLayer >= _grid.GridDimensions.y)
        {
            Debug.Log($"Requested Y Layer {yLayer} is out of bounds");
            return;
        }
        //Create a copy of the grid X Y layer statusses
        bool[,] layer = new bool[_grid.GridDimensions.x, _grid.GridDimensions.z];

        //Store all the new statuses in a temporary collection
        for (int x = 0; x < _grid.GridDimensions.x; x++)
        {
            for (int z = 0; z < _grid.GridDimensions.z; z++)
            {
                Voxel currentVoxel = _grid.GetVoxelByIndex(x, yLayer, z);
                layer[x, z] = GameOfLifeRules(currentVoxel);
            }
        }

        //Propogate grid
        for (int x = 0; x < _grid.GridDimensions.x; x++)
        {
            for (int z = 0; z < _grid.GridDimensions.z; z++)
            {
                _grid.GetVoxelByIndex(x, yLayer, z).Status = layer[x, z] ? VoxelState.Alive : VoxelState.Dead;
            }
        }

    }
    #endregion

    #region private functions
    private bool GameOfLifeRules(Voxel voxel)
    {

        List<Voxel> neighbours = voxel.GetRelatedVoxels(_gameOfLifeNeighbours);

        //loop over all the neighbours to check if they are allive
        int nrOfAliveNeighbours = neighbours.Count(v => v.Status == VoxelState.Alive);

        if (nrOfAliveNeighbours < 2) return false;
        else if (nrOfAliveNeighbours > 3) return false;
        else if (nrOfAliveNeighbours == 3) return true;

        return true;
    }


    #endregion

}
