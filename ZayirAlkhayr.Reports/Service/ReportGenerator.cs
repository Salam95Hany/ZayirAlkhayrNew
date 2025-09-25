using RazorLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Reports.Interface;
using ZayirAlkhayr.Reports.Model;

namespace ZayirAlkhayr.Reports.Service
{
    public abstract class ReportGenerator
    {
        private readonly IRazorLightEngine _razorEngine;
        private readonly IPDFHelper _pDFHelper;
        public ReportType ReportType { get; set; }

        protected ReportGenerator(IRazorLightEngine razorEngine, IPDFHelper pDFHelper)
        {
            _razorEngine = razorEngine;
            _pDFHelper = pDFHelper;
        }

        public async Task<string> BuildAsync(object model, bool isLandScape)
        {
            try
            {
                var html = await _razorEngine.CompileRenderAsync($"{ReportType.ToString()}.cshtml", model);
                ReportFileLogger.Log("Html Loaded Successfully");
                var FilePath = _pDFHelper.SaveHTMLResult(html, isLandScape);
                ReportFileLogger.Log($"PDF File Path: {FilePath}");
                return FilePath;
            }
            catch (Exception ex)
            {
                ReportFileLogger.Log(ex.Message);
                return "";
            }
        }
    }
}
