using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        //declaration of a list with numbers
        List<int> numbers = new List<int>();

        //adding values to the list
        for (int i = 0; i < 10; i++)
        {
            numbers.Add(i);
        }

        //copying a list in a new list
        List<int> copyNumbers = new List<int>(numbers);

        //declaration and direct initialisation of a list
        List<string> numbersText = new List<string>()
        {
            "Zero",
            "One",
            "Two",
            "Three",
            "For",
            "Four",
            "Five",
            "Six"
        };


        //list functions
        int amountOfElements = numbersText.Count; //the amount of elements in the list
        numbersText.Remove("For");//removes and item from the list. The indexes will shift
        numbersText.RemoveAt(6);//remove an item at a certain index
        numbersText.Add("Six"); //add an element to the list

        List<string> moreNumbersText = new List<string> { "Seven", "Eight" };
        numbersText.AddRange(moreNumbersText);//Add a collection to the list
        moreNumbersText.Clear();//remove all the elements from the list
        //much more functions



        //Accessing elements of a list at a certain index
        int a = 2;
        int b = 1;
        int sum = a + b;
        Debug.Log($"the sum of {numbersText[a]} and {numbersText[b]} is {numbers[sum]}");

        a = 3;
        b = 4;
        sum = a + b;
        Debug.Log($"the sum of {numbersText[a]} and {numbersText[b]} is {numbers[sum]}");

        numbers.Sort();


        for (int i = 0; i < numbersText.Count; i++)
        {
            Debug.Log(numbersText[i]);
        }


        foreach (string number in numbersText)
        {
            Debug.Log(number);
        }



        /*foreach (var number in numbers)
        {
            Debug.Log(numbersText[number]);
        }*/

        WriteFibonacciNumbers(30);

        Dictionary<string, int> agesByName = new Dictionary<string, int>();
        agesByName.Add("Kevin", 29);
        agesByName.Add("Patrick", 35);

        string nameToGet = "Patrick";
        Debug.Log($"{nameToGet} is {agesByName[nameToGet] } years old");

        Dictionary<string, int> numbersByName = new Dictionary<string, int>();
        for (int i = 0; i < numbersText.Count; i++)
        {
            numbersByName.Add(numbersText[i], i);
        }

        Debug.Log($"{numbersByName["Four"]} is written as Four");

        
    }

    // Update is called once per frame
    void Update()
    {

    }

  
/// <summary>
/// Output fibonacci number 
/// </summary>
/// <param name="count">Amount of numbers to be outputted</param>
    private List<int> WriteFibonacciNumbers(int count)
    {
        List<int> fibonacciNumbers = new List<int> { 1, 1 };

        //Add fibonacci numbers to the list untill the list contains the required amount of numbers
        while(fibonacciNumbers.Count<count)
        {
            var previous = fibonacciNumbers[fibonacciNumbers.Count - 1];
            var previous2 = fibonacciNumbers[fibonacciNumbers.Count - 2];

            fibonacciNumbers.Add(previous + previous2);
        }
        /*
        for (int i = 2; i < count; i++)
        {
            var previous = fibonacciNumbers[fibonacciNumbers.Count - 1];
            var previous2 = fibonacciNumbers[fibonacciNumbers.Count - 2];

            fibonacciNumbers.Add(previous + previous2);
        }*/

        //log all the fibonacci numbers in the list
        foreach (var number in fibonacciNumbers)
        {
            Debug.Log($"The next number is {number}");
        }
        return fibonacciNumbers;
    }

    private void OnGUI()
    {

    }
}
