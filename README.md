
# Entity Framework Core 2.0.2 Unit Test Subset Results on Xamarin.Android and Xamarin.iOS

When I got all the pieces of this solution together in March, I built the unit test apps with various point releases of Visual Studio 15.6.x. I also tried preview releases of 15.7, and based on observed improvements, and the official 15.7 release just around the corner, I decided that release would be a good time to publish results.

VS 15.7 is out now, so here are the results for the subset of EF Core tests included in the mobile test runner app.

## Xamarin.Android EF Core 2.0.2 Unit Test Subset Results

On both Visual Studio 15.6.6 and 15.7, results were identical for Xamarin.Android. It passed most (9844) of the EF Core 2.0.2 unit tests included in the subset, failing  4 tests &mdash; plus a special case of failed test I'll cover in a moment. Here are the failed tests for Xamarin.Android, each labeled with a letter so I can show the correlation to Xamarin.iOS results:

    [FAIL] // "A"
    Microsoft.EntityFrameworkCore.ApiConsistencyTest.Public_api_arguments_should_have_not_null_annotation : -- Missing NotNull annotations -- Microsoft.EntityFrameworkCore.ChangeTracking.ObservableHashSet`1.Overlaps[other]
    
    [FAIL] // "B"
    Microsoft.EntityFrameworkCore.Migrations.Internal.MigrationsModelDifferTest.Add_column_not_null : System.TypeLoadException : Could not load type System.String, netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51 while decoding custom attribute: (null)
    
    [FAIL] // "C"
    Microsoft.EntityFrameworkCore.Query.WarningsTest.Throws_when_warning_as_error_all : Assert.Equal() Failure
    
    [FAIL] // "D"
    Microsoft.EntityFrameworkCore.Storage.SqliteTypeMappingTest.It_maps_strings_to_not_null_types : System.TypeLoadException : Could not load type System.String, netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51 while decoding custom attribute: (null)

### Special Class of Test Case Failure on Xamarin.Android

One set of unit tests had to be excluded from the test runner altogether, as the tests had a high chance of causing invalid IL code generation, causing the entire test runner app to abort prior to completion. So, I excluded unit tests for "Bug5456" from the app, and at that time filed an issue about the problem:

- [xamarin/xamarin-android issue #1483 - InvalidProgramException (invalid IL code), VerificationException, and SIGSEGV when executing LINQ queries simultaneously using thread pool](https://github.com/xamarin/xamarin-android/issues/1483)


## iOS EF Core 2.0.2 Unit Test Subset Results

Results for Xamarin.iOS on Visual Studio 15.6.6 and 15.7 differed &mdash; significant improvements were noted after moving to VS 15.7:

- For VS 15.6.6, 9459 tests passed, and 389 tests failed.
- For VS 15.7, 9537 tests passed, and 311 tests failed. (78 more tests passing)

*Most* of the failed tests were due to  `System.ExecutionEngineException`. If we set those aside for a moment and look at what remains, the list of failed tests is similar to Xamarin.Android, again labeled to show the correlation:

    [FAIL] // "A"
    Microsoft.EntityFrameworkCore.ApiConsistencyTest.Public_api_arguments_should_have_not_null_annotation : -- Missing NotNull annotations -- Microsoft.EntityFrameworkCore.ChangeTracking.ObservableHashSet`1.IntersectWith[other]

    [FAIL] // "B"
    Microsoft.EntityFrameworkCore.Migrations.Internal.MigrationsModelDifferTest.Add_column_not_null : System.TypeLoadException : Could not load type System.Int32, netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51 while decoding custom attribute: (null)

    [FAIL] // "D"
    Microsoft.EntityFrameworkCore.Storage.SqliteTypeMappingTest.It_maps_strings_to_not_null_types : System.TypeLoadException : Could not load type System.Double, netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51 while decoding custom attribute: (null)


### System.ExecutionEngineException Test Failures on Xamarin.iOS

Back to the `System.ExecutionEngineException` failures. These were responsible for 386 and 308 test failures for Xamarin.iOS on VS 15.6.6 and 15.7, respectively.

Here's a representative example:

    [FAIL]
    Microsoft.EntityFrameworkCore.BuiltInDataTypesSqliteTest.Can_insert_and_read_back_all_nullable_data_types_with_values_set_to_null :
    System.ExecutionEngineException : Attempting to JIT compile method '(wrapper runtime-invoke)
    <Module>:runtime_invoke_void__this___int_Nullable`1<Enum16>_Nullable`1<Enum32>_Nullable`1<Enum64>_Nullable`1<Enum8>_int_object_Nullable`1<bool>_Nullable`1<byte>_Nullable`1<char>_Nullable`1<DateTime>_Nullable`1<DateTimeOffset>_Nullable`1<Decimal>_Nullable`1<double>_Nullable`1<int16>_Nullable`1<int>_Nullable`1<long>_Nullable`1<sbyte>_Nullable`1<single>_Nullable`1<TimeSpan>_Nullable`1<uint16>_Nullable`1<uint>_Nullable`1<ulong>_object (object,intptr,intptr,intptr)'
    while running in aot-only mode. See https://developer.xamarin.com/guides/ios/advanced_topics/limitations/ for more information.


These occurrences were not surprising, given the [known limitations](https://docs.microsoft.com/en-ca/xamarin/ios/internals/limitations) when compiling .NET code ahead-of-time for iOS.

On the one hand, Entity Framework Core doesn't use `Reflection.Emit`, while many other lightweight ORMs for .NET do use it. Emitting tailor-made IL methods for mapping model object data to and from database objects can be many times faster than methods that use reflection-based mapping &mdash; but any use of `Reflection.Emit` precludes use of a library with Xamarin.iOS, where AOT compilation is mandatory. There is no JIT to compile run-time emitted IL into native code.

On the other hand, even without using `Reflection.Emit`, EF Core and .NET LINQ implementations do still still present challenges for running under AOT.

But it's getting better. The Mono team has addressed related issues, e.g. [Xamarin Bugzilla Bug 59184 - MethodInfo.Invoke fails for generic methods with too many/too large arguments](https://bugzilla.xamarin.com/show_bug.cgi?id=59184) was fixed, and that fix was included in VS 15.7. I assume that fix is one reason for the drop in Xamarin.iOS test case failures between 15.6.6 to 15.7.

There's also hope that the [Mono interpreter](http://www.mono-project.com/news/2017/11/13/mono-interpreter/) could address these kinds of issues on Xamarin.iOS, even enabling use of Reflection.Emit. At the Mono interpreter page linked, look under *"Future work"*, *"Improvements for Statically Compiled Mono"*. iOS and Entity Framework are mentioned specifically. I'm hopeful that some day, platforms like Xamarin.iOS that require AOT will have no issues running Entity Framework Core.

Until then, know that some kinds of EF Core operations will fail on iOS. Be prepared to test first on iOS, and potentially adjust your model and logic to work around such issues. If your use of EF Core works on iOS, chances are it will work on every other platform.

