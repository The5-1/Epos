using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class HexMetrics
{

    public const float outerRadius = 1.0f;
    public const float innerRadius = 0.86602540378443864676372317075294f;

    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius) //just add the 1st one again so we dont need any modulo to loop trough them
    };

}

[System.Serializable]
public class HexCell
{
    public Vector3 center;
    public float height;
    public float size;
    public float softness;

    public HexCell(Vector3 pos, float h = 0.0f, float si = 0.5f, float so = 0.5f)
    {
        center = pos;
        height = h;
        size = si;
        size = so;
    }
}

[System.Serializable]
public class HexGridData {

    public int width;
    public int height;

    public float cellRadius;

    public HexCell[] cells;

    public HexGridData(int w, int h, float r = 0.5f)
    {
        width = w;
        height = h;
        cellRadius = r;
        init();
    }

    private void init()
    {
        updateCells();
    }

    private void updateCells()
    {
        cells = new HexCell[height * width];
        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                AddCell(x, z, i);
                i++;
            }
        }
    }

    private void clearCells()
    {
        cells = null;
    }

    public void resize(int w, int h)
    {
        width = w;
        height = h;
        clearCells();
        init();
    }

    private void AddCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerRadius * cellRadius*2.0f;
        position.y = 0.0f;
        position.z = z * HexMetrics.outerRadius * cellRadius*1.5f;

        cells[i] = new HexCell(position);
    }
}
