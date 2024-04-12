using Microsoft.Extensions.Localization;

namespace PertPlan.WebUI.Models.Helpers
{
    /// <summary>
    /// Klasa pomocnicza zawierająca metody do walidacji danych związanych z zadaniami projektowymi.
    /// </summary>
    public static class Validator
    {
        private static IStringLocalizer _localizer;
        private const int _NAME_LENGTH = 50;


        /// <summary>
        /// Ustawia lokalizator tekstu dla walidatora.
        /// </summary>
        /// <param name="localizer">Lokalizator tekstu.</param>
        public static void SetLocalizer(IStringLocalizer localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// Waliduje nazwę zadania.
        /// </summary>
        /// <param name="name">Nazwa zadania.</param>
        public static void ValidateTaskName(string name)
        {
            string trimmedName = name.Trim();
            if (string.IsNullOrEmpty(trimmedName))
            {
                throw new ArgumentException(_localizer["Name is empty or has only spaces in it."]);
            }
            if (trimmedName.Length > _NAME_LENGTH)
            {
                throw new ArgumentException(_localizer["Name is too long. Max size is {0} characters.", _NAME_LENGTH]);
            }
        }

        /// <summary>
        /// Waliduje czas trwania zadania.
        /// </summary>
        /// <param name="positiveTime">Czas pozytywny.</param>
        /// <param name="averageTime">Czas średni.</param>
        /// <param name="negativeTime">Czas negatywny.</param>
        public static void ValidateTaskAverageTime(double positiveTime, double averageTime, double negativeTime)
        {
            if (averageTime <= positiveTime)
            {
                throw new ArgumentException(_localizer["Average finish time can not be shorter than positive."]);
            }
        }
        
        /// <summary>
        /// Waliduje wprowadzone dane dotyczące czasu ukończenia zadania w trybie negatywnym.
        /// </summary>
        /// <param name="averageTime">Czas średni.</param>
        /// <param name="negativeTime">Czas negatywny.</param>
        public static void ValidateNegativeTimeInput(double averageTime, double negativeTime)
        {
            if (negativeTime <= averageTime)
            {
                throw new ArgumentException(_localizer["Negative finish time can not be shorter than average."]);
            }
        }
        
        /// <summary>
        /// Waliduje wprowadzone dane dotyczące poprzednich zadań.
        /// </summary>
        /// <param name="dependentTasks">Poprzednie zadania.</param>
        /// <param name="taskNumber">Numer sprawdzanego zadania.</param>
        public static void ValidateDependentTasksInput(string dependentTasks, int taskNumber)
        {
            string[] tasksNumbers = dependentTasks.Split(',');

            foreach (var numberStr in tasksNumbers)
            {
                string trimmedNumberStr = numberStr.Trim().ToLower();
                if (trimmedNumberStr == "x") 
                {
                    continue;
                }
                else if (!int.TryParse(trimmedNumberStr, out var number))
                {
                    throw new ArgumentException(_localizer["Field contains incorrect data."]);
                }
                else if (number < 0)
                {
                    throw new ArgumentException(_localizer["Task can not have negative number."]);
                }
                else if (number > taskNumber)
                {
                    throw new ArgumentException(_localizer["Task can not depend on undefined task."]);
                }
                else if (number == taskNumber)
                {
                    throw new ArgumentException(_localizer["Task can not depend on itself."]);
                }
            }
        }
    }
}
