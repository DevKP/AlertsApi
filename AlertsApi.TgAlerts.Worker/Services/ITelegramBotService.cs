using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertsApi.TgAlerts.Worker.Services
{
    public interface ITelegramBotService
    {
        Task SendMessageAsync(string message);
        Task Test();
    }
}
