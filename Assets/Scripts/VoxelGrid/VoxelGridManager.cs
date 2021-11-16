using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoxelGridManager : MonoBehaviour
{
    [Header("Voxel grid dimensions", order = 1)]
    //sliders for grid dimensions
    [SerializeField]
    [Range(0, 50)]
    private int _xDimensions = 5;

    [SerializeField]
    [Range(0, 50)]
    private int _yDimensions = 5;

    [SerializeField]
    [Range(0, 50)]
    private int _zDimensions = 5;

    [Header("Parameters", order = 2)]
    [SerializeField]
    [Range(0, 5)]
    private float _voxelSize = 1f;

    [SerializeField]
    private Vector3 _origin = Vector3.zero;

    //When getting the grid dimensions, it will return a vector3int with the input from the sliders
    private Vector3Int _gridDimensions => new Vector3Int(_xDimensions, _yDimensions, _zDimensions);

    //The line above does the same as the commented section below. => is used here as a shorthand syntax to create a getter.
    /*
    private Vector3Int _gridDimensions
    {
        get { return new Vector3Int(_xDimensions, _yDimensions, _zDimensions); }
    }*/

    //This object is used to handle input using the new Unity input system
    private ProjectControlls _projectControlls;


    private VoxelGrid _grid;

    private void Awake()
    {
        _projectControlls = new ProjectControlls();
    }

    private void OnEnable()
    {
        _projectControlls.Enable();
    }
    private void OnDisable()
    {
        _projectControlls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        _grid = new VoxelGrid(_gridDimensions,_voxelSize,_gridDimensions);

    }


    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
            Debug.Log("Clicked the button with text");
    }

    // Update is called once per frame
    void Update()
    {
        PerformRaycast();
    }

    private void PerformRaycast()
    {
        if (_projectControlls.Player.LeftMouseButtonClick.WasPressedThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(_projectControlls.Player.MousePosition.ReadValue<Vector2>());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Voxel")
                {
                    GameObject hitObject = hit.transform.gameObject;
                    Voxel voxel = hitObject.GetComponent<VoxelTrigger>().AttachedVoxel;

                    voxel.ToggleNeighbours();
                }
            }
        }
    }
}

