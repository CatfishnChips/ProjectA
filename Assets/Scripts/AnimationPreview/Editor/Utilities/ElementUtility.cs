using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public static class ElementUtility
{
    public static Label CreateLabel(string text){
        Label label = new Label(text);
        return label;
    }

    public static Toggle CreateToggle(bool value = default, string label = null, EventCallback<ChangeEvent<bool>> onValueChanged = null)
    {
        Toggle toggle = new Toggle(){
            value = value,
            label = label
        };

        if (onValueChanged != null)
        {
            toggle.RegisterValueChangedCallback(onValueChanged);
        }

        return toggle;
    }

    public static ColorField CreateColorField(Color value = default, string label = null, bool hdr = false, bool showAlpha = false, bool showEyeDropper = false, EventCallback<ChangeEvent<Color>> onValueChanged = null)
    {
        ColorField colorField = new ColorField(){
            value = value,
            label = label,
            hdr = hdr,
            showAlpha = showAlpha,
            showEyeDropper = showEyeDropper,
        };

        if (onValueChanged != null)
        {
            colorField.RegisterValueChangedCallback(onValueChanged);
        }

        return colorField;
    }

    public static RadioButtonGroup CreateRadioButtonGroup(string[] choices = null, int value = default, string label = null, EventCallback<ChangeEvent<int>> onValueChanged = null)
    {
        RadioButtonGroup radioButtonGroup = new RadioButtonGroup(){
            choices = choices,
            value = value,
            label = label
        };

        if (onValueChanged != null)
        {
            radioButtonGroup.RegisterValueChangedCallback(onValueChanged);
        }

        return radioButtonGroup;
    }

    public static SliderInt CreateSliderInt(int value = default, string label = null, EventCallback<ChangeEvent<int>> onValueChanged = null)
    {
        SliderInt sliderInt = new SliderInt(){
            value = value,
            label = label
        };

        if (onValueChanged != null){
            sliderInt.RegisterValueChangedCallback(onValueChanged);
        }

        return sliderInt;
    }

    public static FloatField CreateFloatField(float value = default, string label = null, EventCallback<ChangeEvent<float>> onValueChanged = null)
    {
        FloatField floatField = new FloatField(){
            value = value,
            label = label
        };

        if (onValueChanged != null){
            floatField.RegisterValueChangedCallback(onValueChanged);
        }

        return floatField;
    }

    public static ObjectField CreateObjectField(Type type, UnityEngine.Object value = null, string label = null, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged = null)
    {
        ObjectField objectField = new ObjectField(){
            objectType = type,
            value = value,
            label = label,
            allowSceneObjects = false
        };

        if (onValueChanged != null){
            objectField.RegisterValueChangedCallback(onValueChanged);
        }

        return objectField;
    }

    public static EnumField CreateEnumField(System.Enum value = default, string label = null, EventCallback<ChangeEvent<System.Enum>> onValueChanged = null)
    {
        EnumField enumField = new EnumField(value)
        {
            value = value,
            label = label
        };

        if (onValueChanged != null)
        {
            enumField.RegisterValueChangedCallback(onValueChanged);
        }

        return enumField;
    }

    public static Button CreateButton(string text, Action onClick = null)
    {
        Button button = new Button(onClick)
        {
            text = text
        };

        return button;
    }

    public static Foldout CreateFoldout(string title, bool collapsed = false)
    {
        Foldout foldout = new Foldout()
        {
            text = title,
            value = !collapsed
        };

        return foldout;
    }

    public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textField = new TextField()
        {
            value = value,
            label = label
        };

        if (onValueChanged != null)
        {
            textField.RegisterValueChangedCallback(onValueChanged);
        }

        return textField;
    }

    public static IntegerField CreateIntegerField(int value = 0, string label = null, EventCallback<ChangeEvent<int>> onValueChanged = null)
    {
        IntegerField integerField = new IntegerField()
        {
            value = value,
            label = label
        };

        if (onValueChanged != null)
        {
            integerField.RegisterValueChangedCallback(onValueChanged);
        }

        return integerField;
    }

    public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textArea = CreateTextField(value, label, onValueChanged);

        textArea.multiline = true;

        return textArea;
    }
}