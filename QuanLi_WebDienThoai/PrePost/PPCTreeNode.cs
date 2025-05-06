using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLi_WebDienThoai.PrePost
{
    public class PPCTreeNode
    {
        public string Item { get; set; }
        public int Count { get; set; }
        public int Pre { get; set; }  
        public int Post { get; set; } 
        public PPCTreeNode Parent { get; set; }
        public List<PPCTreeNode> Children { get; set; } = new List<PPCTreeNode>();
    }
}