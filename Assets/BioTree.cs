
using System.Collections.Generic;
using UnityEngine;

/*
 function rotate(pitch, roll, yaw) {
    var cosa = Math.cos(yaw);
    var sina = Math.sin(yaw);

    var cosb = Math.cos(pitch);
    var sinb = Math.sin(pitch);

    var cosc = Math.cos(roll);
    var sinc = Math.sin(roll);

    var Axx = cosa*cosb;
    var Axy = cosa*sinb*sinc - sina*cosc;
    var Axz = cosa*sinb*cosc + sina*sinc;

    var Ayx = sina*cosb;
    var Ayy = sina*sinb*sinc + cosa*cosc;
    var Ayz = sina*sinb*cosc - cosa*sinc;

    var Azx = -sinb;
    var Azy = cosb*sinc;
    var Azz = cosb*cosc;

    for (var i = 0; i < points.length; i++) {
        var px = points[i].x;
        var py = points[i].y;
        var pz = points[i].z;

        points[i].x = Axx*px + Axy*py + Axz*pz;
        points[i].y = Ayx*px + Ayy*py + Ayz*pz;
        points[i].z = Azx*px + Azy*py + Azz*pz;
    }
}*/

public class BioTree : MonoBehaviour {
    [SerializeField]
    bool showGizmos = false;
    [SerializeField]
    BranchOptions options; 

    System.Random rootRandom;

    const float
    CILINDER_SEGMENT_Q = 4,
    CILINDER_SEGMENT_Q_PERCENTAGE = 1 / 4;

    bool createdBranchTemp = false;
    Mesh sphereMesh;

    void OnDrawGizmos() {
        options.initBranch();
        rootRandom = new System.Random(options.randomSeed); //OG seed
        BranchRung baseRung = new BranchRung {
            pos = transform.position,
            rot = new Vector2(
                -transform.localEulerAngles.z*Mathf.Deg2Rad,
                transform.localEulerAngles.x*Mathf.Deg2Rad)// x,y
        };

        var randomGen = new System.Random(2);

        List<Branch> allBranches;
        allBranches = new List<Branch>();
        Branch tree = new Branch(baseRung, options, allBranches);




        List<Vector3> vertex = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        int branchNum = allBranches.Count;
        int vertexSoFar = 0;
        for (int i = 0; i < branchNum; i++){
            // MAKE EVERY RUNG have more data
            // CREATE MESH HERE
            //Gizmos.color = new Vector4((float)randomGen.NextDouble(),
              //                       (float)randomGen.NextDouble(),
                //                     (float)randomGen.NextDouble(), 1);
            List<BranchRung> branch = allBranches[i].core;
            var rungNum = branch.Count;
            //TEMP

            //Ring
            var segmentsNum = 0;
            for (int a = 0; a < rungNum;a++){
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
                    tris.Add(upL);
                    tris.Add(lowR);
                    tris.Add(lowL);
                    // tri 2
                    tris.Add(upL);
                    tris.Add(upR);
                    tris.Add(lowR);
                }
                for (int u = 0; u < segmentsNum;u++){
                    var vert = rung.ringData[u] + rung.pos;
                    vertex.Add(vert);

                    // Tris are entirely ID based this is valid
                    if (a>0 && u>0) {
                        var lowL = rungId + vertexSoFar + u- 1;
                        var lowR = rungId + vertexSoFar + u ;
                        var upL = rungIdU + vertexSoFar + u - 1;
                        var upR = rungIdU + vertexSoFar + u ;
                        // tri 1
                        tris.Add(upL);
                        tris.Add(lowR);
                        tris.Add(lowL);
                        // tri 2
                        tris.Add(upL);
                        tris.Add(upR);
                        tris.Add(lowR);
                    }
                    if(showGizmos)
                    Gizmos.DrawCube(vert, Vector3.one*0.03f);
                }

                if (showGizmos) {
                    if (a > 0) {
                        BranchRung prevRung = branch[a - 1];
                        Gizmos.DrawLine(prevRung.pos, rung.pos);
                    }
                }
            }
            vertexSoFar = vertex.Count;
            if (showGizmos) {
                Gizmos.color = new Vector4((float)randomGen.NextDouble(),
                                     (float)randomGen.NextDouble(),
                                           (float)randomGen.NextDouble(), 1);
            }
        }

        vertexSoFar = vertex.Count;
        Vector3[] actualVerts = new Vector3[vertexSoFar];
        for (int i = 0; i < vertexSoFar;i++){
            actualVerts[i] = vertex[i];
        }
        int triCount = tris.Count;
        int[] actualTris = new int[triCount];
        for (int i = 0; i < triCount; i++) {
            actualTris[i] = tris[i];
        }
        //Mesh times, create mesh access filter and renderer.
        sphereMesh = new Mesh();
        MeshFilter sphereMeshFilter = GetComponent<MeshFilter>() as MeshFilter;
        MeshRenderer sphereRenderer = GetComponent<MeshRenderer>() as MeshRenderer;

        sphereMesh.vertices = actualVerts;
        sphereMesh.triangles = actualTris;

        // Open to create UV maps someday. maybe.
        //sphereMesh.uv = GetUvBuffer();

        // Finish Mesh up.
        sphereMeshFilter.mesh = sphereMesh;
        sphereMesh.RecalculateBounds();
        sphereMesh.RecalculateNormals();
    }
}


class Branch {
    System.Random randomGen;
    System.Random branchShape;
    System.Random branchGenerator;
    System.Random fullUtilityRandom;



    public List<BranchRung> core;

    // Growth, branches shouldnt have branch limits.
    int randomSeed;

    public Branch(BranchRung baseRung, BranchOptions seed, List<Branch> root, BranchOptions originalSeed = null) {
        if(originalSeed == null){
            originalSeed = seed;
        }
        core = new List<BranchRung>();
        root.Add(this);
        randomGen = new System.Random(seed.randomSeed);
        int branchSeed = randomGen.Next();
        int rungNum = Mathf.CeilToInt(seed.growth);
        float rungSize = seed.rungSize + (seed.growth * BranchOptions.RUNG_VS_GROWTH_SIZE);
        float rungNumFloatDifference = seed.growth - (float)rungNum;

        float treeRadius = seed.treeRadius * seed.growth * .01f;;
        float treeSegments = Mathf.Round(seed.radialSegments * treeRadius);
        if(treeSegments<3){
            treeSegments = 3;
        }
        float treeScale = 1 / (rungNum * BranchOptions.RUNG_CLOSENESS);
        branchShape = new System.Random(branchSeed);
        branchGenerator = new System.Random(branchSeed);
        core.Add(baseRung);
        BranchRung rootBranch = root[0].core[0];
        //bruteforce

        for (int i = 1; i < rungNum; i++) {
            BranchRung prevRung = core[i - 1];
            BranchRung rung = new BranchRung();
            var percInBranch = 1 - ((float)i / (float)rungNum);
            //init
            rung.ringData = new List<Vector3>();

            // Twisting the branch randomly
            var rndX = ((float)branchShape.NextDouble());
            var rndY = ((float)branchShape.NextDouble());
            rung.rot.x = prevRung.rot.x + (seed.halfTwistX - seed.twistX * rndX);
            rung.rot.y = prevRung.rot.y + (seed.halfTwistY - seed.twistY * rndY);

            // Straightening up the branch, otherwise it will look like sea weed.
            if (seed.correctiveBehavior != 0) {
                rung.rot.x += (rootBranch.rot.x - rung.rot.x) * seed.correctiveBehavior;
                rung.rot.y += (rootBranch.rot.y - rung.rot.y) * seed.correctiveBehavior;
            }
            if (seed.straightness != 0) {
                rung.rot.x += (baseRung.rot.x - rung.rot.x) * seed.straightness;
                rung.rot.y += (baseRung.rot.y - rung.rot.y) * seed.straightness;
            }

            // Creating new branch rung position
            var cosY = Mathf.Cos(rung.rot.y);
            var cartX = (cosY* Mathf.Sin(rung.rot.x));
            var cartY = (cosY * Mathf.Cos(rung.rot.x));
            var cartZ = Mathf.Sin(prevRung.rot.y);
            rungSize += (BranchOptions.MIN_RUNG_SIZE - rungSize) * treeScale * BranchOptions.RUNG_CLOSENESS;
            rung.pos.x = prevRung.pos.x + rungSize * cartX;
            rung.pos.y = prevRung.pos.y + rungSize * cartY;
            rung.pos.z = prevRung.pos.z + rungSize * cartZ;

            //Temp, TODO: use fibonacci
            if (Mathf.RoundToInt(((float)branchGenerator.NextDouble()) * seed.branchHappening) == 0) {
                int newBranchSeed = randomGen.Next();
                int leftOverGrowth = rungNum - i;
                fullUtilityRandom = new System.Random(newBranchSeed);

                // Randomizing branch direction from mother branch
                var newRandX = 1f - (2f) * ((float)fullUtilityRandom.NextDouble());
                var newRandY = 1f - (2f) * ((float)fullUtilityRandom.NextDouble());
                Vector2 newDirection = rung.rot;
                newDirection.x += newRandX;
                newDirection.y += newRandY;
                rung.rot.x -= newRandX * seed.branchSplitForced;
                rung.rot.y -= newRandY * seed.branchSplitForced;
                //To ensure the pattern remains
                baseRung = rung;

                BranchRung newRung = new BranchRung {
                    pos = rung.pos,
                    rot = newDirection
                };
                var reduction =  .9f * percInBranch;
                var newRadialSegments = Mathf.RoundToInt(treeSegments * reduction);
                if(newRadialSegments<3){
                    newRadialSegments = 3;
                }
                var newRadius = treeRadius * reduction;
                if (newRadius < originalSeed.minRadius) {
                    newRadius = originalSeed.minRadius;
                }
                BranchOptions newBranchOpts = new BranchOptions {
                    twistX = seed.twistX,
                    twistY = seed.twistY,
                    correctiveBehavior = seed.correctiveBehavior,
                    randomSeed = newBranchSeed,
                    branchHappening = seed.branchHappening,
                    growth = (leftOverGrowth + rungNumFloatDifference) * (1 - .61802f),
                    //dir does nothing
                    growthDirection = Vector3.zero,
                    treeRadius = newRadius,
                    radialSegments = newRadialSegments
                };

                newBranchOpts.rungSize = rungSize;
                newBranchOpts.initBranch();
                Branch newBranch = new Branch(newRung, newBranchOpts,root, originalSeed);
            }
            //rung.pos.z = prevRung.pos.x + sizePerc * Mathf.Cos(prevRung.rot.x);
            core.Add(rung);
        }

        // brute force
        var branchSize = core.Count;
        for (int i = 0; i < branchSize; i++) {
            float percInBranch = 1 - ((float)i / (float)branchSize);
            BranchRung rung = core[i];
            rung.ringData = new List<Vector3>();
            for (int a = 0; a < treeSegments; a++) {
                
                var aF = (float) a;
                var segmentsF = (float) treeSegments;
                var pieceAngle = (aF / segmentsF) * Mathf.PI * 2;

                //TO SINGLE FUNCTION

                var currentAngleX = -rung.rot.x;
                var currentAngleZ = rung.rot.y;
                var x = Mathf.Cos(pieceAngle) * treeRadius * percInBranch;
                var z = Mathf.Sin(pieceAngle) * treeRadius * percInBranch;
                var cosX = Mathf.Cos(currentAngleX);
                var sinX = Mathf.Sin(currentAngleX);
                var cosZ = Mathf.Cos(currentAngleZ);
                var sinZ = Mathf.Sin(currentAngleZ);

                var Axz = sinX * sinZ;
                var Ayz = -cosX * sinZ;
                var Azz = cosZ;

                var projectX = cosX * x + Axz * z;
                var projectY = sinX * x + Ayz * z;
                var projectZ = Azz * z;

                Vector3 vert = new Vector3 {
                    x = projectX,
                    y = projectY,
                    z = projectZ
                };
                rung.ringData.Add(vert);
            }


        }
    }
}