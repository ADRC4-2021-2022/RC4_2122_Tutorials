using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class VoxelGrid
{
    #region public fields
    public Vector3Int GridDimensions { get; private set; }
    public float VoxelSize { get; private set; }
    public Vector3 Origin { get; private set; }

    public Vector3 Centre => Origin + (Vector3)GridDimensions * VoxelSize / 2;

    public bool ShowAliveVoxels
    {
        get
        {
            return _showAliveVoxels;
        }
        set
        {
            _showAliveVoxels = value;
            foreach (Voxel voxel in GetVoxels())
            {
                voxel.ShowAliveVoxel = value;
            }
        }
    }

    public bool ShowAvailableVoxels
    {
        get
        {
            return _showAvailableVoxels;
        }
        set
        {
            _showAvailableVoxels = value;
            foreach (Voxel voxel in GetVoxels())
            {
                voxel.ShowAvailableVoxel = value;
            }
        }
    }


    /// <summary>
    /// what percentage of the available grid has been filled up in percentage
    /// </summary>
    public float Efficiency
    {
        get
        {
            //We're storing the voxels as a list so that we don't have to get them twice. counting a list is more efficient than counting an IEnumerable
            List<Voxel> flattenedVoxels = GetVoxels().ToList();
            //if we don't cast this value to a float, it always returns 0 as it is rounding down to integer values
            return (float)flattenedVoxels.Count(v => v.Status == VoxelState.Alive) / flattenedVoxels.Where(v => v.Status != VoxelState.Dead).Count() * 100;
        }
    }

    #region Block fields
    public Dictionary<int, GameObject> GOPatternPrefabs
    {
        get
        {
            if (_goPatternPrefabs == null)
            {
                _goPatternPrefabs = new Dictionary<int, GameObject>();
                _goPatternPrefabs.Add(0, Resources.Load("Prefabs/PrefabPatternA") as GameObject);
                _goPatternPrefabs.Add(1, Resources.Load("Prefabs/PrefabPatternB") as GameObject);
            }
            return _goPatternPrefabs;
        }
    }
    #endregion

    #endregion

    #region private fields
    private Voxel[,,] _voxels;
    private bool _showAliveVoxels = false;
    private bool _showAvailableVoxels = false;

    #region block fields
    private List<Block> _blocks = new List<Block>();
    private List<Block> _currentBlocks => _blocks.Where(b => b.State != BlockState.Placed).ToList();

    private Dictionary<int, GameObject> _goPatternPrefabs;
    private int _currentPattern = 1;



    #endregion
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
    public VoxelGrid(int x, int y, int z, float voxelSize, Vector3 origin) : this(new Vector3Int(x, y, z), voxelSize, origin) { }


    #endregion

    #region private functions
    /// <summary>
    /// Create all the voxels in the grid
    /// </summary>
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

        ShowAvailableVoxels = true;
        ShowAliveVoxels = true;
    }

    #endregion

    #region public function
    /// <summary>
    /// Get the Voxels of the <see cref="VoxelGrid"/>
    /// </summary>
    /// <returns>A flattened collections of all the voxels</returns>
    public IEnumerable<Voxel> GetVoxels()
    {
        for (int x = 0; x < GridDimensions.x; x++)
            for (int y = 0; y < GridDimensions.y; y++)
                for (int z = 0; z < GridDimensions.z; z++)
                {
                    yield return _voxels[x, y, z];
                }
    }


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
    /// 
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
    public void SetGridState(VoxelState state)
    {
        foreach (var voxel in _voxels)
        {
            voxel.Status = state;
        }
    }

    /// <summary>
    /// Set the non dead voxels of the  grid to a certain state
    /// </summary>
    /// <param name="state">the state to set</param>
    public void SetNonDeadGridState(VoxelState state)
    {
        foreach (var voxel in GetVoxels().Where(v => v.Status != VoxelState.Dead))
        {
            voxel.Status = state;
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
                    _voxels[x, y, z].Status = _voxels[x, y - 1, z].Status;
                }
            }
        }
    }

    /// <summary>
    /// Get the number of neighbouring voxels that are alive
    /// </summary>
    /// <param name="voxel">the voxel to get the neighbours from</param>
    /// <returns>number of alive neighbours</returns>
    public int GetNumberOfAliveNeighbours(Voxel voxel)
    {
        int nrOfAliveNeighbours = 0;
        foreach (var vox in voxel.GetFaceNeighbourList())
        {
            if (vox.Status == VoxelState.Alive) nrOfAliveNeighbours++;
        }

        return nrOfAliveNeighbours;
    }

    /// <summary>
    /// Get a random voxel with the Status Available
    /// </summary>
    /// <returns>The random available voxel</returns>
    public Voxel GetRandomAvailableVoxel()
    {
        List<Voxel> voxels = new List<Voxel>(GetVoxels().Where(v => v.Status == VoxelState.Available));
        return voxels[Random.Range(0, voxels.Count)];
    }

    #region Block functionality
    /// <summary>
    /// Set a random pattern index based on all the possible patterns in Pattern list.
    /// </summary>
    public void SetRandomPatternIndex() =>
        _currentPattern = Random.Range(0, PatternManager.Patterns.Count);

    /// <summary>
    /// Set a random pattern index based on all the possible patterns in Pattern list.
    /// </summary>
    public void SetPatternIndex(int index)
    {
        if (index >= 0 && index < PatternManager.Patterns.Count)
            _currentPattern = index;
        else
            Debug.LogWarning($"There's not pattern with Index {index}");
    }



    /// <summary>
    /// Temporary add a block to the grid. To confirm the block at it's current position, use the TryAddCurrentBlocksToGrid function
    /// </summary>
    /// <param name="anchor">The voxel where the pattern will start building from index(0,0,0) in the pattern</param>
    /// <param name="rotation">The rotation for the current block. This will be rounded to the nearest x,y or z axis</param>
    public void AddBlock(Vector3Int anchor, Quaternion rotation) => _blocks.Add(new Block(_currentPattern, anchor, rotation, this));

    /// <summary>
    /// Try to add the blocks that are currently pending to the grids
    /// </summary>
    /// <returns>true if the function managed to place all the current blocks. False in all other cases</returns>
    public bool TryAddCurrentBlocksToGrid()
    {
        //Stop if there are no blocks to add
        if (_currentBlocks == null || _currentBlocks.Count == 0) return false;
        //Stop if there are no valid blocks to add
        if (_currentBlocks.Count(b => b.State == BlockState.Valid) == 0) return false;

        //Keep adding blocks to the grid untill all the pending blocks are added
        while (_currentBlocks.Count > 0)
        {
            _currentBlocks.First().ActivateVoxels();
        }

        return true;
    }

    /// <summary>
    /// Remove all pending blocks from the grid
    /// </summary>
    public void PurgeUnplacedBlocks()
    {
        _blocks.RemoveAll(b => b.State != BlockState.Placed);
    }

    public void PurgeAllBlocks()
    {
        foreach (var block in _blocks)
        {
            block.DestroyBlock();
        }
        _blocks = new List<Block>();
    }

    /// <summary>
    /// Counts the number of blocks placed in the voxelgrid
    /// </summary>
    public int NumberOfBlocks => _blocks.Count(b => b.State == BlockState.Placed);

    #endregion
    #endregion
}
