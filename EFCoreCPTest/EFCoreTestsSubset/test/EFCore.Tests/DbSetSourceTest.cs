// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// NOTE: File contains modifications for EFCoreCPTest to compile for and run on additional device targets.

using System;
using Microsoft.EntityFrameworkCore.Internal;
// CHANGED: WAS: using Moq; // which can't be used in Xamarin.iOS
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public class DbSetSourceTest
    {
        /* CHANGED: Can't use Moq in Xamarin.iOS
        [Fact]
        public void Can_create_new_generic_DbSet()
        {
            var context = new Mock<DbContext>().Object;

            var factorySource = new DbSetSource();

            var set = factorySource.Create(context, typeof(Random));

            Assert.IsType<InternalDbSet<Random>>(set);
        } */

        /* CHANGED: Can't use Moq in Xamarin.iOS
        [Fact]
        public void Always_creates_a_new_DbSet_instance()
        {
            var context = new Mock<DbContext>().Object;

            var factorySource = new DbSetSource();

            Assert.NotSame(factorySource.Create(context, typeof(Random)), factorySource.Create(context, typeof(Random)));
        } */
    }
}
