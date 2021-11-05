using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OOPManager : MonoBehaviour
{
    //Collection of dogs, using their names as keys. The use of a dictionary means every name needs to be unique
    private Dictionary<string, Dog> _dogs;
    // Start is called before the first frame update
    void Start()
    {
        Dog myFirstDog = new Dog("Cookie", 2017, Breed.Poodle);

        myFirstDog.Bark(5);


        _dogs = new Dictionary<string,Dog>();

        _dogs.Add("Toby",new Dog("Toby",2004,Breed.Labrador));
        _dogs.Add("Fifi",new Dog("Fifi", 2005, Breed.Labrador));

        _dogs.Add("Ivy",new Dog("Ivy", 2008, Breed.Labrador, _dogs["Toby"], _dogs["Fifi"]));

        _dogs.Add("Lewis", new Dog("Lewis", 2009, Breed.Labrador));

        _dogs["Lewis"].AddMotherFather(_dogs["Toby"], _dogs["Fifi"]);


        _dogs["Ivy"].Bark(5);

        Debug.Log($"Fifi is {_dogs["Fifi"].Age} years old");

        /*
        //Let every dog bark a random amount of times
        foreach (var dog in _dogs.Values)
        {
            dog.Bark(Random.Range(1, 5));
        }*/
    }



    // Update is called once per frame
    void Update()
    {
        //I added these comments
    }
}
