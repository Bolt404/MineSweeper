using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MineSweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();
        int numberOfColumns;
        int numberOfRows;
        Field[,] fieldsArray;


        public MainWindow()
        {
            //xaml stuff
            InitializeComponent();

            //Counting my grid
            numberOfColumns = GrdMineField.ColumnDefinitions.Count;
            numberOfRows = GrdMineField.RowDefinitions.Count;

            //fields array lets go.
            fieldsArray = new Field[numberOfRows, numberOfColumns];

            //InitGame yes please.
            InitGame();
        }

        public void InitGame()
        {
            fieldsArray = new Field[numberOfRows, numberOfColumns];
            GrdMineField.Children.Clear();
            SetupMineField();
            SetupMines(10);

            foreach (var item in fieldsArray)
            {
                List<Field> itemBorders = CheckBorderFields(item);
                int count = 0;
                foreach (Field field in itemBorders)
                {
                    
                    if (field.IsMine)
                    {
                        count++;
                    }
                    

                }
                item.AdjacentMines = count;
                item.Button.Content = count;
            }
        }

        private void SetupMineField()
        {

            int count = 1;

            for (int X = 0; X < numberOfRows; X++)
            {
                for (int Y = 0; Y < numberOfColumns; Y++)
                {
                    Button mybutton = new Button();

                    Field field = new Field(X, Y);
                    field.Button = mybutton;
                    field.IsRevealed = false;

                    Grid.SetColumn(mybutton, Y);
                    Grid.SetRow(mybutton, X);

                    //Row X, Column Y
                    String pos = $"{X},{Y}";

                    //DEBUG CONTENT
                    mybutton.Content = pos;

                    //Setting Button ID to be equal to field
                    //mybutton.Name = field.id.ToString();

                    //Adding listener for clicking
                    mybutton.Click += (Button_Field_Click);

                    GrdMineField.Children.Add(mybutton);
                    fieldsArray[X,Y] = field;
                    

                    //Count for debug ID, maybe delete?
                    count++;

                }

            }

            Debug.WriteLine($"Number of Columns:{numberOfColumns} | Number of Rows:{numberOfRows}");
        }

        private void Button_Field_Click( object sender, RoutedEventArgs e)
        {
            RemoveButton(sender);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitGame();
        }

        public void SetupMines(int mineCount)
        {
            int minesPlaced = 0;
            int totalFields = fieldsArray.Length;
            Debug.WriteLine($"List of Fields = {totalFields}");

            while (minesPlaced < mineCount)
            {
                int randomIndexRow = random.Next(0, numberOfRows); // Pick a random index in the list
                int randomIndexColumn = random.Next(0, numberOfColumns);

                Field selectedField = fieldsArray[randomIndexRow, randomIndexColumn];

                // Ensure we don't place a mine where there's already one
                if (!selectedField.IsMine)
                {
                    selectedField.IsMine = true;
                    selectedField.Button.Background = System.Windows.Media.Brushes.Red;
                    minesPlaced++;

                    Debug.WriteLine($"Mine placed at: {selectedField.Row}, {selectedField.Column}");
                } else
                {
                    Debug.WriteLine($"Already a mine at: {selectedField.Row}, {selectedField.Column}");
                }
            }
            Debug.WriteLine($"Total mines placed: {minesPlaced}");
        }

        public List<Field> CheckBorderFields(Field field)
        {
            int X = field.Row;
            int Y = field.Column;
            List<Field> borderingFields = new List<Field>();

            Debug.WriteLine($"I AM {field.Row},{field.Column}");

            for (int XToCheck = -1; XToCheck <= 1; XToCheck++)
            {
                for (int YToCheck = -1; YToCheck <= 1; YToCheck++)
                {
                    int checkfieldx = XToCheck + X;
                    int checkfieldy = YToCheck + Y;
                    // Skip the current field itself
                    if (XToCheck == 0 && YToCheck == 0)
                    {
                        continue;
                    }
                    // Check if the neighboring field is within bounds (assuming grid size is 10x10)
                    else if (checkfieldx >= 0 && checkfieldx < numberOfRows && checkfieldy >= 0 && checkfieldy < numberOfColumns)
                    {
                        Debug.WriteLine($"This one is next to me: {checkfieldx},{checkfieldy}");

                        // Assuming you already have instances of Field objects in the fieldsArray
                        Field neighboringField = fieldsArray[checkfieldx, checkfieldy];
                        borderingFields.Add(neighboringField);
                    }
                    //CHECK <0 and >9, check if filed.row & column - Then skip
                    //COUNT NUMBER OF MINES
                    //RETURN -- LIST OF FIELDS --
                    //HAVE LOGIC FOR NUMBER OF MINES BE SOME WHERE ELSE
                }

            }



           
            return borderingFields;
        }


        public void RemoveButton(object sender)
        {
            if (sender is Button clickedButton)
            {
                // Remove the clicked button from the grid
                GrdMineField.Children.Remove(clickedButton);

                // Optional: Debugging output
                Debug.WriteLine($"Button at position {clickedButton.Content} was clicked and removed.");
            }
            Debug.WriteLine($"{sender} REMOVED!");
        }

    }
}