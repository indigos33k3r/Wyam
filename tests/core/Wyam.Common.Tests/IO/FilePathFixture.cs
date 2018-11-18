﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Wyam.Common.IO;
using Wyam.Testing;
using Wyam.Testing.Attributes;

namespace Wyam.Common.Tests.IO
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self | ParallelScope.Children)]
    public class FilePathFixture : BaseFixture
    {
        public class HasExtensionTests : FilePathFixture
        {
            [TestCase("assets/shaders/basic.txt", true)]
            [TestCase("assets/shaders/basic", false)]
            [TestCase("assets/shaders/basic/", false)]
            public void CanSeeIfAPathHasAnExtension(string fullPath, bool expected)
            {
                // Given, When
                FilePath path = new FilePath(fullPath);

                // Then
                Assert.AreEqual(expected, path.HasExtension);
            }
        }

        public class ExtensionTests : FilePathFixture
        {
            [TestCase("assets/shaders/basic.frag", ".frag")]
            [TestCase("assets/shaders/basic.frag/test.vert", ".vert")]
            [TestCase("assets/shaders/basic", null)]
            [TestCase("assets/shaders/basic.frag/test", null)]
            public void CanGetExtension(string fullPath, string expected)
            {
                // Given
                FilePath result = new FilePath(fullPath);

                // When
                string extension = result.Extension;

                // Then
                Assert.AreEqual(expected, extension);
            }
        }

        public class DirectoryTests : FilePathFixture
        {
            [Test]
            public void CanGetDirectoryForFilePath()
            {
                // Given
                FilePath path = new FilePath("temp/hello.txt");

                // When
                DirectoryPath directory = path.Directory;

                // Then
                Assert.AreEqual("temp", directory.FullPath);
            }

            [Test]
            public void CanGetDirectoryForFilePathInRoot()
            {
                // Given
                FilePath path = new FilePath("hello.txt");

                // When
                DirectoryPath directory = path.Directory;

                // Then
                Assert.AreEqual(".", directory.FullPath);
            }
        }

        public class RootRelativeTests : FilePathFixture
        {
            [TestCase(@"\a\b\c", "a/b/c")]
            [TestCase("/a/b/c", "a/b/c")]
            [TestCase("a/b/c", "a/b/c")]
            [TestCase(@"a\b\c", "a/b/c")]
            [TestCase("foo.txt", "foo.txt")]
            [TestCase("foo", "foo")]
            [WindowsTestCase(@"c:\a\b\c", "a/b/c")]
            [WindowsTestCase("c:/a/b/c", "a/b/c")]
            public void ShouldReturnRootRelativePath(string fullPath, string expected)
            {
                // Given
                FilePath path = new FilePath(fullPath);

                // When
                FilePath rootRelative = path.RootRelative;

                // Then
                Assert.AreEqual(expected, rootRelative.FullPath);
            }

            [TestCase(@"\a\b\c")]
            [TestCase("/a/b/c")]
            [TestCase("a/b/c")]
            [TestCase(@"a\b\c")]
            [TestCase("foo.txt")]
            [TestCase("foo")]
            [WindowsTestCase(@"c:\a\b\c")]
            [WindowsTestCase("c:/a/b/c")]
            public void ShouldReturnSelfForExplicitRelativePath(string fullPath)
            {
                // Given
                FilePath path = new FilePath(fullPath, PathKind.Relative);

                // When
                FilePath rootRelative = path.RootRelative;

                // Then
                Assert.AreEqual(path.FullPath, rootRelative.FullPath);
            }
        }

        public class ChangeExtensionTests : FilePathFixture
        {
            [TestCase(".dat", "temp/hello.dat")]
            [TestCase("dat", "temp/hello.dat")]
            [TestCase(".txt", "temp/hello.txt")]
            [TestCase("txt", "temp/hello.txt")]
            [TestCase("", "temp/hello.")]
            [TestCase(null, "temp/hello")]
            public void ShouldChangeExtension(string extension, string expected)
            {
                // Given
                FilePath path = new FilePath("temp/hello.txt");

                // When
                path = path.ChangeExtension(extension);

                // Then
                Assert.AreEqual(expected, path.ToString());
            }
        }

        public class AppendExtensionTests : FilePathFixture
        {
            [Test]
            public void ShouldThrowIfExtensionIsNull()
            {
                // Given
                FilePath path = new FilePath("temp/hello.txt");

                // When
                TestDelegate test = () => path.AppendExtension(null);

                // Then
                Assert.Throws<ArgumentNullException>(test);
            }

            [TestCase("dat", "temp/hello.txt.dat")]
            [TestCase(".dat", "temp/hello.txt.dat")]
            public void CanAppendExtensionToPath(string extension, string expected)
            {
                // Given
                FilePath path = new FilePath("temp/hello.txt");

                // When
                path = path.AppendExtension(extension);

                // Then
                Assert.AreEqual(expected, path.ToString());
            }
        }

        public class InsertSuffixTests : FilePathFixture
        {
            [Test]
            public void ShouldThrowIfSuffixIsNull()
            {
                // Given
                FilePath path = new FilePath("temp/hello.txt");

                // When
                TestDelegate test = () => path.InsertSuffix(null);

                // Then
                Assert.Throws<ArgumentNullException>(test);
            }

            [TestCase("temp/hello.txt", "123", "temp/hello123.txt")]
            [TestCase("/hello.txt", "123", "/hello123.txt")]
            [TestCase("temp/hello", "123", "temp/hello123")]
            [TestCase("temp/hello.txt.dat", "123", "temp/hello.txt123.dat")]
            public void CanInsertSuffixToPath(string path, string suffix, string expected)
            {
                // Given
                FilePath filePath = new FilePath(path);

                // When
                filePath = filePath.InsertSuffix(suffix);

                // Then
                Assert.AreEqual(expected, filePath.FullPath);
            }
        }

        public class InserPrefixTests : FilePathFixture
        {
            [Test]
            public void ShouldThrowIfPRefixIsNull()
            {
                // Given
                FilePath path = new FilePath("temp/hello.txt");

                // When
                TestDelegate test = () => path.InsertPrefix(null);

                // Then
                Assert.Throws<ArgumentNullException>(test);
            }

            [TestCase("temp/hello.txt", "123", "temp/123hello.txt")]
            [TestCase("/hello.txt", "123", "/123hello.txt")]
            [TestCase("hello.txt", "123", "123hello.txt")]
            [TestCase("temp/hello", "123", "temp/123hello")]
            [TestCase("temp/hello.txt.dat", "123", "temp/123hello.txt.dat")]
            public void CanInsertPrefixToPath(string path, string prefix, string expected)
            {
                // Given
                FilePath filePath = new FilePath(path);

                // When
                filePath = filePath.InsertPrefix(prefix);

                // Then
                Assert.AreEqual(expected, filePath.FullPath);
            }
        }

        public class FileNameTests : FilePathFixture
        {
            [Test]
            public void CanGetFilenameFromPath()
            {
                // Given
                FilePath path = new FilePath("/input/test.txt");

                // When
                FilePath result = path.FileName;

                // Then
                Assert.AreEqual("test.txt", result.FullPath);
            }

            [Test]
            public void GetsFileNameIfJustFileName()
            {
                // Given
                FilePath path = new FilePath("test.txt");

                // When
                FilePath result = path.FileName;

                // Then
                Assert.AreEqual("test.txt", result.FullPath);
            }

            [Test]
            public void NullProviderSetForReturnPath()
            {
                // Given
                FilePath path = new FilePath("foo", "/input/test.txt");

                // When
                FilePath result = path.FileName;

                // Then
                Assert.AreEqual(null, result.FileProvider);
            }
        }

        public class FileNameWithoutExtensionTests : FilePathFixture
        {
            [TestCase("/input/test.txt", "test")]
            [TestCase("/input/test", "test")]
            [TestCase("test.txt", "test")]
            [TestCase("test", "test")]
            public void ShouldReturnFilenameWithoutExtensionFromPath(string fullPath, string expected)
            {
                // Given
                FilePath path = new FilePath(fullPath);

                // When
                FilePath result = path.FileNameWithoutExtension;

                // Then
                Assert.AreEqual(expected, result.FullPath);
            }

            [Test]
            public void NullProviderSetForReturnPath()
            {
                // Given
                FilePath path = new FilePath("foo", "/input/test.txt");

                // When
                FilePath result = path.FileNameWithoutExtension;

                // Then
                Assert.AreEqual(null, result.FileProvider);
            }

            [TestCase("/input/.test")]
            [TestCase(".test")]
            public void ShouldReturnNullIfOnlyExtension(string fullPath)
            {
                // Given
                FilePath path = new FilePath(fullPath);

                // When
                FilePath result = path.FileNameWithoutExtension;

                // Then
                Assert.IsNull(result);
            }
        }

        public class CollapseTests : FilePathFixture
        {
            [TestCase("/a/b/c/../d/baz.txt", "/a/b/d/baz.txt")]
            [WindowsTestCase("c:/a/b/c/../d/baz.txt", "c:/a/b/d/baz.txt")]
            public void ShouldCollapse(string fullPath, string expected)
            {
                // Given
                FilePath filePath = new FilePath(fullPath);

                // When
                FilePath path = filePath.Collapse();

                // Then
                Assert.AreEqual(expected, path.FullPath);
            }

            [Test]
            public void CollapseRetainsProvider()
            {
                // Given
                FilePath filePath = new FilePath(new Uri("foo:///"), "/a/b/../c/bar.txt");

                // When
                FilePath path = filePath.Collapse();

                // Then
                Assert.AreEqual("/a/c/bar.txt", path.FullPath);
                Assert.AreEqual(new Uri("foo:///"), path.FileProvider);
            }
        }
    }
}
