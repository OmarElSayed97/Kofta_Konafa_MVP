using UnityEngine;
using KoftaAndKonafa.Enums;
using UnityEditor;

namespace KoftaAndKonafa.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewIngredient", menuName = "KoftaAndKonafa/Ingredient", order = 3)]
    public class IngredientSO : ScriptableObject
    {
        [Header("Ingredient Info")]
        public GameEnums.Ingredient ingredientName;
        public int ingredientID;
        public GameObject ingredientPrefab;

        [Header("State Info")]
        public GameEnums.IngredientState currentState;

        [Header("Processing Times")]
        [Tooltip("Time required to prep the ingredient.")]
        [ConditionalField("currentState", GameEnums.IngredientState.Default)]
        public float prepTime;

        
    }
}

#if UNITY_EDITOR


// Custom property drawer to enable conditional visibility in the editor
[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute condAttr = (ConditionalFieldAttribute)attribute;
        SerializedProperty conditionProp = property.serializedObject.FindProperty(condAttr.ConditionalFieldName);

        if (conditionProp != null && conditionProp.enumValueIndex == (int)condAttr.ExpectedValue)
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute condAttr = (ConditionalFieldAttribute)attribute;
        SerializedProperty conditionProp = property.serializedObject.FindProperty(condAttr.ConditionalFieldName);

        if (conditionProp != null && conditionProp.enumValueIndex == (int)condAttr.ExpectedValue)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        return -EditorGUIUtility.standardVerticalSpacing;
    }
}

public class ConditionalFieldAttribute : PropertyAttribute
{
    public string ConditionalFieldName { get; }
    public object ExpectedValue { get; }

    public ConditionalFieldAttribute(string conditionalFieldName, object expectedValue)
    {
        ConditionalFieldName = conditionalFieldName;
        ExpectedValue = expectedValue;
    }
}
#endif
