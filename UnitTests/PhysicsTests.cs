using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsFormsApp1;

namespace UnitTests
{
    [TestClass]
    public class PhysicsTests
    {
        [TestMethod]
        public void Physics_VerifyConstructor()
        {
            Physics physics = new Physics(true);
            Assert.IsNotNull(physics);
            physics = new Physics(false);
            Assert.IsNotNull(physics);
        }
    }
}
