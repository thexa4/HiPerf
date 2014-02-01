using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiPerf;

namespace HiPerfTest
{
    class TestObject : Applyable
    {
        public int _globalManaged1 = 15;
        public string _globalManaged2 = "hello world";
        public object _globalManaged3 = null;
        private string _globalManaged4 = null;

        [LocallyManaged]
        public int _localManaged1 = 2;

        public string Global4 { get { return _globalManaged4; } set { _globalManaged4 = value; } }
    }
}
