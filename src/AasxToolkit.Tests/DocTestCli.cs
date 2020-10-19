// This file was automatically generated by doctest-csharp.
// !!! DO NOT EDIT OR APPEND !!!

using NUnit.Framework;

namespace AasxToolkit.Tests
{
    public class DocTest_Cli_cs
    {
        [Test]
        public void AtLine184AndColumn16()
        {
            Assert.AreEqual("", Cli.Indentation.Indent("", "  "));
        }

        [Test]
        public void AtLine185AndColumn16()
        {
            Assert.AreEqual("  test", Cli.Indentation.Indent("test", "  "));
        }

        [Test]
        public void AtLine186AndColumn16()
        {
            var nl = System.Environment.NewLine;
            Assert.AreEqual($"  test{nl}  me", Cli.Indentation.Indent("test\nme", "  "));
        }

        [Test]
        public void AtLine190AndColumn16()
        {
            var nl = System.Environment.NewLine;
            Assert.AreEqual($"  test{nl}  me", Cli.Indentation.Indent("test\r\nme", "  "));
        }
    }
}

// This file was automatically generated by doctest-csharp.
// !!! DO NOT EDIT OR APPEND !!!
