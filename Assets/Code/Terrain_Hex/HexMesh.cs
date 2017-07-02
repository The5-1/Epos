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
            //hexGridData.cells[i].fill = (hexGridData.cells[i].fill*1.1f)%1.0f; //RANDOMIZE DEBUG
            TriangulateCell(hexGridData.cells[i]);

            //Triangulate neighbour connection (triangulate the connection of 2 cells at a time)
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
        //get the neighbours and the two at the vertices
        HexCell neighbour = cell.GetNeighbour(direction);
        HexCell neighbourL = cell.GetNeighbour(direction.Left());
        HexCell neighbourR = cell.GetNeighbour(direction.Right());

        Vector3 center = cell.center + Vector3.up*cell.height;
        Vector3 center_neighbour = neighbour.center + Vector3.up * neighbour.height;
        Vector3 center_neighbourL = neighbourL.center + Vector3.up * neighbourL.height;
        Vector3 center_neighbourR = neighbourR.center + Vector3.up * neighbourR.height;

        //Debug.Log(string.Format("N: {0}, L: {1}, R: {2}", neighbour.center, neighbourL.center, neighbourR.center));

        //calc normalization factor for the weights
        float normalizeL = 1.0f / (cell.size + neighbour.size + neighbourL.size);
        float normalizeR = 1.0f / (cell.size + neighbour.size + neighbourR.size);
        //apply weights and normalize
        Vector3 weightL = new Vector3(cell.size,neighbour.size,neighbourL.size) * normalizeL;
        Vector3 weightR = new Vector3(cell.size,neighbour.size,neighbourR.size) * normalizeR;

        //calc corner vertex positions based on given weights of the three adjacent hexagons
        Vector3 posL = center * weightL.x + center_neighbour * weightL.y + center_neighbourL * weightL.z;
        Vector3 posR = center * weightR.x + center_neighbour * weightR.y + center_neighbourR * weightR.z;

        //calc vertices for the inner plateau, their distance to the center is based on the hardness
        Vector3 posL_inner = Vector3.Lerp(center, posL, cell.hardness);
        posL_inner.y = Mathf.Lerp(posL_inner.y, center.y, cell.hardness);

        Vector3 posR_inner = Vector3.Lerp(center, posR, cell.hardness);
        posR_inner.y = Mathf.Lerp(posR_inner.y, center.y, cell.hardness);

        float heightdiff = cell.height - neighbour.height;
        //if (Mathf.Abs(heightdiff) >= 1.0f)
        //{
            Vector3 posLplateau = new Vector3(posL.x, center.y, posL.z);
            Vector3 posRplateau = new Vector3(posR.x, center.y, posR.z);

            meshBuilder.AddTriangle(
                center,
                posL_inner,
                posR_inner
            );

            meshBuilder.AddTriangle(
                posR_inner,
                posL_inner,
                posR
            );

            meshBuilder.AddTriangle(
                posR,
                posL_inner,
                posL
            );

        /* }
         else
         {
             meshBuilder.AddTriangle(
                 center,
                 posL,
                 posR
             );
         }*/

        /*
        meshBuilder.AddTriangle(
            center + cell.height * Vector3.up,
            center + HexMetrics.GetLeftCorner(direction)* hexGridData.cellRadius*cell.fill,
            center + HexMetrics.GetRightCorner(direction) * hexGridData.cellRadius*cell.fill
        );
        */

    }

}
