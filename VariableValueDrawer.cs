//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VariableValue;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(VariableFloat))]
public class VariableFloatDrawer : VariableValueDrawer {}
[CustomPropertyDrawer(typeof(VariableVector2))]
public class VariableVector2Drawer : VariableValueDrawer {}
[CustomPropertyDrawer(typeof(VariableVector3))]
public class VariableVector3Drawer : VariableValueDrawer {}

//[CustomPropertyDrawer(typeof(VariableValue<Vector2>))]
//public class VariableValue1Drawer : VariableValueDrawer { }

//[CustomPropertyDrawer(typeof(VariableValue<float>))]
//public class VariableValueDrawer_float : VariableValueDrawer {}
//[CustomPropertyDrawer(typeof(VariableVector4))]
//public class VariableVector4Drawer : VariableValueDrawer<Vector4> {}

public class VariableValueDrawer/*<ValueType>*/ : PropertyDrawer {

    private class PropertyData {
        public SerializedProperty defaultValueProperty;
        public SerializedProperty changeTypeProperty;
        public SerializedProperty addValueProperty;
        public SerializedProperty magnificationProperty;
        public SerializedProperty everyNTimesProperty;
        public SerializedProperty limitTypeProperty;
        public SerializedProperty maxIndexProperty;
        public SerializedProperty selectionProperty;
    }

    private Dictionary<string, PropertyData> _propertyDataPerPropertyPath = new Dictionary<string, PropertyData>();
    private PropertyData _property;

    private float LineHeight { get { return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; } }

    private void Init(SerializedProperty property) {
        if (_propertyDataPerPropertyPath.TryGetValue(property.propertyPath, out _property)) {
            return;
        }

        _property = new PropertyData();
        _property.defaultValueProperty = property.FindPropertyRelative("defaultValue");
        _property.changeTypeProperty = property.FindPropertyRelative("changeType");
        _property.addValueProperty = property.FindPropertyRelative("addValue");
        _property.magnificationProperty = property.FindPropertyRelative("magnification");
        _property.everyNTimesProperty = property.FindPropertyRelative("everyNTimes");
        _property.limitTypeProperty = property.FindPropertyRelative("limitType");
        _property.maxIndexProperty = property.FindPropertyRelative("maxIndex");
        _property.selectionProperty = property.FindPropertyRelative("selection");
        _propertyDataPerPropertyPath.Add(property.propertyPath, _property);
    }

    //int _height;
    //Rect fieldRect;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        Init(property);
        Rect fieldRect = position;
        fieldRect.height = LineHeight;

        using (new EditorGUI.PropertyScope(fieldRect, label, property)) {
            property.isExpanded = EditorGUI.Foldout(new Rect(fieldRect), property.isExpanded, label);
            if (property.isExpanded) {
                using (new EditorGUI.IndentLevelScope()) {
                    //EditorGUI.indentLevel++;
                    PropertyFieldGUI(ref fieldRect, _property.defaultValueProperty);
                    PropertyFieldGUI(ref fieldRect, _property.changeTypeProperty);
                    EachOptionPropertyFieldGUI(ref fieldRect);
                    //EditorGUI.indentLevel--;
                }
            } else MiniPreview(fieldRect);
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        Init(property);
        int height = 1;
        if (property.isExpanded)
            switch (_property.changeTypeProperty.enumValueIndex) {
                case 0:
                    height += 2;
                    break;
                case 1:
                    height += 6;
                    if (_property.limitTypeProperty.enumValueIndex != 0) height++;
                    break;
                case 2:
                    height += 5;
                    if (_property.selectionProperty.isExpanded) height += _property.selectionProperty.arraySize + 1;
                    if (_property.limitTypeProperty.enumValueIndex != 0) height++;
                    break;
            }
        return height * LineHeight;
    }
    //配列のみChildrenの表示対応
    private void PropertyFieldGUI(ref Rect rect, SerializedProperty serializedProperty, bool includeChildren = false) {
        rect.y += LineHeight;
        EditorGUI.PropertyField(new Rect(rect), serializedProperty, includeChildren);
        if (!includeChildren || !serializedProperty.isExpanded) return;
        if (serializedProperty.isArray) rect.y += (serializedProperty.arraySize + 1) * LineHeight;
    }
    /*private void ArrayElementsPropertyFieldGUI(SerializedProperty serializedProperty) {
        AddLineHeight();
        GUIContent label = new GUIContent("Size");
        serializedProperty.arraySize = EditorGUI.DelayedIntField(fieldRect, label, serializedProperty.arraySize);
        for (int i = 0; i < serializedProperty.arraySize; i++) {
            var selection = serializedProperty.GetArrayElementAtIndex(i);
            PropertyFieldGUI(selection);
        }
    }*/
    private void MiniPreview(Rect rect) {
        Rect valueRect = new Rect(rect);
        Rect changeTypeRect = new Rect(rect);
        float middle = 0.9f;
        valueRect.xMin += 100f;
        valueRect.xMax = (rect.xMax - valueRect.xMin) * middle;
        changeTypeRect.xMin = valueRect.xMax;
        //Debug.Log((valueRect.xMin, valueRect.xMax));
        GUI.enabled = false;
        EditorGUI.PropertyField(valueRect, _property.defaultValueProperty, new GUIContent());
        EditorGUI.PropertyField(changeTypeRect, _property.changeTypeProperty, new GUIContent());
        GUI.enabled = true;
    }
    private void EachOptionPropertyFieldGUI(ref Rect rect) {
        switch (_property.changeTypeProperty.enumValueIndex) {
            case 0:
                return;
            case 1:
                PropertyFieldGUI(ref rect, _property.addValueProperty);
                PropertyFieldGUI(ref rect, _property.magnificationProperty);
                break;
            case 2:
                SerializedProperty selectionList = _property.selectionProperty;
                PropertyFieldGUI(ref rect, selectionList, true);
                //EditorGUI.indentLevel++;
                //if (selectionList.isExpanded) ArrayElementsPropertyFieldGUI(selectionList);
                //EditorGUI.indentLevel--;
                break;
        }
        PropertyFieldGUI(ref rect, _property.everyNTimesProperty);
        PropertyFieldGUI(ref rect, _property.limitTypeProperty);
        if (_property.limitTypeProperty.enumValueIndex != 0) //noLimitでない
            PropertyFieldGUI(ref rect, _property.maxIndexProperty);
    }   
}
#endif