using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


/// <summary>
/// Singleton class to manage block patterns in the project
/// </summary>
public class PatternManager
{
    //The pattern manager is a singleton class. This means there is only one instance of the PatternManager class in the entire project and it can be refered to anywhere withing the project

    /// <summary>
    /// Singleton object of the PatternManager class. Refer to this to access the data inside the object.
    /// </summary>
    public static PatternManager Instance { get; } = new PatternManager();

    private static List<Pattern> _patterns;
    public static Dictionary<string, Pattern> _patternsByName;

    /// <summary>
    /// returns a read only list of the patterns defined in the project
    /// </summary>
    public static ReadOnlyCollection<Pattern> Patterns => new ReadOnlyCollection<Pattern>(_patterns);


    /// <summary>
    /// returns a read only dictionary of the patterns defined in the project organised by name
    /// </summary>
    public static ReadOnlyDictionary<string, Pattern> PatternsByName => new ReadOnlyDictionary<string, Pattern>(_patternsByName);

    /// <summary>
    /// private constructor. All initial patterns will be defined in here
    /// </summary>
    private PatternManager()
    {
        _patterns = new List<Pattern>();
        _patternsByName = new Dictionary<string, Pattern>();

        //Define pattern A
        AddPattern(
            new List<Vector3Int>()
                {
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(1, 0, 0),
                    new Vector3Int(2, 0, 0),
                    new Vector3Int(3, 0, 0),
                    new Vector3Int(0, 1, 0),
                    new Vector3Int(0, 2, 0),
                    new Vector3Int(0, 3, 0)
                },
                "pattern A"
                );

        //Define pattern B
        AddPattern(
            new List<Vector3Int>()
                {
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(0, 1, 0),
                    new Vector3Int(0, 2, 0),
                    new Vector3Int(0, 3, 0),
                    new Vector3Int(0, 4, 0)
                },
                "pattern B"
                );

    }
    /// <summary>
    /// Use this method rather than adding directly to the _patterns field. This method will check if the pattern is valid and can be added to the list. Invalid input will be refused.
    /// </summary>
    /// <param name="indices">List of indices that define the patter. The indices should always relate to Vector3In(0,0,0) as anchor point</param>
    /// <param name="type">The PatternType of this pattern to add. Each type can only exist once</param>
    /// <returns></returns>
    public bool AddPattern(List<Vector3Int> indices, string name)
    {

        //only add valid patterns
        if (indices == null) return false;
        if (indices[0] != Vector3Int.zero) return false;
        if (_patterns.Count(p => p.Name == name) > 0) return false;
        _patterns.Add(new Pattern(new List<Vector3Int>(indices), _patterns.Count, name));
        _patternsByName.Add(name,_patterns.Last());
        return true;
    }

    /// <summary>
    /// Return the pattern linked to its index
    /// </summary>
    /// <param name="index">The index to look for</param>
    /// <returns>The pattern linked to the type. Will return null if the type is never defined</returns>
    public static Pattern GetPatternByIndex(int index) => Patterns[(int)index];

    public static Pattern GetPatternByName(string name) => PatternsByName[name];
}
/// <summary>
/// The pattern that defines a block. Object of this class should only be made in the PatternManager
/// </summary>
public class Pattern
{
    /// <summary>
    /// The patterns are saved as ReadOnlyCollections rather than list so that once defined, the pattern can never be changed
    /// </summary>
    public ReadOnlyCollection<Vector3Int> Indices { get; }
    public int Index { get; }

    public string Name { get; }

    /// <summary>
    /// Pattern constructor. The indices will be stored in a ReadOnlyCollection
    /// </summary>
    ///<param name = "indices" > List of indices that define the patter.The indices should always relate to Vector3In(0,0,0) as anchor point</param>
    /// <param name="type">The PatternType of this pattern to add. Each type can only exist once</param>
    public Pattern(List<Vector3Int> indices, int index, string name)
    {
        Indices = new ReadOnlyCollection<Vector3Int>(indices);
        Index = index;
        Name = name;
    }
}

