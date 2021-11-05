using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGridManager : MonoBehaviour
{
    [SerializeField]
    private Vector3Int _gridDimensions = new Vector3Int(10, 1, 10);

    private VoxelGrid _grid;
    // Start is called before the first frame update
    void Start()
    {
        _grid = new VoxelGrid(_gridDimensions);
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
                    var voxel = hitObject.GetComponent<VoxelTrigger>().AttachedVoxel;

                    voxel.ToggleNeighbours();
                }
            }
        }
    }
}

