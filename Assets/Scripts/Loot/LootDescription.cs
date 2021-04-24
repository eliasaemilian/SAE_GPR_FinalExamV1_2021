using UnityEngine;

[CreateAssetMenu]
public class LootDescription : ScriptableObject
{
    [SerializeField] private DropProbabilityPair[] drops;

    public void SetDrops(params DropProbabilityPair[] drops)
    {
        this.drops = drops;
    }

    public Drop SelectDropRandomly()
    {
        float total = 0;
        // sum up drop probabilities 
        for ( int i = 0; i < drops.Length; i++ )
        {
            DropProbabilityPair pair = drops[i];
            total += pair.Probability;
        }

        float rnd = Random.Range(0, total);
        float curr = 0;

        for (int i = 0; i < drops.Length; i++)
        {
            DropProbabilityPair pair = drops[i];

            if ( rnd < ( curr + pair.Probability ) )
            {
                return pair.Drop;
            }
            else curr += pair.Probability;
        }
        return null;
    }
}

[System.Serializable]
public struct DropProbabilityPair
{
    public Drop Drop;

    [Range(0, 1)]
    public float Probability;
}
