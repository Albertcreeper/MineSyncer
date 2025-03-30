using Business.Files;
using NUnit.Framework;

namespace Business.Test.Files
{
    public class FileServiceTest
    {
        protected IFileService _FileService;

        protected static readonly string _LocalPath = @"C:\Users\Albert.Smidt\Documents\Minecraft\Local";
        protected static readonly string _RemotePath = @"C:\Users\Albert.Smidt\Documents\Minecraft\Remote";

        //protected static readonly string _LocalPath = @"C:\Users\asmidt\Documents\Minecraft\Local";
        //protected static readonly string _RemotePath = @"C:\Users\asmidt\Documents\Minecraft\Remote";

        [SetUp]
        public void Setup()
        {
            //_FileService = new FileService();

        }

        [Test]
        public void Copy()
        {
            _FileService.Copy(_RemotePath, _LocalPath, CopyOption.CopyOnlyChanges);
        }
    }
}