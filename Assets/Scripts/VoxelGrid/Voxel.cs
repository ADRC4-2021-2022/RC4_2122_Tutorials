using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel
{

    #region public fields

    //index can not be set outside of the scope of this class
    public Vector3Int Index { get; private set; }
    public bool Alive
    {
        get
        {
            return _alive;
        }
        set
        {
            if(_goVoxelTrigger!=null)
            {
                
                MeshRenderer renderer = _goVoxelTrigger.GetComponent<MeshRenderer>();
                renderer.enabled = value;
            }
            _alive = value;
        }
    }

    /// <summary>
    /// Get the centre point of the voxel in worldspace
    /// </summary>
    public Vector3 Centre => _gridOrigin + (Vector3)Index * _voxelSixe + Vector3.one * 0.5f * _voxelSixe;
    #endregion

    #region private fields
    private GameObject _goVoxelTrigger;
    private VoxelGrid _grid;
    private bool _alive;
    private float _scalefactor = 0.95f;
    private float _voxelSixe => _grid.VoxelSize;
    private Vector3 _gridOrigin => _grid.Origin;

    

    #endregion

    #region constructors
    public Voxel(int x, int y, int z, VoxelGrid grid)
    {
        Index = new Vector3Int(x, y, z);
        _grid = grid;
        
        CreateGameobject();
        Alive = true;
    }
    #endregion

    #region public functions

    public Voxel(Vector3Int index, VoxelGrid grid)
    {
        Index = index;
        _grid = grid;
        CreateGameobject();
        Alive = true;
    }

    public void CreateGameobject()
    {
        _goVoxelTrigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _goVoxelTrigger.name = $"Voxel {Index}";
        _goVoxelTrigger.tag = "Voxel";
        _goVoxelTrigger.transform.position = Centre;
        _goVoxelTrigger.transform.localScale = Vector3.one * _voxelSixe* _scalefactor;
        
        VoxelTrigger trigger = _goVoxelTrigger.AddComponent<VoxelTrigger>();
        trigger.AttachedVoxel = this;
    }

    public List<Voxel> GetNeighbourList()
    {
        List<Voxel> neighbours = new List<Voxel>();
        foreach (Vector3Int direction in Util.Directions)
        {
            Vector3Int neighbourIndex = Index + direction;
            if (Util.CheckInBounds(_grid.GridDimensions, neighbourIndex))
            {
                neighbours.Add(_grid.GetVoxelByIndex(neighbourIndex));
            }
        }

        return neighbours;
    }

    public void ToggleNeighbours()
    {
        List<Voxel> neighbours = GetNeighbourList();

        foreach (var neighbour in neighbours)
        {
            neighbour.Alive = !neighbour.Alive;
        }
    }

    

    #endregion
}
