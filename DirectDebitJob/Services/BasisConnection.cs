using DirectDebitJob.Interfaces;
using DirectDebitJob.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DirectDebitJob.Connections
{
    class BasisConnection:IBasisConnection
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BasisConnection> _logger;        
        public BasisConnection(IConfiguration configuration, ILogger<BasisConnection> logger)
        {
            _configuration = configuration;
            _logger = logger;            
        }
        public async Task<string> PostTransactionDetails(PostTransactionDetails transactiondetails)
        {
            string result = "";
            string log = "";
            using (var con = new OracleConnection(GTBEncryptLibrary.GTBEncryptLib.DecryptText(_configuration.GetSection("BasisConnectionString").Value)))
            {
                try
                {

                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    using (OracleCommand cmd = new OracleCommand("midwareusr.eonepkg_nip.gtbpbsc0_full", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        cmd.Parameters.Add("inp_acct_from", OracleDbType.Varchar2, 21).Value = transactiondetails.accountfrom;
                        cmd.Parameters.Add("inp_acct_to", OracleDbType.Varchar2, 21).Value = transactiondetails.accountto;
                        cmd.Parameters.Add("inp_tra_amt", OracleDbType.Double).Value = transactiondetails.transactionamount;
                        cmd.Parameters.Add("inp_expl_code", OracleDbType.Int32).Value = transactiondetails.expl_code;
                        cmd.Parameters.Add("inp_remarks", OracleDbType.Varchar2).Value = transactiondetails.remarks;
                        cmd.Parameters.Add("inp_rqst_code", OracleDbType.Varchar2).Value = transactiondetails.reqst_code;
                        cmd.Parameters.Add("inp_man_app1", OracleDbType.Int32).Value = transactiondetails.man_app1;
                        cmd.Parameters.Add("inp_tell_id", OracleDbType.Int32).Value = transactiondetails.tellid;
                        cmd.Parameters.Add("inp_doc_alp", OracleDbType.Varchar2).Value = transactiondetails.docalpha;
                        cmd.Parameters.Add("inp_origt_bra_code", OracleDbType.Int32).Value = transactiondetails.originatingbracode;                        
                        cmd.Parameters.Add("inp_tra_seq1", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("out_tra_seq1", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("out_return_status", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;

                        foreach (OracleParameter OraParam in cmd.Parameters)
                        {
                            log = log + OraParam.ParameterName + ": " + OraParam.Value + Environment.NewLine;
                        }
                        _logger.LogInformation("Parameters sent to basis are:" + log);
                        cmd.ExecuteNonQuery();
                        string output = cmd.Parameters["out_return_status"]?.Value.ToString();
                        _logger.LogInformation($"Transaction from account: {transactiondetails?.accountfrom} has an output from basis which is: {output}");
                        if (output == "@ERR7@")
                        {
                            result = cmd.Parameters["out_tra_seq1"].Value.ToString();                            
                        }
                        cmd.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Basis Error: ", ex);
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                    
                }
            
            }              
                
            return result;
        }
    }
}
