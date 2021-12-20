using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//UnityEngine.UI needed to interact with sliders
using UnityEngine.UI;
//TMPro for interaction with TextMeshPro text
using TMPro;

public class VoxeliseManager : MonoBehaviour
{
    #region Serialised Fields
    [SerializeField]
    [Range(0.1f, 1f)]
    private float _voxelSize = 0.5f;

    [SerializeField]
    [Range(0, 5)]
    private int _voxelOffset = 1;

    [SerializeField]
    //Make sure your actually add the  GameObject that has the MeshRenderer and Mesh Filter component, not the parent
    private GameObject _goMesh;

    #endregion

    #region private fields
    private MeshFunctions _meshFunctions;

    private VoxelGrid _grid;
    private MainCamera _camera;

    private Material _matRock;
    private Material _matTransparent;

    private List<Material> _rockMaterials;
    private int _currentMaterial;

    #endregion

    #region public fields
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
    #endregion

    #region Monobehaviour functions


    void Start()
    {
        _rockMaterials = new List<Material>()
        {
            MATRock,
            MATTransparent
        };

        //Add the a meshfunction component to get the required properties for Voxelising the mesh
        _meshFunctions = _goMesh.AddComponent<MeshFunctions>();
        _goMesh.GetComponent<MeshRenderer>().material = MATRock;

        //Update fields to match slider values and add slider text
        UpdateVoxelsize();
        UpdateVoxelOffset();
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Set the status of all voxels alive inside or outside of the mesh
    /// </summary>
    /// <param name="checkInside">if true, the voxels inside the mesh will be enabled.
    /// If false, the voxels outside of the mesh will be enabled.</param>
    private void ToggleVoxelsMesh(bool checkInside)
    {
        foreach (Voxel voxel in _grid.GetVoxels())
        {
            bool isInside = _meshFunctions.IsInsideCentre(voxel);
            //shorthand if statement
            voxel.Alive = checkInside ? isInside : !isInside;

            //This is exactly the same code as the line on top
            if (checkInside) voxel.Alive = isInside;
            else voxel.Alive = !isInside;
        }

    }
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
            sliderValue = (int) slider.GetComponent<Slider>().value;
            _voxelOffset = sliderValue;

            //Set slider text
            GameObject sliderText = GameObject.Find("txtVoxelOffset");
            if (sliderText != null)
                sliderText.GetComponent<TMP_Text>().text = $"Voxelsize: {sliderValue.ToString()}";
        }
    }

    /// <summary>
    /// Create a voxelgrid based on the dimensions of the given mesh
    /// </summary>
    public void CreateVoxelGrid()
    {
        Vector3Int gridDimensions = _meshFunctions.GetGridDimensions(_voxelOffset, _voxelSize);
        Vector3 origin = _meshFunctions.GetOrigin(_voxelOffset, _voxelSize);

        _grid = new VoxelGrid(gridDimensions, _voxelSize, origin);


    }


    /// <summary>
    /// Set the voxels outside of the mesh to not alive
    /// </summary>
    public void VoxeliseMesh()
    {
        ToggleVoxelsMesh(true);
    }

    /// <summary>
    /// Toggle the materials of the voxelised mesh
    /// </summary>
    public void ChangeMeshMaterial()
    {
        _currentMaterial++;
        _currentMaterial %= _rockMaterials.Count;
        MeshRenderer renderer = _goMesh.GetComponent<MeshRenderer>();
        renderer.material = _rockMaterials[_currentMaterial];
    }

    #endregion
}
