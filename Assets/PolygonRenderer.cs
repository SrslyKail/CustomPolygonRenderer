using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PolygonRenderer : MonoBehaviour
{
    //Mesh properties
    private int[] polygonTriangles;

    //Polygon properties
    [Min(3)]
    public int numberOfSides;
    [Min(0f)]
    public float polygonRadius;
    public bool isFilled;
    [Min(0f)]
    public float centerRadius;
    [Min(1)]
    public int numberOfPolygons;
    [Min(1)]
    public int rowSize;

    private List<GameObject> polygons;

    private void Start()
    {
        polygons = new List<GameObject>();
    }

    private void Update()
    {
        for (int i = 0; i < polygons.Count; i++)
        {
            Destroy(polygons[i]);
        }

        polygons = new List<GameObject>();
        for (int i = 0; i < numberOfPolygons; i++)
        {
            polygons.Add(new GameObject());
            polygons[i].AddComponent<MeshRenderer>();
            polygons[i].AddComponent<MeshFilter>();
        }
        // Get the points for the shape; this forms a default list we can use
        Vector3[] shapePoints = GetCircumferencePoints(numberOfSides, polygonRadius).ToArray();


        for (int i = 0; i < polygons.Count; i++)
        {
            polygons[i].name = i.ToString();
            Mesh currentMesh = polygons[i].GetComponent<MeshFilter>().mesh;

            polygons[i].transform.position += new Vector3((polygonRadius * 2) * i, 0, 0);

            if (isFilled)
            {
                DrawFilled(shapePoints, currentMesh);
            }
            else
            {
                DrawHollow(shapePoints, currentMesh);
            }

        }
        
    }

    void DrawFilled(Vector3[] points, Mesh mesh)
    {
        polygonTriangles = DrawFilledTriangles(points);
        mesh.vertices = points;
        mesh.triangles = polygonTriangles;

    }

    void DrawHollow(Vector3[] outerPoints, Mesh mesh)
    {
        List<Vector3> pointsList = new List<Vector3>();
        Vector3[] innerPoints = GetCircumferencePoints(numberOfSides, centerRadius).ToArray();
        pointsList.AddRange(outerPoints);
        pointsList.AddRange(innerPoints);

        Vector3[] polygonPoints = pointsList.ToArray();

        polygonTriangles = DrawHollowTriangles(polygonPoints);
        mesh.vertices = polygonPoints;
        mesh.triangles = polygonTriangles;

    }

    int[] DrawHollowTriangles(Vector3[] points)
    {
        int sides = points.Length / 2;

        List<int> newTriangles = new List<int>();
        for (int i = 0; i < sides; i++)
        {
            int outerIndex = i;
            int innerIndex = i + sides;

            newTriangles.Add(outerIndex);
            newTriangles.Add(innerIndex);
            newTriangles.Add((i + 1) % sides);

            newTriangles.Add(outerIndex);
            newTriangles.Add(sides + ((innerIndex - 1) % sides));
            newTriangles.Add(outerIndex + sides);
        }
        return newTriangles.ToArray();
    }

    List<Vector3> GetCircumferencePoints(int sides, float radius)
    {
        List<Vector3> points = new List<Vector3>();
        float circumferenceStepSize = (float)1 / sides;
        float TAU = 2 * Mathf.PI;
        float radianStepSize = circumferenceStepSize * TAU;

        for (int i = 0; i < sides; i++)
        {
            float currentRadian = radianStepSize * i;
            points.Add(
                // Cos gets us our point on the X axis
                new Vector3(Mathf.Cos(currentRadian) * radius,
                // Sin gets us our point on the Y axis
                Mathf.Sin(currentRadian) * radius,
                //0, because we dont need Z axis
                0
                ));
        }

        return points;
    }

    int[] DrawFilledTriangles(Vector3[] points)
    {
        int numberOfTriangles = points.Length - 2;
        List<int> triangles = new List<int>();
        for (int i = 0; i < numberOfTriangles; i++)
        {
            triangles.Add(0);
            triangles.Add(i + 2);
            triangles.Add(i + 1);
        }
        return triangles.ToArray();
    }
}
