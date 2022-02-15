// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using log4net;

namespace MappingEdu.Service.Providers
{
    public interface ILoggingProvider<T>
    {
        void LogInfo(string infoMessage);

        void LogError(string errorMessage);

        void LogWarn(string warnMessage);

        void LogDebug(string debugMessage);

        void LogFatal(string fatalMessage);
    }

    public class LoggingProvider<T> : ILoggingProvider<T>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof (T));

        public void LogError(string errorMessage)
        {
            _logger.Error(errorMessage);
        }

        public void LogInfo(string infoMessage)
        {
            _logger.Info(infoMessage);
        }

        public void LogWarn(string warnMessage)
        {
            _logger.Warn(warnMessage);
        }

        public void LogDebug(string debugMessage)
        {
            _logger.Debug(debugMessage);
        }

        public void LogFatal(string fatalMessage)
        {
            _logger.Fatal(fatalMessage);
        }
    }
}