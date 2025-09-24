using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;

namespace EmployeeManagement.Helpers
{
    public static class AlertExtensions
    {
        private static void AddMessage(Controller controller, string key, string message)
        {
            // Try to get existing messages
            List<string> messages;
            if (controller.TempData.ContainsKey(key))
            {
                messages = JsonSerializer.Deserialize<List<string>>(controller.TempData[key].ToString());
            }
            else
            {
                messages = new List<string>();
            }

            messages.Add(message);

            // Store back as JSON
            controller.TempData[key] = JsonSerializer.Serialize(messages);
        }

        public static void AddSuccessMessage(this Controller controller, string message)
        {
            AddMessage(controller, "SuccessMessages", message);
        }

        public static void AddWarningMessage(this Controller controller, string message)
        {
            AddMessage(controller, "WarningMessages", message);
        }

        public static void AddErrorMessage(this Controller controller, string message)
        {
            AddMessage(controller, "ErrorMessages", message);
        }

        public static void AddInfoMessage(this Controller controller, string message)
        {
            AddMessage(controller, "InfoMessages", message);
        }
    }
}
