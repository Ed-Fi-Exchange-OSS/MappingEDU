// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Common
{
    /// <summary>
    ///     Application constants
    /// </summary>
    public static class Constants
    {
        public class Api
        {
            public class V1
            {
                public const string AccessTokenRoute = "/" + RoutePrefix + "/accesstoken"; // oauth requires leading slash
                public const string RoutePrefix = "api";
            }
        }

        public static class DataAccess
        {
            public const string AdminConnectionStringName = "DbAdmin";
            public const string ConnectionStringName = "MappingEDU";
            public const int DefaultRecordsPerQuery = 100;
            public const string Schema = "dbo";
        }

        public static class Permissions
        {
            public const string Administrator = "admin";
            public const string Guest = "guest";
            public const string User = "user";
        }

        public static class ResponseHeaders
        {
            public static string ErrorCode = "X-Error-Code";
            public static string Hint = "X-Hint";
            public static string HintCode = "X-Hint-Code";
            public static string TotalCount = "X-Total-Count";
        }

        public static class Security
        {
            public static string GuestPassword = "guest9999";
            public static string GuestUsername = "guest@example.com";
        }
    }
}
