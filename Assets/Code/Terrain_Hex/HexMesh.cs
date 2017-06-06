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

        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            TriangulateDirection(d, cell);
        }
    }

    private void TriangulateDirection(HexDirection direction, HexCell cell)
    {
        Vector3 center = cell.center;
        /*
        HexCell neighbour = cell.GetNeighbour(direction);
        HexCell neighbourL = cell.GetNeighbour(direction.Left());
        HexCell neighbourR = cell.GetNeighbour(direction.Right());

        Debug.Log(string.Format("N: {0}, L: {1}, R: {2}", neighbour.center, neighbourL.center, neighbourR.center));

        
        float divL = 1.0f / (cell.extent+ neighbour.extent+ neighbourL.extent);
        float divR = 1.0f / (cell.extent + neighbour.extent + neighbourR.extent);
        Vector3 weightL = new Vector3(cell.extent,neighbour.extent,neighbourL.extent) * divL;
        Vector3 weightR = new Vector3(cell.extent,neighbour.extent,neighbourR.extent) * divR;

        Vector3 posL = cell.center * weightL.x + neighbour.center * weightL.y + neighbourL.center * weightL.z;
        Vector3 posR = cell.center * weightR.x + neighbour.center * weightR.y + neighbourR.center * weightR.z;

        meshBuilder.AddTriangle(
            center + cell.height * Vector3.up,
            posL,
            posR
        );
        */

        meshBuilder.AddTriangle(
            center + cell.height * Vector3.up,
            center + HexMetrics.GetLeftCorner(direction)* hexGridData.cellRadius*cell.fill,
            center + HexMetrics.GetRightCorner(direction) * hexGridData.cellRadius*cell.fill
        );

    }

}
