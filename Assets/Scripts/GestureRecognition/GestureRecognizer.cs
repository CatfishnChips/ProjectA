using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using MagicaCloth2;

public class GestureRecognizer : MonoBehaviour
{
    MouseInputListener mouseInput;
    DollarRecognizer recognizer;

    public List<Vector2> pointsList;
    public float timeLimit = 3.0f;

    public TMP_InputField drawingName;

    public static Dictionary<string, string> debugDict = new Dictionary<string, string>();

    void Awake()
    {
        mouseInput = transform.GetComponent<MouseInputListener>();
    }

    void Start()
    {
        string jsonStr = JsonConvert.SerializeObject(debugDict);
        File.WriteAllText(Application.persistentDataPath + "/points.json", jsonStr);

        mouseInput.StartedDrawing += OnDrawingStart;
        mouseInput.CurrentlyDrawing += WhileDrawing;
        mouseInput.FinishedDrawing += OnDrawingEnd;

        pointsList = new List<Vector2>();
        recognizer = new DollarRecognizer();
    }

    void OnDisable()
    {
        mouseInput.StartedDrawing -= OnDrawingStart;
        mouseInput.CurrentlyDrawing -= WhileDrawing;
        mouseInput.FinishedDrawing -= OnDrawingEnd;
    }

    void OnDrawingStart()
    {
        pointsList.Clear();
    }

    void WhileDrawing()
    {
        pointsList.Add(mouseInput.mouseWorldPos);
    }

    void OnDrawingEnd()
    {
        foreach (Vector2 point in pointsList)
        {
            Debug.Log($"({point.x}, {point.y}), ");
        }
        DollarRecognizer.Result result = recognizer.Recognize(pointsList);
        string jsonString = JsonConvert.SerializeObject(debugDict, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "\\GestureData.json", jsonString);
        Debug.Log("Drawing is: " + result.Match + "Score: " + result.Score);
    }

    public static string Vector2ListToString(IEnumerable<Vector2> vector2s)
    {
        string result = "";
        foreach (Vector2 point in vector2s)
        {
            result += $"({point.x}, {point.y}), ";
        }
        return result;
    }

    public void WriteGesture()
    {
        if(drawingName.text == "" || pointsList.Count == 0) return;
        string name = drawingName.text;
        recognizer.SavePattern(name, pointsList);

        string debugLog = $"{name} has been recorded as;";
        for(int i = 0; i < pointsList.Count; i++){
            debugLog += $"\n Point {i}: {pointsList[i]}";
        }
        Debug.Log(debugLog);
    }

}
