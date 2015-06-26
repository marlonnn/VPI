using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Summer.System.Data.DbMapping;
using Summer.System.Data;

namespace VPITest.DB
{
    public class MainADO : SmrAdoTmplate<TMain>
    {
    }

    [TableAttribute("T_MAIN")]
    public class TMain
    {
        [FieldAttribute("vc_key", PrimaryKey = true)]
        public string Key;
        [FieldAttribute("dt_test")]
        public DateTime Start;
        [FieldAttribute("nm_running")]
        public long RunningTime;
        [FieldAttribute("vc_tester")]
        public string Tester;
        [FieldAttribute("vc_testtype")]
        public string TestType;
        [FieldAttribute("vc_ispass")]
        public string IsPass;
        [FieldAttribute("vc_note")]
        public string Note;
    }
}
