using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLi_WebDienThoai.PrePost
{
    public class NListEntry
    {
        public int Pre { get; set; }
        public int Post { get; set; }
        public int Count { get; set; }
    }

    public class PrePostPPCTreeViewModel
    {
        public PPCTreeNode FP_Root { get; set; }

        // Thêm dictionary lưu N‑list: key = tên item, value = list các entry (Pre,Post,Count)
        public Dictionary<string, List<NListEntry>> NLists { get; set; }
    }
}