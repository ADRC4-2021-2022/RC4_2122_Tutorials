using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VoxelGridManager : MonoBehaviour
{
    #region serialized fields
    [Header("Voxel grid dimensions", order = 1)]
    //sliders for grid dimensions
    [SerializeField]
    [Range(1, 50)]
    private int _xDimensions = 5;

    [SerializeField]
    [Range(1, 50)]
    private int _yDimensions = 5;

    [SerializeField]
    [Range(1, 50)]
    private int _zDimensions = 5;

    [Header("Parameters", order = 2)]
    [SerializeField]
    [Range(0, 5)]
    private float _voxelSize = 1f;

    [SerializeField]
    private Vector3 _origin = Vector3.zero;

    [Header("Game of life", order = 3)]
    [SerializeField]
    [Range(0, 1)]
    private float _gameOfLifeSpeed = 0.1f;

    #endregion

    #region public fields
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

    public GameObject GOBtnStartStopGameOfLife
    {
        get
        {
            if (_goBtnStartStopGameOfLife == null) _goBtnStartStopGameOfLife = GameObject.Find("btnStartStopGameOfLife");
            return _goBtnStartStopGameOfLife;
        }
    }
    public GameObject GOSldGameOfLifeSpeed
    {
        get
        {
            if (_goSldGameOfLifeSpeed == null) _goSldGameOfLifeSpeed = GameObject.Find("sldGameOfLifeSpeed");
            return _goSldGameOfLifeSpeed;

        }
    }

    public Vector3 GridCentre => _grid.Centre;
    #endregion

    #region private fields
    private MainCamera _camera;

    //When getting the grid dimensions, it will return a vector3int with the input from the sliders
    private Vector3Int _gridDimensions => new Vector3Int(_xDimensions, _yDimensions, _zDimensions);

    //The line above does the same as the commented section below. => is used here as a shorthand syntax to create a getter.
    /*
    private Vector3Int _gridDimensions
    {
        get { return new Vector3Int(_xDimensions, _yDimensions, _zDimensions); }
    }*/

    private GameOfLife _gameOfLife;
    private IEnumerator _runGameOfLife;
    private bool _gameOfLifeRunning = false;

    //This object is used to handle input using the new Unity input system
    private ProjectControls _projectControlls;


    private VoxelGrid _grid;

    private GameObject _goBtnStartStopGameOfLife;
    private GameObject _goSldGameOfLifeSpeed;

    #endregion
    private void Awake()
    {
        _projectControlls = new ProjectControls();
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
        _grid = new VoxelGrid(_gridDimensions, _voxelSize, _gridDimensions);
        _grid.SetGridState(VoxelState.Available);

        _gameOfLife = new GameOfLife(_grid);

        _runGameOfLife = RunGameOfLife();

        //Set the camera target to the centre of the grid to keep rotating around the voxelgrid
        Camera.Target = _grid.Centre;

    }


    private void OnGUI()
    {
        //Code generated button, use for debugging
        /*
        if (GUI.Button(new Rect(10, 70, 100, 30), "Game of Life Step"))
        {
            Debug.Log("Clicked the button with text");

        }
        */

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
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(_projectControlls.Player.MousePosition.ReadValue<Vector2>());
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

    private IEnumerator RunGameOfLife()
    {
        while (true)
        {
            _gameOfLife.GameOfLifeStep(0);
            _grid.CopyLayersUp();

            yield return new WaitForSeconds(_gameOfLifeSpeed);
        }
    }

    #region unity canvas functions
    public void ResetGrid()
    {
        _grid.SetGridState(VoxelState.Available);
    }

    public void RandomGroundLayer()
    {
        _gameOfLife.SetRandomAlive(0, 0.05f);
    }

    public void GameOfLifeStep()
    {
        _gameOfLife.GameOfLifeStep(0);
        _grid.CopyLayersUp();
    }

    public void StartStopGameOfLife()
    {
        _gameOfLifeSpeed = GOSldGameOfLifeSpeed.GetComponent<Slider>().value;

        if (_gameOfLifeRunning)
        {
            _gameOfLifeRunning = false;
            StopCoroutine(_runGameOfLife);
        }
        else
        {
            _gameOfLifeRunning = true;
            StartCoroutine(_runGameOfLife);
        }

        //update button name
        string buttonText = _gameOfLifeRunning ? "Stop game of life" : "Start game of life";
        GOBtnStartStopGameOfLife.GetComponentInChildren<Text>().text = buttonText;
    }

    public void UpdateGameOfLifeSpeed()
    {

        _gameOfLifeSpeed = GOSldGameOfLifeSpeed.GetComponent<Slider>().value;
    }

    #endregion
}

