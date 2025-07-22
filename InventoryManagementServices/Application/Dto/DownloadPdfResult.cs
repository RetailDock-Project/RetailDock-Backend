using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class DownloadPdfResult
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }
        public byte[] FileBytes { get; private set; }
        public string FileName { get; private set; }

        public static DownloadPdfResult Success(byte[] fileBytes, string fileName)
        {
            return new DownloadPdfResult
            {
                IsSuccess = true,
                FileBytes = fileBytes,
                FileName = fileName
            };
        }

        public static DownloadPdfResult Failure(string errorMessage)
        {
            return new DownloadPdfResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }

}
