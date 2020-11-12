using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndigoCardIssuanceService.DataContracts.PINReissue
{
    public class PinReissueWSResult
    {
        public PinReissueWSResult() { }
        public static PinReissueWSResult Create(Veneka.Indigo.Common.Models.PinReissueResult pinReissueResult)
        {
            PinReissueWSResult pinReissueWSResult = new PinReissueWSResult
            {
                authorise_pin_reissue_YN = pinReissueResult.authorise_pin_reissue_YN,
                authorise_username = pinReissueResult.authorise_username,
                authorise_user_id = pinReissueResult.authorise_user_id,
                branch_code = pinReissueResult.branch_code,
                branch_id = pinReissueResult.branch_id,
                branch_name = pinReissueResult.branch_name,
                card_number = pinReissueResult.card_number,
                comments = pinReissueResult.comments,
                failed = pinReissueResult.failed,
                issuer_code = pinReissueResult.issuer_code,
                issuer_id = pinReissueResult.issuer_id,
                issuer_name = pinReissueResult.issuer_name,
                notes = pinReissueResult.notes,
                operator_usename = pinReissueResult.operator_usename,
                operator_user_id = pinReissueResult.operator_user_id,
                pan = pinReissueResult.pan,
                pin_reissue_id = pinReissueResult.pin_reissue_id,
                pin_reissue_statuses_id = pinReissueResult.pin_reissue_statuses_id,
                pin_reissue_statuses_name = pinReissueResult.pin_reissue_statuses_name,
                primary_index_number = pinReissueResult.primary_index_number,
                product_code = pinReissueResult.product_code,
                product_id = pinReissueResult.product_id,
                product_name = pinReissueResult.product_name,
                reissue_date = pinReissueResult.reissue_date.Value.DateTime,
                request_expiry = pinReissueResult.request_expiry.Value.DateTime,
                ROW_NO = pinReissueResult.ROW_NO,
                status_date = pinReissueResult.status_date.Value.DateTime,
                TOTAL_PAGES = pinReissueResult.TOTAL_PAGES,
                TOTAL_ROWS = pinReissueResult.TOTAL_ROWS,
                user_id = pinReissueResult.user_id,
                user_time_zone = pinReissueResult.user_time_zone
            };

            return pinReissueWSResult;
        }

        public static List<PinReissueWSResult> Create(List<Veneka.Indigo.Common.Models.PinReissueResult> pinReissueResults)
        {
            List<PinReissueWSResult> wsResults = new List<PinReissueWSResult>();

            foreach (var result in pinReissueResults)            
                wsResults.Add(PinReissueWSResult.Create(result));

            return wsResults;
        }

        public Int32 issuer_id { get; set; }
        public Int32 branch_id { get; set; }
        public Int32 product_id { get; set; }
        public Byte[] pan { get; set; }
        public DateTime reissue_date { get; set; }
        public Int64 operator_user_id { get; set; }
        public long? authorise_user_id { get; set; }
        public Boolean failed { get; set; }
        public String notes { get; set; }
        public Int64 pin_reissue_id { get; set; }
        public Byte[] primary_index_number { get; set; }
        public String card_number { get; set; }
        public Int32 pin_reissue_statuses_id { get; set; }
        public DateTime status_date { get; set; }
        public Int64 user_id { get; set; }
        public String pin_reissue_statuses_name { get; set; }
        public String operator_usename { get; set; }
        public String issuer_name { get; set; }
        public String issuer_code { get; set; }
        public String branch_name { get; set; }
        public String branch_code { get; set; }
        public String product_code { get; set; }
        public String product_name { get; set; }
        public String comments { get; set; }
        public DateTime request_expiry { get; set; }
        public String authorise_username { get; set; }
        public Boolean authorise_pin_reissue_YN { get; set; }
        public String user_time_zone { get; set; }
        public int? TOTAL_PAGES { get; set; }
        public long? ROW_NO { get; set; }
        public int? TOTAL_ROWS { get; set; }
    }
}