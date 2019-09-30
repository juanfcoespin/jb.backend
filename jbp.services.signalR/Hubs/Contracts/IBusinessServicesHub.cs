using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TechTools.Msg;

namespace jbp.services.signalR.Hubs.Contracts
{
    /// <summary>
    /// Notifica a los clientes del tipo servicio de windows las ordenes de iniciar o parar
    /// </summary>
    public interface IBusinessServicesHub
    {
        Task Start();
        Task Stop();
        Task Status();
        Task PushLog(LogMsg me);
        //para la petición
        Task CheckIsRunning();
        //para la respuesta
        Task IsRunningResponse(bool isRunning);

        Task RequestTodayLogs();
        Task ResponseTodayLogs(List<LogMsg> logs);
        


    }
}
