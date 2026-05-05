using CyntecMESEquipment;
using DocumentFormat.OpenXml.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace VNA_Data_Grabber
{
    public class MESService
    {
        private cls_Eqp _eqp;
        private string _machNo;

        public MESService(string machNo)
        {
            _machNo = machNo;
            _eqp = new cls_Eqp(machNo);
        }

        public void SetUrl(string url)
        {
            _eqp.SetServiceUrl(url);
        }

        public MESResponse UserVerify(string userId, string password, string barcode = "")
        {
            var req = new
            {
                TransactionName = "UserVerify",
                EqpNo = _machNo,
                User2DBarCode = barcode,
                UserID = userId,
                UserPassword = password
            };
            return Execute(req, userId);
        }

        public MESResponse WOQRY(string orderNo, string userId)
        {
            var req = new
            {
                TransactionName = "WOQRY",
                EqpNo = _machNo,
                WONO = orderNo,
                UserID = userId
            };
            return Execute(req, userId);
        }

        public MESResponse WOCHECKIN(string orderNo, string userId, JArray? inputList = null)
        {
            var req = new JObject();
            req["TransactionName"] = "WOCHECKIN";
            req["EqpNo"] = _machNo;
            req["WONO"] = orderNo;
            req["UserID"] = userId;
            req["ComponentBatchIn"] = "N";
            if (inputList != null) req["InputListDt"] = inputList.ToString(Formatting.None);

            return Execute(req, userId);
        }

        public MESResponse EDCDATAADD(string orderNo, string userId, JArray dataList)
        {
            var req = new JObject();
            req["TransactionName"] = "EDCDATAADD";
            req["EqpNo"] = _machNo;
            req["WONO"] = orderNo;
            req["UserID"] = userId;
            req["IsLast"] = "Y";
            req["ISCOMPONENTMODE"] = "N";
            req["dtEDCRawData"] = dataList;

            return Execute(req, userId);
        }

        public MESResponse WOCHECKOUT(string orderNo, string userId, int outputQty, JArray? ngList = null, JArray? inputList = null, JArray? checkList = null, JArray? paraList = null )
        {
            var req = new JObject();
            req["TransactionName"] = "WOCHECKOUT";
            req["EqpNo"] = _machNo;
            req["UserID"] = userId;
            req["WONO"] = orderNo;
            //req["ComponentBatchOut"] = "Y";
            req["File"] = "";
            req["Operator"] = userId;
            req["OutPut"] = outputQty;
            if (ngList != null) req["NGListDt"] = ngList.ToString(Formatting.None);
            
            // 將管理項目正確放入 WOCHECKOUT，且轉為字串格式
            if (inputList != null)
            {
                req["InputListDt"] = inputList.ToString(Formatting.None);
            }
            if (checkList != null)
            {
                req["CheckItemDt"] = checkList.ToString(Formatting.None);
            }
            if (paraList != null)
            {
                req["ParmListDt"] = paraList.ToString(Formatting.None);
            }
            return Execute(req, userId);
        }

        private MESResponse Execute(object parameter, string userId)
        {
            string jsonParam = JsonConvert.SerializeObject(parameter);
            string resultJson = _eqp.EqpTransaction(jsonParam, _machNo, userId);
            
            var res = JsonConvert.DeserializeObject<MESResponse>(resultJson) ?? new MESResponse { Result = "fail", Message = "解析回傳資料失敗" };
            res.RawJson = resultJson;
            return res;
        }
    }

    public class MESResponse
    {
        public string Result { get; set; } = "fail";
        public string ResultCode { get; set; } = "";
        public string Message { get; set; } = "";
        public JToken? Data { get; set; }
        
        [JsonIgnore]
        public string RawJson { get; set; } = "";

        [JsonExtensionData]
        public Dictionary<string, JToken>? RawFields { get; set; }

        public bool IsSuccess => Result.ToLower() == "success";

        public T? GetValue<T>(string key)
        {
            if (RawFields != null && RawFields.TryGetValue(key, out var token))
            {
                return token.ToObject<T>();
            }
            return default;
        }
    }
}
