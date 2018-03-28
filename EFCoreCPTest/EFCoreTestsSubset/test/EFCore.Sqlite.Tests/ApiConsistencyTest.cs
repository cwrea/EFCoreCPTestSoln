// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// NOTE: File contains modifications for EFCoreCPTest to compile for and run on additional device targets.

using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore
{
    public class ApiConsistencyTestSqlite : ApiConsistencyTestBase // CHANGED name only to avoid conflict
    {
        protected override void AddServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityFrameworkSqlite();
        }

        protected override Assembly TargetAssembly => typeof(SqliteRelationalConnection).GetTypeInfo().Assembly;
    }
}
