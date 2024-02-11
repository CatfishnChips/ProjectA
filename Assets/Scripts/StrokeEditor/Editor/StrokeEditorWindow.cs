using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.IO;

public class StrokeEditorWindow : EditorWindow
{
    private readonly string defaultFileName = "StrokeFileName";
    private static TextField _fileNameTextField;
    private static Button _saveButton;
    private static Button _loadButton;
    private static VisualElement _overlayVisualElement;

    private static ListView _listView;
    private static TextField _strokeNameTextField;
    private DollarRecognizer _recognizer = new DollarRecognizer();

    private List<Vector2> _pointList = new List<Vector2>();
    private LineRenderer _lineRenderer;
    private bool _showLineRenderer = true;
    private Material _lineRendererMaterial;
    private AnimationCurve _lineRendererCurve = new AnimationCurve();

    private VisualElement CreateLineRendererControls(){
        VisualElement visualElement = new VisualElement();

        visualElement.AddClasses(
            "blackboard__custom-data-container"
        );

        Toggle lineRendererToggle = ElementUtility.CreateToggle(_showLineRenderer, "Show", callback => {
            _showLineRenderer = callback.newValue;
            _lineRenderer.enabled = _showLineRenderer;
        });

        CurveField curveField = ElementUtility.CreateCurveField(_lineRendererCurve, "Curve", callback => {
            _lineRendererCurve = callback.newValue;
            _lineRenderer.widthCurve = _lineRendererCurve;
        });

        ObjectField materialField = ElementUtility.CreateObjectField(typeof(Material), _lineRendererMaterial, "Material", callback => {
            _lineRendererMaterial = callback.newValue as Material;
            _lineRenderer.material = _lineRendererMaterial;
        });

        visualElement.Add(lineRendererToggle);
        visualElement.Add(curveField);
        visualElement.Add(materialField);

        return visualElement;
    }

    private GameObject CreateLineRenderer(){
        GameObject obj = new GameObject();
        _lineRenderer = obj.AddComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;

        // Curve
        for (int i = 0; i < _lineRendererCurve.keys.Length; i++){
            _lineRendererCurve.RemoveKey(i);
        }
        _lineRendererCurve.AddKey(0f, 0.1f);
        _lineRendererCurve.AddKey(1f, 0.1f);
        _lineRenderer.widthCurve = _lineRendererCurve;

        // Material
        var shader = Shader.Find("Hidden/Internal-Colored");
        _lineRendererMaterial = new Material(shader);
        _lineRendererMaterial.hideFlags = HideFlags.HideAndDontSave;
        _lineRenderer.material = _lineRendererMaterial;

        return obj;
    }

    private void UpdateLineRenderer() {
        int steps = _pointList.Count;
        _lineRenderer.positionCount = steps;
 
        for(int currentStep=0; currentStep < steps; currentStep++)
        {
            float x = _pointList[currentStep].x;
            float y = _pointList[currentStep].y;
            float z = 0f;
 
            Vector3 currentPosition = new Vector3(x,y,z);
 
            _lineRenderer.SetPosition(currentStep,currentPosition);
        }
    }

    void Update()
    {   
        if (_lineRenderer == null) return;
            UpdateLineRenderer(); 
    }

    public EditorWindow editorWindow;
    private Color _backgroundColor = new Color(32f / 256f, 32f / 256f, 32f / 256f, 1f);
    private bool _showMainGrid = true;
    private bool _showSubGrid = true;

    // Preview Scene
    private Camera _sceneCamera;
    private GameObject _sceneRoot;
    private PreviewRenderUtility _previewRenderUtility;
    private Rect _screenRect;
    private Rect _previewRect;
    private Rect _handlesRect;

    private GridOverlay _gridOverlay;
    private Color _mainGridColor = new Color(123f / 256f, 122f / 256f, 121f / 256f, 1f);
    private Color _subGridColor = new Color(108f / 256f, 103f / 256f, 100f / 256f, 1f);
    private float _largeStep = 1f;
    private float _smallStep = 0.25f;

    #region Mouse Variables

    private Vector2 _mousePosition;
    private Vector3 _worldMousePosition;
    private Vector3 _origin;
    private Vector3 _delta;
    private Vector3 _reset;

    #endregion

    private static Toolbar _toolbar;

    // Add menu item named "Stroke Editor" to the Window menu.
    [MenuItem("Window/Stroke Editor")]
    public static void CreateWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        var window = GetWindow<StrokeEditorWindow>("Stroke Editor");
        window.Init(window);
        window.Show();
    }

    public void Init(EditorWindow window){
        editorWindow = window;
        editorWindow.wantsMouseMove = true;
    }

    private void CloseWindow(){
        if (_previewRenderUtility != null)
			_previewRenderUtility.Cleanup();

		if (_sceneRoot != null)
			UnityEngine.Object.DestroyImmediate(_sceneRoot);
    }

    private void CreateGridOverlay(){
        _gridOverlay = new GridOverlay(){
           gridSize = new Vector3(10, 10, 10),
            largeStep = _largeStep,
            smallStep = _smallStep,
            mainColor = _mainGridColor,
            subColor = _subGridColor,
            showMain = _showMainGrid,
            showSub = _showSubGrid
        };
    }

    private Vector3 SnapToGrid(Vector3 position) {
        position.x = Mathf.Round(position.x * _largeStep) * _smallStep;
        position.y = Mathf.Round(position.y * _largeStep) * _smallStep;
        position.z = Mathf.Round(position.z * _largeStep) * _smallStep;
        return position; 
    } 

    private void SetupPreviewScene(){
        // Camera
        _sceneCamera = _previewRenderUtility.camera;
        _sceneCamera.cameraType = CameraType.SceneView;
        _sceneCamera.fieldOfView = 30f;
        _sceneCamera.nearClipPlane = 0.3f;
        _sceneCamera.farClipPlane = 1000;

        _sceneCamera.orthographic = true;
        _sceneCamera.orthographicSize = 3.5f;

        _sceneCamera.transform.position = new Vector3(0f, 0f, -10f);
        _sceneCamera.transform.LookAt(Vector3.zero);

        _sceneCamera.backgroundColor = _backgroundColor;
        _reset = _sceneCamera.transform.position;

        // Lights
        _previewRenderUtility.lights[0].transform.localEulerAngles = new Vector3(30, -30, 0);
        _previewRenderUtility.lights[0].intensity = 2;

        // Ambient Color
        _previewRenderUtility.ambientColor = Color.white;
        
        // Root Object
        _sceneRoot = new GameObject("SceneRoot");
        _sceneRoot.transform.position = Vector3.zero;
        _sceneRoot.hideFlags = HideFlags.HideAndDontSave;
        _previewRenderUtility.AddSingleGO(_sceneRoot);

        // Line Renderer
        GameObject obj = CreateLineRenderer();
        obj.transform.SetParent(_sceneRoot.transform);
    }

    private void HandleCameraMovement(){
        if (_sceneCamera == null) return;

        // This solves the "Screen Position out of View Frustum" error.
        if (_mousePosition.x < 0 || _mousePosition.x >= base.position.width || _mousePosition.y < 0 || _mousePosition.y >= base.position.height)
            return;

        _worldMousePosition = _sceneCamera.ScreenToWorldPoint(new Vector3(_mousePosition.x, -_mousePosition.y, 10f));

        if (Event.current.type == EventType.MouseDown){
            // if (Physics.Raycast(Event.current.mousePosition,))
            _origin = _worldMousePosition;
        }

        if (Event.current.type == EventType.MouseDrag){
            _delta = _worldMousePosition - _sceneCamera.transform.position;
            _sceneCamera.transform.position = _origin - _delta;
        }
    }

    private void HandleCameraZoom(){
        if (_sceneCamera == null) return;

        if (Event.current.type == EventType.ScrollWheel){
            float orthographicSize = _sceneCamera.orthographicSize;
            orthographicSize += Event.current.delta.y / 4;
            orthographicSize = Mathf.Clamp(orthographicSize, 1f, 10f);
            _sceneCamera.orthographicSize = orthographicSize;
            Repaint();
        }
    }

    private void AddCameraSettings(){
        VisualElement cameraSettingsVisualElement = new VisualElement();

        Foldout cameraSettingsFoldout = ElementUtility.CreateFoldout("Camera Settings", false);

        cameraSettingsFoldout.AddClasses(
            "blackboard__custom-data-container"
        );

        ColorField backgroundColorField = ElementUtility.CreateColorField(_backgroundColor, "Background Color", false, false, false, callback => {
            _backgroundColor = callback.newValue;
            
            // Change the background color of the _sceneCamera.
            if (_sceneCamera != null)
                _sceneCamera.backgroundColor = _backgroundColor;
        });

        backgroundColorField.AddClasses(
            "blackboard__color-field"
        );

        VisualElement gridControlPanel = new VisualElement();

        gridControlPanel.AddClasses(
            "blackboard__toggle-custom-data-container"
        );

        Label gridLabel = ElementUtility.CreateLabel("Show Grid");

        Toggle showMainGridToggle = ElementUtility.CreateToggle(_showMainGrid, "Main", callback => {
            _showMainGrid = callback.newValue;

            // Show/Hide Grid
            if (_gridOverlay != null)
                _gridOverlay.showMain = _showMainGrid;
        });

        showMainGridToggle.AddClasses(
            "blackboard__toggle"
        );

        Toggle showSubGridToggle = ElementUtility.CreateToggle(_showSubGrid, "Sub", callback => {
            _showSubGrid = callback.newValue;

            // Show/Hide Grid
            if (_gridOverlay != null)
                _gridOverlay.showSub = _showSubGrid;
        });

        showSubGridToggle.AddClasses(
            "blackboard__toggle"
        );

        // cameraSettingsFoldout.Add(cameraRadioButtonGroup);
        // cameraSettingsFoldout.Add(backgroundColorField);
        // cameraSettingsVisualElement.Add(cameraSettingsFoldout);
        // toolbar.Add(cameraSettingsVisualElement);
        gridControlPanel.Add(gridLabel);
        gridControlPanel.Add(showMainGridToggle);
        gridControlPanel.Add(showSubGridToggle);

        _toolbar.Add(gridControlPanel);

        _toolbar.Add(backgroundColorField);
    }

    private void AddToolbar()
    {
        _toolbar = new Toolbar();

        _fileNameTextField = ElementUtility.CreateTextField(defaultFileName, "File Name:", callback => {
            _fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
        });
        
        _saveButton = ElementUtility.CreateButton("Save", () => Save());
        _loadButton = ElementUtility.CreateButton("Load", () => Load());
        Button clearButton = ElementUtility.CreateButton("Clear", () => Clear());
        Button resetButton = ElementUtility.CreateButton("Reset", () => ResetGraph());

        _toolbar.Add(_fileNameTextField);
        _toolbar.Add(_saveButton);
        _toolbar.Add(_loadButton);
        _toolbar.Add(clearButton);
        _toolbar.Add(resetButton);

        // showGizmosToggle = ElementUtility.CreateToggle(_showGizmos, "Show Gizmos", callback => {
        //     _showGizmos = callback.newValue;
        // });
        // showGizmosToggle.SetEnabled(false);

        // lockSelectionToggle = ElementUtility.CreateToggle(_lockSelection, "Lock Selection", callback => {
        //     _lockSelection = callback.newValue;
        //     LockSelection();
        // });

        // selectButton = ElementUtility.CreateButton("Select", () => {
        //    SelectAnimator();
        //    SelectClip();
        // });

        _toolbar.AddStyleSheets(
            "Animation Preview/AnimationPreviewToolbarStyles.uss"
        );

        rootVisualElement.Add(_toolbar);
    }

    private void AddStyles()
    {
        rootVisualElement.AddStyleSheets(
            "Animation Preview/AnimationPreviewVariables.uss",
            "Animation Preview/AnimationPreviewStyleSheet.uss"
        );
    }

    private void Save()
    {
        if (string.IsNullOrEmpty(_fileNameTextField.value))
        {
            EditorUtility.DisplayDialog("Invalid file name.", "Please ensure the file name you've typed in is valid.", "Roger!");

            return;
        }
        
        // BlackboardIOUtility.Initialize(fileNameTextField.value, this);
        // BlackboardIOUtility.Save();
    }

    private void Load()
    {
        string filePath = EditorUtility.OpenFilePanel("Resources", "Assets/Resources/Gestures", "xml");

        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        Clear();

        //Load pre-made gestures
        TextAsset strokeXml = LoadAsset<TextAsset>($"Assets/Resources/Gestures", Path.GetFileNameWithoutExtension(filePath));

        // if (blackboardData == null)
        // {
        //     EditorUtility.DisplayDialog(
        //         "Could not find the file!",
        //         "The file at the following path could not be found:\n\n" +
        //         $"\"Assets/Blackboards/{blackboardFileName}\".\n\n" +
        //         "Make sure you chose the right file and it's placed at the folder path mentioned above.",
        //         "Thanks!"
        //     );

        //     return;
        // }

		//TextAsset gestureXml = Resources.Load<TextAsset>("Gestures/" + filePath);
		
        DollarRecognizer.Unistroke stroke = GestureIO.ReadGestureFromXML(strokeXml.text);

        //Add loaded gestures to library.
        _recognizer.AddToLibrary(stroke);

        // Set _pointList
        _pointList.Clear();
        _pointList.AddRange(stroke.Points);

        _listView.Rebuild();
            
        // IOUtility.Initialize(Path.GetFileNameWithoutExtension(filePath), this);
        // IOUtility.Load();
    }

    public static T LoadAsset<T>(string path, string assetName) where T : UnityEngine.Object
    {
        string fullPath = $"{path}/{assetName}.xml";

        return AssetDatabase.LoadAssetAtPath<T>(fullPath);
    }

    public static void SaveAsset(UnityEngine.Object asset)
    {
        EditorUtility.SetDirty(asset);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void RemoveAsset(string path, string assetName)
    {
        AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
    }

    private void Clear()
    {
        if (_sceneCamera != null)
            _sceneCamera.transform.position = _reset;

        _pointList.Clear();

        // EnableSaving();
    }

    private void ResetGraph()
    {
        Clear();
        UpdateFileName(defaultFileName);
    }

    public static void UpdateFileName(string newFileName)
    {
        _fileNameTextField.value = newFileName;
    }

    public void EnableSaving()
    {
        _saveButton.SetEnabled(true);
    }

    public void DisableSaving()
    {
        _saveButton.SetEnabled(false);
    }

    private void OnGUI(){
        if (_previewRenderUtility == null) return;

        // Render the preview scene and stick it
		// onto the current editor window. It'll behave like a custom game view.
		Rect rect = new Rect(0, 0, base.position.width, base.position.height);

        //_previewRect = EditorGUILayout.GetControlRect(false, 256, GUIStyle.none);
        _previewRect = new Rect(0, 0, base.position.width, base.position.height);
 
        // Store preview rect area for Layout pass
        if (Event.current.type == EventType.Repaint)
        {
            _handlesRect = _previewRect;
            _screenRect = EditorGUIUtility.GUIToScreenRect(_previewRect);
        }

        GUI.backgroundColor = Color.white;
        GUI.color = Color.white;
        GUI.contentColor = Color.white;

        // Draw preview only in Repaint
        if (Event.current.type == EventType.Repaint)
            _previewRenderUtility.BeginPreview(_previewRect, GUI.skin.box);
        else
        {
            // Required to make handles work properly
            _previewRenderUtility.camera.pixelRect = new Rect(0, 0, _handlesRect.width, _handlesRect.height);
            _previewRenderUtility.camera.aspect = _handlesRect.width / _handlesRect.height;

        }

        // Draw preview only in Repaint
        if (Event.current.type == EventType.Repaint)
        {
            _previewRenderUtility.Render(true, false);
        }
    
        using (new Handles.DrawingScope(Matrix4x4.identity))
        {
            Handles.SetCamera(_previewRenderUtility.camera);
            Handles.DrawGizmos(_previewRenderUtility.camera);
            Handles.DrawCamera(_previewRect, _previewRenderUtility.camera, DrawCameraMode.Textured);

            // Custom handles function.
            OnDrawHandles();

            // Show Gizmos for the boxes. Matrix is set to the _meshAnimator's transform matrix to follow the root motion.
            // if (_showGizmos)
            //     foreach(Boxes box in _boxes){
            //         box.DrawHandles(Matrix4x4.TRS(_meshAnimator.transform.position, _meshAnimator.transform.rotation * Quaternion.Euler(0f, -90f, 0f), _meshAnimator.transform.localScale));
            //     }
        }   

        if (_gridOverlay != null){
            _gridOverlay.startPosition = new Vector3(-5, -5, 0);
            _gridOverlay.OnPostRender();
        }

        // Draw preview only in Repaint
        if (Event.current.type == EventType.Repaint)
            _previewRenderUtility.EndAndDrawPreview(_previewRect);

        switch (Event.current.type){
            case EventType.MouseDown:
                EditorGUIUtility.AddCursorRect(new Rect(SceneView.lastActiveSceneView.position), MouseCursor.Pan);
                //Selection.activeGameObject = HandleUtility.PickGameObject(Event.current.mousePosition, false);
            break;

            case EventType.MouseUp:
                EditorGUIUtility.AddCursorRect(new Rect(SceneView.lastActiveSceneView.position), MouseCursor.Arrow);
            break;

            case EventType.MouseMove: case EventType.MouseDrag:
                EditorGUIUtility.AddCursorRect(new Rect(SceneView.lastActiveSceneView.position), MouseCursor.Pan);
                _mousePosition = Event.current.mousePosition;
                Repaint();
            break;
        }

        HandleCameraMovement();  
        HandleCameraZoom(); 

        if (Application.isPlaying) return;

        Handles.color = Color.green;
        
       for (int i = 0; i < _pointList.Count; i++) {
            Vector2 point = _pointList[i];
            DrawPoint(point, "Point " + i, i);
        }
    }

    private void OnDrawHandles(){
        // if (Selection.activeTransform == null) return;

        // Vector3 targetPosition = Handles.PositionHandle(Selection.activeTransform.position, Selection.activeTransform.rotation);
        // Selection.activeTransform.position = targetPosition;

    }

    private readonly GUIStyle style = new GUIStyle();

    private void DrawPoint(Vector3 point, string name, int index){
        Handles.Label(point, name, style);
        EditorGUI.BeginChangeCheck();
        Vector2 position = Handles.FreeMoveHandle(point, Quaternion.identity, 0.1f, Vector3.one, Handles.SphereHandleCap);
        if (EditorGUI.EndChangeCheck()) {
            _pointList[index] = position;
        }
    }

    private void AddListView(){
        // The "makeItem" function is called when the
        // ListView needs more items to render.
        Func<VisualElement> makeItem = () => ElementUtility.CreateVector3Field();

        // As the user scrolls through the list, the ListView object
        // recycles elements created by the "makeItem" function,
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list).
        Action<VisualElement, int> bindItem = (e, i) => {
            Vector3Field field = e as Vector3Field;
            field.value = _pointList[i];
            field.RegisterValueChangedCallback((callback) => {
                _pointList[i] = callback.newValue;
            });
        };

        // Provide the list view with an explicit height for every row
        // so it can calculate how many items to actually display
        const int itemHeight = 16;

        _listView = new ListView(_pointList, itemHeight, makeItem, bindItem)
        {
            // Enables multiple selection using shift or ctrl/cmd keys.
            selectionType = SelectionType.Multiple,
            reorderable = true,
        };

        // Set up list view so that you can add or remove items dynamically.
        _listView.showAddRemoveFooter = false;

        // Implement functionality on the list view to add or remove items.
        _listView.itemsAdded += view =>
        {
            var itemsSourceCount = _listView.itemsSource.Count;
            _listView.itemsSource.Add(itemsSourceCount.ToString());
            _listView.RefreshItems();
            _listView.ScrollToItem(itemsSourceCount);
        };
        _listView.itemsRemoved += view =>
        {
            var itemsSourceCount = _listView.itemsSource.Count;
            _listView.itemsSource.RemoveAt(itemsSourceCount - 1);
            _listView.RefreshItems();
            _listView.ScrollToItem(itemsSourceCount - 2);
        };

        // Single click triggers "selectionChanged" with the selected items. (f.k.a. "onSelectionChange")
        // Use "selectedIndicesChanged" to get the indices of the selected items instead. (f.k.a. "onSelectedIndicesChange")
        _listView.onSelectionChange += objects => Debug.Log($"Selected: {string.Join(", ", objects)}");

        // Double-click triggers "itemsChosen" with the selected items. (f.k.a. "onItemsChosen")
        _listView.onItemsChosen += objects => Debug.Log($"Double-clicked: {string.Join(", ", objects)}");

        _listView.style.flexGrow = 1.0f;

        _overlayVisualElement.Add(_listView);
    }

    private void OnEnable(){
        // Set Style
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        AddToolbar();
        AddCameraSettings();

        // Create _overlayVisualElement for floating controls.
        _overlayVisualElement = new VisualElement();
        _overlayVisualElement.AddClasses(
            "blackboard__overlay-custom-data-container"
        );
        rootVisualElement.Add(_overlayVisualElement);

        AddListView();

        AddStyles();
        CreateGridOverlay();

        if (_previewRenderUtility != null)
            _previewRenderUtility.Cleanup();

        _previewRenderUtility = new PreviewRenderUtility(true);
        System.GC.SuppressFinalize(_previewRenderUtility);

        SetupPreviewScene();

        VisualElement lineRendererControls = CreateLineRendererControls();
        _overlayVisualElement.Add(lineRendererControls);
    }

    private void OnDisable(){
        CloseWindow();
    }

    public void RecordGesture() 
    {   
        string name = _strokeNameTextField.value;
        _recognizer.SavePattern(name, _pointList);

        // DEBUG
        string debugLog = $"{name} has been recorded as;";
        for(int i = 0; i < _pointList.Count; i++){
            debugLog += $"\n Point {i}: "  + _pointList[i].ToString();
        }
        Debug.Log(debugLog);
    }

    public void WriteGesture() 
    {
        string name = _strokeNameTextField.value;
        DollarRecognizer.Unistroke unistroke = _recognizer.SavePattern(name, _pointList);

        string fileName = string.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, name, DateTime.Now.ToFileTime());
        //GestureIO.WriteGesture(unistroke, gestureName, fileName);
        GestureIO.WriteGesture(_pointList.ToArray(), name, fileName);

        // DEBUG
        string debugLog = $"{name} has been written as;";
        for(int i = 0; i < unistroke.Points.Length; i++){
            debugLog += $"\n Point {i}: "  + unistroke.Points[i].ToString();
        }
        Debug.Log(debugLog);
    }

    private void ReadGesture() 
    {
        //Load pre-made gestures
		TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("Gestures/");
		foreach (TextAsset gestureXml in gesturesXml)
            //Add loaded gestures to library.
			_recognizer.AddToLibrary(GestureIO.ReadGestureFromXML(gestureXml.text));
    }
}
