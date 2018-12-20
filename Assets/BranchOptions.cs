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


using UnityEngine;
using System;

[Serializable]
public class BranchOptions {
    public const float
        FI = 1.61803398875f,
        REV_FI = 0.61803398875f,

        RUNG_VS_GROWTH_SIZE = .001f,
        MIN_RUNG_SIZE = 0.00001f,
        RUNG_CLOSENESS = 0.001f;

    public int randomSeed;
    public Vector2 growthDirection;
    public float growth = 15;
    public float twistX = 0;
    public float twistY = 0;
    public float correctiveBehavior = 0.1f;
    public float straightness = 0.1f;
    public float rungSize = .1f; //TODO:Change uniformity
    public float branchSplitForced = 0;
    public float branchSplitDynamic = 0;
    public float branchHappening = 10;
    public float treeRadius = 2;
    public float minRadius = 0.1f;
    public int radialSegments = 40;
    [HideInInspector]
    public float halfTwistX;
    [HideInInspector]
    public float halfTwistY;
    public void initBranch() {
        halfTwistX = twistX * .5f;
        halfTwistY = twistY * .5f;
    }
}
