﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jbp.services.signalR.Hubs.Contracts
{
    /// <summary>
    /// Notifica a los clientes del tipo servicio de windows las ordenes de iniciar o parar
    /// </summary>
    public interface IBusinessServicesHub
    {
        Task Start();
        Task Stop();
    }
}