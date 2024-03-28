using Business.Files;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Test.Files
{
    public class OverlayIconInfoTest
    {
        [Test]
        public void ExtractIconInfoTest()
        {
            // FileInfo fileInfo = new FileInfo(@"C:\Users\alber\OneDrive\Savegames\Minecraft\MineSyncer.ini");
            DirectoryInfo fileInfo = new DirectoryInfo(@"C:\Users\alber\OneDrive\Savegames\Minecraft\nIwYANKBAQA=");
            OverlayIconInfo icon = new OverlayIconInfo(fileInfo);

            var attributes = File.GetAttributes(@"C:\Users\alber\OneDrive\Savegames\Minecraft\MineSyncer.ini");

            FileAttributes fileAttr = fileInfo.Attributes;

            Assert.AreEqual(icon.IconOverlayGuid, new Guid("{F241C880-6982-4CE5-8CF7-7085BA96DA5A}"));
        }
    }
}
