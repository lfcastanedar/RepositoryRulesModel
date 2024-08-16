using System;
using System.Collections;
using System.Text.RegularExpressions;
using Infrastructure.DTO;
using Microsoft.Extensions.Configuration;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;


namespace Infrastructure.Ethereum
{
    public class SmartContractContext
    {
        private string _urlEther { get; set; } 
        private string _passwordEther { get; set; }
        private string _hashFunction { get; set; }


        private string _bytecodeSource { get; set; }
        private string _abiSource { get; set; }

        private Account _account { get; set; }
        private Web3 _web3 { get; set; }


        public SmartContractContext(IConfigurationSection config) {
            this._urlEther = config["UrlEther"] ?? "";
            this._passwordEther = config["key"] ?? "";

            this._account = new Account(_passwordEther, 1337);
            this._web3 = new Web3(_account, _urlEther);

            using (StreamReader r = new StreamReader(String.Concat(config["SolidityFolder"], "ABI.json")))
            {
                this._abiSource = r.ReadToEnd();
            }

            using (StreamReader r = new StreamReader(String.Concat(config["SolidityFolder"], "Bytecode.txt")))
            {
                this._bytecodeSource = r.ReadToEnd();
            }

        }

        public EtherAccountDTO GetBalance() {
            var balance = _web3.Eth.GetBalance.SendRequestAsync(_account.Address).Result;
            var etherAmount = Web3.Convert.FromWei(balance.Value);

            return new EtherAccountDTO(){ Ether = etherAmount, Address = this._account.Address };
        }


        public DetailSmartContractDTO Get(string contractAddress)
        {
            DetailSmartContractDTO data = new DetailSmartContractDTO();

            var contract = _web3.Eth.GetContract(this._abiSource, contractAddress);
            var function = contract.GetFunction("get_detail");
            var result = function.CallAsync<object>().Result;

            IList collection = (IList)result;

            System.Reflection.PropertyInfo pi = collection[0].GetType().GetProperty("Result");
            data.Title = pi.GetValue(collection[0]).ToString().Replace("'", "");

            pi = collection[1].GetType().GetProperty("Result");
            data.Module = pi.GetValue(collection[1]).ToString().Replace("'", "");

            pi = collection[2].GetType().GetProperty("Result");
            data.Document_number = pi.GetValue(collection[2]).ToString().Replace("'", "");

            pi = collection[3].GetType().GetProperty("Result");
            data.Owner = pi.GetValue(collection[3]).ToString().Replace("'", "");

            pi = collection[4].GetType().GetProperty("Result");
            data.is_open = (bool)pi.GetValue(collection[4]);

            pi = collection[5].GetType().GetProperty("Result");
            data.HashDocument = pi.GetValue(collection[5]).ToString().Replace("'", ""); 

            pi = collection[6].GetType().GetProperty("Result");
            data.HashAlgorithmDocument = pi.GetValue(collection[6]).ToString().Replace("'", "");

            return data;
        }

        public string CreateContract(ReportDTO values)
        {
            object[] contractParameters = {
                values.title,
                values.module,
                values.document_number,
                values.office
            };

            var gas = _web3.Eth.DeployContract.EstimateGasAsync(
                _abiSource,
                _bytecodeSource,
                _account.Address,
                contractParameters).Result;

            var transactionHash = _web3.Eth.DeployContract.SendRequestAsync(_abiSource, _bytecodeSource, _account.Address, new HexBigInteger(gas.Value), contractParameters).Result;

            var receipt = _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).Result;

            var contractAddress = receipt.ContractAddress;

            return contractAddress;
        }

        public List<PaperControlSmartContractDTO> GetPageControls(string contractAddress)
        {
            var contract = _web3.Eth.GetContract(this._abiSource, contractAddress);

            var function = contract.GetFunction("get_documents");

            var data = function.CallAsync<List<PaperControlSmartContractDTO>>().Result;

            return data;
        }

        public void AddDocument(ControlPaperDTO values, string contractAddress)
        {
            var contract = _web3.Eth.GetContract(this._abiSource, contractAddress);
            var function = contract.GetFunction("set_document");

            var parameters = values.GetType().GetProperties().Select(item =>
            {
                return item.GetValue(values);
            }).ToArray();

            var gasAmount = function.EstimateGasAsync(_account.Address, null, null, parameters).Result;
            var receipt = function.SendTransactionAsync(_account.Address, new HexBigInteger(gasAmount), null, parameters).Result;
        }

        public void SetDocumentHash(FolderHash values, string contractAddress)
        {
            var contract = _web3.Eth.GetContract(this._abiSource, contractAddress);
            var function = contract.GetFunction("set_document_hash");

            var parameters = values.GetType().GetProperties().Select(item =>
            {
                return item.GetValue(values);
            }).ToArray();

            var gasAmount = function.EstimateGasAsync(_account.Address, null, null, parameters).Result;
            var receipt = function.SendTransactionAsync(_account.Address, new HexBigInteger(gasAmount), null, parameters).Result;
        }

        public void CloseFolder(string contractAddress)
        {
            var contract = _web3.Eth.GetContract(this._abiSource, contractAddress);
            var function = contract.GetFunction("close_process");
            object[] parameters = { };

            var gasAmount = function.EstimateGasAsync(_account.Address, null, null, null).Result;
            var receipt = function.SendTransactionAsync(_account.Address, new HexBigInteger(gasAmount), null, parameters).Result;
        }

       
    }
}
