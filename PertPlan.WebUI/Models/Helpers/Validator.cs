using Microsoft.Extensions.Localization;

namespace PertPlan.WebUI.Models.Helpers
{
    public static class Validator
    {
        private static IStringLocalizer _localizer;
        private const int _NAME_LENGTH = 100;

        public static void SetLocalizer(IStringLocalizer localizer)
        {
            _localizer = localizer;
        }

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

        public static void ValidateTaskAverageTime(double positiveTime, double averageTime, double negativeTime)
        {
            if (averageTime < positiveTime)
            {
                throw new ArgumentException(_localizer["Average finish time can not be shorter than positive."]);
            }
            else if (averageTime > negativeTime)
            {
                throw new ArgumentException(_localizer["Average finish time can not be longer than negative."]);
            }
        }

        public static void ValidateNegativeTimeInput(double averageTime, double negativeTime)
        {
            if (negativeTime < averageTime)
            {
                throw new ArgumentException(_localizer["Negative finish time can not be shorter than average."]);
            }
        }

        public static void ValidateDependentTasksInput(string dependentTasks, int taskNumber)
        {
            var tasksNumbers = dependentTasks.Split(',');

            foreach (var numberStr in tasksNumbers)
            {
                if (!int.TryParse(numberStr.Trim(), out var number))
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
