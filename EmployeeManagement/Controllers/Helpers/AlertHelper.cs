using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EmployeeManagement.Helpers
{
    public static class AlertHelper
    {
        public static void AddSuccessMessage(Controller controller, string message) =>
            AddMessage(controller, "SuccessMessages", message);

        public static void AddErrorMessage(Controller controller, string message) =>
            AddMessage(controller, "ErrorMessages", message);

        public static void AddWarningMessage(Controller controller, string message) =>
            AddMessage(controller, "WarningMessages", message);

        public static void AddInfoMessage(Controller controller, string message) =>
            AddMessage(controller, "InfoMessages", message);

        private static void AddMessage(Controller controller, string key, string message)
        {
            var messages = new List<string>();

            if (controller.TempData[key] != null)
            {
                try
                {
                    messages = JsonSerializer.Deserialize<List<string>>(controller.TempData[key].ToString());
                }
                catch
                {
                    messages = new List<string>();
                }
            }

            messages.Add(message);
            controller.TempData[key] = JsonSerializer.Serialize(messages);
        }
    }
}
