using UnityEditor;
using UnityEngine;

namespace TheDates
{
    [CustomPropertyDrawer(typeof(GridCollection<>))]
    public class GridDrawer : PropertyDrawer 
    {
        private const float Padding = 2f;
        private const float LabelWidth = 60f; 

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            
            // Find the relevant properties
            SerializedProperty columns = property.FindPropertyRelative("columns");
            SerializedProperty colLabels = property.FindPropertyRelative("columnLabels");
            SerializedProperty rowLabels = property.FindPropertyRelative("rowLabels");
            
            // Exit & display a message if the collection is empty or null
            if (columns == null || columns.arraySize == 0) {
                EditorGUI.LabelField(position, label.text, "Initialize columns in inspector");
                EditorGUI.EndProperty();
                return;
            }
            
            // Get the x and y count of elements
            int colCount = columns.arraySize;
            int rowCount = columns.GetArrayElementAtIndex(0).FindPropertyRelative("rows").arraySize;
            
            // Draw the header label
            Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(headerRect, label, EditorStyles.boldLabel);
            
            float currentY = position.y + EditorGUIUtility.singleLineHeight + Padding;
            float cellWidth = (position.width - LabelWidth) / colCount;

            // ! Draw the column labels
            for (int c = 0; c < colCount; c++) {
                Rect colRect = new Rect(position.x + LabelWidth + (c * cellWidth), currentY, cellWidth, EditorGUIUtility.singleLineHeight);
                string colText = (c < colLabels.arraySize) ? colLabels.GetArrayElementAtIndex(c).stringValue : $"Col {c}";
                EditorGUI.LabelField(colRect, colText, EditorStyles.centeredGreyMiniLabel);
            }
            currentY += EditorGUIUtility.singleLineHeight + Padding;

            // ! Draw the row labels & fields
            for (int r = 0; r < rowCount; r++) {
                // Row Label
                Rect rowLabelRect = new Rect(position.x, currentY, LabelWidth, EditorGUIUtility.singleLineHeight);
                string rowText = (r < rowLabels.arraySize) ? rowLabels.GetArrayElementAtIndex(r).stringValue : $"Row {r}";
                EditorGUI.LabelField(rowLabelRect, rowText, EditorStyles.miniLabel);

                // Row Cells
                for (int c = 0; c < colCount; c++) {
                    SerializedProperty rowProperty = columns.GetArrayElementAtIndex(c).FindPropertyRelative("rows");
                    
                    // !! Ensure all columns match row count (resizes if needed)
                    if (rowProperty.arraySize != rowCount) rowProperty.arraySize = rowCount;
                    
                    Rect cellRect = new Rect(position.x + LabelWidth + (c * cellWidth), currentY, cellWidth, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(cellRect, rowProperty.GetArrayElementAtIndex(r), GUIContent.none);
                }
                currentY += EditorGUIUtility.singleLineHeight + Padding;
            }

            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            SerializedProperty columns = property.FindPropertyRelative("columns");
            if (columns == null || columns.arraySize == 0) return EditorGUIUtility.singleLineHeight;

            int rowCount = columns.GetArrayElementAtIndex(0).FindPropertyRelative("rows").arraySize;
            
            // Main Label + Column Header Row + (Rows * RowHeight)
            return (EditorGUIUtility.singleLineHeight + Padding) * (rowCount + 2);
        }
    }
}