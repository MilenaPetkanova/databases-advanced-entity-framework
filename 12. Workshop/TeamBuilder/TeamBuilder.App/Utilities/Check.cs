namespace TeamBuilder.App.Utilities
{
    using System;

    public static class Check
    {
        public static void CheckLength(int expectedLenght, string[] array)
        {
            if (!expectedLenght.Equals(array.Length))
            {
                throw new FormatException(Constants.ErrorMessages.InvalidArgumentsCount);
            }
        }
    }
}
