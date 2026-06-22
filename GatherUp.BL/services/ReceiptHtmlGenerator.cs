using GatherUp.Core.DO.Finance;

namespace GatherUp.BL.Services
{
    public static class ReceiptHtmlGenerator
    {
        public static string Generate(ReceiptDetails r) => $@"<!DOCTYPE html>
<html lang=""he"" dir=""rtl"">
<head>
<meta charset=""UTF-8"">
<title>קבלה {r.ReceiptNumber}</title>
<style>
  body {{ font-family: 'Segoe UI', sans-serif; background:#f4f6fb; display:flex; justify-content:center; padding:40px; }}
  .receipt {{ background:white; border-radius:14px; padding:40px; width:420px; box-shadow:0 4px 20px rgba(0,0,0,0.1); }}
  h1 {{ color:#667eea; font-size:24px; margin-bottom:8px; }}
  .logo {{ font-size:28px; font-weight:900; color:#667eea; margin-bottom:20px; }}
  hr {{ border:none; border-top:2px solid #f0f0f0; margin:20px 0; }}
  .row {{ display:flex; justify-content:space-between; margin-bottom:12px; font-size:15px; }}
  .label {{ color:#888; }}
  .value {{ font-weight:600; color:#333; }}
  .total {{ background:#f8f9ff; border-radius:10px; padding:16px; margin-top:20px; display:flex; justify-content:space-between; }}
  .total .label {{ font-size:16px; font-weight:700; color:#667eea; }}
  .total .value {{ font-size:20px; font-weight:900; color:#667eea; }}
  .footer {{ text-align:center; color:#bbb; font-size:12px; margin-top:24px; }}
</style>
</head>
<body>
<div class=""receipt"">
  <div class=""logo"">GatherUp</div>
  <h1>קבלה</h1>
  <hr>
  <div class=""row""><span class=""label"">מספר קבלה</span><span class=""value"">#{r.ReceiptNumber}</span></div>
  <div class=""row""><span class=""label"">תאריך</span><span class=""value"">{r.Date:dd/MM/yyyy HH:mm}</span></div>
  <hr>
  <div class=""total""><span class=""label"">סכום לתשלום</span><span class=""value"">{r.Amount:F2} ₪</span></div>
  <div class=""footer"">תודה על השימוש ב-GatherUp</div>
</div>
</body>
</html>";
    }
}
