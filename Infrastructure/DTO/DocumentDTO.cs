using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTO
{
    public class ReportDTO
    {
        public string title { get; set; }
        public string module { get; set; }
        public string document_number { get; set; }
        public string office { get; set; }
    }

    [FunctionOutput]
    public class DetailSmartContractDTO
    {
        [Parameter("string", 1)]
        public string Title { get; set; }

        [Parameter("string", 2)]
        public string Module { get; set; }

        [Parameter("string", 3)]
        public string Document_number { get; set; }

        [Parameter("string", 4)]
        public string Owner { get; set; }

        [Parameter("bool", 5)]
        public bool is_open { get; set; }

        [Parameter("string", 6)]
        public string HashDocument { get; set; }

        [Parameter("string", 7)]
        public string HashAlgorithmDocument { get; set; }
    }

    public class ControlPaperParamsDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? HashCode { get; set; }
        public string? HashAlgorithm { get; set; }
        public int NumberPages { get; set; }
        public string? Format { get; set; }
        public string Origen { get; set; }
        public string CreatedBy { get; set; }
        public string CreateOn { get; set; }
        public string? base64 { get; set; }
        public string DocumentNumber { get; set; }
        public string ContractAddress { get; set; }
    }

    public class ControlPaperDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string HashCode { get; set; }
        public string HashAlgorithm { get; set; }
        public int NumberPages { get; set; }
        public string Format { get; set; }
        public string Origen { get; set; }
        public string CreatedBy { get; set; }
        public string CreateOn { get; set; }

    }

    public class PaperControlSmartContractDTO
    {
        [Parameter("string", "id", 1)]
        public string Id { get; set; }

        [Parameter("string", "name", 2)]
        public string Name { get; set; }

        [Parameter("string", "hash", 3)]
        public string Hash { get; set; }

        [Parameter("string", "hash_algorithm", 4)]
        public string HashAlgorithm { get; set; }

        [Parameter("uint32", "order_document", 5)]
        public int OrderDocument { get; set; }

        [Parameter("uint32", "start_page", 5)]
        public int StartPage { get; set; }

        [Parameter("uint32", "end_page", 5)]
        public int EndPage { get; set; }

        [Parameter("string", "format", 5)]
        public string Format { get; set; }

        [Parameter("string", "origen", 5)]
        public string Origen { get; set; }

        [Parameter("string", "created_by", 5)]
        public string CreatedBy { get; set; }

        [Parameter("string", "created_on", 5)]
        public string CreateOn { get; set; }
    }

    public class GetDocumentParamsDTO
    {
        public string Id { get; set; }
        public string DocumentNumber { get; set; }
    }

    public class GetFolderDocumentDTO
    {
        public string Address { get; set; }
        public string DocumentNumber { get; set; }
        public string Title { get; set; }
        public string PathDocument { get; set; }
    }

    public class FolderHash {
        public string HashDocument {get; set;}
        public string HashAlgorithmDocument { get; set; }
    }

    public class GetFolderDocument {
        public string Address { get; set; }
        public string DocumentNumber { get; set; }
    }
}
