﻿using UnityEngine;
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