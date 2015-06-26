using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Summer.System.Data.DbMapping;
using Summer.System.Data;

namespace VPITest.DB
{
    public class BoardADO : SmrAdoTmplate<TBoard>
    {
    }

    [TableAttribute("T_BOARD")]
    public class TBoard
    {
        [FieldAttribute("vc_id", PrimaryKey = true)]
        public string Id;
        [FieldAttribute("vc_key")]
        public string Key;
        [FieldAttribute("vc_type")]
        public string Type;
        [FieldAttribute("vc_name")]
        public string Name;
        [FieldAttribute("vc_sn")]
        public string Sn;
        [FieldAttribute("vc_component")]
        public string Component;
        [FieldAttribute("vc_ispass")]
        public string IsPass;
    }
}
