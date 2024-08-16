using ApplicationCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Ethereum;
using NLog;
using Microsoft.Extensions.Options;
using Infrastructure.Configuration;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.Extensions.Logging;

namespace WalletApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private SmartContractContext _smartContractContext;
        private ILogger<AccountController> _logger;
        private readonly IOptions<ParamConfiguration> _config;

        public AccountController(SmartContractContext smartContractContext, ILogger<AccountController> logger, IOptions<ParamConfiguration> config)
        {
            this._smartContractContext = smartContractContext;
            _logger = logger;
            _config = config;
        }

        [Route("getAccount"), HttpGet]
        public IActionResult GetAccount()
        {
            try
            {
                var result = _smartContractContext.GetBalance();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
            
        }
    }
}
