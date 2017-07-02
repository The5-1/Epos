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
        HexCell neighbourM = cell.GetNeighbour(direction); //middle neighbour, edge connected
        HexCell neighbourL = cell.GetNeighbour(direction.Left()); //neighbour touching left vertex
        HexCell neighbourR = cell.GetNeighbour(direction.Right()); //neighbour touching right vertex

        //center positions
        Vector3 center = cell.center + Vector3.up*cell.height;
        Vector3 center_neighbourM = neighbourM.center + Vector3.up * neighbourM.height;
        Vector3 center_neighbourL = neighbourL.center + Vector3.up * neighbourL.height;
        Vector3 center_neighbourR = neighbourR.center + Vector3.up * neighbourR.height;

        //heightDiff
        float heightDiffM = cell.height - neighbourM.height;
        float heightDiffL = cell.height - neighbourL.height;
        float heightDiffR = cell.height - neighbourR.height;

        //hardness
        float hardness = 0.5f + (cell.hardness/2);
        float hardnessM = 0.5f + (neighbourM.hardness / 2);
        float hardnessL = 0.5f + (neighbourL.hardness / 2);
        float hardnessR = 0.5f + (neighbourR.hardness / 2);


        //Debug.Log(string.Format("N: {0}, L: {1}, R: {2}", neighbour.center, neighbourL.center, neighbourR.center));


        //size weights for shared vertices
        float normalizeM = 1.0f / (cell.size + neighbourM.size);
        float normalizeL = 1.0f / (cell.size + neighbourM.size + neighbourL.size);
        float normalizeR = 1.0f / (cell.size + neighbourM.size + neighbourR.size);
        Vector2 weightM = new Vector3(cell.size, neighbourM.size) * normalizeM;
        Vector3 weightL = new Vector3(cell.size,neighbourM.size,neighbourL.size) * normalizeL;
        Vector3 weightR = new Vector3(cell.size,neighbourM.size,neighbourR.size) * normalizeR;

        //calc shared vertex positions between adjacent hexagons based on their sizes
        Vector3 posM = center * weightM.x + center_neighbourM * weightM.y;
        Vector3 posL = center * weightL.x + center_neighbourM * weightL.y + center_neighbourL * weightL.z;
        Vector3 posR = center * weightR.x + center_neighbourM * weightR.y + center_neighbourR * weightR.z;

        Vector3 posLM = Vector3.Lerp(posM, posL, (hardness + hardnessM)/2);
        posLM.y = Mathf.Lerp(posLM.y, posM.y, (hardness + hardnessM)/2);

        Vector3 posRM = Vector3.Lerp(posM, posR, (hardness + hardnessM) / 2);
        posRM.y = Mathf.Lerp(posRM.y, posM.y, (hardness + hardnessM) / 2);

        //calc vertices for the inner plateau, their distance to the center is based on the hardness
        Vector3 posM_inner = Vector3.Lerp(center, posM, hardness);
        posM_inner.y = Mathf.Lerp(posM_inner.y, center.y, hardness);

        Vector3 posL_inner = Vector3.Lerp(center, posL, hardness);
        posL_inner.y = Mathf.Lerp(posL_inner.y, center.y, hardness);

        Vector3 posR_inner = Vector3.Lerp(center, posR, hardness);
        posR_inner.y = Mathf.Lerp(posR_inner.y, center.y, hardness);

        Vector3 posLplateau = new Vector3(posL.x, center.y, posL.z);
        Vector3 posRplateau = new Vector3(posR.x, center.y, posR.z);


#if false //simple connections (consider 3 neigbors at VERTEX only)
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
#elif true //complex connections (bridge adjacent EDGES too)

        meshBuilder.ColorTrinagles(cell.color, 8);

        //center Left
        meshBuilder.AddTriangle(
            center,
            posL_inner,
            posM_inner
        );

        //center right
        meshBuilder.AddTriangle(
            center,
            posM_inner,
            posR_inner
        );

        //outer left
        meshBuilder.AddTriangle(
            posL_inner,
            posL,
            posLM
        );

        //outer left middle
        meshBuilder.AddTriangle(
            posL_inner,
            posLM,
            posM
        );

        //outer middle left
        meshBuilder.AddTriangle(
            posL_inner,
            posM,
            posM_inner
        );

        //outer middle right
        meshBuilder.AddTriangle(
            posR_inner,
            posM_inner,
            posM
        );

        //outer right middle
        meshBuilder.AddTriangle(
            posR_inner,
            posM,
            posRM
        );

        //outer right
        meshBuilder.AddTriangle(
            posR_inner,
            posRM,
            posR
        );
#endif

    }

}
