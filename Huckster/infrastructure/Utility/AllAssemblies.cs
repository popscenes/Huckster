using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Utility
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Web;

    namespace Infrastructure.Framework
    {


        // Code from nservice bus, don't redistribute

        /// <summary>
        /// Supporting the fluent interface in <seealso cref="AllAssemblies"/>
        /// </summary>
        public interface IIncludesBuilder : IEnumerable<Assembly>
        {
            /// <summary>
            /// Indicate that assemblies matching the given expression should also be included.
            /// You can call this method multiple times.
            /// </summary>
            /// <param name="assemblyExpression"><see cref="Configure.IsMatch"/></param>
            /// <seealso cref="Configure.IsMatch"/>
            /// <returns></returns>
            IIncludesBuilder And(string assemblyExpression);

            /// <summary>
            /// Indicate that assemblies matching the given expression should be excluded.
            /// Use the 'And' method to indicate other assemblies to be skipped.
            /// </summary>
            /// <param name="assemblyExpression"><see cref="Configure.IsMatch"/></param>
            /// <seealso cref="Configure.IsMatch"/>
            /// <returns></returns>
            IExcludesBuilder Except(string assemblyExpression);
        }

        /// <summary>
        /// Supporting the fluent interface in <seealso cref="AllAssemblies"/>
        /// </summary>
        public interface IExcludesBuilder : IEnumerable<Assembly>
        {
            /// <summary>
            /// Indicate that the given assembly expression should also be excluded.
            /// You can call this method multiple times.
            /// </summary>
            /// <param name="assemblyExpression"><see cref="Configure.IsMatch"/></param>
            /// <seealso cref="Configure.IsMatch"/>
            /// <returns></returns>
            IExcludesBuilder And(string assemblyExpression);
        }

        /// <summary>
        /// Class for specifying which assemblies not to load.
        /// </summary>
        public class AllAssemblies : IExcludesBuilder, IIncludesBuilder
        {

            public static string[] DefaultAssemblyPrefixStrings;
            static readonly string Directory;

            static AllAssemblies()
            {
                Directory = AppDomain.CurrentDomain.BaseDirectory;
                DefaultAssemblyPrefixStrings = new[] { Assembly.GetExecutingAssembly().GetName().Name.Split('.')[0] + '.' };
            }

            public static string RuntimeDir
            {
                get { return Directory; }
            }

            /// <summary>
            /// Indicate that assemblies matching the given expression are not to be used.
            /// Use the 'And' method to indicate other assemblies to be skipped.
            /// </summary>
            /// <param name="assemblyExpression"><see cref="Configure.IsMatch"/></param>
            /// <seealso cref="Configure.IsMatch"/>
            /// <returns></returns>
            public static IExcludesBuilder Except(string assemblyExpression)
            {
                return new AllAssemblies
                {
                    assembliesToExclude =
                       {
                           assemblyExpression
                       }
                };
            }

            /// <summary>
            /// just uses prefix of assembly that this class is in to match assemblies
            /// </summary>
            /// <returns></returns>
            public static AllAssemblies MatchingDefault()
            {
                return Matching(DefaultAssemblyPrefixStrings) as AllAssemblies;
            }

            /// <summary>
            /// Indicate that assemblies matching the given expression are to be used.
            /// Use the 'And' method to indicate other assemblies to be included.
            /// </summary>
            /// <param name="assemblyExpressions"><see cref="Configure.IsMatch"/></param>
            /// <seealso cref="Configure.IsMatch"/>
            /// <returns></returns>
            public static IIncludesBuilder Matching(params string[] assemblyExpressions)
            {
                return new AllAssemblies
                {
                    assembliesToInclude = new List<string>(assemblyExpressions)
                };
            }

            IExcludesBuilder IExcludesBuilder.And(string assemblyExpression)
            {
                if (!assembliesToExclude.Contains(assemblyExpression))
                {
                    assembliesToExclude.Add(assemblyExpression);
                }

                return this;
            }

            IExcludesBuilder IIncludesBuilder.Except(string assemblyExpression)
            {
                if (!assembliesToExclude.Contains(assemblyExpression))
                {
                    assembliesToExclude.Add(assemblyExpression);
                }

                return this;
            }

            IIncludesBuilder IIncludesBuilder.And(string assemblyExpression)
            {
                if (!assembliesToInclude.Contains(assemblyExpression))
                {
                    assembliesToInclude.Add(assemblyExpression);
                }

                return this;
            }

            /// <summary>
            /// Returns an enumerator for looping over the assemblies to be loaded.
            /// </summary>
            /// <returns></returns>
            public IEnumerator<Assembly> GetEnumerator()
            {
                return GetOrdered()
                    .GetEnumerator();
            }

            public IOrderedEnumerable<Assembly> GetOrdered()
            {
                Func<Assembly, int> getAssemblyOrder = assembly =>
                {
                    if (assembliesToInclude == null || assembliesToInclude.Count == 0)
                        return 0;
                    var file = Path.GetFileName(assembly.Location);
                    for (var i = 0; i < assembliesToInclude.Count; i++)
                    {
                        if (AssemblyScanner.IsMatch(assembliesToInclude[i], file))
                            return i;
                    }
                    return assembliesToInclude.Count;
                };

                return new AssemblyScanner(Directory)
                {
                    IncludeAppDomainAssemblies = true,
                    AssembliesToInclude = assembliesToInclude,
                    AssembliesToSkip = assembliesToExclude
                }
                    .GetScannableAssemblies()
                    .Assemblies.Distinct().OrderBy(getAssemblyOrder);
            }

            /// <summary>
            /// Return a non-generic enumerator.
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            AllAssemblies()
            {

            }


            List<string> assembliesToExclude = new List<string>();
            List<string> assembliesToInclude = new List<string>();
        }


        /// <summary>
        ///     Helpers for assembly scanning operations
        /// </summary>
        public class AssemblyScanner
        {
            public AssemblyScanner()
                : this(AppDomain.CurrentDomain.BaseDirectory)
            {
            }

            public AssemblyScanner(string baseDirectoryToScan)
            {
                AssembliesToInclude = new List<string>();
                AssembliesToSkip = new List<string>();
                this.baseDirectoryToScan = baseDirectoryToScan;

            }

            /// <summary>
            ///     Traverses the specified base directory including all sub-directories, generating a list of assemblies that can be
            ///     scanned for handlers, a list of skipped files, and a list of errors that occurred while scanning.
            ///     Scanned files may be skipped when they're either not a .NET assembly, or if a reflection-only load of the .NET
            ///     assembly
            ///     reveals that it does not reference NServiceBus.
            /// </summary>
            public AssemblyScannerResults GetScannableAssemblies()
            {
                var results = new AssemblyScannerResults();

                if (IncludeAppDomainAssemblies)
                {
                    var matchingAssembliesFromAppDomain = AppDomain.CurrentDomain
                        .GetAssemblies()
                        .Where(assembly => IsIncluded(assembly.GetName().Name));

                    results.Assemblies.AddRange(matchingAssembliesFromAppDomain);
                }

                var assemblyFiles = ScanDirectoryForAssemblyFiles();

                foreach (var assemblyFile in assemblyFiles)
                {
                    Assembly assembly;

                    if (!IsIncluded(assemblyFile.Name))
                    {
                        results.SkippedFiles.Add(new SkippedFile(assemblyFile.FullName,
                            "File was explicitly excluded from scanning"));
                        continue;
                    }

                    var compilationMode = Image.GetCompilationMode(assemblyFile.FullName);
                    if (compilationMode == Image.CompilationMode.NativeOrInvalid)
                    {
                        results.SkippedFiles.Add(new SkippedFile(assemblyFile.FullName, "File is not a .NET assembly"));
                        continue;
                    }

                    if (!Environment.Is64BitProcess && compilationMode == Image.CompilationMode.CLRx64)
                    {
                        results.SkippedFiles.Add(new SkippedFile(assemblyFile.FullName,
                            "x64 .NET assembly can't be loaded by a 32Bit process"));
                        continue;
                    }

                    try
                    {

                        assembly = Assembly.LoadFrom(assemblyFile.FullName);
                    }
                    catch (BadImageFormatException badImageFormatException)
                    {
                        var errorMessage =
                            string.Format(
                                "Could not load {0}. Consider using 'Configure.With(AllAssemblies.Except(\"{1}\"))' to tell NServiceBus not to load this file.",
                                assemblyFile.FullName, assemblyFile.Name);
                        var error = new ErrorWhileScanningAssemblies(badImageFormatException, errorMessage);
                        results.Errors.Add(error);
                        continue;
                    }

                    try
                    {
                        //will throw if assembly cannot be loaded
                        assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        var errorMessage = FormatReflectionTypeLoadException(assemblyFile.FullName, e);
                        var error = new ErrorWhileScanningAssemblies(e, errorMessage);
                        results.Errors.Add(error);
                        continue;
                    }

                    results.Assemblies.Add(assembly);
                }

                return results;
            }

            public static string FormatReflectionTypeLoadException(string fileName, ReflectionTypeLoadException e)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Could not scan assembly: {0}. Exception message {1}.", fileName, e);
                if (e.LoaderExceptions.Any())
                {
                    sb.Append(Environment.NewLine + "Scanned type errors: ");
                    foreach (var ex in e.LoaderExceptions)
                    {
                        sb.Append(Environment.NewLine + ex.Message);
                    }
                }

                return sb.ToString();
            }

            IEnumerable<FileInfo> ScanDirectoryForAssemblyFiles()
            {
                var baseDir = new DirectoryInfo(baseDirectoryToScan);
                var searchOption = ScanNestedDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                return GetFileSearchPatternsToUse()
                    .SelectMany(extension => baseDir.GetFiles(extension, searchOption))
                    .ToList();
            }

            IEnumerable<string> GetFileSearchPatternsToUse()
            {
                yield return "*.dll";

                if (IncludeExesInScan)
                {
                    yield return "*.exe";
                }
            }

            //        bool AssemblyReferencesNServiceBus(FileSystemInfo assemblyFile)
            //        {
            //            var lightLoad = Assembly.ReflectionOnlyLoadFrom(assemblyFile.FullName);
            //            var referencedAssemblies = lightLoad.GetReferencedAssemblies();
            //
            //            var nameOfAssemblyDefiningHandlersInterface =
            //                typeof(IHandleMessages<>).Assembly.GetName().Name;
            //
            //            return referencedAssemblies
            //                .Any(a => a.Name == nameOfAssemblyDefiningHandlersInterface);
            //        }

            /// <summary>
            ///     Determines whether the specified assembly name or file name can be included, given the set up include/exclude
            ///     patterns and default include/exclude patterns
            /// </summary>
            bool IsIncluded(string assemblyNameOrFileName)
            {
                var isExplicitlyExcluded = AssembliesToSkip.Any(excluded => IsMatch(excluded, assemblyNameOrFileName));

                if (isExplicitlyExcluded)
                {
                    return false;
                }

                var noAssembliesWereExplicitlyIncluded = !AssembliesToInclude.Any();
                var isExplicitlyIncluded = AssembliesToInclude.Any(included => IsMatch(included, assemblyNameOrFileName));

                return noAssembliesWereExplicitlyIncluded || isExplicitlyIncluded;
            }

            public static bool IsMatch(string expression, string scopedNameOrFileName)
            {
                if (DistillLowerAssemblyName(scopedNameOrFileName).StartsWith(expression.ToLower()))
                {
                    return true;
                }

                if (DistillLowerAssemblyName(expression).TrimEnd('.') == DistillLowerAssemblyName(scopedNameOrFileName))
                {
                    return true;
                }

                return false;
            }

            public static bool IsAllowedType(Type type)
            {
                return !type.IsValueType &&
                       !(type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0);
            }

            static string DistillLowerAssemblyName(string assemblyOrFileName)
            {
                var lowerAssemblyName = assemblyOrFileName.ToLowerInvariant();
                if (lowerAssemblyName.EndsWith(".dll"))
                {
                    lowerAssemblyName = lowerAssemblyName.Substring(0, lowerAssemblyName.Length - 4);
                }
                return lowerAssemblyName;
            }

            readonly string baseDirectoryToScan;
            public List<string> AssembliesToInclude;
            public List<string> AssembliesToSkip;
            public bool IncludeAppDomainAssemblies;
            public bool IncludeExesInScan = true;
            public bool ScanNestedDirectories = true;
        }

        /// <summary>
        /// Holds GetScannableAssemblies results.
        /// Contains list of errors and list of scan-able assemblies.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public class AssemblyScannerResults
        {
            /// <summary>
            /// Constructor to initialize AssemblyScannerResults
            /// </summary>
            public AssemblyScannerResults()
            {
                Errors = new List<ErrorWhileScanningAssemblies>();
                Assemblies = new List<Assembly>();
                SkippedFiles = new List<SkippedFile>();
            }
            /// <summary>
            /// Dump error to console.
            /// </summary>
            public override string ToString()
            {
                if ((Errors == null) || (Errors.Count < 1)) return string.Empty;
                var sb = new StringBuilder();

                foreach (var error in Errors)
                {
                    sb.Append(error);
                    if (error.Exception is ReflectionTypeLoadException)
                    {
                        var e = error.Exception as ReflectionTypeLoadException;
                        if (e.LoaderExceptions.Any())
                        {
                            sb.Append(Environment.NewLine + "Scanned type errors: ");
                            foreach (var ex in e.LoaderExceptions)
                                sb.Append(Environment.NewLine + ex.Message);
                        }
                    }
                }

                return sb.ToString();
            }

            /// <summary>
            /// List of errors that occurred while attempting to load an assembly
            /// </summary>
            public List<ErrorWhileScanningAssemblies> Errors { get; private set; }

            /// <summary>
            /// List of successfully found and loaded assemblies
            /// </summary>
            public List<Assembly> Assemblies { get; private set; }

            /// <summary>
            /// List of files that were skipped while scanning because they were a) explicitly excluded
            /// by the user, b) not a .NET DLL, or c) not referencing NSB and thus not capable of implementing
            /// <see cref="IHandleMessages{T}"/>
            /// </summary>
            public List<SkippedFile> SkippedFiles { get; private set; }
        }

        /// <summary>
        /// Error information that occurred while scanning assemblies.
        /// </summary>
        public class ErrorWhileScanningAssemblies
        {
            /// <summary>
            /// Adding an error
            /// </summary>
            /// <param name="ex"></param>
            /// <param name="errorMessage"></param>
            internal ErrorWhileScanningAssemblies(Exception ex, string errorMessage)
            {
                Exception = ex;
                ErrorMessage = errorMessage;
            }
            /// <summary>
            /// Convert to string errors while scanning assemblies
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return ErrorMessage + Environment.NewLine + Exception;
            }
            /// <summary>
            /// Exception message.
            /// </summary>
            internal string ErrorMessage { get; private set; }
            /// <summary>
            /// Exception that occurred.
            /// </summary>
            internal Exception Exception { get; private set; }
        }

        /// <summary>
        /// Contains information about a file that was skipped during scanning along with a text describing
        /// the reason why the file was skipped
        /// </summary>
        public class SkippedFile
        {
            public SkippedFile(string filePath, string message)
            {
                FilePath = filePath;
                SkipReason = message;
            }

            /// <summary>
            /// The full path to the file that was skipped
            /// </summary>
            public string FilePath { get; private set; }

            /// <summary>
            /// Description of the reason why this file was skipped
            /// </summary>
            public string SkipReason { get; private set; }
        }

        // Code kindly provided by the mono project: https://github.com/jbevain/mono.reflection/blob/master/Mono.Reflection/Image.cs
        // Image.cs
        //
        // Author:
        //   Jb Evain (jbevain@novell.com)
        //
        // (C) 2009 - 2010 Novell, Inc. (http://www.novell.com)
        //
        // Permission is hereby granted, free of charge, to any person obtaining
        // a copy of this software and associated documentation files (the
        // "Software"), to deal in the Software without restriction, including
        // without limitation the rights to use, copy, modify, merge, publish,
        // distribute, sublicense, and/or sell copies of the Software, and to
        // permit persons to whom the Software is furnished to do so, subject to
        // the following conditions:
        //
        // The above copyright notice and this permission notice shall be
        // included in all copies or substantial portions of the Software.
        //
        // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
        // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
        // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
        // NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
        // LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
        // OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
        // WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
        class Image : IDisposable
        {
            readonly Stream stream;

            public enum CompilationMode
            {
                NativeOrInvalid,
                CLRx86,
                CLRx64
            }

            public static CompilationMode GetCompilationMode(string file)
            {
                if (file == null)
                {
                    throw new ArgumentNullException("file", "You must specify a file name");
                }

                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var image = new Image(stream))
                {
                    return image.GetCompilationMode();
                }
            }

            Image(Stream stream)
            {
                this.stream = stream;
            }

            CompilationMode GetCompilationMode()
            {
                if (stream.Length < 318)
                    return CompilationMode.NativeOrInvalid;
                if (ReadUInt16() != 0x5a4d)
                    return CompilationMode.NativeOrInvalid;
                if (!Advance(58))
                    return CompilationMode.NativeOrInvalid;
                if (!MoveTo(ReadUInt32()))
                    return CompilationMode.NativeOrInvalid;
                if (ReadUInt32() != 0x00004550)
                    return CompilationMode.NativeOrInvalid;
                if (!Advance(20))
                    return CompilationMode.NativeOrInvalid;

                var result = CompilationMode.NativeOrInvalid;
                switch (ReadUInt16())
                {
                    case 0x10B:
                        if (Advance(206))
                        {
                            result = CompilationMode.CLRx86;
                        }

                        break;
                    case 0x20B:
                        if (Advance(222))
                        {
                            result = CompilationMode.CLRx64;
                        }
                        break;
                }

                if (result == CompilationMode.NativeOrInvalid)
                {
                    return result;
                }

                return ReadUInt32() != 0 ? result : CompilationMode.NativeOrInvalid;
            }

            bool Advance(int length)
            {
                if (stream.Position + length >= stream.Length)
                    return false;

                stream.Seek(length, SeekOrigin.Current);
                return true;
            }

            bool MoveTo(uint position)
            {
                if (position >= stream.Length)
                    return false;

                stream.Position = position;
                return true;
            }

            void IDisposable.Dispose()
            {
                stream.Dispose();
            }

            ushort ReadUInt16()
            {
                return (ushort)(stream.ReadByte()
                                | (stream.ReadByte() << 8));
            }

            uint ReadUInt32()
            {
                return (uint)(stream.ReadByte()
                              | (stream.ReadByte() << 8)
                              | (stream.ReadByte() << 16)
                              | (stream.ReadByte() << 24));
            }
        }
    }

}
