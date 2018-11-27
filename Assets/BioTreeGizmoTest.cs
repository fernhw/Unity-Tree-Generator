
using System.Collections.Generic;
using UnityEngine;

public class BioTreeGizmoTest : MonoBehaviour {
    [SerializeField]
    BranchOptions options;

    System.Random rootRandom;

    const float
    CILINDER_SEGMENT_Q = 4,
    CILINDER_SEGMENT_Q_PERCENTAGE = 1/4;

    bool createdBranchTemp = false;

    void OnDrawGizmos() {
        options.initBranch();
        rootRandom = new System.Random(options.randomSeed); //OG seed

        BranchRung baseRung = new BranchRung {
            pos = transform.position,
            rot = options.growthDirection // x,y
        };

        BranchGizmoTest tree = new BranchGizmoTest(baseRung, options);

    }
}


class BranchGizmoTest{
    System.Random randomGen;
    System.Random branchShape;
    System.Random branchGenerator;
    System.Random fullUtilityRandom;


    // Branches that stem from it
    public List<BranchGizmoTest> branches = new List<BranchGizmoTest>();

    public List<BranchRung> core = new List<BranchRung>();

    // Growth, branches shouldnt have branch limits.
    int randomSeed;

    //
    public BranchGizmoTest (BranchRung baseRung, BranchOptions seed){
        randomGen = new System.Random(seed.randomSeed);
        int branchSeed = randomGen.Next();
       
        int rungNum = Mathf.CeilToInt(seed.growth);
        float rungSize = seed.rungSize + (seed.growth * BranchOptions.RUNG_VS_GROWTH_SIZE);
        float rungNumFloatDifference = seed.growth - (float)rungNum;
        float treeScale = 1 / (rungNum * BranchOptions.RUNG_CLOSENESS);

        List<BranchRung> rungs = new List<BranchRung>();

        branchShape = new System.Random(branchSeed);
        branchGenerator = new System.Random(branchSeed);
        rungs.Add(baseRung);

        for (int i = 1; i < rungNum; i++) {
            BranchRung prevRung = rungs[i - 1];
            BranchRung rung = new BranchRung();

            // Twisting the branch randomly
            var rndX = ((float)branchShape.NextDouble());
            var rndY = ((float)branchShape.NextDouble());
            rung.rot.x = prevRung.rot.x + (seed.halfTwistX - seed.twistX * rndX);
            rung.rot.y = prevRung.rot.y + (seed.halfTwistY - seed.twistY * rndY);

            // Straightening up the branch, otherwise it will look like sea weed.
            if (seed.correctiveBehavior != 0) {
                rung.rot.x += (baseRung.rot.x - rung.rot.x) * seed.correctiveBehavior;
                rung.rot.y += (baseRung.rot.y - rung.rot.y) * seed.correctiveBehavior;
            }

            // Creating new branch rung position
            var cartX = (Mathf.Cos(prevRung.rot.y) * Mathf.Sin(prevRung.rot.x));
            var cartY = (Mathf.Cos(prevRung.rot.y) * Mathf.Cos(prevRung.rot.x));
            var cartZ = Mathf.Sin(prevRung.rot.y);
            rungSize += (BranchOptions.MIN_RUNG_SIZE - rungSize) * treeScale * BranchOptions.RUNG_CLOSENESS;
            rung.pos.x = prevRung.pos.x + rungSize * cartX;
            rung.pos.y = prevRung.pos.y + rungSize * cartY;
            rung.pos.z = prevRung.pos.z + rungSize * cartZ;

            //Temp, TODO: use fibonacci
            if(Mathf.RoundToInt(((float)branchGenerator.NextDouble())*seed.branchHappening) == 0){
                
                Gizmos.color = new Vector4((float)randomGen.NextDouble(),
                                           (float)randomGen.NextDouble(),
                                           (float)randomGen.NextDouble(),1);
                
                int newBranchSeed = randomGen.Next();
                int leftOverGrowth = rungNum - i;
                fullUtilityRandom = new System.Random(newBranchSeed);
                BranchRung newRung = new BranchRung {
                    pos = rung.pos,
                    rot = rung.rot
                };

                // Randomizing branch direction from mother branch
                Vector2 newDirection = newRung.rot; 
                newRung.rot.x += 1f - (2f) * ((float)fullUtilityRandom.NextDouble());
                newRung.rot.y += 1f - (2f) * ((float)fullUtilityRandom.NextDouble());

                BranchOptions newBranchOpts = new BranchOptions {
                    twistX = seed.twistX,
                    twistY = seed.twistY,
                    correctiveBehavior = seed.correctiveBehavior,
                    randomSeed = newBranchSeed,
                    branchHappening = seed.branchHappening,
                    growth = (leftOverGrowth + rungNumFloatDifference)*(1-.61802f),
                    growthDirection = newDirection
                };


                newBranchOpts.rungSize = rungSize;
                newBranchOpts.initBranch();
                BranchGizmoTest newBranch = new BranchGizmoTest(newRung,newBranchOpts);
                branches.Add(newBranch);
            }
          
            //rung.pos.z = prevRung.pos.x + sizePerc * Mathf.Cos(prevRung.rot.x);
            rungs.Add(rung);
        }


       // Gizmos.color = Color.white;

        // brute force
        for (int i = 1; i < rungs.Count; i++) {
            BranchRung prevRung = rungs[i - 1];
            BranchRung rung = rungs[i];
            Gizmos.DrawLine(prevRung.pos, rung.pos);
        }
    }


}