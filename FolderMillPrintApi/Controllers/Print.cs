using FolderMillPrintApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FolderMillPrintApi.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class Print : Controller
    {
        private readonly IAppSettings _appSettings;
        private readonly PrintConfig _printConfig;

        #region snippet_Constructor
        public Print(IAppSettings appSettings)
        {
            _appSettings = appSettings;
            if (_appSettings != null)
                _printConfig = _appSettings.PrintConfig;
        }
        #endregion

        #region snippet_Index_GET
        [HttpGet]
        public IActionResult Index()
        {
            List<string> printers = _printConfig.Printers;

            return Ok(printers);
        }
        #endregion

        #region snippet_Index_POST
        [HttpPost]
        public async Task<IActionResult> Index([FromBody] PrintRequest printRequest)
        {
            // validate parameters
            if (printRequest == null)
            {
                ModelState.AddModelError("printRequest", "Request print harus diisi json.");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(printRequest.Document))
            {
                ModelState.AddModelError("Document", "Property Document harus diisi dengan string base64 dari file yang akan dicetak.");
            }

            if (string.IsNullOrEmpty(printRequest.FileName))
            {
                ModelState.AddModelError("FileName", "FileName harus diisi.");
            }

            if (string.IsNullOrEmpty(printRequest.PrinterName))
            {
                ModelState.AddModelError("PrinterName", "PrinterName harus diisi.");
            }

            if (string.IsNullOrEmpty(printRequest.Username))
            {
                ModelState.AddModelError("Username", "Username harus diisi.");
            }

            // decode base64 back to byte arrays
            byte[] document = null;
            try
            {
                document = Convert.FromBase64String(printRequest.Document);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ConvertBase64", "Gagal melakukan konversi Document dari Base64. " + ex.Message);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert filename to FILENAME_USERNAME_PRINTERNAME.EXT
            string fileName = System.IO.Path.GetFileNameWithoutExtension(printRequest.FileName);
            string fileExt = System.IO.Path.GetExtension(printRequest.FileName);
            string printerName = printRequest.PrinterName.Replace(" ", "_");
            string formattedFileName = $"{DateTime.Now.ToString("yyMMdd_hhmmss")}_{fileName}_{printRequest.Username}_{printerName.Trim()}{fileExt}";

            // save file to destination folder
            string destinationFileName = "";
            try
            {
                destinationFileName = System.IO.Path.Combine(_printConfig.HotFolder, formattedFileName);
                await System.IO.File.WriteAllBytesAsync(destinationFileName, document);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("WriteAllBytesAsync", $"Gagal menulis file '{destinationFileName}'. {ex.Message}");
                return BadRequest(ModelState);
            }

            return Ok(200);
        }
        #endregion
    }
}
