using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HiPerfTest
{
    [TestClass]
    public class ApplyableTest
    {
        [TestMethod]
        public void Applyable_Apply1()
        {
            var o = new TestObject();
            o._globalManaged1 = 0;
            o.Queue(() => o._globalManaged1++);
            o.Apply();
            Assert.AreEqual(o._globalManaged1, 1);
        }

#if CHECK_APPLY
        [TestMethod]
        public void Applyable_Apply2()
        {
            var o = new TestObject();
            o._globalManaged1 = 0;
            o.Queue(() => o._globalManaged1++);
            o.Apply();
            o.Apply();
            o._globalManaged1++;
            try
            {
                o.Apply();
            } catch (UnauthorizedAccessException)
            {
                return;
            }
            Assert.Fail("Unauthorized access not detected!");
        }

        [TestMethod]
        public void Applyable_Apply3()
        {
            var o = new TestObject();
            o._globalManaged1 = 0;
            o.Queue(() => o._globalManaged1++);
            o.Apply();
            o.Apply();
            o._localManaged1++;
            o.Apply();
        }
#endif
    }
}
