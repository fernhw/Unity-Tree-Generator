/** 
 * Copyright (c) 2018 Fernando Holguin Weber - All Rights Reserved
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of the Unity Tree Generator 
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute,
 * sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial 
 * portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
 * NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * Fernando Holguín Weber, <contact@fernhw.com>,<http://fernhw.com>,<@fern_hw>
 * 
 */


using System.Collections.Generic;
using UnityEngine;

public class BioTree : MonoBehaviour {

    public static readonly Vector3 ZERO = new Vector3(0, 0, 0);

    const float
        CILINDER_SEGMENT_Q = 4,
        CILINDER_SEGMENT_Q_PERCENTAGE = 1 / 4;

    [SerializeField]
    Seed seed;

    [SerializeField]
    bool showGizmos = false;

    Mesh treeMesh;
    Seed oldSeed;
    readonly System.Random colorRandomizerForGizmos = new System.Random(2);
    bool createdBranchTemp = false;
    bool inRuntime = false;


    void Start() {
        inRuntime = true;
    }

    void Update() {
        CreateTree();
    }

    void OnDrawGizmos() {
        if (!inRuntime)
            CreateTree();
    }


    void CreateTree() {
        if (oldSeed == null) {
            oldSeed = new Seed();
            oldSeed.InitBranch();
        }

        // If seed hasn't changed return;
        if (Seed.CompareSeeds(oldSeed, seed)) {
            return;
        }

        if (seed.randomSeedFromObjectHash) {
            int hash = this.transform.GetHashCode();
            if (hash != seed.randomSeed) {
                seed.randomSeed = hash;
            }
        }

        Seed.CopySeedDataIntoOtherSeed(oldSeed, seed);

        //FIX: Might be for the better reduce function calls
        seed.InitBranch();

        BranchRung firstTreeRung = new BranchRung {
            pos = ZERO,
            rot = new Vector2(
                -transform.localEulerAngles.z * Mathf.Deg2Rad,
                transform.localEulerAngles.x * Mathf.Deg2Rad)
        };

        List<Branch> allBranches = new List<Branch>();

        // This draws whole Tree
        Branch tree = new Branch(firstTreeRung, seed, allBranches);

        // 3D Model Logic:
        DrawTreeDModel(allBranches);
    }

    void DrawTreeDModel(List<Branch> branches) {

        // FIX: Change to regular [] arrays.
        List<Vector3> vertexList = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> trisList = new List<int>();

        int branchNum = branches.Count;
        int vertexSoFar = 0;
        for (int i = 0; i < branchNum; i++) {

            List<BranchRung> branch = branches[i].core;
            var rungNum = branch.Count;
            var segmentsNum = 0;
            for (int a = 0; a < rungNum; a++) {
                BranchRung rung = branch[a];
                segmentsNum = rung.ringData.Count;

                var rungId = segmentsNum * (a - 1);
                var rungIdU = segmentsNum * (a);
                if (a > 0) {
                    var lowL = rungId + vertexSoFar + segmentsNum - 1;
                    var lowR = rungId + vertexSoFar;
                    var upL = rungIdU + vertexSoFar + segmentsNum - 1;
                    var upR = rungIdU + vertexSoFar;

                    // tri 1
                    trisList.Add(upL);
                    trisList.Add(lowR);
                    trisList.Add(lowL);

                    // tri 2
                    trisList.Add(upL);
                    trisList.Add(upR);
                    trisList.Add(lowR);
                }

                // Closing the 3D model
                for (int u = 0; u < segmentsNum; u++) {
                    var vert = rung.ringData[u] + rung.pos;
                    vertexList.Add(vert);

                    // Tris are entirely ID based this is valid
                    if (a > 0 && u > 0) {
                        var lowL = rungId + vertexSoFar + u - 1;
                        var lowR = rungId + vertexSoFar + u;
                        var upL = rungIdU + vertexSoFar + u - 1;
                        var upR = rungIdU + vertexSoFar + u;

                        // tri 1
                        trisList.Add(upL);
                        trisList.Add(lowR);
                        trisList.Add(lowL);

                        // tri 2
                        trisList.Add(upL);
                        trisList.Add(upR);
                        trisList.Add(lowR);
                    }
                    if (showGizmos)
                        Gizmos.DrawCube(vert, Vector3.one * 0.03f);
                }
                if (showGizmos) {
                    if (a > 0) {
                        BranchRung prevRung = branch[a - 1];
                        Gizmos.DrawLine(prevRung.pos, rung.pos);
                    }
                }
            }
            vertexSoFar = vertexList.Count;

            if (showGizmos) {
                var r = (float)colorRandomizerForGizmos.NextDouble();
                var g = (float)colorRandomizerForGizmos.NextDouble();
                var b = (float)colorRandomizerForGizmos.NextDouble();
                Gizmos.color = new Vector4(r, g, b, 1);
            }
        }

        // TEMP: FIX: Convert lists to Arrays, temporary.
        Vector3[] verts = new Vector3[vertexSoFar];
        for (int i = 0; i < vertexSoFar; i++) {
            verts[i] = vertexList[i];
        }
        int triCount = trisList.Count;
        int[] tris = new int[triCount];
        for (int i = 0; i < triCount; i++) {
            tris[i] = trisList[i];
        }

        //Mesh times, create mesh access filter and renderer.
        MeshFilter meshFilter = GetComponent<MeshFilter>() as MeshFilter;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>() as MeshRenderer;

        treeMesh = new Mesh();      
        treeMesh.vertices = verts;
        treeMesh.triangles = tris;
        //treeMesh.uv = uvs; //not yet

        meshFilter.mesh = treeMesh;
        treeMesh.RecalculateBounds();
        treeMesh.RecalculateNormals();
    }


}


