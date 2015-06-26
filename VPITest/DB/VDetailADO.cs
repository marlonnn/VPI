using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Summer.System.Data.DbMapping;
using Summer.System.Data;

namespace VPITest.DB
{
    public class DetailADO : SmrAdoTmplate<VDetail>
    {
    }

    [TableAttribute("V_DETAIL")]
    public class VDetail
    {
        [FieldAttribute("vc_id", PrimaryKey = true)]
        public string Id;
        [FieldAttribute("vc_key")]
        public string Key;
        [FieldAttribute("dt_test")]
        public DateTime Start;
        [FieldAttribute("nm_running")]
        public long RunningTime;
        [FieldAttribute("vc_tester")]
        public string Tester;
        [FieldAttribute("vc_testtype")]
        public string TestType;
        [FieldAttribute("vc_istestpass")]
        public string IsTestPass;
        [FieldAttribute("vc_note")]
        public string TestNote;
        [FieldAttribute("vc_boardtype")]
        public string BoardType;
        [FieldAttribute("vc_boardname")]
        public string BoardName;
        [FieldAttribute("vc_sn")]
        public string BoardSn;
        [FieldAttribute("vc_component")]
        public string Component;
        [FieldAttribute("vc_istesteditempass")]
        public string IsTestedItemPass;
    }
}
