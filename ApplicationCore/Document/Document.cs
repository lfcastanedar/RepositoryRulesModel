using AutoMapper;
using Infrastructure.Configuration;
using Infrastructure.DTO;
using Infrastructure.Ethereum;
using iText.Html2pdf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static iText.Svg.SvgConstants;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;


namespace ApplicationCore.Document
{
    public  class Document
    {
        private SmartContractContext _smartContractContext;
        private ParamConfiguration _paramConfiguration { get; set; }

        public Document(Infrastructure.Ethereum.SmartContractContext smartContractContext)
        {
            _smartContractContext = smartContractContext;
        }

        public Document(Infrastructure.Ethereum.SmartContractContext smartContractContext, ParamConfiguration paramConfiguration) {
            _smartContractContext = smartContractContext;
            _paramConfiguration = paramConfiguration;
        }        

        public DetailSmartContractDTO GetDetail(string contractAddress) {
            return _smartContractContext.Get(contractAddress);
        }

        public List<PaperControlSmartContractDTO> GetPageControlsFromBlockchain(string contractAddress) { 
            return _smartContractContext.GetPageControls(contractAddress);
        }

        public object CreateContract(ReportDTO values) {
            string contractAddress = _smartContractContext.CreateContract(values);

            return new { Address = contractAddress };
        }

        public ControlPaperParamsDTO AddDocument(ControlPaperParamsDTO values) {
            string documentNumber = values.DocumentNumber.Replace("/", "_");
            string format = String.Empty;
            string pathDocumentFile = String.Empty;
            string pathUnifyDocument = String.Empty;
            string pathFolder = String.Concat(_paramConfiguration.DocumentFolder, documentNumber);

            bool existFolder = System.IO.Directory.Exists(pathFolder);
            if (!existFolder)
            {
                System.IO.Directory.CreateDirectory(pathFolder);
            }


            format = values.base64.Split(";")[0];
            format = format.Split("/")[1].ToUpper();
            values.Format = format;


            
            pathDocumentFile = String.Concat(pathFolder, "/", values.Id, ".", format.ToLower());
            values.NumberPages = SaveDocument(values.base64, pathDocumentFile);

            switch (_paramConfiguration.HashFunction.ToUpper())
            {
                case "SHA256":
                    values.HashAlgorithm = "SHA256";
                    values.HashCode = GetSHA256V2(pathDocumentFile);
                    break;
                default:
                    values.HashAlgorithm = "MD5";
                    values.HashCode = GetMD5V2(pathDocumentFile);
                    break;
            }



            

            var config = new MapperConfiguration(cfg => cfg.CreateMap<ControlPaperParamsDTO, ControlPaperDTO>());
            var mapper = new Mapper(config);
            var objectParameter = mapper.Map<ControlPaperDTO>(values);
            _smartContractContext.AddDocument(objectParameter, values.ContractAddress);

            var folderHash = GetDocumentFolderV2(new GetFolderDocumentDTO()
            {
                Address = values.ContractAddress,
                DocumentNumber = documentNumber,
                Title = "Prueba",
                PathDocument = pathFolder
            });


            _smartContractContext.SetDocumentHash(folderHash, values.ContractAddress);

            return values;
        }

        

        public object GetDocument(GetDocumentParamsDTO values)
        {
            string pathFolder = String.Concat(_paramConfiguration.DocumentFolder, values.DocumentNumber.Split("/")[0]);

            DirectoryInfo d = new DirectoryInfo(pathFolder);
            FileInfo[] Files = d.GetFiles("*.pdf");


            string fullpath = Files.Where(x => x.Name.Contains(values.Id)).FirstOrDefault().FullName;

            return new
            {
                base64 = GetBase64(fullpath)
            };
        }

        public object GetDocumentFolder(GetFolderDocument values)
        {
            string documentFolderPath = $"{_paramConfiguration.DocumentFolder}/{values.DocumentNumber}/{values.DocumentNumber}.pdf";
            return new
            {
                base64 = GetBase64(documentFolderPath)
            };

        }

        public FolderHash GetDocumentFolderV2(GetFolderDocumentDTO values, string signature = "")
        {
            FolderHash folderHash = new FolderHash();
            string bodyHtml = "";
            string converTemplatePath = "";
            List<FileInfo> documentsToDownload = new List<FileInfo>();
            string fileSavedPath = $"{values.PathDocument}/{values.DocumentNumber}.pdf";
            string html = "";
            string pathFolder = values.PathDocument;
            string pathFolderFile = $"{values.PathDocument}/{values.DocumentNumber}.pdf";
            string pathTempFile = $"{values.PathDocument}/{values.DocumentNumber}_temp.pdf";



            DirectoryInfo d = new DirectoryInfo(pathFolder);
            var files = d.GetFiles("*.pdf").ToList();

            var controlPages = GetPageControlsFromBlockchain(values.Address);

            if (controlPages.Count() == 0) {
                documentsToDownload.Add(files.FirstOrDefault());
            }
            controlPages.ForEach(x =>
            {
                if (ValidateDocument(pathFolder, x))
                {
                    documentsToDownload.Add(files.Where(y => y.Name.Contains(x.Id)).First());
                }
                else
                {
                    for (int i = 0; i < (x.EndPage - x.StartPage + 1); i++)
                    {
                        documentsToDownload.Add(GetNoValidatedTemplate());
                    }
                }

            });            


            converTemplatePath = String.Concat(_paramConfiguration.TemplateUrl, "CoverTemplate.html");


            html = File.ReadAllText(converTemplatePath, Encoding.UTF8);
            html = html.Replace("{ #folder }", values.DocumentNumber).Replace("{ #title }", values.Title).Replace("{ #address }", values.Address).Replace("{ #signature }", signature);


            var items = GetPageControlsFromBlockchain(values.Address);
            foreach (var item in items)
            {
                string row = "<tr>";
                row += "<td>" + item.CreateOn + "</td>";
                row += "<td>" + item.Name.ToUpper() + "</td>";
                row += "<td class=\"text-center\">" + item.OrderDocument + "</td>";
                row += "<td class=\"text-center\">" + item.StartPage + " - " + item.EndPage + "</td>";
                row += "<td>" + item.CreatedBy.ToUpper() + "</td>";
                row += "</tr>";
                bodyHtml += row;
            }

            html = html.Replace("{ #body }", bodyHtml);
            ConvertHtmlToPdf(pathTempFile, html);

            /*
            PdfDocument pdfOutput = new PdfDocument(new PdfReader(pathTempFile), new PdfWriter(pathTempFile));

            foreach (var file in documentsToDownload)
            {
                PdfDocument pdfToCombine = new PdfDocument(new PdfReader(file.FullName));
                PdfMerger merger = new PdfMerger(pdfOutput);
                merger.Merge(pdfToCombine, 1, pdfToCombine.GetNumberOfPages());
                pdfToCombine.Close();
            }

            pdfOutput.Close();
            */

            using (PdfDocument pdfOutput = new PdfDocument(new PdfWriter(pathFolderFile))) {
                using (var pdfToCombine = new PdfDocument(new PdfReader(pathTempFile))) {
                    PdfMerger merger = new PdfMerger(pdfOutput);
                    merger.Merge(pdfToCombine, 1, pdfToCombine.GetNumberOfPages());
                    pdfToCombine.Close();
                }

                foreach (var file in documentsToDownload)
                {
                    using (var pdfToCombine2 = new PdfDocument(new PdfReader(file.FullName)))
                    {
                        PdfMerger merger = new PdfMerger(pdfOutput);
                        merger.Merge(pdfToCombine2, 1, pdfToCombine2.GetNumberOfPages());
                        pdfToCombine2.Close();
                    }                    
                }
            }

            switch (_paramConfiguration.HashFunction.ToUpper())
            {
                case "SHA256":
                    folderHash.HashAlgorithmDocument = "SHA256";
                    folderHash.HashDocument = GetSHA256V2(pathFolderFile);
                    break;
                default:
                    folderHash.HashAlgorithmDocument = "MD5";
                    folderHash.HashDocument = GetMD5V2(pathFolderFile);
                    break;
            }

            return folderHash;

        }

        public void CloseFolder(string contractAddress) {
            var details = _smartContractContext.Get(contractAddress);

            string signaturePath = $"{_paramConfiguration.TemplateUrl}signature.png";

            var imageArray = File.ReadAllBytes(signaturePath);
            string base64ImageRepresentation = $"data:image/png;base64, {Convert.ToBase64String(imageArray)}";

            string pathDocumentFile = $"{_paramConfiguration.DocumentFolder}{details.Document_number}";
                //String.Concat(pathFolder, "/", values.Id, ".", format.ToLower());

            var documento = new GetFolderDocumentDTO() {
                Address = contractAddress,
                DocumentNumber = details.Document_number,
                Title = details.Title,
                PathDocument = pathDocumentFile
            };
            GetDocumentFolderV2(documento, base64ImageRepresentation);

            _smartContractContext.CloseFolder(contractAddress);
        }



        private static void ConvertHtmlToPdf(string pathDest, string html)
        {
            if (File.Exists(pathDest)) {
                File.Delete(pathDest);
            }

            using (FileStream pdfDest = File.Open(pathDest, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                PdfWriter writer = new PdfWriter(pdfDest);
                PdfDocument pdf = new PdfDocument(writer);
                pdf.SetDefaultPageSize(PageSize.A4);

                ConverterProperties converterProperties = new ConverterProperties();
                HtmlConverter.ConvertToPdf(html, pdf, converterProperties);
                pdfDest.Close();
            }
        }

        private string GetMD5(string value)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(value);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }

        private string GetBase64(string path)
        {
            Byte[] bytes = File.ReadAllBytes(path);
            String file = Convert.ToBase64String(bytes);

            return file;
        }

        private string GetSHA256(string value) {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(value));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private string GetMD5V2(string path)
        {
            string hash = String.Empty;
            MD5 md5 = MD5.Create();
            FileStream fileStream = File.OpenRead(path);
            byte[] crypto = md5.ComputeHash(fileStream);

            return Convert.ToHexString(crypto);
        }

        private string GetSHA256V2(string path)
        {
            string hash = String.Empty;
            SHA256 SHA256 = SHA256Managed.Create();
            FileStream fileStream = File.OpenRead(path);
            byte[] crypto = SHA256.ComputeHash(fileStream);

            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }

            return hash;
        }

        private int SaveDocument(string base64Origin, string pathFile) {
            var base64 = base64Origin.Split(",")[1];
            byte[] bytes = Convert.FromBase64String(base64);
            System.IO.FileStream stream = new FileStream(pathFile, FileMode.CreateNew);
            System.IO.BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(bytes, 0, bytes.Length);
            writer.Close();

            using (PdfReader pdfReader = new PdfReader(pathFile))
            {
                using (var document = new PdfDocument(pdfReader))
                {
                    int numberPages = document.GetNumberOfPages();
                    PdfDocumentInfo info = document.GetDocumentInfo();
                    //info.SetAuthor("Robert Louis Stevenson");
                    document.Close();

                    return numberPages;
                }
            }
        }

        private bool ValidateDocument(string pathFolder, PaperControlSmartContractDTO paperControl) {
            string hash = "";
            DirectoryInfo d = new DirectoryInfo(pathFolder);

            var files = d.GetFiles("*.pdf").ToList();            
            var file = files.Where(y => y.Name.Contains(paperControl.Id));

            if (file.FirstOrDefault() != null)
            {
                switch (paperControl.HashAlgorithm.ToUpper())
                {
                    case "SHA256":
                        hash = GetSHA256V2(file.FirstOrDefault().FullName);
                        break;
                    default:
                        hash = GetMD5V2(file.FirstOrDefault().FullName);
                        break;
                }
            }

            if (hash == paperControl.Hash)
            {
                return true;
            }

            return false;
        }

        private FileInfo GetNoValidatedTemplate() {
            DirectoryInfo d = new DirectoryInfo(_paramConfiguration.TemplateUrl);
            var files = d.GetFiles("*.pdf").ToList();
            return files.Where(y => y.Name.Contains("noValidated")).First();
        }
    }
}
