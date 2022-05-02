using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlertsApi.TgAlerts.Worker.Models;

namespace AlertsApi.TgAlerts.Worker.Services
{
    public interface ITelegramBotService
    {
        Task Invoke(string locationName, string message);
        Task Start();
    }
}
