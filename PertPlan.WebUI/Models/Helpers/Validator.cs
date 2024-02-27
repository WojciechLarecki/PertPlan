namespace PertPlan.WebUI.Models.Helpers
{
    public static class Validator
    {
        public static void ValidateTaskName(string name)
        {
            string trimmedName = name.Trim();
            if (string.IsNullOrEmpty(trimmedName))
            {
                throw new ArgumentException("Name is empty or has only spaces in it.");
            }
            if (trimmedName.Length > 100)
            {
                throw new ArgumentException("Name is too long. Max size is 100 characters.");
            }
        }

        public static void ValidateTaskAverageTime(double positiveTime, double averageTime, double negativeTime)
        {
            if (averageTime < positiveTime)
            {
                throw new ArgumentException("Średni czas wykonania zadania nie może być krótszy niż pozytywny.");
            }
            else if (averageTime > negativeTime)
            {
                throw new ArgumentException("Średni czas wykonania zadania nie może być dłuższy niż negatywny.");
            }
        }

        public static void ValidateNegativeTimeInput(double averageTime, double negativeTime)
        {
            if (negativeTime < averageTime)
            {
                throw new ArgumentException("Negatywny czas wykonania zadania nie może być krótszy niż średni.");
            }
        }

        public static void ValidateDependentTasksInput(string dependentTasks, int taskNumber)
        {
            var tasksNumbers = dependentTasks.Split(',');

            foreach (var numberStr in tasksNumbers)
            {
                if (!int.TryParse(numberStr.Trim(), out var number))
                {
                    throw new ArgumentException("Ciąg zadań ma niepoprawną strukturę.");
                }
                else if (number < 0)
                {
                    throw new ArgumentException("Zadania nie mają ujemnych numerów.");
                }
                else if (number > taskNumber)
                {
                    throw new ArgumentException("Zadanie nie może polegać na jeszcze niezdefiniowanym zadaniu.");
                }
                else if (number == taskNumber)
                {
                    throw new ArgumentException("Zadanie nie może polegać na samym sobie.");
                }
            }
        }
    }
}
