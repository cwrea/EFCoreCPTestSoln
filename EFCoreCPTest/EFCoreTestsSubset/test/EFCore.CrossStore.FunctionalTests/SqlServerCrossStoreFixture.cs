// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// NOTE: File contains modifications for EFCoreCPTest to compile for and run on additional device targets.

using System;
using Microsoft.EntityFrameworkCore.TestModels;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    /* CHANGED: No SQL Server on Xamarin.iOS
    public class SqlServerCrossStoreFixture : CrossStoreFixture
    {
        private readonly SharedCrossStoreFixture _sharedCrossStoreFixture;

        public SqlServerCrossStoreFixture()
        {
            _sharedCrossStoreFixture = new SharedCrossStoreFixture(
                new ServiceCollection()
                    .AddEntityFrameworkSqlServer()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider());
        }

        public override TestStore CreateTestStore(Type testStoreType)
        {
            Assert.Equal(typeof(SqlServerTestStore), testStoreType);

            return SqlServerTestStore.Create("SqlServerCrossStore");
        }

        public override CrossStoreContext CreateContext(TestStore testStore)
            => _sharedCrossStoreFixture.CreateContext(testStore);
    } */
}
