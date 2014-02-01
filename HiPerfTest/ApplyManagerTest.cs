using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HiPerf;
using System.Linq;

namespace HiPerfTest
{
    [TestClass]
    public class ApplyManagerTest
    {
        [TestMethod]
        public void ApplyManager_AddRemove1()
        {
            var m = new ApplyManager();
            m.Add(new TestObject());
            var o = m.First();
            Assert.AreEqual(1, m.Count);
            m.Remove(o);
            Assert.AreEqual(0, m.Count);
        }

        [TestMethod]
        public void ApplyManager_TestApply1()
        {
            var m = new ApplyManager<TestObject>();
            m.Add(new TestObject());
            m.Add(new TestObject());
            m.Add(new TestObject());
            m.Add(new TestObject());
            m.Add(new TestObject());
            foreach (var o in m)
                o.Queue(() => o._globalManaged2 = "passed");
            m.Apply();
            foreach (var o in m)
                Assert.AreEqual("passed", o._globalManaged2);
        }
    }
}
