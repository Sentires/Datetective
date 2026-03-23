using System;

namespace TheDates.Runtime.General
{
    [Serializable]
    public class GridColumn<T>
    {
        public T[] rows;

        public GridColumn(params T[] rows) {
            this.rows = rows;
        }
        
        public GridColumn(int rowCount) {
            this.rows = new T[rowCount];
        }
        
    }
}