// Domain/Helpers/VnpayLibrary.cs
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web; // Cần tham chiếu System.Web hoặc cài package System.Web.HttpUtility

namespace Domain.Helpers;

public class VnpayLibrary
{
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _requestData.Add(key, value);
        }
    }

    /// <summary>
    /// Tạo URL thanh toán VNPAY với chữ ký bảo mật.
    /// </summary>
    /// <param name="baseUrl">URL cơ sở của VNPAY (ví dụ: https://sandbox.vnpayment.vn/paymentv2/vpcpay.html)</param>
    /// <param name="vnpHashSecret">Chuỗi bí mật được VNPAY cung cấp.</param>
    /// <returns>URL thanh toán hoàn chỉnh.</returns>
    public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
    {
        StringBuilder data = new StringBuilder();
        foreach (KeyValuePair<string, string> kv in _requestData)
        {
            if (!string.IsNullOrEmpty(kv.Value))
            {
                data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }
        }
        string queryString = data.ToString();

        baseUrl += "?" + queryString;
        string signData = queryString;
        if (signData.Length > 0)
        {

            signData = signData.Remove(data.Length - 1, 1);
        }
        string vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);
        baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

        return baseUrl;
    }


    /// <summary>
    /// Xác thực chữ ký trả về từ VNPAY.
    /// </summary>
    /// <param name="vnpSecureHash">Giá trị vnp_SecureHash nhận được từ VNPAY.</param>
    /// <param name="vnpHashSecret">Chuỗi bí mật của bạn.</param>
    /// <param name="vnpayData">Tất cả các tham số query VNPAY trả về (ngoại trừ vnp_SecureHashType và vnp_SecureHash).</param>
    /// <returns>True nếu chữ ký hợp lệ, ngược lại là false.</returns>
    public bool ValidateSignature(string vnpSecureHash, string vnpHashSecret, IQueryCollection vnpayData)
    {
        var dataToHashBuilder = new StringBuilder();

        // Sắp xếp và nối chuỗi từ dữ liệu callback (đã được decode)
        foreach (var kvp in vnpayData.Where(kvp => kvp.Key.StartsWith("vnp_")).OrderBy(k => k.Key))
        {
            // Bỏ qua hai trường vnp_SecureHash và vnp_SecureHashType
            if (!kvp.Key.Equals("vnp_SecureHash", StringComparison.InvariantCultureIgnoreCase) &&
                !kvp.Key.Equals("vnp_SecureHashType", StringComparison.InvariantCultureIgnoreCase))
            {
                // Nối key=value lại với nhau
                dataToHashBuilder.Append(kvp.Key + "=" + kvp.Value + "&");
            }
        }

        string dataToHash = dataToHashBuilder.ToString().TrimEnd('&');

        string calculatedHash = HmacSHA512(vnpHashSecret, dataToHash);

        return calculatedHash.Equals(vnpSecureHash, StringComparison.InvariantCultureIgnoreCase);
    }

    public static string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            foreach (var b in hashValue)
            {
                hash.Append(b.ToString("x2")); // định dạng hex chữ thường
            }
        }
        return hash.ToString();
    }
}

// Lớp Compare để sắp xếp tham số theo đúng thứ tự VNPAY yêu cầu
public class Utils
{


    public static string HmacSHA512(string key, string inputData)
    {
        var hash = new StringBuilder();
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }

        return hash.ToString();
    }
}

public class VnPayCompare : IComparer<string>
{
    public int Compare(string x, string y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        var vnpCompare = CompareInfo.GetCompareInfo("en-US");
        return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
    }
}