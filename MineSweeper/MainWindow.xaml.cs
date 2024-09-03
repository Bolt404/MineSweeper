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
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Threading;

namespace MineSweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DispatcherTimer _timer;
        private int _elapsedTime;

        private int _revealedFieldsCount;

        Random random = new Random();

        int numberOfColumns;
        int numberOfRows;
        int numberOfMines;

        Field[,] fieldsArray;
        List<Field> minesArray = new List<Field>();

        bool gameOver = false;
        int gameOverCnt = 0;

        private int _countClicks;
        public int CountClicks
        {
            get { return _countClicks; }
            set
            {
                Debug.WriteLine($"CountClicks: {value} SETTER");
                _countClicks = value;
                OnPropertyChanged(nameof(CountClicks));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Timer_Tick(object sender, EventArgs e)
        {
            _elapsedTime++;
            TbTime.Text = FormatTime(_elapsedTime);  
        }


        protected void OnPropertyChanged(string propertyName)
        {
            Debug.WriteLine($"PropertyChanged: {propertyName}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public MainWindow()
        {
            //xaml stuff
            InitializeComponent();

            // Timer initialization
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;

            //Counting my grid
            numberOfColumns = GrdMineField.ColumnDefinitions.Count;
            numberOfRows = GrdMineField.RowDefinitions.Count;

            //fields array lets go.
            fieldsArray = new Field[numberOfRows, numberOfColumns];

            //InitGame yes please.
            this.DataContext = this;
            InitGame();

            
        }

        // Need to format this apperently
        private string FormatTime(int elapsedTime)
        {
            int minutes = elapsedTime / 60;
            int seconds = elapsedTime % 60;
            return $"{minutes:00}:{seconds:00}";
        }

        //  Init game setting up th game.
        public void InitGame()
        {
            _revealedFieldsCount = 0;

            _elapsedTime = 0;
            TbTime.Text = "00:00";

            gameOverCnt = 0;
            gameOver = false;

            CountClicks = 0;

            fieldsArray = new Field[numberOfRows, numberOfColumns];

            GrdMineField.Children.Clear();

            SetupMineField();
            SetupMines(10);
            CheckForMines();

            _timer.Start();
        }

        //Checking for mines
        public void CheckForMines()
        {
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
                

            }
        }


        private void RevealField(Field field)
        {

           
            if (field.IsMine && field != null)
            {
                
                field.Button.Content = "💣";

                field.Button.IsEnabled = false;
                field.Button.Background = Brushes.Red;

                gameOver = true;    
                    
            }  else if (!field.IsRevealed)
            {
                _revealedFieldsCount++;
                field.IsRevealed = true;
                field.Button.Content = field.AdjacentMines;
                field.Button.IsEnabled = false;
            } else
            {
                Debug.WriteLine("Something went wrong");
            }


            if (gameOver && gameOverCnt == 0)
            {
                gameOverCnt++;
                GameOver();
            } else if (!gameOver)
            {
                CheckIfPlayerWon();
            }
        }

        private void GameOver()
        {
            _timer.Stop();
            foreach (var item in fieldsArray)
            {
                RevealField(item);
            }
            // Format the message with time and clicks
            string message = $"You Lost!\n" +
                             $"Time: {_elapsedTime}\n" +
                             $"Clicks: {CountClicks}";

            // Display the message box
            MessageBox.Show(message, "Game Over!", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void SetupMineField()
        {

            int count = 1;

            for (int X = 0; X < numberOfRows; X++)
            {
                for (int Y = 0; Y < numberOfColumns; Y++)
                {
                    Button mybutton = new Button();
                    //Adds tag to button for later use
                    mybutton.Tag = (X, Y);

                    Field field = new Field(X, Y, mybutton);

                    Grid.SetColumn(mybutton, Y);
                    Grid.SetRow(mybutton, X);

                    //Row X, Column Y
                    String pos = $"{X},{Y}";

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

        private void CheckIfPlayerWon()
        {


            int totalFields = numberOfRows * numberOfColumns;

            Debug.WriteLine($"Revealed Fields: {_revealedFieldsCount} | Mines: {numberOfMines} | Total Fields: {totalFields}");
            if (_revealedFieldsCount + numberOfMines == totalFields)
            {
                gameOver = true;

                // Stop the timer
                _timer.Stop();

                // Format the message with time and clicks
                string message = $"Congratulations, you won!\n" +
                                 $"Time: {_elapsedTime}\n" +
                                 $"Clicks: {CountClicks}";

                // Display the message box
                MessageBox.Show(message, "Game Won", MessageBoxButton.OK, MessageBoxImage.Information);

                InitGame();
            }
        }

        private void FakeFloodFill(Field field)
        {
            List<Field> bordersfields = new List < Field > (CheckBorderFields(field));
            foreach (Field item in bordersfields)
            {


                if (!item.IsMine && !item.IsRevealed && item.AdjacentMines == 0)
                {
                    //We have to maniuplate the button and field here, since we are not clicking it
                    item.IsRevealed = true;
                    _revealedFieldsCount++;
                    RemoveButton(item.Button);
                    FakeFloodFill(item);
                }
                else if (!item.IsRevealed)
                {
                    RevealField(item);
                }
            }


        }

        private void Button_Field_Click( object sender, RoutedEventArgs e)
        {
            //Finding button that fits with field.
            if (sender is Button clickedButton && clickedButton.Tag is (int row, int col))
            {
                CountClicks++;
                Debug.WriteLine($"{CountClicks} clicks");
                Field findField = fieldsArray[row, col];

                //Checking if null, might be redundant.
                if (findField != null && findField.AdjacentMines == 0)
                {
                    FakeFloodFill(findField);
                    
                    Debug.WriteLine($"{findField.Row},{findField.Column}");
                    
                }
                else
                {
                    RevealField(findField);
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitGame();
        }

        public void SetupMines(int mineCount)
        {
            numberOfMines = mineCount;
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
                    //UNCOMMENT TO SEE MINES
                    //selectedField.Button.Content = "💣";

                    selectedField.IsMine = true;

                    minesPlaced++;
                    minesArray.Add(selectedField);

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

            for (int XToCheck = -1; XToCheck <= 1; XToCheck++)
            {
                for (int YToCheck = -1; YToCheck <= 1; YToCheck++)
                {
                    int checkfieldx = XToCheck + X;
                    int checkfieldy = YToCheck + Y;

                    if (checkfieldx >= 0 && checkfieldx < numberOfRows && checkfieldy >= 0 && checkfieldy < numberOfColumns)
                    {
                        Field neighboringField = fieldsArray[checkfieldx, checkfieldy];
                        borderingFields.Add(neighboringField);
                    }
                }
            }
            return borderingFields;
        }


        public void RemoveButton(object sender)
        {

            if (sender is Button clickedButton)
            {
                Debug.WriteLine($"Button at position {clickedButton.Tag} was clicked and removed.");

                GrdMineField.Children.Remove(clickedButton);
            }

        }

    }
}