using FolderMillPrintApi;
using FolderMillPrintApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FolderMillPrintApiTest
{
    public class UnitTest1
    {
        #region snippet_Index_ReturnOkResult_WithListOfPrinterNames
        [Fact]
        public void Index_ReturnOkResult_WithListOfPrinterNames()
        {
            // Arrange
            var mockConfig = new Mock<IAppSettings>();
            mockConfig.Setup(cfg => cfg.PrintConfig).Returns(SetupConfig());
            var controller = new FolderMillPrintApi.Controllers.Print(mockConfig.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var models = Assert.IsAssignableFrom<IEnumerable<string>>(viewResult.Value);
            Assert.Equal(2, models.Count());
        }
        #endregion

        #region snippet_IndexPost_ReturnsBadRequestResult_WhenParameterNull
        [Fact]
        public async Task IndexPost_ReturnsBadRequestResult_WhenParameterNull()
        {
            // Arrange
            PrintRequest printRequest = null;
            var mockConfig = new Mock<IAppSettings>();
            mockConfig.Setup(cfg => cfg.PrintConfig).Returns(SetupConfig());
            var controller = new FolderMillPrintApi.Controllers.Print(mockConfig.Object);

            // Act
            var result = await controller.Index(printRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }
        #endregion

        #region snippet_IndexPost_ReturnsBadRequestResult_WhenParameterEmpty
        [Fact]
        public async Task IndexPost_ReturnsBadRequestResult_WhenParameterEmpty()
        {
            // Arrange
            var printRequest = new PrintRequest();
            var mockConfig = new Mock<IAppSettings>();
            mockConfig.Setup(cfg => cfg.PrintConfig).Returns(SetupConfig());
            var controller = new FolderMillPrintApi.Controllers.Print(mockConfig.Object);

            // Act
            var result = await controller.Index(printRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }
        #endregion

        #region snippet_IndexPost_ReturnsBadRequestResult_WhenFailedConvertBase64
        [Fact]
        public async Task IndexPost_ReturnsBadRequestResult_WhenFailedConvertBase64()
        {
            // Arrange
            var printRequest = GetPrintRequest(false);
            var mockConfig = new Mock<IAppSettings>();
            mockConfig.Setup(cfg => cfg.PrintConfig).Returns(SetupConfig());
            var controller = new FolderMillPrintApi.Controllers.Print(mockConfig.Object);

            // Act
            var result = await controller.Index(printRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }
        #endregion

        #region snippet_IndexPost_ReturnsBadRequestResult_WhenFailedWriteFile
        [Fact]
        public async Task IndexPost_ReturnsBadRequestResult_WhenFailedWriteFile()
        {
            // Arrange
            var printRequest = GetPrintRequest();
            string fileName = System.IO.Path.GetFileNameWithoutExtension(printRequest.FileName);
            string fileExt = System.IO.Path.GetExtension(printRequest.FileName);

            var mockConfig = new Mock<IAppSettings>();
            mockConfig.Setup(cfg => cfg.PrintConfig).Returns(SetupConfig(false));
            var controller = new FolderMillPrintApi.Controllers.Print(mockConfig.Object);

            // Act
            var result = await controller.Index(printRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }
        #endregion

        #region snippet_IndexPost_ReturnsOkResult_WhenSuccess
        [Fact]
        public async Task IndexPost_ReturnsOkResult_WhenSuccess()
        {
            // Arrange
            var printRequest = GetPrintRequest();
            string fileName = System.IO.Path.GetFileNameWithoutExtension(printRequest.FileName);
            string fileExt = System.IO.Path.GetExtension(printRequest.FileName);

            var mockConfig = new Mock<IAppSettings>();
            mockConfig.Setup(cfg => cfg.PrintConfig).Returns(SetupConfig());
            var controller = new FolderMillPrintApi.Controllers.Print(mockConfig.Object);

            string expectedFileName = $"{DateTime.Now.ToString("yyMMdd_hhmmss")}_{fileName}_{printRequest.Username}_{printRequest.PrinterName}{fileExt}";
            string fullPathFileName = System.IO.Path.Combine(SetupConfig().HotFolder, expectedFileName);

            // Act
            var result = await controller.Index(printRequest);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(System.IO.File.Exists(fullPathFileName));
        }
        #endregion

        private PrintConfig SetupConfig(bool validConfig = true)
        {
            string hotFolder = "/invalidpath";
            if (validConfig)
            {
                hotFolder = AppDomain.CurrentDomain.BaseDirectory;
            }

            return new PrintConfig()
            {
                HotFolder = hotFolder,
                Printers = new System.Collections.Generic.List<string>()
                {
                    {"Printer 1" },
                    {"Printer 2" }
                }
            };
        }

        private PrintRequest GetPrintRequest(bool valid = true)
        {
            var printRequest = new PrintRequest()
            {
                Document = "JVBERi0xLjcKJeLjz9MKMTAgMCBvYmoKPDwvRmlsdGVyL0ZsYXRlRGVjb2RlL0xlbmd0aCAzNj4+c3RyZWFtCnicK+QyVADBonQuYzMFIDI1MlYwNzdQKErlSuMK5AIAa5sGiAplbmRzdHJlYW0KZW5kb2JqCjUgMCBvYmoKPDwvRmlsdGVyL0ZsYXRlRGVjb2RlL0xlbmd0aCA2MDY+PnN0cmVhbQp4nK2V247TMBCG7/0UvoSLuvb4mL1bVFhVYlfdEnEfutk20ANNWhBCvAuPyiRNSk9eGxX1InE7/mbGv+fvmrxJSf+doIIzTdNnIkBS6yQTCU2fCGcCEiUoPpVuHoZbR8speTVKGb29f6DD5dNqmVdF9jr9TN6m5JGsiXQMKMePtEg1mhlNrTb4PlmQ/nAh6GDVBF5KbgRLZCD5x3xWTOY5fb+aFtWmmFR0UHwrqmK1PKiigztmG7pUkhmBCbA6FepuW05mWZXTcb7e5tXmANu/qwSdVvjGmx4xXFkG2KFMmLJU87obXictc/Lc7NlHagzFH6ziDIAuup275fwYNN9Hn6678Bp+0KndnaJy1AblGw7OT6rdLxLFlIpg3LyAQLEhAiG8CMBro2UEYjSmg2yTe0HSGmZiQP52pMU7JCIQAL37rOwBB39jtTzc1DoH+3rYLj7lZVCoGFpYq7iawoLFcMb5Iiu/VEHNruusky2GkuKQF8spHY0vOAg0ujWzaRyweh10j2z21z3oIN9kxfysX+gq1bJhG2is6h8Hd0/B0kwSR+n87dzejpHgEqZVJDMr/SCFHmYhDvS4+eHnWMW0i+PsT//YHjw7xnekMe3vndIamNPo0a3rtus5+XDhqrUCSheuyj9BrX4xkJ8nArYX7OTbX/5hbXW9JhfK7U/Q6R11IuDHtHLHYKQIm2+rk1BhnL+oVqcYyP/S6ZpcUTrFJBA6qFMMhove7dcznThzlusEI50zYPGZcPxrOh9LcMdjuVt3YxmN4eoYs1ufTPeh82vHw87/e9/RH6FwhqkKZW5kc3RyZWFtCmVuZG9iago0IDAgb2JqCjw8L0NvbnRlbnRzWzEwIDAgUiA1IDAgUl0vTWVkaWFCb3hbMCAwIDU5NSA4NDJdL1BhcmVudCAyIDAgUi9SZXNvdXJjZXM8PC9FeHRHU3RhdGU8PC9HczEgMTEgMCBSPj4vRm9udDw8L0YxIDYgMCBSL0YyIDcgMCBSPj4vWE9iamVjdDw8L0ltMSAxMiAwIFI+Pj4+L1RyaW1Cb3hbMCAwIDU5NSA4NDJdL1R5cGUvUGFnZT4+CmVuZG9iagoxIDAgb2JqCjw8L1BhZ2VzIDIgMCBSL1R5cGUvQ2F0YWxvZz4+CmVuZG9iagozIDAgb2JqCjw8L0NyZWF0aW9uRGF0ZShEOjIwMjEwMzIyMDc0MDUwKzA3JzAwJykvTW9kRGF0ZShEOjIwMjEwMzIyMDc0MDUwKzA3JzAwJykvUHJvZHVjZXIoaVRleHSuIDcuMS4xNCCpMjAwMC0yMDIwIGlUZXh0IEdyb3VwIE5WIFwoQUdQTC12ZXJzaW9uXCkpL1RpdGxlKERvY3VtZW50KS92aWV3cG9ydCh3aWR0aD1kZXZpY2Utd2lkdGgsIGluaXRpYWwtc2NhbGU9MSk+PgplbmRvYmoKNiAwIG9iago8PC9CYXNlRm9udC9UaW1lcy1Sb21hbi9FbmNvZGluZy9XaW5BbnNpRW5jb2RpbmcvU3VidHlwZS9UeXBlMS9UeXBlL0ZvbnQ+PgplbmRvYmoKNyAwIG9iago8PC9CYXNlRm9udC9UaW1lcy1Cb2xkL0VuY29kaW5nL1dpbkFuc2lFbmNvZGluZy9TdWJ0eXBlL1R5cGUxL1R5cGUvRm9udD4+CmVuZG9iagoyIDAgb2JqCjw8L0NvdW50IDEvS2lkc1s0IDAgUl0vVHlwZS9QYWdlcz4+CmVuZG9iagoxMSAwIG9iago8PC9DQSAwLjI1L2NhIDAuMjU+PgplbmRvYmoKMTIgMCBvYmoKPDwvQml0c1BlckNvbXBvbmVudCA4L0NvbG9yU3BhY2UvRGV2aWNlUkdCL0ZpbHRlci9EQ1REZWNvZGUvSGVpZ2h0IDEwNy9MZW5ndGggNDkxMi9TdWJ0eXBlL0ltYWdlL1R5cGUvWE9iamVjdC9XaWR0aCAxMDk+PnN0cmVhbQr/2P/gABBKRklGAAEBAQDcANwAAP/bAEMAAwICAwICAwMDAwQDAwQFCAUFBAQFCgcHBggMCgwMCwoLCw0OEhANDhEOCwsQFhARExQVFRUMDxcYFhQYEhQVFP/bAEMBAwQEBQQFCQUFCRQNCw0UFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQU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",
                FileName = "Sample.pdf",
                PrinterName = "Bullzip PDF Printer",
                Username = "hskartono"
            };
            if (valid) return printRequest;

            printRequest = new PrintRequest()
            {
                Document = "invalid",
                FileName = "Sample.pdf",
                PrinterName = "Bullzip PDF Printer",
                Username = "hskartono"
            };
            return printRequest;
        }
    }
}
