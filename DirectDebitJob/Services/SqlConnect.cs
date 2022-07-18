using DirectDebitJob.Interfaces;
using DirectDebitJob.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DirectDebitJob.Connections
{
    class SqlConnect:ISqlConnect
    {
        private IConfiguration configuration { get; set; }
        private readonly ILogger<SqlConnect> _logger;
        
        
        public SqlConnect(IConfiguration Configuration, ILogger<SqlConnect> logger)
        {
            configuration = Configuration;
            _logger = logger;            
        }
        public async Task<DataTable> GetAllMandateRequests()
        {            
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(GTBEncryptLibrary.GTBEncryptLib.DecryptText(configuration.GetSection("DirectDebitconnectionstring").Value)))
            {
                try
                {
                    if (sqlcon.State != ConnectionState.Open)
                    {
                        sqlcon.Open();
                    }
                    using (SqlCommand sqlcmd = new SqlCommand("getmandatetransactiondetails", sqlcon))
                    {
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sdap = new SqlDataAdapter(sqlcmd);
                        sdap.Fill(dt);
                        sqlcmd.Dispose();
                    }                        
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error retrieving direct debit data with error: ", ex);
                }
                finally
                {
                    sqlcon.Close();
                    sqlcon.Dispose();
                }
            }               
            
            return dt;
        }  

        

        public async Task<bool> UpdateUserMandateDetails(UpdateMandateDetails mandatedetails)
        {
            bool result = false;
            using (SqlConnection sqlcon = new SqlConnection(GTBEncryptLibrary.GTBEncryptLib.DecryptText(configuration.GetSection("DirectDebitconnectionstring").Value)))
            {
                try
                {
                    if (sqlcon.State != ConnectionState.Open)
                    {
                        sqlcon.Open();
                    }
                    using (SqlCommand sqlcmd = new SqlCommand("updatemandatedetails", sqlcon))
                    {
                        sqlcmd.Parameters.Add("@referencenumber", SqlDbType.VarChar).Value = mandatedetails.referencenumber;
                        sqlcmd.Parameters.Add("@nextpaymentdate", SqlDbType.DateTime).Value = mandatedetails.nextpaymentdate;
                        sqlcmd.Parameters.Add("@lastPaymentAmount", SqlDbType.Decimal).Value = mandatedetails.lastPaymentAmount;
                        sqlcmd.Parameters.Add("@lastPaymentDate", SqlDbType.DateTime).Value = mandatedetails.lastPaymentDate;
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.ExecuteNonQuery();
                        sqlcmd.Dispose();
                    }                    
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error getting mandate details: ",ex);
                }  
                finally
                {
                    sqlcon.Close();
                    sqlcon.Dispose();
                }
            }
                
            return result;
        }

        public async Task<bool> UpdateTransactionLog(TransactionLogDetails transactiondetails)
        {
            bool result = false;
            using (SqlConnection sqlcon = new SqlConnection(GTBEncryptLibrary.GTBEncryptLib.DecryptText(configuration.GetSection("DirectDebitconnectionstring").Value)))
            {
                try
                {
                    if (sqlcon.State != ConnectionState.Open)
                    {
                        sqlcon.Open();
                    }
                    using (SqlCommand sqlcmd = new SqlCommand("inserttransactionlog", sqlcon))
                    {
                        sqlcmd.Parameters.Add("@referencenumber", SqlDbType.VarChar).Value = transactiondetails.referencenumber;
                        sqlcmd.Parameters.Add("@transactiondate", SqlDbType.DateTime).Value = transactiondetails.transactiondate;
                        sqlcmd.Parameters.Add("@amount", SqlDbType.Decimal).Value = transactiondetails.amount;
                        sqlcmd.Parameters.Add("@basisreference", SqlDbType.Decimal).Value = transactiondetails.basisreference;
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.ExecuteNonQuery();
                        sqlcmd.Dispose();
                    }                        
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error updating transaction log: ",ex);
                }
                finally
                {
                    sqlcon.Close();
                    sqlcon.Dispose();
                }
                
            }                
            return result;
        }

        public async Task<DataTable> GetEnterpriseLifeApiDetails(string referencenumber, string basisReference)
        {            
            DataTable dt = new DataTable();            
            using (SqlConnection sqlcon = new SqlConnection(GTBEncryptLibrary.GTBEncryptLib.DecryptText(configuration.GetSection("DirectDebitconnectionstring").Value)))
            {
                try
                {
                    if (sqlcon.State != ConnectionState.Open)
                    {
                        sqlcon.Open();
                    }
                    using (SqlCommand sqlcmd = new SqlCommand("gettransactionlogdetails", sqlcon))
                    {
                        sqlcmd.Parameters.Add("@referencenumber", SqlDbType.VarChar).Value = referencenumber;
                        sqlcmd.Parameters.Add("@basisReference", SqlDbType.VarChar).Value = basisReference;
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sdap = new SqlDataAdapter(sqlcmd);
                        sdap.Fill(dt);
                        sqlcmd.Dispose();
                    }                   
                                        
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error getting Enterprise API details: ",ex);
                }
                finally
                {
                    sqlcon.Close();
                    sqlcon.Dispose();
                }
            }
            return dt;
        }
    }
}
