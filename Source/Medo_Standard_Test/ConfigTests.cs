using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Medo.Configuration;
using Xunit;

namespace Test {
    public class PropertiesTests {

        [Fact(DisplayName = "Properties: Null key throws exception")]
        void NullKey() {
            var ex = Assert.Throws<ArgumentNullException>(() => {
                Config.Read(null, "");
            });
            Assert.StartsWith("Key cannot be null.", ex.Message);
        }

        [Fact(DisplayName = "Properties: Empty key throws exception")]
        void EmptyKey() {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => {
                Config.Read("   ", "");
            });
            Assert.StartsWith("Key cannot be empty.", ex.Message);
        }


        [Fact(DisplayName = "Properties: Empty file Load/Save")]
        void EmptySave() {
            using (var loader = new ConfigLoader("Empty.cfg")) {
                Assert.True(Config.Load(), "File should exist before load.");
                Assert.True(Config.Save(), "Save should succeed.");
                Assert.Equal(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
            }
        }


        [Fact(DisplayName = "Properties: CRLF preserved on Save")]
        void EmptyLinesCRLF() {
            using (var loader = new ConfigLoader("EmptyLinesCRLF.cfg")) {
                Config.Save();
                Assert.Equal(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
            }
        }

        [Fact(DisplayName = "Properties: LF preserved on Save")]
        void EmptyLinesLF() {
            using (var loader = new ConfigLoader("EmptyLinesLF.cfg")) {
                Config.Save();
                Assert.Equal(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
            }
        }

        [Fact(DisplayName = "Properties: CR preserved on Save")]
        void EmptyLinesCR() {
            using (var loader = new ConfigLoader("EmptyLinesCR.cfg")) {
                Config.Save();
                Assert.Equal(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
            }
        }

        [Fact(DisplayName = "Properties: Mixed line ending gets normalized on Save")]
        void EmptyLinesMixed() {
            using (var loader = new ConfigLoader("EmptyLinesMixed.cfg", "EmptyLinesMixed.Good.cfg")) {
                Config.Save();
                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
            }
        }

        [Fact(DisplayName = "Properties: Comments are preserved on Save")]
        void CommentsOnly() {
            using (var loader = new ConfigLoader("CommentsOnly.cfg")) {
                Config.Save();
                Assert.Equal(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
            }
        }

        [Fact(DisplayName = "Properties: Values with comments are preserved on Save")]
        void CommentsWithValues() {
            using (var loader = new ConfigLoader("CommentsWithValues.cfg")) {
                Config.Save();
                Assert.Equal(Encoding.UTF8.GetString(loader.Bytes), Encoding.UTF8.GetString(File.ReadAllBytes(loader.FileName)));
                Assert.Equal(BitConverter.ToString(loader.Bytes), BitConverter.ToString(File.ReadAllBytes(loader.FileName)));
            }
        }

        [Fact(DisplayName = "Properties: Leading spaces are preserved on Save")]
        void SpacingEscape() {
            using (var loader = new ConfigLoader("SpacingEscape.cfg", "SpacingEscape.Good.cfg")) {
                Assert.Equal(" Value 1", Config.Read("Key1", null));
                Assert.Equal("Value 2 ", Config.Read("Key2", null));
                Assert.Equal(" Value 3 ", Config.Read("Key3", null));
                Assert.Equal("  Value 4  ", Config.Read("Key4", null));
                Assert.Equal("\tValue 5\t", Config.Read("Key5", null));
                Assert.Equal("\tValue 6", Config.Read("Key6", null));
                Assert.Equal("\0", Config.Read("Null", null));

                Config.Save();
                Config.Write("Null", "\0Null\0");

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
            }
        }

        [Fact(DisplayName = "Properties: Basic write")]
        void WriteBasic() {
            using (var loader = new ConfigLoader("Empty.cfg", "WriteBasic.Good.cfg")) {
                Config.Write("Key1", "Value 1");
                Config.Write("Key2", "Value 2");

                Config.Save();

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
            }
        }

        [Fact(DisplayName = "Properties: Basic write (without empty line ending)")]
        void WriteNoEmptyLine() {
            using (var loader = new ConfigLoader("WriteNoEmptyLine.cfg", "WriteNoEmptyLine.Good.cfg")) {
                Config.Write("Key1", "Value 1");
                Config.Write("Key2", "Value 2");

                Config.Save();

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
            }
        }

        [Fact(DisplayName = "Properties: Separator equals (=) is preserved upon save")]
        void WriteSameSeparatorEquals() {
            using (var loader = new ConfigLoader("WriteSameSeparatorEquals.cfg", "WriteSameSeparatorEquals.Good.cfg")) {
                Config.Write("Key1", "Value 1");
                Config.Write("Key2", "Value 2");

                Config.Save();

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
            }
        }

        [Fact(DisplayName = "Properties: Separator space ( ) is preserved upon save")]
        void WriteSameSeparatorSpace() {
            using (var loader = new ConfigLoader("WriteSameSeparatorSpace.cfg", "WriteSameSeparatorSpace.Good.cfg")) {
                Config.Write("Key1", "Value 1");
                Config.Write("Key2", "Value 2");

                Config.Save();

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
            }
        }

        [Fact(DisplayName = "Properties: Write replaces existing entry")]
        void Replace() {
            using (var loader = new ConfigLoader("Replace.cfg", "Replace.Good.cfg")) {
                Config.Write("Key1", "Value 1a");
                Config.Write("Key2", "Value 2a");

                Config.Save();

                Assert.Equal("Value 1a", Config.Read("Key1", null));
                Assert.Equal("Value 2a", Config.Read("Key2", null));

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
            }
        }

        [Fact(DisplayName = "Properties: Write preserves spacing")]
        void SpacingPreserved() {
            using (var loader = new ConfigLoader("SpacingPreserved.cfg", "SpacingPreserved.Good.cfg")) {
                Config.Write("KeyOne", "Value 1a");
                Config.Write("KeyTwo", "Value 2b");
                Config.Write("KeyThree", "Value 3c");

                Config.Save();

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
            }
        }

        [Fact(DisplayName = "Properties: Write preserves spacing on add")]
        void SpacingPreservedOnAdd() {
            using (var loader = new ConfigLoader("SpacingPreservedOnAdd.cfg", "SpacingPreservedOnAdd.Good.cfg")) {
                Config.Write("One", "Value 1a");
                Config.Write("Two", new string[] { "Value 2a", "Value 2b" });
                Config.Write("Three", "Value 3a");
                Config.Write("Four", "Value 4a");
                Config.Write("Five", new string[] { "Value 5a", "Value 5b", "Value 5c" });
                Config.Write("FourtyTwo", 42);

                Config.Save();

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
            }
        }

        [Fact(DisplayName = "Properties: Write without preexisting file")]
        void WriteToEmpty() {
            using (var loader = new ConfigLoader(null, "Replace.Good.cfg")) {
                Config.Write("Key1", "Value 1a");
                Config.Write("Key2", "Value 2a");

                Config.Save();

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
            }
        }

        [Fact(DisplayName = "Properties: Write replaces only last instance of same key")]
        void ReplaceOnlyLast() {
            using (var loader = new ConfigLoader("ReplaceOnlyLast.cfg", "ReplaceOnlyLast.Good.cfg")) {
                Config.Write("Key1", "Value 1a");
                Config.Write("Key2", "Value 2a");

                Config.Save();

                Assert.Equal("Value 1a", Config.Read("Key1", null));
                Assert.Equal("Value 2a", Config.Read("Key2", null));
                Assert.Equal("Value 3", Config.Read("Key3", null));

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
            }
        }


        [Fact(DisplayName = "Properties: Write creates directory")]
        void SaveInNonexistingDirectory1() {
            var propertiesFile = Path.Combine(Path.GetTempPath(), "PropertiesDirectory", "Test.cfg");
            try {
                Directory.Delete(Path.Combine(Path.GetTempPath(), "PropertiesDirectory"), true);
            } catch (IOException) { }
            Config.FileName = propertiesFile;

            Assert.False(Config.Load(), "No file present for load.");

            var x = Config.Read("Test", "test");
            Assert.Equal("test", x);

            Assert.True(Config.Save(), "Save should create directory structure and succeed.");


            Assert.True(File.Exists(propertiesFile));
        }

        [Fact(DisplayName = "Properties: Write creates directory (2 levels deep)")]
        void SaveInNonexistingDirectory2() {
            var propertiesFile = Path.Combine(Path.GetTempPath(), "PropertiesDirectoryOuter", "PropertiesDirectoryInner", "Test.cfg");
            try {
                Directory.Delete(Path.Combine(Path.GetTempPath(), "PropertiesDirectoryOuter"), true);
            } catch (IOException) { }
            Config.FileName = propertiesFile;

            Assert.False(Config.Load(), "No file present for load.");

            var x = Config.Read("Test", "test");
            Assert.Equal("test", x);

            Assert.True(Config.Save(), "Save should create directory structure and succeed.");


            Assert.True(File.Exists(propertiesFile));
        }

        [Fact(DisplayName = "Properties: Write creates directory (3 levels deep)")]
        void SaveInNonexistingDirectory3() {
            var propertiesFile = Path.Combine(Path.GetTempPath(), "PropertiesDirectoryOuter", "PropertiesDirectoryMiddle", "PropertiesDirectoryInner", "Test.cfg");
            try {
                Directory.Delete(Path.Combine(Path.GetTempPath(), "PropertiesDirectoryOuter"), true);
            } catch (IOException) { }
            Config.FileName = propertiesFile;

            Assert.False(Config.Load(), "No file present for load.");

            var x = Config.Read("Test", "test");
            Assert.Equal("test", x);

            Assert.True(Config.Save(), "Save should create directory structure and succeed.");


            Assert.True(File.Exists(propertiesFile));
        }


        [Fact(DisplayName = "Properties: Removing entry")]
        void RemoveSingle() {
            using (var loader = new ConfigLoader("Remove.cfg", "Remove.Good.cfg")) {
                Config.Delete("Key1");
                Config.Save();

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
            }
        }

        [Fact(DisplayName = "Properties: Removing multiple entries")]
        void RemoveMulti() {
            using (var loader = new ConfigLoader("RemoveMulti.cfg", "RemoveMulti.Good.cfg")) {
                Config.Delete("Key2");
                Config.Save();

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));
            }
        }


        [Fact(DisplayName = "Properties: Override is used first")]
        void UseOverrideFirst() {
            using (var loader = new ConfigLoader("Replace.cfg", resourceOverrideFileName: "Replace.Good.cfg")) {
                Assert.Equal("Value 1a", Config.Read("Key1", null));
            }
        }

        [Fact(DisplayName = "Properties: Override is not written")]
        void DontOverwriteOverride() {
            using (var loader = new ConfigLoader("Replace.cfg", resourceOverrideFileName: "Replace.Good.cfg")) {
                Config.Write("Key1", "XXX");
                Assert.Equal("Value 1a", Config.Read("Key1", null));
                Config.OverrideFileName = null;
                Assert.Equal("XXX", Config.Read("Key1", null));
            }
        }


        [Fact(DisplayName = "Properties: Reading multiple entries")]
        void ReadMulti() {
            using (var loader = new ConfigLoader("ReplaceOnlyLast.Good.cfg")) {
                var list = new List<string>(Config.Read("Key2"));
                Assert.Equal(2, list.Count);
                Assert.Equal("Value 2", list[0]);
                Assert.Equal("Value 2a", list[1]);
            }
        }

        [Fact(DisplayName = "Properties: Reading multiple entries from override")]
        void ReadMultiFromOverride() {
            using (var loader = new ConfigLoader("ReplaceOnlyLast.Good.cfg", resourceOverrideFileName: "RemoveMulti.cfg")) {
                var list = new List<string>(Config.Read("Key2"));
                Assert.Equal(3, list.Count);
                Assert.Equal("Value 2a", list[0]);
                Assert.Equal("Value 2b", list[1]);
                Assert.Equal("Value 2c", list[2]);
            }
        }

        [Fact(DisplayName = "Properties: Reading multi entries when override is not found")]
        void ReadMultiFromOverrideNotFound() {
            using (var loader = new ConfigLoader("ReplaceOnlyLast.Good.cfg", resourceOverrideFileName: "RemoveMulti.cfg")) {
                var list = new List<string>(Config.Read("Key3"));
                Assert.Equal(1, list.Count);
                Assert.Equal("Value 3", list[0]);
            }
        }

        [Fact(DisplayName = "Properties: Multi-value write")]
        void MultiWrite() {
            using (var loader = new ConfigLoader(null, resourceFileNameGood: "WriteMulti.Good.cfg")) {
                Config.Write("Key1", "Value 1");
                Config.Write("Key2", new string[] { "Value 2a", "Value 2b", "Value 2c" });
                Config.Write("Key3", "Value 3");
                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));

                Assert.Equal("Value 1", Config.Read("Key1", null));
                Assert.Equal("Value 3", Config.Read("Key3", null));

                var list = new List<string>(Config.Read("Key2"));
                Assert.Equal(3, list.Count);
                Assert.Equal("Value 2a", list[0]);
                Assert.Equal("Value 2b", list[1]);
                Assert.Equal("Value 2c", list[2]);
            }
        }

        [Fact(DisplayName = "Properties: Multi-value replace")]
        void MultiReplace() {
            using (var loader = new ConfigLoader("WriteMulti.cfg", resourceFileNameGood: "WriteMulti.Good.cfg")) {
                Config.Write("Key2", new string[] { "Value 2a", "Value 2b", "Value 2c" });
                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));

                Assert.Equal("Value 1", Config.Read("Key1", null));
                Assert.Equal("Value 3", Config.Read("Key3", null));

                var list = new List<string>(Config.Read("Key2"));
                Assert.Equal(3, list.Count);
                Assert.Equal("Value 2a", list[0]);
                Assert.Equal("Value 2b", list[1]);
                Assert.Equal("Value 2c", list[2]);
            }
        }

        [Fact(DisplayName = "Properties: Multi-value override is not written")]
        void DontOverwriteOverrideMulti() {
            using (var loader = new ConfigLoader("ReplaceOnlyLast.Good.cfg", resourceOverrideFileName: "RemoveMulti.cfg")) {
                Config.Write("Key2", "Value X");
                var list = new List<string>(Config.Read("Key2"));
                Assert.Equal(3, list.Count);
                Assert.Equal("Value 2a", list[0]);
                Assert.Equal("Value 2b", list[1]);
                Assert.Equal("Value 2c", list[2]);
            }
        }


        [Fact(DisplayName = "Properties: Test conversion")]
        void TestConversion() {
            using (var loader = new ConfigLoader(null, resourceFileNameGood: "WriteConverted.Good.cfg")) {
                Config.Write("Integer", 42);
                Config.Write("Integer Min", int.MinValue);
                Config.Write("Integer Max", int.MaxValue);
                Config.Write("Long", 42L);
                Config.Write("Long Min", long.MinValue);
                Config.Write("Long Max", long.MaxValue);
                Config.Write("Boolean", true);
                Config.Write("Double", 42.42);
                Config.Write("Double Pi", Math.PI);
                Config.Write("Double Third", 1.0 / 3);
                Config.Write("Double Seventh", 1.0 / 7);
                Config.Write("Double Min", double.MinValue);
                Config.Write("Double Max", double.MaxValue);
                Config.Write("Double NaN", double.NaN);
                Config.Write("Double Infinity+", double.PositiveInfinity);
                Config.Write("Double Infinity-", double.NegativeInfinity);

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));

                using (var loader2 = new ConfigLoader(loader.FileName, resourceFileNameGood: "WriteConverted.Good.cfg")) {
                    Assert.Equal(42, Config.Read("Integer", 0));
                    Assert.Equal(int.MinValue, Config.Read("Integer Min", 0));
                    Assert.Equal(int.MaxValue, Config.Read("Integer Max", 0));
                    Assert.Equal(42, Config.Read("Long", 0L));
                    Assert.Equal(long.MinValue, Config.Read("Long Min", 0L));
                    Assert.Equal(long.MaxValue, Config.Read("Long Max", 0L));
                    Assert.Equal(true, Config.Read("Boolean", false));
                    Assert.Equal(42.42, Config.Read("Double", 0.0));
                    Assert.Equal(Math.PI, Config.Read("Double Pi", 0.0));
                    Assert.Equal(1.0 / 3, Config.Read("Double Third", 0.0));
                    Assert.Equal(1.0 / 7, Config.Read("Double Seventh", 0.0));
                    Assert.Equal(double.MinValue, Config.Read("Double Min", 0.0));
                    Assert.Equal(double.MaxValue, Config.Read("Double Max", 0.0));
                    Assert.Equal(double.NaN, Config.Read("Double NaN", 0.0));
                    Assert.Equal(double.PositiveInfinity, Config.Read("Double Infinity+", 0.0));
                    Assert.Equal(double.NegativeInfinity, Config.Read("Double Infinity-", 0.0));
                }
            }
        }

        [Fact(DisplayName = "Properties: Key whitespace reading and saving")]
        void KeyWhitespace() {
            using (var loader = new ConfigLoader("KeyWhitespace.cfg", "KeyWhitespace.Good.cfg")) {
                Config.Save();

                Assert.Equal(loader.GoodText, File.ReadAllText(loader.FileName));

                Assert.Equal("Value 1", Config.Read("Key 1", null));
                Assert.Equal("Value 3", Config.Read("Key 3", null));

                var list = new List<string>(Config.Read("Key 2"));
                Assert.Equal(3, list.Count);
                Assert.Equal("Value 2a", list[0]);
                Assert.Equal("Value 2b", list[1]);
                Assert.Equal("Value 2c", list[2]);
            }
        }


        //TODO: Read multi with whitespace in key (\_)


        #region Utils

        private class ConfigLoader : IDisposable {

            public string FileName { get; }
            public byte[] Bytes { get; }
            public byte[] GoodBytes { get; }

            public ConfigLoader(string resourceFileName, string resourceFileNameGood = null, string resourceOverrideFileName = null) {
                if (File.Exists(resourceFileName)) {
                    this.Bytes = File.ReadAllBytes(resourceFileName);
                } else {
                    this.Bytes = (resourceFileName != null) ? GetResourceStreamBytes(resourceFileName) : null;
                }
                this.GoodBytes = (resourceFileNameGood != null) ? GetResourceStreamBytes(resourceFileNameGood) : null;
                var overrideBytes = (resourceOverrideFileName != null) ? GetResourceStreamBytes(resourceOverrideFileName) : null;

                this.FileName = Path.GetTempFileName();
                if (resourceFileName == null) {
                    File.Delete(this.FileName); //to start fresh
                } else {
                    File.WriteAllBytes(this.FileName, this.Bytes);
                }

                Config.FileName = this.FileName;

                var overrideFileName = (resourceOverrideFileName != null) ? Path.GetTempFileName() : null;
                if (overrideFileName != null) {
                    File.WriteAllBytes(overrideFileName, overrideBytes);
                    Config.OverrideFileName = overrideFileName;
                } else {
                    Config.OverrideFileName = null;
                }
            }

            private readonly Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            public string Text { get => Utf8.GetString(Bytes); }
            public string GoodText { get => Utf8.GetString(GoodBytes ?? new byte[0]); }

            #region IDisposable Support

            ~ConfigLoader() {
                this.Dispose(false);
            }

            protected virtual void Dispose(bool disposing) {
                try {
                    File.Delete(this.FileName);
                } catch (IOException) { }
            }

            public void Dispose() {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion

        }

        private static byte[] GetResourceStreamBytes(string fileName) {
            var resStream = typeof(PropertiesTests).GetTypeInfo().Assembly.GetManifestResourceStream("Test.Resources.Config." + fileName);
            var buffer = new byte[(int)resStream.Length];
            resStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        #endregion

    }
}