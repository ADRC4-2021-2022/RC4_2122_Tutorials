using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Breed { Chihuahua, Maltese, Pomeranioan, Poodle, Pug, Labrador, GermanShepherd, GoldenRetriever }
public class Dog
{
    #region public fields
    //These fields or member variables are accessible in the entire namespace
    //Getters and Setters are used to run bits of code when getting or setting a variable
    // {get; set;} is the standard notation and can be left out
    public string Name { get; set; }

    //this fields doesn't have a setter. The age can never be set, only be retrieved. This field will does not store any information
    public int Age
    {
        get { return GetAge(); }
    }

    #endregion

    #region private fields
    //These fields or member variables are only accessible within the scope of the class
    private Breed _dogBreed;
    private Dog _mother;
    private Dog _father;
    private int _birthYear;

    #endregion

    #region constructors

    //Constructors are bits of code that initialise a new object of this class
    //Constructors need to be public
    public Dog(string name, int birthyear, Breed breed)
    {
        Name = name;
        _birthYear = birthyear;
        _dogBreed = breed;
    }

    public Dog(string name, int birthyear, Breed breed, Dog mother, Dog father)
    {
        Name = name;

        _birthYear = birthyear;
        _dogBreed = breed;
        _mother = mother;
        _father = father;
    }

    #endregion

    #region public functions
    //These functions are accessible in the entire namespace
    public void Bark(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Debug.Log($"{Name}: Woef");
        }
        //Whenever a dog barks, the parents are concerned and also bark
        if (_mother != null)
        {
            _mother.Bark(1);
        }
        if (_father != null)
        {
            _father.Bark(1);
        }
    }

    public void AddMotherFather(Dog mother, Dog father)
    {
        _mother = mother;
        _father = father;
    }

    #endregion

    #region private functions
    //These functions are only accessible in the scope of the class
    private int GetAge()
    {
        int currentYear = System.DateTime.Now.Year;
        return currentYear - _birthYear;
    }

    #endregion


}
