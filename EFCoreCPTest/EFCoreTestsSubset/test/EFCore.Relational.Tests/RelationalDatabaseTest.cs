// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// NOTE: File contains modifications for EFCoreCPTest to compile for and run on additional device targets.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.DependencyInjection;
// CHANGED: WAS: using Moq; // which can't be used in Xamarin.iOS
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    /* CHANGED: Can't use Moq in Xamarin.iOS
    public class RelationalDatabaseTest
    {
        [Fact]
        public async Task SaveChangesAsync_delegates()
        {
            var commandBatchPreparerMock = new Mock<ICommandBatchPreparer>();
            var batchExecutorMock = new Mock<IBatchExecutor>();
            var relationalConnectionMock = new Mock<IRelationalConnection>();
            var fragmentTranslatorMock = new Mock<IExpressionFragmentTranslator>();

            var customServices = new ServiceCollection()
                .AddSingleton(commandBatchPreparerMock.Object)
                .AddSingleton(batchExecutorMock.Object)
                .AddSingleton(relationalConnectionMock.Object)
                .AddSingleton(fragmentTranslatorMock.Object)
                .AddScoped<RelationalDatabase>();

            var contextServices = RelationalTestHelpers.Instance.CreateContextServices(customServices);

            var relationalDatabase = contextServices.GetRequiredService<RelationalDatabase>();

            var entries = new List<InternalEntityEntry>();
            var cancellationToken = new CancellationTokenSource().Token;

            await relationalDatabase.SaveChangesAsync(entries, cancellationToken);

            commandBatchPreparerMock.Verify(c => c.BatchCommands(entries));
            batchExecutorMock.Verify(be => be.ExecuteAsync(It.IsAny<IEnumerable<ModificationCommandBatch>>(), relationalConnectionMock.Object, cancellationToken));
        }

        [Fact]
        public void SaveChanges_delegates()
        {
            var commandBatchPreparerMock = new Mock<ICommandBatchPreparer>();
            var batchExecutorMock = new Mock<IBatchExecutor>();
            var relationalConnectionMock = new Mock<IRelationalConnection>();

            var fragmentTranslatorMock = new Mock<IExpressionFragmentTranslator>();

            var customServices = new ServiceCollection()
                .AddSingleton(commandBatchPreparerMock.Object)
                .AddSingleton(batchExecutorMock.Object)
                .AddSingleton(relationalConnectionMock.Object)
                .AddSingleton(fragmentTranslatorMock.Object)
                .AddScoped<RelationalDatabase>();

            var contextServices = RelationalTestHelpers.Instance.CreateContextServices(customServices);

            var relationalDatabase = contextServices.GetRequiredService<RelationalDatabase>();

            var entries = new List<InternalEntityEntry>();

            relationalDatabase.SaveChanges(entries);

            commandBatchPreparerMock.Verify(c => c.BatchCommands(entries));
            batchExecutorMock.Verify(be => be.Execute(It.IsAny<IEnumerable<ModificationCommandBatch>>(), relationalConnectionMock.Object));
        }
    } */
}
