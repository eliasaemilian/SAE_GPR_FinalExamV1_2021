using NUnit.Framework;
using UnityEngine;

public class LootTests
{
    [Test]
    public void EmptyLootDescriptionTest()
    {
        var lootDescription = ScriptableObject.CreateInstance<LootDescription>();

        lootDescription.SetDrops();
        for (int i = 0; i < 100; i++)
        {
            var drop = lootDescription.SelectDropRandomly();
            Assert.AreEqual(null, drop);
        }

        Object.DestroyImmediate(lootDescription);
    }

    [Test]
    public void CertainDropLootDescriptionTest()
    {
        var lootDescription = ScriptableObject.CreateInstance<LootDescription>();
        var testDrop = ScriptableObject.CreateInstance<Drop>();
        testDrop.DropName = "TestDrop";

        lootDescription.SetDrops(new DropProbabilityPair[] {
            new DropProbabilityPair() { Drop = testDrop, Probability = 1 } });

        for (int i = 0; i < 100; i++)
        {
            var drop = lootDescription.SelectDropRandomly();
            Assert.AreEqual(testDrop, drop);
        }

        Object.DestroyImmediate(lootDescription);
        Object.DestroyImmediate(testDrop);
    }

    [Test]
    public void TestDropHalfHalfDistribution()
    {
        var lootTable = ScriptableObject.CreateInstance<LootDescription>();
        var dropOne = ScriptableObject.CreateInstance<Drop>();
        var dropTwo = ScriptableObject.CreateInstance<Drop>();

        var pairOne = new DropProbabilityPair();
        pairOne.Drop = dropOne;
        pairOne.Probability = .5f;

        var pairTwo = new DropProbabilityPair();
        pairTwo.Drop = dropTwo;
        pairTwo.Probability = .5f;

        DropProbabilityPair[] pair = { pairOne, pairTwo };

        lootTable.SetDrops( pair );


        for ( int i = 0; i < 100; i++ )
        {
            Drop drop = lootTable.SelectDropRandomly();
            Assert.IsNotNull( drop );                    
        }
    }

    [Test]
    public void TestThatOrderIsIrrelevant()
    {
        var lootTable = ScriptableObject.CreateInstance<LootDescription>();
        var lootTableSwitched = ScriptableObject.CreateInstance<LootDescription>();
        var dropOne = ScriptableObject.CreateInstance<Drop>();
        var dropTwo = ScriptableObject.CreateInstance<Drop>();
        dropOne.DropName = "One";
        dropTwo.DropName = "Two";

        var pairOne = new DropProbabilityPair();
        pairOne.Drop = dropOne;
        pairOne.Probability = .1f;

        var pairTwo = new DropProbabilityPair();
        pairTwo.Drop = dropTwo;
        pairTwo.Probability = .9f;

        DropProbabilityPair[] pair = { pairOne, pairTwo };
        DropProbabilityPair[] pairSwitched = { pairTwo, pairOne };

        lootTable.SetDrops( pair );
        lootTableSwitched.SetDrops( pairSwitched );
        Drop[] resultOne = new Drop[1000];
        Drop[] resultSwitched = new Drop[1000];

        for ( int i = 0; i < 1000; i++ )
        {
            resultOne[i] = lootTable.SelectDropRandomly();
            resultSwitched[i] = lootTableSwitched.SelectDropRandomly();
        }

        int rOneCount = 0, rSwitchedCount = 0;
        foreach ( Drop drop in resultOne )
        {
            if ( drop.DropName == "One" ) rOneCount++;
        }
        foreach ( Drop drop in resultSwitched )
        {
            if ( drop.DropName == "One" ) rSwitchedCount++;
        }

        Assert.GreaterOrEqual( rOneCount, 80 );
        Assert.GreaterOrEqual( rSwitchedCount, 80 );
        Assert.LessOrEqual( rOneCount, 130 );
        Assert.LessOrEqual( rSwitchedCount, 130 );
    }
}

