using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class HexGridMesh {

    public MeshBuilder meshBuilder;
    public HexGridData hexGridData;

    public HexGridMesh(HexGridData data)
    {
        hexGridData = data;
        meshBuilder = new MeshBuilder();
    }

    public Mesh getMesh()
    {
        return meshBuilder.UpdateMesh();
    }

    public Mesh recalculateMesh()
    {
        meshBuilder.Clear();
        TriangulateGrid(hexGridData.cells);
        return meshBuilder.UpdateMesh();
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
        Vector3 center = cell.center;
        for (int i = 0; i < 6; i++)
        {
            meshBuilder.AddTriangle(
                center + cell.height * Vector3.up, 
                center + HexMetrics.corners[i] * hexGridData.cellRadius, 
                center + HexMetrics.corners[i+1] * hexGridData.cellRadius
            );
        }
    }

}
