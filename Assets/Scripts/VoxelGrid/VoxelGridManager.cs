using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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



    private VoxelGrid _grid;
    // Start is called before the first frame update
    void Start()
    {
        _grid = new VoxelGrid(_gridDimensions);

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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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

