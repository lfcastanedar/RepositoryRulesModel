using ApplicationCore;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Ethereum;
using Infrastructure.DTO;
using ApplicationCore.Document;
using Microsoft.Extensions.Options;
using Infrastructure.Configuration;
using static iText.Svg.SvgConstants;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WalletApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private SmartContractContext _smartContractContext;
        private ILogger<DocumentController> _logger;
        private readonly IOptions<ParamConfiguration> _config;

        public DocumentController(SmartContractContext smartContractContext, ILogger<DocumentController> logger, IOptions<ParamConfiguration> config)
        {
            _smartContractContext = smartContractContext;
            _logger = logger;
            _config = config;
        }

        [Route("get_detail/{contractAddress}"), HttpGet]
        public IActionResult GetDetail(string contractAddress)
        {
            try
            {
                var document = new Document(_smartContractContext);
                return Ok(document.GetDetail(contractAddress));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Route("get_page_controls/{contractAddress}"), HttpGet]
        public IActionResult GetPageControls(string contractAddress)
        {
            try
            {
                var document = new Document(_smartContractContext);
                return Ok(document.GetPageControlsFromBlockchain(contractAddress));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Route("create"), HttpPost]
        public IActionResult Post(ReportDTO values)
        {
            try
            {
                var document = new Document(_smartContractContext);
                return Ok(document.CreateContract(values));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.StackTrace);
                return BadRequest(ex.StackTrace);
            }

        }

        [Route("add_document"), HttpPost]
        public IActionResult AddDocument(ControlPaperParamsDTO values)
        {
            try
            {
                _logger.LogInformation("Parametros: " + _config.Value.HashFunction);
                var document = new Document(_smartContractContext, _config.Value);
                return Ok(document.AddDocument(values));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [Route("GetDocument"), HttpPost]
        public IActionResult GetDocument(GetDocumentParamsDTO values)
        {
            try
            {
                var document = new Document(_smartContractContext, _config.Value);
                return Ok(document.GetDocument(values));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Route("GetDocumentFolder"), HttpPost]
        public IActionResult GetDocumentFolder(GetFolderDocument values)
        {
            try
            {
                var document = new Document(_smartContractContext, _config.Value);
                return Ok(document.GetDocumentFolder(values));

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }

        }

        [Route("GetCloseFolder/{address}"), HttpGet]
        public IActionResult PostCloseFolder(string address) {
            try
            {
                var document = new Document(_smartContractContext, _config.Value);
                document.CloseFolder(address);
                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
