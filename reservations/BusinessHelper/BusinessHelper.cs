using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reservation.DBFactory;
using System.Data;
using System.Reflection;
using System.Web.Mvc;

namespace Reservation
{

    public class Response
    {
        public ResCode? ResponseCode { get; set; } //Success,Error
        public ResMsg? ResponseMessage { get; set; } //msg
        public TranType? ResponseTranType { get; set; } //insert,update,delete
        public string ErrResponse{get;set;}

    }

    public enum ResMsg
    {
        Record_Saved_Successfully,Record_Deleted_Successfully,Something_Wrong_happened
    }
    public enum ResCode
    {
        Suceess,Error
    }
    public enum TranType
    {
        InsertORUpdate,Delete
    }
    public class BusinessHelper
    {
        private DBHelper _dbHelper = new DBHelper();

        public Response Transaction_Opportunities(string Opportunitiesdata)
        {
            Response response = new Response();
            int rowsAffected = 0;

            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("@Opportunitiesdata", Opportunitiesdata));
            IDbTransaction transaction = _dbHelper.BeginTransaction();
            try
            {
                rowsAffected = _dbHelper.ExecuteNonQuery("sp_Opportunities", paramCollection, transaction, CommandType.StoredProcedure);
                response.ResponseCode = rowsAffected > 0 ? ResCode.Suceess : ResCode.Error;            
                _dbHelper.CommitTransaction(transaction);
            }
            catch (Exception err)
            {
                _dbHelper.RollbackTransaction(transaction);
                response.ResponseCode = ResCode.Error;
                response.ResponseMessage = ResMsg.Something_Wrong_happened;
                response.ErrResponse = err.ToString();

            }
            return response;

        }

        public string CreateHTMLmessage(Response res)
        {
            if (res.ErrResponse != null && res.ErrResponse != "")
            {
                return "<div class='alert " + "alert-danger" + " alert-dismissible fade in' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button><strong> Oh snap! </strong>" + res.ResponseMessage.ToString().Replace("_", " ") + "</br>" + res.ErrResponse + "</div>";
            }
            else if (res.ResponseCode == ResCode.Error || res.ResponseCode == ResCode.Suceess)
            {

                return "<div class='alert " + "alert-success" + " alert-dismissible fade in' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button><strong> Well done! </strong>" + res.ResponseMessage.ToString().Replace("_", " ") + "</div>";
            }
            else
            {
                return "";
            }

        }
    }

}

