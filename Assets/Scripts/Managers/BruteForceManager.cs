using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BruteForceManager : MonoBehaviour
{
    #region Serialised fields

    #endregion

    #region Private fields
    private float _voxelSize = 0.2f;
    private int _voxelOffset = 2;

    #region Brute force filler fields
    private int _triesPerIteration = 10000;
    private int _iterations = 1000;

    private int _tryCounter = 0;
    private int _iterationCounter = 0;

    private bool _generating = false;
    private int _seed = 0;

    ///Collection to store the seeds and their efficiencies
    private Dictionary<int, float> _efficencyPerSeed = new Dictionary<int, float>();

    //Ordered list of most efficient seeds
    private List<int> _orderedEfficiencyIndex = new List<int>();

    private List<Voxel> _nonDeadVoxels;

    #endregion

    private VoxelGrid _grid;

    private MainCamera _camera;

    //MaterialManager
    private Material _matRock;
    private Material _matTransparent;

    private List<Material> _boundingMateials;
    private int _currentMaterial;

    private TMP_Text _tmpStatText;
    private TMP_Text _tmpEfficiencyText;
    #endregion

    #region Public fields
    public Material MATRock
    {
        get
        {
            if (_matRock == null)
            {
                _matRock = Resources.Load("Materials/matRock", typeof(Material)) as Material;
            }
            return _matRock;
        }
    }
    public Material MATTransparent
    {
        get
        {
            if (_matTransparent == null)
            {
                _matTransparent = Resources.Load("Materials/matTransparent", typeof(Material)) as Material;
            }
            return _matTransparent;
        }
    }

    public MainCamera Camera
    {
        get
        {
            if (_camera == null)
            {
                _camera = GameObject.Find("Main Camera").GetComponent<MainCamera>();
            }
            return _camera;
        }

    }

    public int Seed
    {
        get
        {
            return _seed;
        }
        set
        {
            _seed = value;
            GameObject goSeedInput = GameObject.Find("/Canvas/pnlRight/inpSeed");
            if (goSeedInput != null)
            {
                goSeedInput.GetComponent<TMP_InputField>().text = _seed.ToString();
            }
            else
                Debug.LogWarning("inpSeed not found");

        }
    }

    public TMP_Text TMPStatText
    {
        get
        {
            if (_tmpStatText == null)
            {
                GameObject goText = GameObject.Find("/Canvas/pnlRight/txtGridStats");
                if (goText != null)
                    _tmpStatText = goText.GetComponent<TMP_Text>();
            }
            return _tmpStatText;
        }
    }

    public TMP_Text TMPEfficiencyText
    {
        get
        {
            if (_tmpEfficiencyText == null)
            {
                GameObject goText = GameObject.Find("/Canvas/pnlRight/txtBestSeeds");
                if (goText != null)
                    _tmpEfficiencyText = goText.GetComponent<TMP_Text>();
            }
            return _tmpEfficiencyText;
        }
    }
    #endregion

    #region MonoBehaviour functions
    void Start()
    {
        //Set the material collection
        _boundingMateials = new List<Material>()
        {
            MATRock,
            MATTransparent
        };

        //Update fields to match slider values and add slider text
        UpdateVoxelsize();
        UpdateVoxelOffset();
    }
    #endregion

    #region Private functions
    /// <summary>
    /// Set the status of all voxels dead inside or outside of the mesh
    /// </summary>
    /// <param name="checkInside">if true, the voxels outside the mesh will be dead.
    /// If false, the voxels inside of the mesh will be dead.</param>
    private void KillVoxelsInOutBounds(bool checkInside)
    {
        foreach (Voxel voxel in _grid.GetVoxels())
        {
            bool isInside = BoundingMesh.IsInsideCentre(voxel);
            if (checkInside && !isInside)
                voxel.Status = VoxelState.Dead;
            if (!checkInside && isInside)
                voxel.Status = VoxelState.Dead;
        }

        Debug.Log($"Number of available voxels: {_grid.GetVoxels().Count(v => v.Status == VoxelState.Available)} of {_grid.GetVoxels().Count()} voxels");
        _nonDeadVoxels = _grid.GetVoxels().Where(v => v.Status != VoxelState.Dead).ToList();
    }

    /// <summary>
    /// Method to test adding one block to the brid
    /// </summary>
    private void BlockTest()
    {
        //Set grid status to available
        _grid.SetNonDeadGridState(VoxelState.Available);

        var anchor = new Vector3Int(2, 8, 0);
        var rotation = Quaternion.Euler(0, 0, -90);

        _grid.SetPatternIndex(0);
        _grid.AddBlock(anchor, rotation);
        _grid.TryAddCurrentBlocksToGrid();
    }

    #region Brute force engine
    /// <summary>
    /// Try adding a random block to the grid every given time. This will run as much times as defined in the _tries field
    /// </summary>
    /// <returns>Wait 0.01 seconds between each iteration</returns>
    IEnumerator BruteForceSeed()
    {
        _grid.PurgeAllBlocks();
        Random.InitState(Seed);
        _tryCounter = 0;
        while (_tryCounter < _triesPerIteration)
        {
            TryAddRandomBlock();
            UpdateStats();
            _tryCounter++;
            yield return new WaitForSeconds(0.001f);
        }
        UpdateEfficiency();
        yield break;
    }

    /// <summary>
    /// Brute force an entire iteration every tick
    /// </summary>
    /// <returns></returns>
    private IEnumerator BruteForceEngine()
    {
        while (_iterationCounter < _iterations)
        {
            Random.InitState(Seed++);
            BruteForceStep();
            _iterationCounter++;
            yield return new WaitForSeconds(0.01f);
        }
    }

    /// <summary>
    /// Brute force random blocks in the available grid for a given number of tries
    /// </summary>
    private void BruteForceStep()
    {
        _grid.PurgeAllBlocks();
        _tryCounter = 0;
        while (_tryCounter < _triesPerIteration)
        {
            TryAddRandomBlock();
            _tryCounter++;
        }

        UpdateEfficiency();
    }

    /// <summary>
    /// Try to add a random block to the grid
    /// </summary>
    /// <returns>true is a block is succelfully added, false if not</returns>
    private bool TryAddRandomBlock()
    {
        _grid.SetRandomPatternIndex();

        //Add function the gets the voxels around the generated block
        //==> Get the available neighbours of all the alive voxels


        //Random available voxel: Most accurate, but very expensive ==> slow EG Seed 0: 64% filled
        //_grid.AddBlock(_grid.GetRandomAvailableVoxel().Index, Util.RandomCarthesianRotation());
        //Random voxel in the entire grid: Least accurate but non expensive ==> very fast EG Seed 0: 35%
        //_grid.AddBlock(Util.RandomIndex(_grid.GridDimensions), Util.RandomCarthesianRotation());
        //Random voxel in the non dead voxels: Medium accurate, low expensive ==> medium fast EG Seed 0: 59% filled
        _grid.AddBlock(_nonDeadVoxels[Random.Range(0, _nonDeadVoxels.Count)].Index, Util.RandomCarthesianRotation());

        bool blockAdded = _grid.TryAddCurrentBlocksToGrid();
        _grid.PurgeUnplacedBlocks();



        return blockAdded;
    }
    #endregion
    #endregion

    #region Public functions

    #endregion

    #region Canvas functions
    /// <summary>
    /// Change the slider text and add the value to the Field
    /// </summary>
    public void UpdateVoxelsize()
    {
        //Get slider value
        float sliderValue;
        GameObject slider = GameObject.Find("sldVoxelSize");
        if (slider != null)
        {
            sliderValue = slider.GetComponent<Slider>().value;
            _voxelSize = sliderValue;

            //Set slider text
            GameObject sliderText = GameObject.Find("txtVoxelSize");
            if (sliderText != null)
                sliderText.GetComponent<TMP_Text>().text = $"Voxelsize: {sliderValue.ToString()}";
        }
    }

    /// <summary>
    /// Change the slider text and add the value to the Field
    /// </summary>
    public void UpdateVoxelOffset()
    {
        //Get slider value
        int sliderValue;
        GameObject slider = GameObject.Find("sldVoxelOffset");
        if (slider != null)
        {
            sliderValue = (int)slider.GetComponent<Slider>().value;
            _voxelOffset = sliderValue;

            //Set slider text
            GameObject sliderText = GameObject.Find("txtVoxelOffset");
            if (sliderText != null)
                sliderText.GetComponent<TMP_Text>().text = $"Voxel Offset: {sliderValue.ToString()}";
        }
    }

    /// <summary>
    /// Create a voxelgrid based on the dimensions of the given mesh
    /// </summary>
    public void CreateVoxelGrid()
    {
        _grid = BoundingMesh.GetVoxelGrid(_voxelOffset, _voxelSize);
    }


    /// <summary>
    /// Set the voxels outside of the mesh to not alive
    /// </summary>
    public void VoxeliseMesh()
    {
        KillVoxelsInOutBounds(true);
    }

    /// <summary>
    /// Toggle the materials of the voxelised mesh
    /// </summary>
    public void ChangeMeshMaterial()
    {
        _currentMaterial++;
        _currentMaterial %= _boundingMateials.Count;
        BoundingMesh.Instance.ChangeColliderMaterials(_boundingMateials[_currentMaterial]);
    }

    /// <summary>
    /// Show all the alive voxels
    /// </summary>
    public void ShowAliveVoxels()
    {
        _grid.ShowAliveVoxels = !_grid.ShowAliveVoxels;
    }

    /// <summary>
    /// Show all the available voxels
    /// </summary>
    public void ShowAvailableVoxels()
    {
        _grid.ShowAvailableVoxels = !_grid.ShowAvailableVoxels;
    }

    /// <summary>
    /// Change the seed value based on the inpSeed text
    /// </summary>
    public void UpdateSeed()
    {
        GameObject goSeedInput = GameObject.Find("/Canvas/pnlRight/inpSeed");
        if (goSeedInput != null)
        {
            if (!int.TryParse(goSeedInput.GetComponent<TMP_InputField>().text, out _seed))
                Debug.LogWarning("The input value is not an whole number");
        }
        else
            Debug.LogWarning("inpSeed not found");
    }

    /// <summary>
    /// Trigger the block test for testing purposes
    /// </summary>
    public void AddOneBlockToGrid()
    {
        TryAddRandomBlock();
    }

    public void StartBruteForceSeed()
    {
        StopAllCoroutines();

        StartCoroutine(BruteForceSeed());

    }

    /// <summary>
    /// Start/ Stop the Brute force filler
    /// </summary>
    public void StartStopBruteForceFiller()
    {
        StopAllCoroutines();
        if (!_generating)
        {
            StartCoroutine(BruteForceEngine());
        }
        _generating = !_generating;
    }

    /// <summary>
    /// Update the statistics of the grid in the UI
    /// </summary>
    public void UpdateStats()
    {
        string text =
    $"Try {_tryCounter} of {_triesPerIteration}" + System.Environment.NewLine +
    $"Iteraterion {_iterationCounter} of {_iterations}" + System.Environment.NewLine +
    $"Grid {(int)_grid.Efficiency} % filled" + System.Environment.NewLine +
    $"Grid {_grid.NumberOfBlocks} blocks added";

        TMPStatText.text = text;
    }

    public void UpdateEfficiency()
    {
        //Keep track of the most efficient seeds. Only store the top 10 most efficient seeds
        _efficencyPerSeed.Add(Seed, _grid.Efficiency);
        _orderedEfficiencyIndex = _efficencyPerSeed.Keys.OrderByDescending(k => _efficencyPerSeed[k]).Take(11).ToList();
        if (_orderedEfficiencyIndex.Count == 11)
        {
            _efficencyPerSeed.Remove(_orderedEfficiencyIndex[10]);
            _orderedEfficiencyIndex.RemoveAt(_orderedEfficiencyIndex.Count - 1);
        }

        UpdateStats();

        string text = string.Empty;
        foreach (var seed in _orderedEfficiencyIndex)
        {
            text += $"Seed {seed}: Efficiency: {(int)_efficencyPerSeed[seed]}%" + System.Environment.NewLine + System.Environment.NewLine;
        }
        TMPEfficiencyText.text = text;

    }
    #endregion
}
