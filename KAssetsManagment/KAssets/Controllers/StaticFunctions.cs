using KAssets.Resources.Translation.EventsTr;
using KAssets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace KAssets.Controllers
{
    public static class StaticFunctions
    {
        /// <summary>
        /// Check user has certain right
        /// </summary>
        /// <param name="right"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool IsHasRihgt(string right, string userId)
        {
            var service = new RightService();
       
            if (service.IsUserHasRightById(userId, "Admin right"))
            {
                return true;
            }

            return service.IsUserHasRightById(userId, right);
        }

        /// <summary>
        /// Validate IBAN
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <returns></returns>
        public static bool ValidateBankAccount(string bankAccount)
        {
            bankAccount = bankAccount.ToUpper(); //IN ORDER TO COPE WITH THE REGEX BELOW
            if (String.IsNullOrEmpty(bankAccount))
                return false;
            else if (System.Text.RegularExpressions.Regex.IsMatch(bankAccount, "^[A-Z0-9]"))
            {
                bankAccount = bankAccount.Replace(" ", String.Empty);
                string bank =
                bankAccount.Substring(4, bankAccount.Length - 4) + bankAccount.Substring(0, 4);
                int asciiShift = 55;
                StringBuilder sb = new StringBuilder();
                foreach (char c in bank)
                {
                    int v;
                    if (Char.IsLetter(c)) v = c - asciiShift;
                    else v = int.Parse(c.ToString());
                    sb.Append(v);
                }
                string checkSumString = sb.ToString();
                int checksum = int.Parse(checkSumString.Substring(0, 1));
                for (int i = 1; i < checkSumString.Length; i++)
                {
                    int v = int.Parse(checkSumString.Substring(i, 1));
                    checksum *= 10;
                    checksum += v;
                    checksum %= 97;
                }
                return checksum == 1;
            }
            else
                return false;
        }
        //То second point
        public static double FormattedTo2(this double number)
        {
            try
            {
                string numberAsText = number.ToString();
                if (numberAsText.Contains(','))
                {
                    var pointIndex = numberAsText.IndexOf(',');

                    var newD = numberAsText.Substring(0, pointIndex + 3);

                    return double.Parse(newD);
                }
                else if (numberAsText.Contains('.'))
                {
                    var pointIndex = numberAsText.IndexOf('.');

                    var newD = numberAsText.Substring(0, pointIndex + 3);

                    return double.Parse(newD);
                }
                else
                {
                    return number;
                }
            }
            catch
            {
                return number;
            }
        }

        public static string TranslateDynamicEvent(string content)
        {
            string result = "";

            switch (content)
            {
                case "You have new accident to answer !":
                    {
                        result = EventsTr.e10;
                        break;
                    }

                case "You have new answer from accident !":
                    {
                        result = EventsTr.e11;
                        break;
                    }

                case "There is a new asset request for approving !":
                    {
                        result = EventsTr.e13;
                        break;
                    }

                case "There is a new approved request for asset !":
                    {
                        result = EventsTr.e15;
                        break;
                    }

                case "You have a request for finishing !":
                    {
                        result = EventsTr.e17;
                        break;
                    }

                case "You have a new request for finishing !":
                    {
                        result = EventsTr.e17;
                        break;
                    }

                case "You have a new invoice for approving !":
                    {
                        result = EventsTr.e18;
                        break;
                    }

                case "You have a new invice for paying !":
                    {
                        result = EventsTr.e19;
                        break;
                    }

                case "Your invoice was not approved !":
                    {
                        result = EventsTr.e20;
                        break;
                    }

                case "Your invoice was payed !":
                    {
                        result = EventsTr.e21;
                        break;
                    }

                case "There is a new request for items !":
                    {
                        result = EventsTr.e22;
                        break;
                    }

                case "There is new approved request for items !":
                    {
                        result = EventsTr.e24;
                        break;
                    }

                case "There are new request to provider for approving! ":
                    {
                        result = EventsTr.e26;
                        break;
                    }

                case "Your request to provider was not approved.":
                    {
                        result = EventsTr.e27;
                        break;
                    }

                case "Your request to provider was approved. ":
                    {
                        result = EventsTr.e28;
                        break;
                    }

                case "You have a new requst for relocation for approving !":
                    {
                        result = EventsTr.e29;
                        break;
                    }

                case "There are new request for renovation ! ":
                    {
                        result = EventsTr.e34;
                        break;
                    }

                case "The request for renovation was removed by user !":
                    {
                        result = EventsTr.e36;
                        break;
                    }

                case "There are new asset for renovatings ! ":
                    {
                        result = EventsTr.e37;
                        break;
                    }

                case "There is a new sccraping request for approving !":
                    {
                        result = EventsTr.e40;
                        break;
                    }
            }

            if (result == "")
            {
                if(content.Contains("The items of request with number")
                    &&content.Contains("was received"))
                {
                    result = content;
                    result = result.Replace("The items of request with number", EventsTr.e12);
                    result = result.Replace("was received", EventsTr.e121);
                }

                if (content.Contains("Your request for assets with inventory numbers")
                   && content.Contains("was not approved."))
                {
                    result = content;
                    result = result.Replace("Your request for assets with inventory numbers", EventsTr.e14);
                    result = result.Replace("was not approved.", EventsTr.e141);
                }

                if (content.Contains("Your request for assets with inventory numbers")
                   && content.Contains("was approved. But the asset cannot be gave !"))
                {
                    result = content;
                    result = result.Replace("Your request for assets with inventory numbers", EventsTr.e16);
                    result = result.Replace("was approved. But the asset cannot be gave !", EventsTr.e161);
                }

                if (content.Contains("Your request for")
                   && content.Contains("was not approved."))
                {
                    result = content;
                    result = result.Replace("Your request for", EventsTr.e23);
                    result = result.Replace("was not approved.", EventsTr.e231);
                }

                if (content.Contains("Your request for")
                   && content.Contains("was approved. But the items cannot be gave !"))
                {
                    result = content;
                    result = result.Replace("Your request for", EventsTr.e25);
                    result = result.Replace("was approved. But the items cannot be gave !", EventsTr.e251);
                }

                if (content.Contains("Your request for relocate the asset with inventory number")
                   && content.Contains("was approved."))
                {
                    result = content;
                    result = result.Replace("Your request for relocate the asset with inventory number", EventsTr.e30);
                    result = result.Replace("was approved.", EventsTr.e301);
                }

                if (content.Contains("Your request for relocate the asset with inventory number")
                   && content.Contains("was not approved."))
                {
                    result = content;
                    result = result.Replace("Your request for relocate the asset with inventory number", EventsTr.e31);
                    result = result.Replace("was not approved.", EventsTr.e311);
                }

                if (content.Contains("You have a new asset with inventory number")
                   && content.Contains("for receiving."))
                {
                    result = content;
                    result = result.Replace("You have a new asset with inventory number", EventsTr.e32);
                    result = result.Replace("for receiving.", EventsTr.e321);
                }

                if (content.Contains("Аsset with inventory number")
                   && content.Contains("was relocated."))
                {
                    result = content;
                    result = result.Replace("Аsset with inventory number", EventsTr.e33);
                    result = result.Replace("was relocated.", EventsTr.e331);
                }

                if (content.Contains("Your request for renovation the asset with inventory number")
                   && content.Contains("was approved."))
                {
                    result = content;
                    result = result.Replace("Your request for renovation the asset with inventory number", EventsTr.e35);
                    result = result.Replace("was approved.", EventsTr.e351);
                }

                if (content.Contains("Your asset with inventory number ")
                   && content.Contains("was renovated and now it is active !"))
                {
                    result = content;
                    result = result.Replace("Your asset with inventory number ", EventsTr.e38);
                    result = result.Replace("was renovated and now it is active !", EventsTr.e381);
                }

                if (content.Contains("Asset with inventory number")
                   && content.Contains("was not renovated and now it is active."))
                {
                    result = content;
                    result = result.Replace("Asset with inventory number", EventsTr.e39);
                    result = result.Replace("was not renovated and now it is active.", EventsTr.e391);
                }

                if (content.Contains("Your request for scrap asset with inventory number")
                   && content.Contains("was approved."))
                {
                    result = content;
                    result = result.Replace("Your request for scrap asset with inventory number", EventsTr.e41);
                    result = result.Replace("was approved.", EventsTr.e411);
                }

                if (content.Contains("Аsset with inventory number")
                   && content.Contains("was scrapped."))
                {
                    result = content;
                    result = result.Replace("Аsset with inventory number", EventsTr.e42);
                    result = result.Replace("was scrapped.", EventsTr.e421);
                }
                if (content.Contains("Your request for scrapping asset with inventory number")
                   && content.Contains("was not approved."))
                {
                    result = content;
                    result = result.Replace("Your request for scrapping asset with inventory number", EventsTr.e43);
                    result = result.Replace("was not approved.", EventsTr.e431);
                }
            }

            if(result=="")
            { 

}
            return result;
        }

        public class RSAALg
        {
            public static string Encryption(string strText)
            {
                var publicKey = "<RSAKeyValue><Modulus>21wEnTU+mcD2w0Lfo1Gv4rtcSWsQJQTNa6gio05AOkV/Er9w3Y13Ddo5wGtjJ19402S71HUeN0vbKILLJdRSES5MHSdJPSVrOqdrll/vLXxDxWs/U0UT1c8u6k/Ogx9hTtZxYwoeYqdhDblof3E75d9n2F0Zvf6iTb4cI7j6fMs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

                var testData = Encoding.UTF8.GetBytes(strText);

                using (var rsa = new RSACryptoServiceProvider(1024))
                {
                    try
                    {
                        // client encrypting data with public key issued by server                    
                        rsa.FromXmlString(publicKey.ToString());

                        var encryptedData = rsa.Encrypt(testData, true);

                        var base64Encrypted = Convert.ToBase64String(encryptedData);

                        return base64Encrypted;
                    }
                    finally
                    {
                        rsa.PersistKeyInCsp = false;
                    }
                }
            }

            public static string Decryption(string strText)
            {
                var privateKey = "<RSAKeyValue><Modulus>21wEnTU+mcD2w0Lfo1Gv4rtcSWsQJQTNa6gio05AOkV/Er9w3Y13Ddo5wGtjJ19402S71HUeN0vbKILLJdRSES5MHSdJPSVrOqdrll/vLXxDxWs/U0UT1c8u6k/Ogx9hTtZxYwoeYqdhDblof3E75d9n2F0Zvf6iTb4cI7j6fMs=</Modulus><Exponent>AQAB</Exponent><P>/aULPE6jd5IkwtWXmReyMUhmI/nfwfkQSyl7tsg2PKdpcxk4mpPZUdEQhHQLvE84w2DhTyYkPHCtq/mMKE3MHw==</P><Q>3WV46X9Arg2l9cxb67KVlNVXyCqc/w+LWt/tbhLJvV2xCF/0rWKPsBJ9MC6cquaqNPxWWEav8RAVbmmGrJt51Q==</Q><DP>8TuZFgBMpBoQcGUoS2goB4st6aVq1FcG0hVgHhUI0GMAfYFNPmbDV3cY2IBt8Oj/uYJYhyhlaj5YTqmGTYbATQ==</DP><DQ>FIoVbZQgrAUYIHWVEYi/187zFd7eMct/Yi7kGBImJStMATrluDAspGkStCWe4zwDDmdam1XzfKnBUzz3AYxrAQ==</DQ><InverseQ>QPU3Tmt8nznSgYZ+5jUo9E0SfjiTu435ihANiHqqjasaUNvOHKumqzuBZ8NRtkUhS6dsOEb8A2ODvy7KswUxyA==</InverseQ><D>cgoRoAUpSVfHMdYXW9nA3dfX75dIamZnwPtFHq80ttagbIe4ToYYCcyUz5NElhiNQSESgS5uCgNWqWXt5PnPu4XmCXx6utco1UVH8HGLahzbAnSy6Cj3iUIQ7Gj+9gQ7PkC434HTtHazmxVgIR5l56ZjoQ8yGNCPZnsdYEmhJWk=</D></RSAKeyValue>";

                var testData = Encoding.UTF8.GetBytes(strText);

                using (var rsa = new RSACryptoServiceProvider(1024))
                {
                    try
                    {
                        var base64Encrypted = strText;

                        // server decrypting data with private key                    
                        rsa.FromXmlString(privateKey);

                        var resultBytes = Convert.FromBase64String(base64Encrypted);
                        var decryptedBytes = rsa.Decrypt(resultBytes, true);
                        var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                        return decryptedData.ToString();
                    }
                    finally
                    {
                        rsa.PersistKeyInCsp = false;
                    }
                }
            }
        }
    }
}
