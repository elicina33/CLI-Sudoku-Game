using System;
using System.Threading;

class Sudoku
{

    public static Random random = new Random();
    public static int lives = 4;
    public static int score = 0;
    public static int mistakes = 0;

    public static bool isValid(int[,] matrix, int row, int col, int number)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            if (matrix[row, i] == number)
                return false;
        }
        for (int j = 0; j < matrix.GetLength(1); j++)
        {
            if (matrix[j, col] == number)
                return false;
        }
        int size = matrix.GetLength(0);
        int box = (int)Math.Sqrt(size);
        int startRow = row - row % box;
        int startCol = col - col % box;
        for (int i = 0; i < box; i++)
        {
            for (int j = 0; j < box; j++)
            {
                if (matrix[startRow + i, startCol + j] == number)
                    return false;
            }
        }
        return true;
    }
    public static void mix(int[] array)
    {

        for (int i = 0; i < array.Length; i++)
        {
            int randIndex = random.Next(i, array.Length);
            int temp = array[i];
            array[i] = array[randIndex];
            array[randIndex] = temp;
        }
    }
    public static bool fillMatrix(int[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (matrix[i, j] == 0)
                {
                    // --------- OPTION 1: 9x9 ----------
                    if (matrix.GetLength(0) == 9)
                    {
                        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                        mix(numbers);

                        for (int n = 0; n < numbers.Length; n++)
                        {
                            if (isValid(matrix, i, j, numbers[n]))
                            {
                                matrix[i, j] = numbers[n];
                                if (fillMatrix(matrix))
                                    return true;
                                matrix[i, j] = 0;
                            }
                        }
                        return false;
                    }

                    // --------- OPTION 2: 16x16 ----------
                    if (matrix.GetLength(0) == 16)
                    {
                        int[] numbers = new int[16];
                        for (int k = 0; k < 16; k++)
                            numbers[k] = k + 1;

                        mix(numbers);

                        for (int n = 0; n < numbers.Length; n++)
                        {
                            if (isValid(matrix, i, j, numbers[n]))
                            {
                                matrix[i, j] = numbers[n];
                                if (fillMatrix(matrix))
                                    return true;
                                matrix[i, j] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public static bool findLeastEmptyCell(int[,] matrix, out int row, out int col)
    {
        int size = matrix.GetLength(0);
        row = -1; col = -1;//indicating that there is no column or row

        int leastCount = size + 1;

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if (matrix[r, c] == 0)
                {
                    int count = 0;
                    for (int num = 1; num <= size; num++)
                        if (isValid(matrix, r, c, num))
                        { count++; }

                    if (count < leastCount)
                    {
                        leastCount = count;
                        row = r; col = c;
                        if (leastCount <= 1) return true;
                    }
                }


            }
        }

        return row != -1;// returns false when there are no empty cells
    }



    public static int countSolutions(int[,] matrix, int limit)
    {
        int size = matrix.GetLength(0);


        if (!findLeastEmptyCell(matrix, out int row, out int col))
            return 1;

        int solutions = 0;

        for (int num = 1; num <= size; num++)
        {
            if (isValid(matrix, row, col, num))
            {
                matrix[row, col] = num;
                solutions += countSolutions(matrix, limit);

                if (solutions >= limit)
                {
                    matrix[row, col] = 0;
                    return limit; 
                }

                matrix[row, col] = 0;
            }
        }

        return solutions;
    }


    public static bool uniqueSolution(int[,] matrix)
    {
        int[,] copy = (int[,])matrix.Clone();
        int limit = 2;
        return countSolutions(copy, limit) == 1;
    }

    public static void removeNumbers(int[,] matrix, int count)
    {

        int attempts = 0;//number of attempts that program has tried to delete cell
        int size = matrix.GetLength(0);
        int maxAttempts = size * size * 200;//making sure that program does not delete infinitly many times

        while (count > 0 && attempts < maxAttempts)
        {
            attempts++;
            int row = random.Next(matrix.GetLength(0));
            int col = random.Next(matrix.GetLength(1));
            if (matrix[row, col] != 0)
            {
                int backup = matrix[row, col];
                matrix[row, col] = 0;

                if (!uniqueSolution(matrix))
                {
                    matrix[row, col] = backup;//if it doesnt have unique solution it goes back (backtracking)

                }
                else
                {
                    count--;
                }
            }
        }
    }

    public static void separate(int[,] matrix, int box, int digits)
    {
        int size = matrix.GetLength(0);
        int segment = box * (digits + 2);
        string separator = new string('-', segment);

        for (int s = 0; s < size / box; s++)
        { Console.Write("+" + separator); }

        Console.WriteLine("+");



    }

    public static void printMatrix(int[,] matrix)
    {


        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        int box = (int)Math.Sqrt(rows);
        int digits = rows.ToString().Length;

        separate(matrix, box, digits);

        for (int i = 0; i < rows; i++)
        {
            if (i > 0 && i % box == 0)
            {
                separate(matrix, box, digits);
            }


            Console.Write("|");
          
            for (int j = 0; j < cols; j++)
            {
                if (matrix[i, j] == 0)
                {
                    string emptySpace = new string('.', digits);
                    Console.Write(" " + emptySpace + " ");
                }


                else
                {
                    string text = matrix[i, j].ToString();
                    string alignedText = text.PadLeft(digits);
                    Console.Write(" " + alignedText + " ");
                    

                }


                if ((j + 1) % box == 0)
                    Console.Write("|");

            }
            if (i == 0)
                Console.Write("  SCORE: "+score);
            else if(i==1)
                Console.Write("  MISTAKES: " + mistakes);
            else if(i == 2)
                Console.Write("  LIVES LEFT : " + lives);

            Console.WriteLine();

        }
        separate(matrix, box, digits);

    }



    public static int choose_level(int dimension)
    {
        
        Console.Write("Choose level (enter number): 1.Easy  2.Medium  3.Hard ");
        Console.WriteLine();
        

        try
        {
            int level = int.Parse(Console.ReadLine());
            if (dimension == 9)
            {
                switch (level)
                {
                    case 1:
                        return 2;
                    case 2:
                        return 48;
                    case 3:
                        return 53;
                    default:
                        Console.Write("Invalid input!");
                        Console.WriteLine();
                        return choose_level(dimension);
                }
            }
            else if (dimension == 16)
            {
                switch (level)
                {
                    case 1:
                        return 90;
                    case 2:
                        return 110;
                    case 3:
                        return 130;
                    default:
                        Console.Write("Invalid input!");
                        Console.WriteLine();
                        return choose_level(dimension);
                }
            }
            else
            {
                throw new ArgumentException("Invalid dimension value.");
            }
        }
        catch
        {
            Console.Write("Invalid input");
            Console.WriteLine();
            return choose_level(dimension);
        }
    }

    public static int[,] choose_matrix()
    {
        
       
        Console.Write("Choose matrix type (enter number): 1.9x9  2. 16x16 ");
        Console.WriteLine();
       


        try
        {
            int dimension = int.Parse(Console.ReadLine());

            if (dimension == 1)
            {
                return new int[9, 9];
            }
            else if (dimension == 2)
            {
                return new int[16, 16];
            }

            else
            {
                Console.Write("Invalid input!");
                Console.WriteLine();
                return choose_matrix();
            }

        }

        catch
        {
            Console.Write("Invalid input");
            Console.WriteLine();
            return choose_matrix();
        }
       
    }
    public static (int row, int column, int value) userPlaceInput(int[,] matrix, int[,] solutionMatrix)
    {
        try
        {
            int row;
            int column;
            int value;
           
            Console.Write("Enter row you want to access:\n");
            row = int.Parse(Console.ReadLine());
            row = row - 1;
            
            while (row > matrix.GetLength(0) - 1 || row<0)
            {
                Console.Write("Row needs to be in range from 1 to "+matrix.GetLength(0)+".\n\n");
                Console.Write("Enter row you want to access:\n");
                row = int.Parse(Console.ReadLine());
                row = row - 1;


            }
            Console.Clear();
            printMatrix(matrix);

            Console.Write("Enter column you want to access:\n");
            column = int.Parse(Console.ReadLine());
            column = column - 1;

            while (column > matrix.GetLength(1) - 1 || column<0)
            {
                Console.Write("Column needs to be in range from 1 to " + matrix.GetLength(1)+ ".\n\n");
                Console.Write("Enter column you want to access:\n");
                column = int.Parse(Console.ReadLine());
                column = column - 1;

            }
            Console.Clear();
            printMatrix(matrix);
            if (matrix[row, column] != 0)
            {
                Console.Write("Cell not empty.\n");
                return userPlaceInput(matrix, solutionMatrix);
            }
            Console.Clear();
            printMatrix(matrix);
            while (true)
            {
                try
                {
                    Console.Write("Enter the number you want in that place: ");
                    value = int.Parse(Console.ReadLine());
                    if (value < 0 || value > matrix.GetLength(0))
                    {
                        Console.Write("Value is not valid. It has to be 1-"+matrix.GetLength(0)+"\n");
                        continue;
                    }
                    if (!userNumberValidateInput(solutionMatrix, row, column, value))
                    {
                        if (lives <= 0)
                        {
                            return (-1, -1, -1); // signal for end of the game
                        }
                        Console.Write("Do you want to continue solving this cell? Answer with yes or no.\n");
                        string answer = Console.ReadLine();
                        if (answer == "yes")
                        {
                            Console.Clear();
                            printMatrix(matrix);
                            continue;
                        }
                        else if (answer == "no")
                        {   Console.Clear();
                            printMatrix(matrix);
                            Console.Write("Now choose another cell!\n");
                            return userPlaceInput(matrix, solutionMatrix);
                        }



                        while (answer != "yes" && answer != "no")
                        {
                            Console.Clear();
                            printMatrix(matrix);
                            Console.Write("Invalid input! Please enter yes or no\n");
                            Console.Write("Do you want to continue solving this cell? Answer with yes or no.\n");
                            answer = Console.ReadLine();
                        }
                        if (answer == "no")
                        {   Console.Clear();
                            printMatrix(matrix);
                            Console.Write("Now choose another cell!\n");
                            return userPlaceInput(matrix, solutionMatrix);
                        }




                    }

                    else
                    {
                        matrix[row, column] = value;
                        completePartOfMatrix(matrix, row, column);
                        if (lives <= 0)
                        {
                            return (-1, -1, -1); 
                        }
                        break;
                    }
                }
                catch
                {
                    Console.Write("Invalid input. You need to enter the value again. \n\n");
                    continue;
                }

            }

            return (row, column, value);
        }
        catch
        {
            Console.Clear();
            printMatrix(matrix);
            Console.Write("Invalid input. You need to enter row and column again. \n\n");
            return userPlaceInput(matrix, solutionMatrix);
        }
    }
    public static bool userNumberValidateInput(int[,] solutionMatrix, int row, int column, int number)
    {
        if (number == solutionMatrix[row, column])
        {
            Console.Write("Correct move! +10 points!!! \n");
            score += 10;
            return true;
        }
        Console.Write("Incorrect move! -5 points!!! \n");
        lives--;
        score -= 5;
        mistakes++;
        
        if (lives <= 0)
        {
            Console.Write("No more lives left!\n");
            return false;

        }

        return false;
    }
    public static void completePartOfMatrix(int[,] matrix, int row, int column)
    {
        bool isRowComplete = true;
        bool isColumnComplete = true;
        bool isBoxComplete = true;
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            if (matrix[row, i] == 0)
            {
                isRowComplete = false;
                break;
            }

        }
        if (isRowComplete)
        {
            Console.Write("Row completed! You got +20 points!\n");
            score += 20;
        }

        for (int j = 0; j < matrix.GetLength(1); j++)
        {
            if (matrix[j, column] == 0)
            {
                isColumnComplete = false;
                break;

            }
        }
        if (isColumnComplete)
        {
            Console.Write("Column completed! You got +20 points!\n");
            score += 20;
        }
        int size = matrix.GetLength(0);
        int box = (int)Math.Sqrt(size);
        int startRow = row - row % box;
        int startCol = column - column % box;
        for (int i = 0; i < box; i++)
        {
            for (int j = 0; j < box; j++)
            {
                if (matrix[startRow + i, startCol + j] == 0)
                {
                    isBoxComplete = false;
                    goto endBoxCheck;
                }

            }
        }
    endBoxCheck:
        if (isBoxComplete)
        {
            score += 10;
            Console.Write("Box completed! You got +20 points!\n");
        }


    }
    public static void endScreen()
    {
        Thread.Sleep(5000);
        Console.Clear();
        int blinking = 0;
        string endGameTitle = new string('#', 20);
        string resultsTitle = new string('-', 20);
        Console.WriteLine("\n\n\n\n\n\n");
        while (blinking < 4)
        {

            Thread.Sleep(500);
            Console.Write(endGameTitle + " GAME OVER " + endGameTitle + "\n");
            Thread.Sleep(700);
            Console.Clear();
            blinking++;
            Console.WriteLine("\n\n\n\n\n\n");

        }
        Console.Clear();
        Console.Write(resultsTitle + " GAME RESULTS " + resultsTitle + "\n");
        Console.Write("LIVES REMAINING: " + lives + "\n");
        Console.Write("TOTAL SCORE: " + score + "\n");
        Console.Write("MISTAKES MADE: " + mistakes + "\n");


        Console.WriteLine("\nPress ENTER to exit...");
        Console.ReadLine();

    }

    public static bool isMatrixCompleted(int [,] matrix, int[,] solutionMatrix)
    {
        for (int i=0;i<matrix.GetLength(0);i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (matrix[i, j] != solutionMatrix[i, j])
                    return false;
            }
        }
        return true;
        
    }
    static void Main()
    {
        string titleSeparator = new string('*', 19);
        Console.WriteLine();
        Console.WriteLine(titleSeparator + "WELCOME TO SUDOKU GAME" + titleSeparator + "\n\n");



        int[,] matrix;

        
        matrix = choose_matrix();
        Console.Clear();
        int numberOfEmptySpaces = choose_level(matrix.GetLength(0));
        Console.Clear();
        fillMatrix(matrix);
        int[,] solutionMatrix = (int[,])matrix.Clone();
        removeNumbers(matrix, numberOfEmptySpaces);
        Console.WriteLine(titleSeparator + "YOUR SUDOKU GAME WILL START SOON" + titleSeparator);
        Console.WriteLine(titleSeparator + " (Please wait 4 seconds) " + titleSeparator + "\n\n");
        printMatrix(matrix);
        while (lives > 0 && !isMatrixCompleted(matrix,solutionMatrix))
        {
            Thread.Sleep(4000);
            Console.Clear();
            printMatrix(matrix);
            (int row, int column, int value) = userPlaceInput(matrix, solutionMatrix);
            if (row == -1) 
                break;
        }
        if (score < 0)
            score = 0;

        endScreen();




    }
}