// ConsoleHelper.cs

namespace PlantCareTracker.Utils
{
    public static class ConsoleHelper
    {
        // CenterText()

        public static string CenterText(string text, int width)
        /*
        Centers a string within a fixed width.
        */
        {
            if (string.IsNullOrEmpty(text))
                text = "";

            int padding = width - text.Length;

            if (padding <= 0)
                return text;

            int padLeft = padding / 2 + text.Length;

            return text.PadLeft(padLeft).PadRight(width);
        }

        //*****************************************************************************
        // ReadRequiredInput()

        public static string ReadRequiredInput(string label)
        /*
        Reads a required string input from the user.

        Ensures the input is not empty or whitespace.
        */
        {
            while (true)
            {
                Console.Write($"{label}: ");
                string input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                    return input;

                Console.WriteLine("Input cannot be empty. Try again.");
            }
        }

        //*****************************************************************************
        // ReadIntInput()

        public static int ReadIntInput(string label)
        /*
        Reads a valid integer input from the user.

        Ensures the value is a number greater than 0.
        */
        {
            while (true)
            {
                Console.Write($"{label}: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int value) && value > 0)
                    return value;

                Console.WriteLine("Invalid number. Enter a value greater than 0.");
            }
        }
    }
}