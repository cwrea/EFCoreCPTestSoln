// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// NOTE: File contains modifications for EFCoreCPTest to compile for and run on additional device targets.

using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore
{
    public class ApiConsistencyTestRelational : ApiConsistencyTestBase // CHANGED name only to avoid conflict
    {
        protected override void AddServices(ServiceCollection serviceCollection)
        {
            new EntityFrameworkRelationalServicesBuilder(serviceCollection).TryAddCoreServices();
        }

        protected override Assembly TargetAssembly => typeof(RelationalDatabase).GetTypeInfo().Assembly;
    }
}
