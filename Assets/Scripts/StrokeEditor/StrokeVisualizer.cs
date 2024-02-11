using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class StrokeVisualizer : MonoBehaviour
{
    [SerializeField] private List<Vector2> _points;
    public List<Vector2> Points { get => _points; set => _points = value; }
    private LineRenderer _lineRenderer;

    private void OnEnable(){
        if (TryGetComponent<LineRenderer>(out LineRenderer lineRenderer)){
            _lineRenderer = lineRenderer;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if (_lineRenderer == null) return;
            UpdateLineRenderer(); 
    }

    private void UpdateLineRenderer() {
        int steps = _points.Count;

        _lineRenderer.useWorldSpace = false;
        //lineRenderer.widthCurve = _lineCurve;
        //lineRenderer.material = _lineMaterial;
        _lineRenderer.positionCount = steps;
 
        for(int currentStep=0; currentStep < steps; currentStep++)
        {
            float x = _points[currentStep].x;
            float y = _points[currentStep].y;
            float z = 0f;
 
            Vector3 currentPosition = new Vector3(x,y,z);
 
            _lineRenderer.SetPosition(currentStep,currentPosition);
        }
    }

    // public void RecordGesture() 
    // {   
    //     string name = _nameInput.text;
    //     _recognizer.SavePattern(name, _pointList);

    //     // DEBUG
    //     string debugLog = $"{name} has been recorded as;";
    //     for(int i = 0; i < _pointList.Count; i++){
    //         debugLog += $"\n Point {i}: "  + _pointList[i].ToString();
    //     }
    //     Debug.Log(debugLog);
    // }

    // public void WriteGesture() 
    // {
    //     string name = _nameInput.text;
    //     DollarRecognizer.Unistroke unistroke = _recognizer.SavePattern(name, _pointList);
    //     string gestureName = _nameInput.text;
    //     string fileName = string.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, gestureName, DateTime.Now.ToFileTime());
    //     //GestureIO.WriteGesture(unistroke, gestureName, fileName);
    //     GestureIO.WriteGesture(_pointList.ToArray(), gestureName, fileName);

    //     // DEBUG
    //     string debugLog = $"{gestureName} has been written as;";
    //     for(int i = 0; i < unistroke.Points.Length; i++){
    //         debugLog += $"\n Point {i}: "  + unistroke.Points[i].ToString();
    //     }
    //     Debug.Log(debugLog);
    // }

    // private void ReadGesture() 
    // {
    //     //Load pre-made gestures
	// 	TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("Gestures/");
	// 	foreach (TextAsset gestureXml in gesturesXml)
	// 		_recognizer.AddToLibrary(GestureIO.ReadGestureFromXML(gestureXml.text));
    //         //Add loaded gestures to library.
    // }
}
