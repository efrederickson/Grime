using Grime.Core.Executor;

namespace Grime.Core.Tests.Executor
{
    [TestClass]
    public sealed class TestGMath
    {
        [TestMethod]
        public void TestUnsignedToSigned()
        {
            var res = GMath.UnsignedToSigned(0xC1);
            Assert.IsTrue(res == -63, $"Expected -63, got {res}");
        }
        [TestMethod]
        public void TestSignedToUnsigned()
        {
            var res = GMath.SignedToUnsigned(-63);
            Assert.IsTrue(res == 0b1100001, $"Expected 0b1100001 got {res:B}");
        }
    }
}
