using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using DelaunatorSharp;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEditor.Sprites;
using GK;
using ConcaveHull;
using Math = System.Math;

public class Mapgenerator : MonoBehaviour
{
    public GameObject mapPlane;
    public Texture2D mapImage;
    public Color buildingColor;
    public Texture2D debugResult;
    public Texture2D debugResult2;
    public float buildingHeight = .01f;

    public Material material;
    private ConvexHullCalculator hullCalculator = new ConvexHullCalculator();

    private const float ALLOWED_COLOR_DELTA = 0.000001f;
    private float aspectFactor = 1;
    IEnumerable<ITriangle> triangles;

    private void Start()
    {
        //set plane to correct aspect ratio
        aspectFactor = mapImage.width / (mapImage.height * 1f);
        mapPlane.transform.localScale = new Vector3(mapPlane.transform.localScale.x * aspectFactor, 1, mapPlane.transform.localScale.z);

        //Reset result so floodfill can operate properly
        for (int x = 0; x < debugResult.width; x++)
        {
            for (int y = 0; y < debugResult.height; y++)
            {
                debugResult.SetPixel(x, y, mapImage.GetPixel(x, y));
            }
        }

        for (int x = 0; x < debugResult.width; x += 100)
        {
            for (int y = 0; y < debugResult.height; y += 100)
            {
                Color mapColor = mapImage.GetPixel(x, y);

                if (colorsEqual(mapColor, buildingColor) && mapPlane.transform.childCount < 2)
                {


                    //Get Building
                    List<Vector2> buildingPixels = FloodFill(debugResult, new Vector2Int(x, y), Color.red);
                    print(buildingPixels.Count);
                    generateBuilding(buildingPixels);
                }
            }
        }

        debugResult.Apply();
    }

    private void generateBuilding(List<Vector2> buildingPixels)
    {
        if (buildingPixels.Count == 0)
            return;

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector3> points = new List<Vector3>();

        List<Node> buildingPoints = new List<Node>();


        Vector2 centerOfMass = CalculateNormalizedCenterOfMass(buildingPixels);

        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < buildingPixels.Count; i++)
        {
            float x = buildingPixels[i].x;
            float y = buildingPixels[i].y;

            x = NormalizeX(x);
            y = NormalizeY(y);

            //Move point towards origin by distance from center of mass to origin so the Mesh is not offset by origin
            x -= centerOfMass.x;
            y -= centerOfMass.y;


            //Add Points at ground level and higher up
            Node node = new Node(x, y, i);
            buildingPoints.Add(node);
            // buildingPoints.Add(new Vector3(x, 1, y));

        }

        print("Setup Buildpoints in: " + (stopwatch.Elapsed));
        Hull.setConvexHull(buildingPoints);
        print("Setup Convex Hull in: " + (stopwatch.Elapsed));

        List<Line> lines = Hull.setConcaveHull(-0.5f, 1);
        print("Setup Concave Hull in: " + (stopwatch.Elapsed));

        // for (int i = 1; i < buildingPoints.Count; i++)
        // {
        //     Node nodeA = new Node(buildingPoints[i - 1].x, buildingPoints[i - 1].y, i - 1);
        //     Node nodeB = new Node(buildingPoints[i].x, buildingPoints[i].y, i);
        //     lines.Add(new Line(nodeA, nodeB));
        // }
        // Node a = new Node(buildingPoints[buildingPoints.Count - 1].x, buildingPoints[buildingPoints.Count - 1].y, buildingPoints.Count - 1);
        // Node b = new Node(buildingPoints[0].x, buildingPoints[0].y, buildingPoints.Count + 2);
        // lines.Add(new Line(a, b));
        // print("Setup lines in: " + (stopwatch.Elapsed));



        ProBuilderMesh proBuilderMesh = GenerateMesh(lines);
        print("Setup Mesh in: " + (stopwatch.Elapsed));
        if (proBuilderMesh == null)
        {
            return;
        }

        // *2 since the distance center of mass to origin was subtracted previously
        proBuilderMesh.transform.position = new Vector3(centerOfMass.x * 2, 0, centerOfMass.y * 2);
        proBuilderMesh.transform.parent = mapPlane.transform;
        print("Placed Object in: " + (stopwatch.Elapsed));

    }

    private float NormalizeY(float y)
    {
        //Move positions to origin of panel
        y -= (mapImage.height / 2f);

        // Normalize buildpoints to Size of Map
        y /= (mapImage.height * 1f);

        //Normalize buildpoints to Size of Plane
        return y *= 5 * mapPlane.transform.localScale.z;
    }

    private float NormalizeX(float x)
    {
        //Move positions to origin of panel
        x -= (mapImage.width / 2f);

        // Normalize buildpoints to Size of Map
        x /= (mapImage.width * 1f);

        //Normalize buildpoints to Size of Plane
        return x *= 5 * mapPlane.transform.localScale.x;
    }
    private Vector2 CalculateNormalizedCenterOfMass(List<Vector2> buildingPixels)
    {
        float meanX = 0;
        float meanY = 0;
        buildingPixels.ForEach((p) =>
         {
             meanX += NormalizeX(p.x);
             meanY += NormalizeY(p.y);
         }
          );
        meanX /= buildingPixels.Count;
        meanY /= buildingPixels.Count;

        return new Vector2(meanX, meanY);
    }

    private Vector2Int CalculateSpriteDimensions(List<Vector2Int> buildingPixels)
    {
        int minX = int.MaxValue;
        int maxX = 0;

        int minY = int.MaxValue;
        int maxY = 0;

        buildingPixels.ForEach((p) =>
        {
            minX = Math.Min(minX, p.x);
            maxX = Math.Max(maxX, p.x);

            minY = Math.Min(minY, p.y);
            maxY = Math.Max(maxY, p.y);
        }
        );

        return new Vector2Int(maxX - minX, maxY - minY);

    }

    private bool colorsEqual(Color colorA, Color colorB)
    {
        if (Math.Abs(colorA.r - colorB.r) < ALLOWED_COLOR_DELTA &&
        Math.Abs(colorA.g - colorB.g) < ALLOWED_COLOR_DELTA &&
        Math.Abs(colorA.b - colorB.b) < ALLOWED_COLOR_DELTA)
        {
            return true;
        }

        return false;
    }

    private List<Vector2> FloodFill(Texture2D targetTexture, Vector2Int start, Color replacementColor)
    {
        Color targetColor = targetTexture.GetPixel(start.x, start.y);
        HashSet<Vector2> buildingPixels = new HashSet<Vector2>();
        if (colorsEqual(targetColor, replacementColor))
        {
            return new List<Vector2>(buildingPixels);
        }

        Stack<Vector2Int> pixels = new Stack<Vector2Int>();

        pixels.Push(start);
        buildingPixels.Add(start);
        while (pixels.Count != 0)
        {
            Vector2Int temp = pixels.Pop();
            int y1 = temp.y;
            while (y1 >= 0 && targetTexture.GetPixel(temp.x, y1) == targetColor)
            {
                y1--;
            }
            y1++;
            bool spanLeft = false;
            bool spanRight = false;
            while (y1 <= targetTexture.height && colorsEqual(targetTexture.GetPixel(temp.x, y1), targetColor))
            {
                targetTexture.SetPixel(temp.x, y1, replacementColor);
                bool equalLeft = colorsEqual(mapImage.GetPixel(temp.x - 1, y1), targetColor);
                bool equalRight = colorsEqual(mapImage.GetPixel(temp.x + 1, y1), targetColor);
                bool equalDown = colorsEqual(mapImage.GetPixel(temp.x, y1 - 1), targetColor);
                bool equalUp = colorsEqual(mapImage.GetPixel(temp.x, y1 + 1), targetColor);

                if (!equalLeft || !equalRight || !equalRight || !equalDown)
                {

                    //Only add to buildingpixel if not all of the surrounding pixels are targetcolor
                    //which means it is vorder pixel
                    //way to cut down calculations in hull generation
                    buildingPixels.Add(new Vector2(temp.x, y1));
                }

                if (!spanLeft && temp.x > 0 && equalLeft)
                {
                    pixels.Push(new Vector2Int(temp.x - 1, y1));
                    spanLeft = true;
                }
                else if (spanLeft && temp.x - 1 == 0 && !equalLeft)
                {
                    spanLeft = false;
                }
                if (!spanRight && temp.x < targetTexture.width - 1 && equalRight)
                {
                    pixels.Push(new Vector2Int(temp.x + 1, y1));
                    spanRight = true;
                }
                else if (spanRight && temp.x < targetTexture.width - 1 && !equalRight)
                {
                    spanRight = false;
                }
                y1++;
            }

        }

        return new List<Vector2>(buildingPixels);

    }

    private ProBuilderMesh GenerateMesh(List<Line> concaveHull)
    {
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Face> faces = new List<Face>();


        Line startLine = concaveHull[0];


        Line curLine = concaveHull.Find((line) => line.nodes[1].id == startLine.nodes[0].id);


        //Go back the edge until there is no Line ending with startnode
        while (curLine != null)
        {
            startLine = curLine;
            curLine = concaveHull.Find((line) => line.nodes[1].id == curLine.nodes[0].id);
        }

        curLine = startLine;

        Vector3 v1;
        Vector3 v2;
        Vector3 v3;
        while (curLine != startLine || verts.Count == 0)
        {
            tris.Clear();
            tris.Add(verts.Count);
            v3 = new Vector3((float)curLine.nodes[0].x, buildingHeight, (float)curLine.nodes[0].y);
            verts.Add(v3);

            tris.Add(verts.Count);
            v2 = new Vector3((float)curLine.nodes[1].x, 0, (float)curLine.nodes[1].y);
            verts.Add(v2);

            tris.Add(verts.Count);
            v1 = new Vector3((float)curLine.nodes[0].x, 0, (float)curLine.nodes[0].y);
            verts.Add(v1);

            faces.Add(new Face(tris));


            tris.Clear();
            tris.Add(verts.Count);
            v3 = new Vector3((float)curLine.nodes[0].x, buildingHeight, (float)curLine.nodes[0].y);
            verts.Add(v3);

            tris.Add(verts.Count);
            v2 = new Vector3((float)curLine.nodes[1].x, buildingHeight, (float)curLine.nodes[1].y);
            verts.Add(v2);

            tris.Add(verts.Count);
            v1 = new Vector3((float)curLine.nodes[1].x, 0, (float)curLine.nodes[1].y);
            verts.Add(v1);
            faces.Add(new Face(tris));


            Line nextLine = concaveHull.Find((line) => line.nodes[0].id == curLine.nodes[1].id);

            if (nextLine == null)
            {
                //Connect endpoint with start line


                tris.Clear();
                tris.Add(verts.Count);
                v3 = new Vector3((float)curLine.nodes[1].x, buildingHeight, (float)curLine.nodes[1].y);
                verts.Add(v3);

                tris.Add(verts.Count);
                v2 = new Vector3((float)startLine.nodes[0].x, 0, (float)startLine.nodes[0].y);
                verts.Add(v2);

                tris.Add(verts.Count);
                v1 = new Vector3((float)curLine.nodes[1].x, 0, (float)curLine.nodes[1].y);
                verts.Add(v1);

                faces.Add(new Face(tris));


                tris.Clear();
                tris.Add(verts.Count);
                v3 = new Vector3((float)curLine.nodes[1].x, buildingHeight, (float)curLine.nodes[1].y);
                verts.Add(v3);

                tris.Add(verts.Count);
                v2 = new Vector3((float)startLine.nodes[0].x, buildingHeight, (float)startLine.nodes[0].y);
                verts.Add(v2);

                tris.Add(verts.Count);
                v1 = new Vector3((float)startLine.nodes[0].x, 0, (float)startLine.nodes[0].y);
                verts.Add(v1);
                faces.Add(new Face(tris));

                break;
            }
            curLine = nextLine;
        }


        List<IPoint> upperPoints = new List<IPoint>();

        for (int i = 0; i < verts.Count; i++)
        {
            if (verts[i].y == buildingHeight)
            {
                upperPoints.Add(new Point(verts[i].x, verts[i].z));
            }
        }


        Delaunator d = new Delaunator(upperPoints.ToArray());
        triangles = d.GetTriangles();
        IEnumerator<ITriangle> trisEnumerator = triangles.GetEnumerator();
        while (trisEnumerator.MoveNext())
        {
            Triangle triangle = (Triangle)trisEnumerator.Current;

            IEnumerator<IPoint> pointsEnumerator = triangle.Points.GetEnumerator();

            pointsEnumerator.MoveNext();

            Point point = (Point)pointsEnumerator.Current;
            tris.Clear();
            tris.Add(verts.Count);
            v1 = new Vector3((float)point.X, buildingHeight, (float)point.Y);
            verts.Add(v1);

            pointsEnumerator.MoveNext();
            point = (Point)pointsEnumerator.Current;

            tris.Add(verts.Count);
            v2 = new Vector3((float)point.X, buildingHeight, (float)point.Y);
            verts.Add(v2);

            pointsEnumerator.MoveNext();
            point = (Point)pointsEnumerator.Current;

            tris.Add(verts.Count);
            v3 = new Vector3((float)point.X, buildingHeight, (float)point.Y);
            verts.Add(v3);


            faces.Add(new Face(tris));
        }

        ProBuilderMesh pm = ProBuilderMesh.Create(verts, faces);
        pm.unwrapParameters = new UnwrapParameters();
        pm.Refresh();
        pm.ToMesh();
        pm.SetMaterial(faces, material);


        return pm;

    }

}
