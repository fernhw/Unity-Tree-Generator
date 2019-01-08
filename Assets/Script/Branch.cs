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

class Branch {

    // Growth, branches shouldnt have branch limits.
    int randomSeed;

    // Randomizers
    readonly System.Random 
        randomGen,
        branchShape,
        branchGenerator,
        fullUtilityRandom;

    public List<BranchRung> core;

    public Branch(BranchRung baseRung, Seed seed, List<Branch> root, Seed originalSeed = null) {
        if (originalSeed == null) {
            originalSeed = seed;
        }
        core = new List<BranchRung>();
        root.Add(this);
        randomGen = new System.Random(seed.randomSeed);
        int branchSeed = randomGen.Next();
        int rungNum = Mathf.CeilToInt(seed.growth);
        float rungSize = seed.rungSize + (seed.growth * Seed.RUNG_VS_GROWTH_SIZE);
        float rungNumFloatDifference = seed.growth - (float)rungNum;
        float treeRadius = seed.treeRadius * seed.growth * .01f; ;
        float treeSegments = Mathf.Round(seed.radialSegments * treeRadius);

        if (treeSegments < 3) {
            treeSegments = 3;
        }

        float treeScale = 1 / (rungNum * Seed.RUNG_CLOSENESS);

        branchShape = new System.Random(branchSeed);
        branchGenerator = new System.Random(branchSeed);
        core.Add(baseRung);

        //Accessing first branch
        BranchRung rootBranch = root[0].core[0];
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
            var cartX = (cosY * Mathf.Sin(rung.rot.x));
            var cartY = (cosY * Mathf.Cos(rung.rot.x));
            var cartZ = Mathf.Sin(prevRung.rot.y);
            rungSize += (Seed.MIN_RUNG_SIZE - rungSize) * treeScale * Seed.RUNG_CLOSENESS;
            rung.pos.x = prevRung.pos.x + rungSize * cartX;
            rung.pos.y = prevRung.pos.y + rungSize * cartY;
            rung.pos.z = prevRung.pos.z + rungSize * cartZ;

            // TODO: FIX, randomizer is too random, use FI somehow.
            if (Mathf.RoundToInt(((float)branchGenerator.NextDouble()) * seed.branchHappening) == 0) {
                int newBranchSeed = randomGen.Next();
                int leftOverGrowth = rungNum - i;
                fullUtilityRandom = new System.Random(newBranchSeed);

                // Randomizing branch direction from mother branch.
                var newRandX = 1f - (2f) * ((float)fullUtilityRandom.NextDouble());
                var newRandY = 1f - (2f) * ((float)fullUtilityRandom.NextDouble());
                Vector2 newDirection = rung.rot;
                newDirection.x += newRandX;
                newDirection.y += newRandY;
                rung.rot.x -= newRandX * seed.branchSplitForced;
                rung.rot.y -= newRandY * seed.branchSplitForced;

                // This ensures the pattern of random generators doesn't break.
                baseRung = rung;

                BranchRung newRung = new BranchRung {
                    pos = rung.pos,
                    rot = newDirection
                };
                var reduction = .9f * percInBranch;
                var newRadialSegments = Mathf.RoundToInt(treeSegments * reduction);
                if (newRadialSegments < 3) {
                    newRadialSegments = 3;
                }
                var newRadius = treeRadius * reduction;
                if (newRadius < originalSeed.minRadius) {
                    newRadius = originalSeed.minRadius;
                }
                Seed newBranchOpts = new Seed {
                    twistX = seed.twistX,
                    twistY = seed.twistY,
                    correctiveBehavior = seed.correctiveBehavior,
                    randomSeed = newBranchSeed,
                    branchHappening = seed.branchHappening,
                    growth = (leftOverGrowth + rungNumFloatDifference) * (1 - .61802f),

                    //TODO: FIX, dir does nothing
                    growthDirection = Vector3.zero,

                    treeRadius = newRadius,
                    radialSegments = newRadialSegments
                };

                newBranchOpts.rungSize = rungSize;
                newBranchOpts.InitBranch();
                Branch newBranch = new Branch(newRung, newBranchOpts, root, originalSeed);
            }

            core.Add(rung);
        }

        var branchSize = core.Count;
        for (int i = 0; i < branchSize; i++) {
            float percInBranch = 1 - ((float)i / (float)branchSize);
            BranchRung rung = core[i];
            rung.ringData = new List<Vector3>();
            for (int a = 0; a < treeSegments; a++) {

                var aF = (float)a;
                var segmentsF = (float)treeSegments;
                var pieceAngle = (aF / segmentsF) * Mathf.PI * 2;

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