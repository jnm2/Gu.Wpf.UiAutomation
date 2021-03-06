// ReSharper disable RedundantNameQualifier
namespace Gu.Wpf.UiAutomation.UiTests
{
    using System.IO;
    using NUnit.Framework;

    public class ImageAssertTests
    {
        private const string ExeFileName = "WpfApplication.exe";

        [SetUp]
        public void SetUp()
        {
            ImageAssert.OnFail = OnFail.DoNothing;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(ExeFileName);
        }

        [Test]
        public void WhenEqualExplicitPath()
        {
            using (var app = Application.AttachOrLaunch(ExeFileName, "SizeWindow"))
            {
                var window = app.MainWindow;
                var button = window.FindButton("SizeButton");
                var fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Images\button.png");
                ImageAssert.AreEqual(fileName, button);
            }
        }

        [Test]
        public void WhenEqualRelativePath()
        {
            using (var app = Application.AttachOrLaunch(ExeFileName, "SizeWindow"))
            {
                var window = app.MainWindow;
                var button = window.FindButton("SizeButton");
                ImageAssert.AreEqual(@".\Images\button.png", button);
            }
        }

        [Test]
        public void WhenEqualResourceName()
        {
            using (var app = Application.AttachOrLaunch(ExeFileName, "SizeWindow"))
            {
                var window = app.MainWindow;
                var button = window.FindButton("SizeButton");
                ImageAssert.AreEqual(@"button_resource", button);
            }
        }

        [Test]
        public void WhenEqualResource()
        {
            using (var app = Application.AttachOrLaunch(ExeFileName, "SizeWindow"))
            {
                var window = app.MainWindow;
                var button = window.FindButton("SizeButton");
                ImageAssert.AreEqual(Properties.Resources.button_resource, button);
            }
        }

        [Test]
        public void WhenEqualBmp()
        {
            ImageAssert.AreEqual(Properties.Resources.SquareBmp, Properties.Resources.SquareBmp);
            ImageAssert.AreEqual(Properties.Resources.SquareBmp, Properties.Resources.SquareBmp, _ => { });
        }

        [Test]
        public void WhenEqualPng()
        {
            ImageAssert.AreEqual(Properties.Resources.SquarePng, Properties.Resources.SquarePng);
            ImageAssert.AreEqual(Properties.Resources.SquarePng, Properties.Resources.SquarePng, _ => { });
        }

        [Test]
        public void WhenEqualBmpPng()
        {
            ImageAssert.AreEqual(Properties.Resources.SquareBmp, Properties.Resources.SquarePng);
            ImageAssert.AreEqual(Properties.Resources.SquareBmp, Properties.Resources.SquarePng, _ => { });
            ImageAssert.AreEqual(Properties.Resources.SquarePng, Properties.Resources.SquareBmp);
            ImageAssert.AreEqual(Properties.Resources.SquarePng, Properties.Resources.SquareBmp, _ => { });
        }

        [Test]
        public void WhenNotEqual()
        {
            using (var app = Application.AttachOrLaunch(ExeFileName, "SizeWindow"))
            {
                var window = app.MainWindow;
                var fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Images\button.png");
                var exception = Assert.Throws<NUnit.Framework.AssertionException>(() => ImageAssert.AreEqual(fileName, window));
                Assert.AreEqual("Sizes did not match\r\nExpected: {Width=200, Height=100}\r\nActual:   {Width=300, Height=300}", exception.Message);
            }
        }

        [Test]
        public void WhenNotEqualSaveFileToTempRootedPath()
        {
            using (var app = Application.AttachOrLaunch(ExeFileName, "SizeWindow"))
            {
                var window = app.MainWindow;
                var fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Images\button.png");
                var tempFile = Path.Combine(Path.GetTempPath(), "button.png");
                File.Delete(tempFile);
                ImageAssert.OnFail = OnFail.SaveImageToTemp;
                Assert.Throws<NUnit.Framework.AssertionException>(() => ImageAssert.AreEqual(fileName, window));
                Assert.AreEqual(true, File.Exists(tempFile));
            }
        }

        [TestCase(@".\Images\button.png")]
        [TestCase(@"Images\button.png")]
        public void WhenNotEqualSaveFileToTempRelativePath(string fileName)
        {
            using (var app = Application.AttachOrLaunch(ExeFileName, "SizeWindow"))
            {
                var window = app.MainWindow;
                var tempFile = Path.Combine(Path.GetTempPath(), "button.png");
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }

                ImageAssert.OnFail = OnFail.SaveImageToTemp;
                Assert.Throws<NUnit.Framework.AssertionException>(() => ImageAssert.AreEqual(fileName, window));
                Assert.AreEqual(true, File.Exists(tempFile));
            }
        }

        [TestCase("button_resource")]
        public void WhenNotEqualSaveFileToResourceName(string fileName)
        {
            using (var app = Application.AttachOrLaunch(ExeFileName, "SizeWindow"))
            {
                var window = app.MainWindow;
                var tempFile = Path.Combine(Path.GetTempPath(), "button_resource.png");
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }

                ImageAssert.OnFail = OnFail.SaveImageToTemp;
                Assert.Throws<NUnit.Framework.AssertionException>(() => ImageAssert.AreEqual(fileName, window));
                Assert.AreEqual(true, File.Exists(tempFile));
            }
        }

        [Test]
        public void WhenNotEqualWithAction()
        {
            using (var app = Application.AttachOrLaunch(ExeFileName, "SizeWindow"))
            {
                var window = app.MainWindow;
                var fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Images\button.png");
                var count = 0;
                Assert.Throws<NUnit.Framework.AssertionException>(() => ImageAssert.AreEqual(fileName, window, (exception, bitmap) => count++));
                Assert.AreEqual(1, count);
            }
        }
    }
}