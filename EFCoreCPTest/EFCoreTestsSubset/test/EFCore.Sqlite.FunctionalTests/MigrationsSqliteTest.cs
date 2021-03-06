// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// NOTE: File contains modifications for EFCoreCPTest to compile for and run on additional device targets.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public class MigrationsSqliteTest : MigrationsTestBase<MigrationsSqliteFixture>
    {
        // CHANGED: See in tests below where Normalize() has wrapped expected and actual SQL strings. This project
        // tests EF Core using copies of its own tests, but with EF Core itself referenced as external assemblies
        // from an official NuGet build. Tests had been failing only on account of line ending mismatch.
        // Originally, there was a FileLineEnding field, and a simple string replacement.

        private static string Normalize(string input)
        {
            var result = Regex.Replace(input, @"\r\n?|\n", " ");
            return result;
        }
        
        public MigrationsSqliteTest(MigrationsSqliteFixture fixture)
            : base(fixture)
        {
        }

        public override void Can_generate_migration_from_initial_database_to_initial()
        {
            base.Can_generate_migration_from_initial_database_to_initial();

            Assert.Equal(
                Normalize(@"CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
    ""MigrationId"" TEXT NOT NULL CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY,
    ""ProductVersion"" TEXT NOT NULL
);

"),
                Normalize(Sql));
        }

        public override void Can_generate_no_migration_script()
        {
            base.Can_generate_no_migration_script();

            Assert.Equal(
                Normalize(@"CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
    ""MigrationId"" TEXT NOT NULL CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY,
    ""ProductVersion"" TEXT NOT NULL
);

"),
                Normalize(Sql));
        }

        public override void Can_generate_up_scripts()
        {
            base.Can_generate_up_scripts();

            Assert.Equal(
                Normalize(@"CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
    ""MigrationId"" TEXT NOT NULL CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY,
    ""ProductVersion"" TEXT NOT NULL
);

CREATE TABLE ""Table1"" (
    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Table1"" PRIMARY KEY
);

INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
VALUES ('00000000000001_Migration1', '7.0.0-test');

ALTER TABLE ""Table1"" RENAME TO ""Table2"";

INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
VALUES ('00000000000002_Migration2', '7.0.0-test');

INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
VALUES ('00000000000003_Migration3', '7.0.0-test');

"),
                Normalize(Sql));
        }

        public override void Can_generate_one_up_script()
        {
            base.Can_generate_one_up_script();

            Assert.Equal(
                Normalize(@"ALTER TABLE ""Table1"" RENAME TO ""Table2"";

INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
VALUES ('00000000000002_Migration2', '7.0.0-test');

"),
                Normalize(Sql));
        }

        public override void Can_generate_up_script_using_names()
        {
            base.Can_generate_up_script_using_names();

            Assert.Equal(
                Normalize(@"ALTER TABLE ""Table1"" RENAME TO ""Table2"";

INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
VALUES ('00000000000002_Migration2', '7.0.0-test');

"),
                Normalize(Sql));
        }

        public override void Can_generate_idempotent_up_scripts()
        {
            Assert.Throws<NotSupportedException>(() => base.Can_generate_idempotent_up_scripts());
        }

        public override void Can_generate_down_scripts()
        {
            base.Can_generate_down_scripts();

            Assert.Equal(
                Normalize(@"ALTER TABLE ""Table2"" RENAME TO ""Table1"";

DELETE FROM ""__EFMigrationsHistory""
WHERE ""MigrationId"" = '00000000000002_Migration2';

DROP TABLE ""Table1"";

DELETE FROM ""__EFMigrationsHistory""
WHERE ""MigrationId"" = '00000000000001_Migration1';

"),
                Normalize(Sql));
        }

        public override void Can_generate_one_down_script()
        {
            base.Can_generate_one_down_script();

            Assert.Equal(
                Normalize(@"ALTER TABLE ""Table2"" RENAME TO ""Table1"";

DELETE FROM ""__EFMigrationsHistory""
WHERE ""MigrationId"" = '00000000000002_Migration2';

"),
                Normalize(Sql));
        }

        public override void Can_generate_down_script_using_names()
        {
            base.Can_generate_down_script_using_names();

            Assert.Equal(
                Normalize(@"ALTER TABLE ""Table2"" RENAME TO ""Table1"";

DELETE FROM ""__EFMigrationsHistory""
WHERE ""MigrationId"" = '00000000000002_Migration2';

"),
                Normalize(Sql));
        }

        public override void Can_generate_idempotent_down_scripts()
        {
            Assert.Throws<NotSupportedException>(() => base.Can_generate_idempotent_down_scripts());
        }

        public override void Can_get_active_provider()
        {
            base.Can_get_active_provider();

            Assert.Equal("Microsoft.EntityFrameworkCore.Sqlite", ActiveProvider);
        }

        protected override void AssertFirstMigration(DbConnection connection)
        {
            var sql = GetDatabaseSchemaAsync(connection);
            Assert.Equal(
                Normalize(@"
CreatedTable
    Id INTEGER NOT NULL
    ColumnWithDefaultToDrop INTEGER NULL DEFAULT 0
    ColumnWithDefaultToAlter INTEGER NULL DEFAULT 1
"),
                Normalize(sql));
        }

        protected override void BuildSecondMigration(MigrationBuilder migrationBuilder)
        {
            base.BuildSecondMigration(migrationBuilder);

            for (var i = migrationBuilder.Operations.Count - 1; i >= 0; i--)
            {
                var operation = migrationBuilder.Operations[i];
                if (operation is AlterColumnOperation
                    || operation is DropColumnOperation)
                {
                    migrationBuilder.Operations.RemoveAt(i);
                }
            }
        }

        protected override void AssertSecondMigration(DbConnection connection)
        {
            var sql = GetDatabaseSchemaAsync(connection);
            Assert.Equal(
                Normalize(@"
CreatedTable
    Id INTEGER NOT NULL
    ColumnWithDefaultToDrop INTEGER NULL DEFAULT 0
    ColumnWithDefaultToAlter INTEGER NULL DEFAULT 1
"),
                Normalize(sql));
        }

        private string GetDatabaseSchemaAsync(DbConnection connection)
        {
            var builder = new IndentedStringBuilder();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT name
                FROM sqlite_master
                WHERE type = 'table'
                ORDER BY name;";

            var tables = new List<string>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
            }

            var first = true;
            foreach (var table in tables)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.DecrementIndent();
                }

                builder
                    .AppendLine()
                    .AppendLine(table)
                    .IncrementIndent();

                command.CommandText = "PRAGMA table_info(" + table + ");";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        builder
                            .Append(reader[1]) // Name
                            .Append(" ")
                            .Append(reader[2]) // Type
                            .Append(" ")
                            .Append(reader.GetBoolean(3) ? "NOT NULL" : "NULL");

                        if (!reader.IsDBNull(4))
                        {
                            builder
                                .Append(" DEFAULT ")
                                .Append(reader[4]);
                        }

                        builder.AppendLine();
                    }
                }
            }

            return builder.ToString();
        }
    }
}
