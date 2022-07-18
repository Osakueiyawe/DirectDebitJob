using DirectDebitJob.Interfaces;
using DirectDebitJob.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace DirectDebitJob.Methods
{   

    class DirectDebitProcess : IDirectDebitProcess
    {        
        private readonly IConfiguration configuration;
        private readonly ISqlConnect _sqlconnection;        
        private readonly IAccountUtility _transactions;
        private readonly IBasisConnection _basisconnection;        
        private readonly IApiConnection _apiconnection;
        private readonly ILogger<DirectDebitProcess> _logger;
        private readonly IDateUtility _date;
        public DirectDebitProcess(ILogger<DirectDebitProcess> logger, ISqlConnect sqlconnection,IConfiguration Configuration, IAccountUtility transactions, IBasisConnection basisconnection, IApiConnection apiconnection, IDateUtility date)
        {
            _sqlconnection = sqlconnection;
            configuration = Configuration;
            _transactions = transactions;
            _basisconnection = basisconnection;         
            _apiconnection = apiconnection;
            _logger = logger;
            _date = date;
        }
        public async Task ProcessTransactions()
        {            
            try
            {                
                DataTable dt = await _sqlconnection.GetAllMandateRequests();
                DateTime today = DateTime.Today;
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        _logger.LogInformation($"Number of data retrieved are: {dt.Rows.Count.ToString()}");
                        foreach (DataRow dr in dt.Rows)
                        {
                            try
                            {
                                string referenceNumber = dr["ReferenceNumber"].ToString();
                                string[] debitOldAccountDetails = new string[] { };
                                string[] creditOldAccountDetails = new string[] { };
                                int interval = 1;
                                int frequency = Convert.ToInt32(dr["Frequency"]);
                                if (Convert.ToDateTime(dr["NextPaymentDate"]) < today)
                                {                                    
                                    //for transactions that have passed the due date
                                    interval = await _date.GetMonthInterval(Convert.ToDateTime(dr["NextPaymentDate"].ToString()), today, frequency);
                                }

                                string fullaccountkey = dr["FullAccountKey"].ToString();
                                string merchantaccountkey = dr["Merchant Full Account Key"].ToString();
                                string debitoldaccount = await _transactions.GetOldAccountfromFullAcctKey(fullaccountkey);
                                string creditoldaccount = await _transactions.GetOldAccountfromFullAcctKey(merchantaccountkey);
                                double amount = (double)(Convert.ToDouble(dr["Amount"].ToString()) * (double)interval);
                                if (debitoldaccount.Contains("/") && creditoldaccount.Contains("/"))
                                {
                                    debitOldAccountDetails = debitoldaccount.Split('/');
                                    creditOldAccountDetails = creditoldaccount.Split('/');
                                }

                                if (debitOldAccountDetails.Length != 5 && creditOldAccountDetails.Length != 5)
                                {
                                    continue;
                                }
                                //Post transaction by debiting customer and crediting merchant
                                PostTransactionDetails transactiondetails = new PostTransactionDetails();
                                transactiondetails.accountfrom = Convert.ToInt32(debitOldAccountDetails[0]).ToString("0000") + Convert.ToInt32(debitOldAccountDetails[1]).ToString("0000000") + Convert.ToInt32(debitOldAccountDetails[2]).ToString("000") + Convert.ToInt32(debitOldAccountDetails[3]).ToString("0000") + Convert.ToInt32(debitOldAccountDetails[4]).ToString("000");
                                transactiondetails.accountto = Convert.ToInt32(creditOldAccountDetails[0]).ToString("0000") + Convert.ToInt32(creditOldAccountDetails[1]).ToString("0000000") + Convert.ToInt32(creditOldAccountDetails[2]).ToString("000") + Convert.ToInt32(creditOldAccountDetails[3]).ToString("0000") + Convert.ToInt32(creditOldAccountDetails[4]).ToString("000");
                                transactiondetails.docalpha = configuration.GetSection("Inp_doc_alp").Value;
                                transactiondetails.expl_code = Convert.ToInt32(configuration.GetSection("Inp_expl_code").Value);
                                transactiondetails.man_app1 = Convert.ToInt32(configuration.GetSection("Inp_man_app1").Value);
                                transactiondetails.tellid = Convert.ToInt32(configuration.GetSection("Inp_tell_id").Value);
                                transactiondetails.reqst_code = configuration.GetSection("Inp_rqst_code").Value;
                                transactiondetails.transactionamount = Math.Round(amount, 2);
                                transactiondetails.remarks = configuration.GetSection("Inp_remarks").Value;
                                transactiondetails.originatingbracode = Convert.ToInt32(debitOldAccountDetails[0]);
                                _logger.LogInformation($"About to post to basis for {referenceNumber}");
                                string postresult = await _basisconnection.PostTransactionDetails(transactiondetails);
                                if (!String.IsNullOrEmpty(postresult))
                                {
                                    DateTime paymentdate = Convert.ToDateTime(dr["NextPaymentDate"]);
                                    DateTime enddate = (Convert.ToDateTime(dr["EndDate"].ToString().Replace("-", "/"), CultureInfo.GetCultureInfo("ur-PK").DateTimeFormat)).Date;
                                    //update the next payment date of the mandate
                                    UpdateMandateDetails mandatedetails = new UpdateMandateDetails();
                                    mandatedetails.referencenumber = dr["ReferenceNumber"].ToString();
                                    mandatedetails.lastPaymentDate = DateTime.Now;
                                    mandatedetails.lastPaymentAmount = Convert.ToDecimal(amount);
                                    DateTime nextpayment = paymentdate.AddMonths(frequency * interval);
                                    if (nextpayment.Date <= enddate.Date)
                                    {
                                        //Only update nextpayment date if the next payment date does not exceed the end date otherwise set nextpayment date to null
                                        mandatedetails.nextpaymentdate = nextpayment;
                                    }

                                    Task<bool> updatemandate = _sqlconnection.UpdateUserMandateDetails(mandatedetails);

                                    //Log the transaction details on a table
                                    TransactionLogDetails transactionlogdetails = new TransactionLogDetails();
                                    transactionlogdetails.referencenumber = dr["ReferenceNumber"].ToString();
                                    transactionlogdetails.transactiondate = DateTime.Now;
                                    transactionlogdetails.amount = Convert.ToDecimal(amount);
                                    transactionlogdetails.basisreference = postresult;
                                    Task<bool> updatetransactionlog = _sqlconnection.UpdateTransactionLog(transactionlogdetails);

                                    bool mandateupdated = await updatemandate;
                                    bool transactionlogupdated = await updatetransactionlog;
                                    if (mandateupdated && transactionlogupdated)
                                    {
                                        EnterpriseLifeRequest enterpriselifepayload = new EnterpriseLifeRequest();
                                        DataTable transactionLogDetails = await _sqlconnection.GetEnterpriseLifeApiDetails(dr["ReferenceNumber"].ToString(), postresult);
                                        if (transactionLogDetails != null)
                                        {
                                            if (transactionLogDetails.Rows.Count > 0)
                                            {
                                                string sessionId = transactionLogDetails.Rows[0]["ReferenceNumber"].ToString() + postresult;
                                                enterpriselifepayload.transactionAmount = amount.ToString();
                                                enterpriselifepayload.debitBankCode = configuration.GetSection("BankCode").Value;
                                                enterpriselifepayload.creditBankCode = configuration.GetSection("BankCode").Value;
                                                enterpriselifepayload.creditAccountName = "";
                                                enterpriselifepayload.creditAccountNumber = dr["Merchant Account Number (NUBAN)"].ToString();
                                                enterpriselifepayload.currency = configuration.GetSection("Currency").Value;
                                                enterpriselifepayload.debitAccountName = "";
                                                enterpriselifepayload.debitAccountNumber = dr["AccountNumber(NUBAN)"].ToString();
                                                enterpriselifepayload.paymentReference = postresult;
                                                enterpriselifepayload.transactionDate = transactionLogDetails.Rows[0]["TransactionDate"].ToString();
                                                enterpriselifepayload.sessionID = sessionId;                                                
                                                bool sendtoenterpriselife = await _apiconnection.SendSuccessNotificationToEnterpriseLife(enterpriselifepayload);
                                                if (sendtoenterpriselife)
                                                {
                                                    _logger.LogInformation($"Call to Enterprise Life API for {referenceNumber} was successful");
                                                }
                                                else
                                                {
                                                    _logger.LogWarning($"Call to Enterprise Life API for {referenceNumber} was not successful");
                                                }
                                            }                                            
                                        }

                                    }
                                }
                                else
                                {
                                    _logger.LogWarning($"Failed to post to basis for reference Number: {referenceNumber}");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error in loop");
                            }

                        }
                    }
                    else
                    {
                        _logger.LogError("No record was retrieved");
                    }
                }                
                
            }            
            catch(Exception ex)
            {
                _logger.LogError(ex,"");
            }
        }
    }
}
