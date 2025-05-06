using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLi_WebDienThoai.PrePost;
using QuanLi_WebDienThoai.Models;

namespace QuanLi_WebDienThoai.Controllers
{
    public class GoiYSanPhamController : Controller
    {
        Models.CSDLDataContext db = new Models.CSDLDataContext();
        CSDLDataContext data = new CSDLDataContext();
        // GET: GoiYSanPham
        PPCTreeNode fp = new PPCTreeNode();
        Dictionary<string, List<NListEntry>> nLists = new Dictionary<string, List<NListEntry>>();
        int minsup = 3;

        //Xây dựng cây PPCTree
        public void PrePost()
        {
            // --- Bước 1: Lấy dữ liệu từ bảng đon hàng và chi tiết đơn hàng ---
            var transactions = db.ChiTiet_DonHangs
                                 .Where(x => x.DonHang_id != null && x.SanPham_id != null && x.DonHang.TrangThai == 0)
                                 .GroupBy(x => x.DonHang_id.Value)
                                 .Select(g => g
                                     .Select(x => new { Item = x.SanPham_id.Value.ToString(), Count = x.SoLuong ?? 1 })
                                     .OrderBy(i => i.Item)
                                     .ToList())
                                 .ToList();

            // --- Bước 2: Tính tần suất các item ---
            var frequent = transactions.SelectMany(t => t)
                                   .GroupBy(i => i.Item)
                                   .ToDictionary(g => g.Key, g => g.Sum(i => i.Count));

            // --- Bước 3: Lọc theo minsup ---

            var frequentItems = frequent
                .Where(kv => kv.Value >= minsup)
                .Select(kv => kv.Key)
                .ToHashSet();

            // --- Bước 4: Chuẩn bị các giao dịch đã lọc và sắp xếp theo tần suất giảm ---
            var ordered = transactions
                .Select(t => t
                    .Where(i => frequentItems.Contains(i.Item))
                    .OrderByDescending(i => frequent[i.Item])
                    .ToList())
                .Where(t => t.Any())
                .ToList();

            // --- Bước 5: Xây dựng FP-Tree ---
            var fpRoot = new PPCTreeNode { Item = "null", Count = 0 };
            foreach (var t in ordered)
            {
                var curr = fpRoot;
                foreach (var entry in t)
                {
                    var item = entry.Item;
                    var weight = entry.Count;

                    var child = curr.Children.FirstOrDefault(c => c.Item == item);
                    if (child == null)
                    {
                        child = new PPCTreeNode
                        {
                            Item = item,
                            Count = weight,
                            Parent = curr
                        };
                        curr.Children.Add(child);
                    }
                    else
                    {
                        child.Count += weight;
                    }

                    curr = child;
                }
            }

            // --- Sau khi build xong fpRoot ---
            // Khởi tạo 2 counter riêng
            int preCounter = 0;
            int postCounter = 0;

            // Hàm đệ quy gán Pre & Post
            void AssignPrePost(PPCTreeNode node)
            {
                // Pre-order: gán trước khi xuống con
                node.Pre = preCounter++;

                // Duyệt con theo thứ tự trong list (tức "trái → phải")
                foreach (var child in node.Children)
                    AssignPrePost(child);

                // Post-order: gán sau khi đã duyệt hết con
                node.Post = postCounter++;
            }

            // Gọi bắt đầu từ gốc
            AssignPrePost(fpRoot);
            // sau AssignPrePost(fpRoot);

            fp = fpRoot;
        }

        //Sao chép N-list từ PPCTree đã tạo ra
        void BuildNList(PPCTreeNode node, Dictionary<string, List<NListEntry>> nLists)
        {
            if (node.Item != "null")
            {
                if (!nLists.ContainsKey(node.Item))
                    nLists[node.Item] = new List<NListEntry>();

                nLists[node.Item].Add(new NListEntry
                {
                    Pre = node.Pre,
                    Post = node.Post,
                    Count = node.Count
                });
            }
            foreach (var c in node.Children) BuildNList(c, nLists);
        }

        //Hiện thị thuật toán ra màn hình
        public ActionResult HienThiPrePost()
        {
            PrePost();

            BuildNList(fp, nLists);
            var vm = new PrePostPPCTreeViewModel
            {
                FP_Root = fp,
                NLists = nLists
            };
            return View(vm);
        }


        List<(List<int> ids, int support)> TapPhoBien = new List<(List<int> ids, int support)>();

        // Hàm giao hai N-List
        List<NListEntry> IntersectNList(List<NListEntry> a, List<NListEntry> b)
        {
            var result = new List<NListEntry>();
            foreach (var x in a)
            {
                foreach (var y in b)
                {
                    if (y.Pre > x.Pre && y.Post < x.Post)
                    {
                        result.Add(new NListEntry
                        {
                            Pre = y.Pre,
                            Post = y.Post,
                            Count = Math.Min(x.Count, y.Count)
                        });
                    }
                }
            }
            return result;
        }

        // Hàm đệ quy để sinh các tập phổ biến
        void GenerateFrequentItemsets(List<int> prefix, List<NListEntry> currentNList, List<string> remainingItems, Dictionary<string, List<NListEntry>> nLists, List<(List<int> ids, int support)> freq)
        {
            // Tính support của tập hiện tại và kiểm tra tính phổ biến
            int support = currentNList.Sum(x => x.Count);
            if (support >= minsup)
            {
                freq.Add((new List<int>(prefix), support));
            }
            else
            {
                return; // Cắt tỉa nếu không phổ biến
            }

            // Mở rộng với từng item còn lại
            for (int i = 0; i < remainingItems.Count; i++)
            {
                int newItem = int.Parse(remainingItems[i]);
                List<NListEntry> newNList = IntersectNList(currentNList, nLists[newItem.ToString()]);
                if (newNList.Any()) // Chỉ tiếp tục nếu giao không rỗng
                {
                    List<int> newPrefix = new List<int>(prefix) { newItem };
                    List<string> newRemaining = remainingItems.Skip(i + 1).ToList();
                    GenerateFrequentItemsets(newPrefix, newNList, newRemaining, nLists, freq);
                }
            }
        }

        //Giao các N-list để tìm tập phổ biến
        List<(List<int> ids, int support)> FrequentItemset()
        {
            PrePost();

            BuildNList(fp, nLists);

            // Danh sách lưu các tập phổ biến
            //var freq = new List<string>();
            var frequent = new List<(List<int> ids, int support)>();

            // Lấy danh sách item và sắp xếp
            var items = nLists.Keys.OrderBy(i => i).ToList();

            // Gọi hàm đệ quy cho từng item làm tiền tố ban đầu
            for (int i = 0; i < items.Count; i++)
            {
                int item = int.Parse(items[i]);
                List<int> prefix = new List<int> { item };
                List<NListEntry> nList = nLists[item.ToString()];
                List<string> remaining = items.Skip(i + 1).ToList();
                GenerateFrequentItemsets(prefix, nList, remaining, nLists, frequent);
            }

            return frequent;
        }

        //Hiển thị các tập phổ biến
        public ActionResult HienThiTepPhoBien()
        {
            TapPhoBien = FrequentItemset();
            ViewBag.FrequentItemsets = TapPhoBien;
            return View();
        }

        //Hàm gợi ý sản phẩm cho khách hàng
        public ActionResult SanPhamGoiY()
        {
            TapPhoBien = FrequentItemset();

            if (Session["UserID"] != null)
            {
                var dh = db.DonHangs
                    .Where(d => d.User_id == (int)Session["UserID"])
                    .OrderByDescending(d => d.id)
                    .FirstOrDefault();
                var sp = db.ChiTiet_DonHangs
                     .Where(ct => ct.DonHang_id == dh.id)
                     .Select(ct => ct.SanPham_id.Value) // ép sang int
                     .Distinct()
                     .ToList();

                // Lấy tối đa 3 danh sách gợi ý
                var goiY = TapPhoBien
                    .Where(tap => !sp.Except(tap.ids).Any())
                    .Select(tap => new
                    {
                        SanPhamGoiY = tap.ids.Except(sp).ToList(),
                        Support = tap.support
                    })
                    .Where(x => x.SanPhamGoiY.Any())
                    .OrderByDescending(x => x.Support)
                    .Take(3) // Lấy tối đa 3 tập
                    .Select(x => x.SanPhamGoiY)
                    .ToList(); // goiY là List<List<int>>

                // Gộp tất cả id từ các danh sách gợi ý
                var allSanPhamIds = goiY.SelectMany(list => list).Distinct().ToList();

                // Lấy thông tin sản phẩm gợi ý
                var listGoiY = db.San_Phams
                    .Where(s => allSanPhamIds.Contains(s.id))
                    .Select(s => new Models.SanPhamModel
                    {
                        SP = s,
                        HinhAnh = db.Images
                            .Where(i => i.SanPham_id == s.id)
                            .Select(i => i.Name)
                            .FirstOrDefault()
                    })
                    .ToList();

                return PartialView(listGoiY);

            }

            return PartialView(new List<Models.SanPhamModel>());

        }
    }
}