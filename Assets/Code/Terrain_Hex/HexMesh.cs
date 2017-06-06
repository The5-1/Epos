using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexMesh {

    private MeshBuilder meshBuilder;
    private HexGridData hexGridData;

    public HexMesh(HexGridData data)
    {
        hexGridData = data;
        meshBuilder = new MeshBuilder();
    }

    public Mesh getMesh()
    {
        return meshBuilder.CreateMesh();
    }

    public Mesh recalculateMesh()
    {
        meshBuilder.Clear();
        TriangulateGrid(hexGridData.cells);
        return meshBuilder.CreateMesh();
    }

    private void TriangulateGrid(HexCell[] cells)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            TriangulateCell(hexGridData.cells[i]);
        }
    }

    private void TriangulateCell(HexCell cell)
    {
        Vector3 center = cell.center + cell.height*Vector3.up;
        for (int i = 0; i < 6; i++)
        {
            meshBuilder.AddTriangle(
                center + Vector3.up*0.1f, 
                center + HexMetrics.corners[i] * hexGridData.cellRadius, 
                center + HexMetrics.corners[i+1] * hexGridData.cellRadius
            );
        }
    }

}
