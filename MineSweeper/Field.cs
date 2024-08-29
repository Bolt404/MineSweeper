using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MineSweeper
{
    public class Field
    {
        // Constructor to initialize the Field with row and column values
        public Field(int row, int column)
        {
            Debug.WriteLine($"FIELD MADE WITH ROW:{row} - COLUMN:{column}");
            Row = row;
            Column = column;
            IsMine = false;  // Default value; set this later when placing mines
            IsRevealed = false;  // Default to not revealed
            AdjacentMines = 0;  // Default to 0; calculate this after mine placement
            id = int.Parse(row.ToString() + column.ToString());
        }




        // Properties of the Field
        public int Row { get; private set; }
        public int Column { get; private set; }
        public Button? Button { get; set; } 
        public bool IsMine { get; set; }
        public bool IsRevealed { get; set; }
        public int AdjacentMines { get; set; }

        public int id { get; set; }
    }
}
