// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore
{
    public class ConcurrencyDetectorSqliteTest : ConcurrencyDetectorRelationalTest<NorthwindQuerySqliteFixture>
    {
        public ConcurrencyDetectorSqliteTest(NorthwindQuerySqliteFixture fixture)
            : base(fixture)
        {
        }
    }
}
