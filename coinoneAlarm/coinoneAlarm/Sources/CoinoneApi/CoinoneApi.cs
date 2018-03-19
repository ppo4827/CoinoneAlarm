using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace coinoneAlarm.Sources
{
    class CoinoneApi
    {
        private readonly Uri uri = new Uri("https://api.coinone.co.kr/");

        public async void GetCoinPrice(TextBlock tbPrice, TextBlock tbPriceMax, TextBlock tbPriceMin, string coinName)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var httpRequestMessage = new HttpRequestMessage(new HttpMethod("GET"), string.Format("/ticker/?currency={0}&format=json", coinName)))
                    {
                        httpClient.BaseAddress = uri;

                        var Req = await httpClient.SendAsync(httpRequestMessage);
                        var Res = await Req.Content.ReadAsStringAsync();

                        dynamic res = JsonConvert.DeserializeObject(Res);

                        if (res["errorCode"] != "0") return;

                        tbPrice.Text = string.Format("{0} KRW", res["last"]);
                        tbPriceMax.Text = string.Format("Highest in 24 hours - {0} KRW", res["high"]);
                        tbPriceMin.Text = string.Format("Lowest in 24 hours - {0} KRW", res["low"]);
                    }
                }
            }
            catch
            {
                return;
            }
        }

        public async void GetCoinBalance(string strApiKey, string strAccessToekn,
                                         TextBlock tbBtcBalance, TextBlock tbBchBalance, TextBlock tbEthBalance,
                                         TextBlock tbEtcBalance, TextBlock tbXrpBalance, TextBlock tbQtumBalance,
                                         TextBlock tbLtcBalance, TextBlock tbIotaBalance, TextBlock tbBtgBalance,
                                         TextBlock tbTop)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var httpRequestMessage = new HttpRequestMessage(new HttpMethod("POST"), string.Format("/v2/account/balance")))
                    {
                        httpClient.BaseAddress = uri;

                        string strPayload = GetPayload(strAccessToekn);

                        httpRequestMessage.Headers.Add("X-COINONE-PAYLOAD", strPayload);
                        httpRequestMessage.Headers.Add("X-COINONE-SIGNATURE", GetSignature(strApiKey, strPayload));

                        var varReq = await httpClient.SendAsync(httpRequestMessage);
                        var varRes = await varReq.Content.ReadAsStringAsync();
                        
                        dynamic res = JsonConvert.DeserializeObject(varRes);

                        if (res["errorCode"] != "0") return;

                        tbBtcBalance.Text = string.Format("You have {0} BTC(s)", res["btc"]["avail"]);
                        tbBchBalance.Text = string.Format("You have {0} BCH(s)", res["bch"]["avail"]);
                        tbEthBalance.Text = string.Format("You have {0} ETH(s)", res["eth"]["avail"]);
                        tbEtcBalance.Text = string.Format("You have {0} ETC(s)", res["etc"]["avail"]);
                        tbXrpBalance.Text = string.Format("You have {0} XRP(s)", res["xrp"]["avail"]);
                        tbQtumBalance.Text = string.Format("You have {0} QTUM(s)", res["qtum"]["avail"]);
                        tbLtcBalance.Text = string.Format("You have {0} LTC(s)", res["ltc"]["avail"]);
                        tbIotaBalance.Text = string.Format("You have {0} IOTA(s)", res["iota"]["avail"]);
                        tbBtgBalance.Text = string.Format("You have {0} BTG(s)", res["btg"]["avail"]);
                        tbTop.Text = string.Format("You have {0} KRW(s)", res["krw"]["avail"]);
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private string GetPayload(string strAccessToekn)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();

            parameter.Add("access_token", strAccessToekn);
            parameter.Add("nonce", DateTime.Now.ToString("yyyyMMddHHmmssffffff"));
            
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(parameter)));
        }

        private string GetSignature(string strApiKey, string strPayload)
        {
            Common.EncryptData encryptData = new Common.EncryptData();

            string strResult = string.Empty;

            byte[] byteArray = Encoding.Default.GetBytes(encryptData.ComputeHmacSha512(strPayload, encryptData.ComputeSha512(strApiKey)));

            foreach (byte byteStr in byteArray)
                strResult += string.Format("{0:X2}", byteStr);

            return strResult.ToLower();
        }

    }
}
