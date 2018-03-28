// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// NOTE: File contains modifications for EFCoreCPTest to compile for and run on additional device targets.

using System;
// CHANGED: WAS: using Moq; // which can't be used in Xamarin.iOS
using Xunit;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class ClrPropertyGetterFactoryTest
    {
        /* CHANGED: Can't use Moq in Xamarin.iOS
        [Fact]
        public void Property_is_returned_if_it_implements_IClrPropertyGetter()
        {
            var getterMock = new Mock<IClrPropertyGetter>();
            var propertyMock = getterMock.As<IProperty>();

            var source = new ClrPropertyGetterFactory();

            Assert.Same(getterMock.Object, source.Create(propertyMock.Object));
        } */

        [Fact]
        public void Delegate_getter_is_returned_for_IProperty_property()
        {
            var entityType = new Model().AddEntityType(typeof(Customer));
            var idProperty = entityType.AddProperty("Id", typeof(int));

            Assert.Equal(7, new ClrPropertyGetterFactory().Create(idProperty).GetClrValue(new Customer { Id = 7 }));
        }

        [Fact]
        public void Delegate_getter_is_returned_for_property_info()
        {
            Assert.Equal(7, new ClrPropertyGetterFactory().Create(typeof(Customer).GetAnyProperty("Id")).GetClrValue(new Customer { Id = 7 }));
        }

        #region Fixture

        private class Customer
        {
            internal int Id { get; set; }
        }

        #endregion
    }
}
