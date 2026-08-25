using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheDates.Runtime.General
{
    [Serializable]
    public class GridCollection<T> : ISerializationCallbackReceiver
    {
        public string[] rowLabels; // Serialised row labels
        public string[] columnLabels; // Serialised column labels
        public GridColumn<T>[] columns; // Actual serialised data
        
        private HashSet<string> _columnLabels;
        private HashSet<string> _rowLabels;
    
        public GridCollection() {
            rowLabels = Array.Empty<string>();
            columnLabels = Array.Empty<string>();
            columns = Array.Empty<GridColumn<T>>();
        }
        
        public GridCollection(string[] columns, string[] rows) {
            _columnLabels = new HashSet<string>(columns ?? Array.Empty<string>());
            _rowLabels = new HashSet<string>(rows ?? Array.Empty<string>());
            columnLabels = _columnLabels.ToArray();
            rowLabels = _rowLabels.ToArray();
            
            this.columns = Array.Empty<GridColumn<T>>();
            Initialize();
        }
        
        public void OnBeforeSerialize()
        { }

        public void Initialize() {
            // Unique strings only
            _columnLabels = new HashSet<string>(columnLabels);
            _rowLabels = new HashSet<string>(rowLabels);
            
            // Back to an array
            columnLabels = _columnLabels.ToArray();
            rowLabels = _rowLabels.ToArray();
            
            int newColCount = columnLabels.Length;
            int newRowCount = rowLabels.Length;

            // Resize the column array
            if (columns.Length != newColCount) {
                GridColumn<T>[] newColumns = new GridColumn<T>[newColCount];
                for (int c = 0; c < newColCount; c++) {
                    // Copy existing column if within bounds, otherwise create a new one
                    newColumns[c] = (c < columns.Length) ? columns[c] : new GridColumn<T>();
                }
                columns = newColumns;
            }

            // Resize each of the Columns' internal Row arrays
            for (int c = 0; c < columns.Length; c++) {
                if (columns[c].rows.Length != newRowCount) {
                    T[] newRows = new T[newRowCount];
                    // Copy existing values from the old row to the new one
                    int copyLength = Math.Min(columns[c].rows.Length, newRowCount);
                    Array.Copy(columns[c].rows, newRows, copyLength);
                
                    columns[c].rows = newRows;
                }
            }
        }

        public void OnAfterDeserialize() {
            Initialize();
        }
    }
}