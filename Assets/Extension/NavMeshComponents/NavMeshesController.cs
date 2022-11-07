using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshesController : MonoBehaviour
{

    public Vector2Int GridSize = new Vector2Int(5, 5);
    public Vector2Int Offset = new Vector2Int(5, 5);

    public NavMeshSurface Surface;

    public GameObject NavMeshObject;

    private void Start()
    {
        for(int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                GameObject newNavMeshSurface = Instantiate(NavMeshObject, transform);
                newNavMeshSurface.SetActive(true);

                newNavMeshSurface.name = $"{x},{y}";
                newNavMeshSurface.transform.localPosition = new Vector3(x*Offset.x, 0, y*Offset.y);
            }
        }

        Surface.BuildNavMesh();

        //BakeAllNavMesh();
    }

    void BakeAllNavMesh()
    {
        NavMeshSurface[] allSurfaces = GetComponentsInChildren<NavMeshSurface>();
        foreach(NavMeshSurface surface in allSurfaces)
        {
            surface.BuildNavMesh();
        }
    }
}
