using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using System.Data;
using System.Linq;
using UnityEngine.Animations;
using UnityEditor.Animations;
using UnityEngine.Rendering;

public class AnimationPreviewEditorWindow : EditorWindow
{
    public EditorWindow _editorWindow;

    public GameObject _asset;
    public Editor _assetEditor;
    public AnimationClip _meshAnimation;
    public AnimationClip _boxAnimation;

    public AnimationWindow _animationWindow;

    private int _frameCount;

    public AnimationClip MeshAnimation {get => _meshAnimation; set {
        _meshAnimation = value;
        _frameCount = Mathf.FloorToInt(_meshAnimation.length * _meshAnimation.frameRate);
        sliderInt.highValue = _frameCount;

        // Reset values.
        _frame = 0;
        frameIntegerField.SetValueWithoutNotify(0);
        sliderInt.SetValueWithoutNotify(0);
    }}

    public AnimationClip BoxAnimation {get => _boxAnimation; set {
        _boxAnimation = value;
        _frameCount = Mathf.FloorToInt(_boxAnimation.length * _boxAnimation.frameRate);
        sliderInt.highValue = _frameCount;

        // Reset values.
        _frame = 0;
        frameIntegerField.SetValueWithoutNotify(0);
        sliderInt.SetValueWithoutNotify(0);
    }}

    private Animator _meshAnimator;
    private Animator _boxAnimator;
    private AnimatorController _animatorController;
    private List<AnimatorState> _stateList;
    private AnimatorState _state;

    private int _frame = 0;
    private float _time = 0.0f;
    private bool _lockSelection;
    private bool _showGizmos;

    private Color _backgroundColor = Color.clear;
    private float _direction = -10f;
    private bool _showGrid = true;
    private int CameraDirection {get { return Mathf.Sign(_direction) == -1 ? 0 : 1; } 
        set {
            _direction = value == 0 ? -10f : 10f;
        }
    }

    private List<Boxes> _boxes;

    private Camera _sceneCamera;
    private GameObject _sceneRoot;
    private GameObject _sceneObject = null;

    private static Toolbar toolbar;
    private static VisualElement overlayVisualElement;
    private static ObjectField prefabObjectField;
    private static ObjectField meshAnimationObjectField;
    private static ObjectField boxAnimationObjectField;
    private static Toggle lockSelectionToggle;
    private static Toggle showGizmosToggle;
    private static Toggle animationModeToggle;
    private static Button rewindButton;
    private static Button playButton;
    private static Button selectButton;
    private static ObjectField actionObjectField;
    private static Button actionButton;
    private static RadioButtonGroup cameraRadioButtonGroup;
    private static ColorField backgroundColorField;
    private static Toggle showGridToggle;

    private ActionAttack _actionAttack;
    private Hitbox _hitbox;

    private PreviewRenderUtility _previewRenderUtility;
    private Rect _previewRect;
    private Rect _screenRect;
    private Rect _handlesRect;

    #region Mouse Variables

    private Vector2 _mousePosition;
    private Vector3 _worldMousePosition;
    private Vector3 _origin;
    private Vector3 _delta;
    private Vector3 _reset;

    #endregion

    private GridOverlay _gridOverlay;

    private SliderInt sliderInt;
    private IntegerField frameIntegerField;
    private FloatField timeFloatField;

    // Add menu item named "Animation Preview" to the Window menu.
    [MenuItem("Window/Animation Preview")]
    public static void CreateWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        var window = GetWindow<AnimationPreviewEditorWindow>("Animation Preview");
        window.Show();
        window.Init(window);
    }

    public void Init(EditorWindow window){
        _editorWindow = window;
        _editorWindow.wantsMouseMove = true;

        _animationWindow = EditorWindow.CreateWindow<AnimationWindow>();
        Docker.Dock(_editorWindow, _animationWindow, Docker.DockPosition.Bottom);
    }

    private void LockSelection(){
        if (_lockSelection)
        {
            if (_boxAnimator == null) return;
            Selection.activeGameObject = _boxAnimator.gameObject;
            Repaint();
        }
    }

    private void OnSelectionChange(){
        LockSelection();
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
            _previewRenderUtility.camera.pixelRect = new Rect(0,0, _handlesRect.width, _handlesRect.height);
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
            //Handles.DrawGizmos(_previewRenderUtility.camera);
            //Handles.DrawCamera(_previewRect, _previewRenderUtility.camera, DrawCameraMode.Textured);

            // Custom handles function.
            //OnDrawHandles();

            // Show Gizmos for the boxes. Matrix is set to the _meshAnimator's transform matrix to follow the root motion.
            if (_showGizmos)
                foreach(Boxes box in _boxes){
                    box.DrawHandles(Matrix4x4.TRS(_meshAnimator.transform.position, _meshAnimator.transform.rotation * Quaternion.Euler(0f, -90f, 0f), _meshAnimator.transform.localScale));
                }
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
            break;

            case EventType.MouseUp:;
            break;

            case EventType.MouseMove: case EventType.MouseDrag:
                _mousePosition = Event.current.mousePosition;
                Repaint();
            break;
        }

        HandleCameraMovement();  
        HandleCameraZoom(); 

        //if (!_assetEditor) return;
        
        // _previewRenderUtility.BeginPreview(new Rect(0, 0, 128, 128), GUIStyle.none);
        // _previewRenderUtility.Render();
        // _previewRenderUtility.EndAndDrawPreview(new Rect(0, 0, 300, 300));

		// _outputTexture = _previewRenderUtility.EndPreview();
		// GUI.DrawTexture(rect, _outputTexture);

        // if (!_editorScene.IsValid() || !_editorScene.isLoaded)
        // {
        //     //OnEditorUnLoaded();
        //     return;
        // }
    
        //isInTopDownOrtho = _sceneCamera.orthographic && _sceneCamera.transform.rotation == downView;
    
        // if (patternRepositoryCurrentlyBeingEdited == null)
        //     AccountForNewRepository();
    
        // patternRepositoryCurrentlyBeingEdited.Update();
    
        //GUI.Box(rect, GUIContent.none);
        //GUISkin skin = GUI.skin;
        //GUI.skin = style;
    
        //DrawGUI();
        // if (currentPattern != null)
        //     currentPattern.DrawEditor();
    
        //SceneView.RepaintAll();
        // GUI.skin = skin;
    
        // patternRepositoryCurrentlyBeingEdited.ApplyModifiedProperties();

        // GUILayout.BeginHorizontal();
        // GUILayout.FlexibleSpace();
        // _assetEditor.OnPreviewSettings();
        // GUILayout.EndHorizontal();
        //_assetEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), EditorStyles.whiteLabel);

        // if (_previewRenderUtility != null || _outputTexture != null){}
        //     //GUI.DrawTexture(new Rect(0,0, 300, 300), _outputTexture);
    }

    void DrawGUI()
    {
        // Handles.SetCamera(_sceneCamera);
        // Handles.DrawCamera(new Rect(0, 0, position.width, position.height), _sceneCamera);
        // Handles.BeginGUI();


        // EditorGUILayout.BeginVertical("BackgroundWindow");
    
        // EditorGUILayout.BeginHorizontal();
        // LabelField("Show Full Menu");
        // showMenuProp.boolValue = EditorGUILayout.Toggle(showMenuProp.boolValue, toggleStyle, toggleWidth, toggleHeight);
        // EditorGUILayout.EndHorizontal();
    
        // if (showMenuProp.boolValue)
        //     DrawFullMenu();
        // else
        //     DrawHiddenMenu();
    }

    private void OnEnable(){
        AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        Selection.selectionChanged += LockSelection;

        AddToolbar();
        AddCameraSettings();

        // Create overlayVisualElement for floating controls.
        overlayVisualElement = new VisualElement();
        overlayVisualElement.AddClasses(
            "blackboard__overlay-custom-data-container"
        );
        rootVisualElement.Add(overlayVisualElement);

        AddAnimationControls();
        AddTools();
        AddStyles();
        CreateGridOverlay();

        if (_previewRenderUtility != null)
            _previewRenderUtility.Cleanup();

        _previewRenderUtility = new PreviewRenderUtility(true);
        System.GC.SuppressFinalize(_previewRenderUtility);

        SetupPreviewScene();
    }

    private void CreateGridOverlay(){
        _gridOverlay = new GridOverlay(){
           gridSize = new Vector3(10, 10, 10),
            largeStep = 1,
            smallStep = 0.25f,
            mainColor = new Color(0.75f, 0.75f, 0.75f, 1f),
            subColor = new Color(0.25f, 0.25f, 0.25f, 1f),
            showMain = _showGrid,
            showSub = _showGrid
        };
    }

    public void OnBeforeAssemblyReload()
    {
        AnimationMode.StopAnimationMode();
        Repaint();
        //Debug.Log($"Before Assembly Reload, {EditorApplication.timeSinceStartup}");
    }

    public void OnAfterAssemblyReload()
    {
        if (AnimationMode.InAnimationMode())
        {
            AnimationMode.StartAnimationMode();
            Repaint();
        }
        //Debug.Log($"After Assembly Reload, {EditorApplication.timeSinceStartup}");
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

        _sceneCamera.transform.position = new Vector3(0f, 0f, _direction);
        _sceneCamera.transform.LookAt(Vector3.zero);

        _sceneCamera.backgroundColor = _backgroundColor;
        _reset = _sceneCamera.transform.position;

        // Lights
        _previewRenderUtility.lights[0].transform.localEulerAngles = new Vector3(30, -30, 0);
        _previewRenderUtility.lights[0].intensity = 2;

        // Ambient Color
        _previewRenderUtility.ambientColor = Color.white;
        
        // Root Object
        _sceneRoot = new GameObject("Animation_Preview_Scene_Root");
        _sceneRoot.transform.position = Vector3.zero;
        _sceneRoot.hideFlags = HideFlags.HideAndDontSave;
        _previewRenderUtility.AddSingleGO(_sceneRoot);
    }

    private void Update(){
        if (_sceneObject == null) return;

        HandleAnimationWindow();
        
        // Animate the GameObject
        if (!EditorApplication.isPlaying && AnimationMode.InAnimationMode()){
            AnimationMode.BeginSampling();
            if (_meshAnimation != null && _meshAnimator != null) AnimationMode.SampleAnimationClip(_meshAnimator.gameObject, _meshAnimation, _time);
            if (_boxAnimation != null && _boxAnimator != null) AnimationMode.SampleAnimationClip(_boxAnimator.gameObject, _boxAnimation, _time);
            AnimationMode.EndSampling();
            Repaint();
        }
    }

    private void OnDisable() {
        AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
        AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        Selection.selectionChanged -= LockSelection;

        CloseWindow();
    }

    private void CloseWindow(){
        if (_previewRenderUtility != null)
			_previewRenderUtility.Cleanup();

		if (_sceneObject != null)
			Object.DestroyImmediate(_sceneObject);

        if (AnimationMode.InAnimationMode())
            AnimationMode.StopAnimationMode();

        // Clear the clip in "Edit" state.
        if (_boxAnimator != null){
            if (_state != null){
                _state.motion = null;
                UnityEditor.EditorUtility.SetDirty(_boxAnimator);
            }
        }

        _animationWindow.Close();
    }

    private void ToggleAnimationMode(){
        if (AnimationMode.InAnimationMode())
            AnimationMode.StopAnimationMode();
        else
            AnimationMode.StartAnimationMode();
    }

    private bool TryGetBoxes(out List<Boxes> boxes){
        boxes = new List<Boxes>();
        if (_sceneObject == null) return false;
        Boxes[] results = _sceneObject.GetComponentsInChildren<Boxes>();
        if (results == null || results.Count() == 0) return false;
        boxes.AddRange(results);
        return true;
    }

    private void HandleCameraMovement(){
        if (_sceneCamera == null) return;
        if (_sceneObject ==  null) return;

        // This solves the "Screen Position out of View Frustum" error.
        if (_mousePosition.x < 0 || _mousePosition.x >= base.position.width || _mousePosition.y < 0 || _mousePosition.y >= base.position.height)
            return;

        _worldMousePosition = _sceneCamera.ScreenToWorldPoint(new Vector3(_mousePosition.x, -_mousePosition.y, 10f));

        if (Event.current.type == EventType.MouseDown){
            _origin = _worldMousePosition;
        }

        if (Event.current.type == EventType.MouseDrag){
            _delta = _worldMousePosition - _sceneCamera.transform.position;
            _sceneCamera.transform.position = _origin - _delta;
        }
    }

    private void HandleCameraZoom(){
        if (_sceneCamera == null) return;
        if (_sceneObject ==  null) return;

        if (Event.current.type == EventType.ScrollWheel){
            float orthographicSize = _sceneCamera.orthographicSize;
            orthographicSize += Event.current.delta.y / 4;
            orthographicSize = Mathf.Clamp(orthographicSize, 1f, 10f);
            _sceneCamera.orthographicSize = orthographicSize;
            Repaint();
        }
    }

    private void HandleAnimationWindow(){
        if (_animationWindow == null) return;
        if (_boxAnimator == null) return;
        if (_boxAnimation == null) return;

        // Update Animation Window clip to match the _boxAnimation.
        if (_animationWindow.animationClip != _boxAnimation){
            _animationWindow.animationClip = _boxAnimation;
            _animationWindow.Repaint();
        }
        else{
            // Update _frame to match the Animation Window playhead.
            if (_frame != _animationWindow.frame){
                _frame = _animationWindow.frame;

                ConvertFrameToTime();

                // Update UI elements.
                sliderInt.SetValueWithoutNotify(_frame);
                frameIntegerField.SetValueWithoutNotify(_frame);
                timeFloatField.SetValueWithoutNotify(_time);

                Repaint();
            }

            // Update animationModeToggle to match the Animation Window preview.
            if (_animationWindow.previewing != animationModeToggle.value){
                animationModeToggle.SetValueWithoutNotify(AnimationMode.InAnimationMode());
            }

            // IMPORTANT : Play button needs to be reconsidered since it does not play the _meshAnimation.
            // Update playButton to match the Animation Window preview.
            // if (_animationWindow.playing){
            //     playButton.text = "Stop";
            // }
            // else{
            //     playButton.text = "Play";
            // }
        }
    }

    private void ConvertFrameToTime(){
        // Convert Frame into Time for sampling animation clip.
        _frame = Mathf.Clamp(_frame, 0, _frameCount);
        _time = _frame / _boxAnimation.frameRate;
    }

    private void AddCameraSettings(){
        VisualElement cameraSettingsVisualElement = new VisualElement();

        Foldout cameraSettingsFoldout = ElementUtility.CreateFoldout("Camera Settings", false);

        cameraSettingsFoldout.AddClasses(
            "blackboard__custom-data-container"
        );

        cameraRadioButtonGroup = ElementUtility.CreateRadioButtonGroup(new string[]{"Front", "Back"}, CameraDirection, "Camera", callback => {
            CameraDirection = callback.newValue;

            // Flip the camera look at direction.
            if (_sceneCamera != null){
                _sceneCamera.transform.position = new Vector3(0f, 0f, _direction);
                _sceneCamera.transform.LookAt(Vector3.zero);
            }
        });

        cameraRadioButtonGroup.AddClasses(
            "blackboard__radio-button-group"
        );

        backgroundColorField = ElementUtility.CreateColorField(_backgroundColor, "Background Color", false, false, false, callback => {
            _backgroundColor = callback.newValue;
            
            // Change the background color of the _sceneCamera.
            if (_sceneCamera != null)
                _sceneCamera.backgroundColor = _backgroundColor;
        });

        backgroundColorField.AddClasses(
            "blackboard__color-field"
        );

        showGridToggle = ElementUtility.CreateToggle(_showGrid, "Show Grid", callback => {
            _showGrid = callback.newValue;

            // Show/Hide Grid
            if (_gridOverlay != null)
                _gridOverlay.showMain = _showGrid;
                _gridOverlay.showSub = _showGrid;
        });

        // cameraSettingsFoldout.Add(cameraRadioButtonGroup);
        // cameraSettingsFoldout.Add(backgroundColorField);
        // cameraSettingsVisualElement.Add(cameraSettingsFoldout);
        // toolbar.Add(cameraSettingsVisualElement);

        toolbar.Add(showGridToggle);
        toolbar.Add(cameraRadioButtonGroup);
        toolbar.Add(backgroundColorField);
    }

    private void AddTools(){
        VisualElement toolVisualElement = new VisualElement();

        Foldout toolFoldout = ElementUtility.CreateFoldout("Tools", false);

        toolFoldout.AddClasses(
            "blackboard__custom-data-container"
        );

        // Override Menu
        VisualElement overrideVisualElement = new VisualElement();

        overrideVisualElement.AddClasses(
            "blackboard__info-custom-data-container"
        );

        Label actionLabel = ElementUtility.CreateLabel("Override Action AI Property");
        actionLabel.style.marginBottom = 10;

        actionObjectField = ElementUtility.CreateObjectField(typeof(ActionAttack), _actionAttack, "Action", callback => {
            _actionAttack = callback.newValue as ActionAttack;
        });

        actionButton = ElementUtility.CreateButton("Override Property", () => {
           OverrideActionProperty();
        });

        actionButton.AddClasses(
            "blackboard__button"
        );

        overrideVisualElement.Add(actionLabel);
        overrideVisualElement.Add(actionObjectField);
        overrideVisualElement.Add(actionButton);
        toolFoldout.Add(overrideVisualElement);
        toolVisualElement.Add(toolFoldout);
        overlayVisualElement.Add(toolVisualElement);
    }

    private void OverrideActionProperty(){
        if (_actionAttack == null) return;
        if (_boxAnimator == null) return;
        if (_meshAnimator == null) return;
        
        // This requires a Hitbox object named "Hitbox" to be present.
        _hitbox = _boxAnimator.transform.Find("Hitbox").GetComponent<Hitbox>();
       
        Vector2 position = new Vector2(_meshAnimator.transform.position.x, _meshAnimator.transform.position.y);
        Vector2 location =  position + _hitbox.Offset;
        
        _actionAttack.HitboxLocation = location;
        _actionAttack.HitboxOffset = _hitbox.Offset;
        _actionAttack.HitboxSize = _hitbox.Size;
        _actionAttack.HitboxFrame = _frame;
        Debug.Log(_actionAttack.name + "'s AI properties were set to Frame: " + "(" + _frame + ")" + " Location: " + location + " Offset: " + _hitbox.Offset + " Size: " + _hitbox.Size);
    }

    private void AddAnimationControls(){
        VisualElement animationControls = new VisualElement();

        Foldout controlsFoldout = ElementUtility.CreateFoldout("Animation Controls", false);

        controlsFoldout.AddClasses(
            "blackboard__custom-data-container"
        );
        //animationControls.style.justifyContent = Justify.FlexStart;

        VisualElement playbackControls = new VisualElement();

        playbackControls.AddClasses(
            "blackboard__content-custom-data-container"
        );

        VisualElement frameControls = new VisualElement();

        animationModeToggle = ElementUtility.CreateToggle(AnimationMode.InAnimationMode(), "Animation Mode", callback => {
            ToggleAnimationMode();
            
            // Update Animation Window preview.
            if (_animationWindow.previewing == true)
                _animationWindow.previewing = false;
            _animationWindow.previewing = AnimationMode.InAnimationMode();
            _animationWindow.Repaint();

            animationModeToggle.SetValueWithoutNotify(AnimationMode.InAnimationMode());
        });
        animationModeToggle.SetEnabled(false);

        animationModeToggle.AddClasses(
            "blackboard__toggle"
        );

        playbackControls.Add(animationModeToggle);

        rewindButton = ElementUtility.CreateButton("Rewind", () => {
            _frame = 0;
           ConvertFrameToTime();

            // Update Animation Window playhead.
            _animationWindow.time = 0;
            _animationWindow.Repaint();

            // Update UI elements.
            frameIntegerField.SetValueWithoutNotify(0);
            sliderInt.SetValueWithoutNotify(0);
            timeFloatField.SetValueWithoutNotify(0); 
        });
        rewindButton.SetEnabled(false);

        rewindButton.AddClasses(
            "blackboard__button"
        );

        playbackControls.Add(rewindButton);

        playButton = ElementUtility.CreateButton("Play", () => {
            _animationWindow.playing = !_animationWindow.playing;
        });
        playButton.SetEnabled(false);

        playButton.AddClasses(
            "blackboard__button"
        );
        
        playbackControls.Add(playButton);

        sliderInt = ElementUtility.CreateSliderInt(_frame, null, callback => {
            _frame = callback.newValue;
            ConvertFrameToTime();

            // Update Animation Window playhead.
            _animationWindow.frame = _frame;
            _animationWindow.Repaint();

            // Update UI elements.
            frameIntegerField.SetValueWithoutNotify(callback.newValue);
            timeFloatField.value = _time;
        });
        sliderInt.SetEnabled(false);

        sliderInt.AddClasses(
            "blackboard__slider"
        );

        controlsFoldout.Add(sliderInt);

        frameIntegerField = ElementUtility.CreateIntegerField(_frame, "Frame", callback => {
            int value = callback.newValue;
            value = Mathf.Clamp(value, 0, _frameCount);
            if (value != callback.newValue) frameIntegerField.SetValueWithoutNotify(value);

            _frame = value;
            ConvertFrameToTime();

            // Update Animation Window playhead.
            _animationWindow.frame = _frame;
            _animationWindow.Repaint();

            // Update UI elements.
            sliderInt.SetValueWithoutNotify(callback.newValue);
            timeFloatField.value = _time;
        });
        frameIntegerField.SetEnabled(false);

        frameControls.Add(frameIntegerField);

        timeFloatField = ElementUtility.CreateFloatField(_time, "Time", null);
        timeFloatField.SetEnabled(false);

        frameControls.Add(timeFloatField);

        controlsFoldout.Add(frameControls);
        controlsFoldout.Add(playbackControls);
        animationControls.Add(controlsFoldout);
        overlayVisualElement.Add(animationControls);
    }

    private void AddToolbar()
    {
        toolbar = new Toolbar();

        prefabObjectField = ElementUtility.CreateObjectField(typeof(GameObject), _asset, "Prefab", callback =>{
            _asset = callback.newValue as GameObject;

            if (_sceneObject != null)
                GameObject.DestroyImmediate(_sceneObject);
            _sceneObject = Instantiate(_asset, _sceneRoot.transform);
            _sceneObject.hideFlags = HideFlags.HideAndDontSave;
            _sceneObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, 90f, 0f));

            // Try getting boxes if the object has them.
            if (TryGetBoxes(out _boxes)){
                showGizmosToggle.SetEnabled(true);
                showGizmosToggle.value = true;
            }
            else{
                showGizmosToggle.value = false;
                showGizmosToggle.SetEnabled(false);
            }

            // Get animators if the object has them.
            // Animators must be in the correct order in hierarchy.
            Animator[] animators = _sceneObject.GetComponentsInChildren<Animator>();
            if (animators != null && animators.Count() == 2){
                _meshAnimator = animators[0];
                _boxAnimator = animators[1];
            }

            // Update UI elements.
            if (_asset != null && _boxAnimator != null){
                meshAnimationObjectField.SetEnabled(true);
                boxAnimationObjectField.SetEnabled(true);
            }
            else{
                meshAnimationObjectField.SetEnabled(false);
                boxAnimationObjectField.SetEnabled(false);
            }

            // Get AnimatorController from the box Animator.
            // Then populate the _stateList.
            if (_boxAnimator != null){
                var runtimeController = _boxAnimator.runtimeAnimatorController as AnimatorOverrideController;
                _animatorController = _boxAnimator.runtimeAnimatorController as AnimatorController;

                if (_animatorController == null)
                    _animatorController = runtimeController.runtimeAnimatorController as AnimatorController;

                AnimatorControllerLayer[] layers = _animatorController.layers;
                AnimatorControllerLayer defaultLayer = layers[0];

                _stateList = new List<AnimatorState>();
                _stateList = _ExpandStatesInLayer(defaultLayer.stateMachine);

                // Find the state named "Edit".
                foreach( var state in _stateList )
                {
                    // This requires the Animator Controller to have a state named "Edit".
                    if (state.name == "Edit"){
                        _state = state;
                    }
                    
                    // Debug for logging each state in the current layer.
                    // Motion m = state.motion;
                    // if( m is AnimationClip ) // It is single animation clip
                    //     Debug.Log( "clip " + m.name );
                    // if( m is BlendTree ) // It is blendtree with multiple clips!
                    //     Debug.Log( "tree " + m.name );
                }
            }
               
            // string filePath = $"Assets/{_asset.name}";
            // IOUtility.Initialize(Path.GetFileNameWithoutExtension(filePath), this);
            // IOUtility.Load();
        });

        showGizmosToggle = ElementUtility.CreateToggle(_showGizmos, "Show Gizmos", callback => {
            _showGizmos = callback.newValue;
        });
        showGizmosToggle.SetEnabled(false);

        meshAnimationObjectField = ElementUtility.CreateObjectField(typeof(AnimationClip), _meshAnimation, "Mesh Animation", callback => {
            //MeshAnimation = callback.newValue as AnimationClip;
            _meshAnimation = callback.newValue as AnimationClip;

            // Update UI elements.
            if (_meshAnimation != null || _boxAnimation != null){
                animationModeToggle.SetEnabled(true);
                sliderInt.SetEnabled(true);
                frameIntegerField.SetEnabled(true);
            }
            else{
                animationModeToggle.SetEnabled(false);
                sliderInt.SetEnabled(false);
                frameIntegerField.SetEnabled(false);
            }    
        });
        meshAnimationObjectField.SetEnabled(false);

        boxAnimationObjectField = ElementUtility.CreateObjectField(typeof(AnimationClip), _boxAnimation, "Box Animation", callback => {
            BoxAnimation = callback.newValue as AnimationClip;
            if (_boxAnimator != null){
                // Insert the current clip into "Edit" state.
                if (_state != null)
                    _state.motion = _boxAnimation;
                UnityEditor.EditorUtility.SetDirty(_boxAnimator);
                _animationWindow.Repaint();
                SelectAnimator();
                SelectClip();
            }

            // Update UI elements.
            if (_meshAnimation != null || _boxAnimation != null){
                animationModeToggle.SetEnabled(true);
                sliderInt.SetEnabled(true);
                frameIntegerField.SetEnabled(true);
                playButton.SetEnabled(true);
                rewindButton.SetEnabled(true);
            }
            else{
                animationModeToggle.SetEnabled(false);
                sliderInt.SetEnabled(false);
                frameIntegerField.SetEnabled(false);
                playButton.SetEnabled(false);
                rewindButton.SetEnabled(false);
            }    
        });
        boxAnimationObjectField.SetEnabled(false);

        lockSelectionToggle = ElementUtility.CreateToggle(_lockSelection, "Lock Selection", callback => {
            _lockSelection = callback.newValue;
            LockSelection();
        });

        selectButton = ElementUtility.CreateButton("Select", () => {
           SelectAnimator();
           SelectClip();
        });

        // Button clearButton = ElementUtility.CreateButton("Clear", () => Clear());
        // Button resetButton = ElementUtility.CreateButton("Reset", () => Reset());

        toolbar.Add(prefabObjectField);

        toolbar.Add(showGizmosToggle);

        toolbar.Add(meshAnimationObjectField);

        toolbar.Add(boxAnimationObjectField);

        toolbar.Add(lockSelectionToggle);

        toolbar.Add(selectButton);

        // toolbar.Add(clearButton);

        // toolbar.Add(resetButton);

        toolbar.AddStyleSheets(
            "Animation Preview/AnimationPreviewToolbarStyles.uss"
        );

        rootVisualElement.Add(toolbar);
    }

    private void SelectAnimator(){
        // Select the Animator to edit.
        if (_boxAnimator == null) return;
        Selection.activeGameObject = _boxAnimator.gameObject;
        Repaint();
    }

    private void SelectClip(){
        // Select the inserted Animation Clip in the Animation Window.
        if (_animationWindow == null) return;
        if (_boxAnimator == null) return;
        if (_boxAnimation == null) return;

        if (_animationWindow.animationClip != _boxAnimation)
            _animationWindow.animationClip = _boxAnimation;
    }

    private List<AnimatorState> _ExpandStatesInLayer( AnimatorStateMachine sm, List<AnimatorState> collector = null )
    {
        if( collector == null )
            collector = new List<AnimatorState>( );

        foreach( var state in sm.states )
        {
            collector.Add( state.state );

            foreach( var subSm in sm.stateMachines ) // Jump into nested state machine
                _ExpandStatesInLayer( subSm.stateMachine, collector );
        }
        return collector;
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
        // if (string.IsNullOrEmpty(fileNameTextField.value))
        // {
        //     EditorUtility.DisplayDialog("Invalid file name.", "Please ensure the file name you've typed in is valid.", "Roger!");

        //     return;
        // }
        
        // BlackboardIOUtility.Initialize(fileNameTextField.value, this);
        // BlackboardIOUtility.Save();
    }

    private void Load()
    {
        string filePath = EditorUtility.OpenFilePanel("Asset", "Assets/", "prefab");

        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        Clear();

        IOUtility.Initialize(Path.GetFileNameWithoutExtension(filePath), this);
        IOUtility.Load();
    }

    private void Clear()
    {
        if (_sceneCamera != null)
            _sceneCamera.transform.position = _reset;
        // EnableSaving();
    }

    private void Reset()
    {
        Clear();
        // UpdateFileName(defaultFileName);
    }
}
