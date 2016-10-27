using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class WWWS /*: IEnumerable*/ {
    HttpWebRequest request;
    public string text { get; private set; }
    public string method { get; private set; }
    public string url { get; private set; }
    public string error { get; private set; }
    public bool isDone { get; private set; }
    public string[][] formdata { get; private set; }
    
    private byte[] postBytes;
    
    public WWWS(string url, string method="GET", string[][] formdata=null) {
        this.url = url;
        this.method = method;
        this.formdata = formdata;
        isDone = false;
    }

    //IEnumerator IEnumerable.GetEnumerator()
    public IEnumerator next()
    {
        isDone = false;
        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
        
        error = null;
        try {
            request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = method;
            request.AllowAutoRedirect = true;
            bool letStreamRequestStartResponseThread = false;
            if (request.Method == "POST")
            {
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                if (formdata != null)
                {
                    System.Collections.Specialized.NameValueCollection outgoingQueryString = new System.Collections.Specialized.NameValueCollection{};
                    foreach (string[] keyval in formdata)
                        outgoingQueryString.Add(keyval[0], keyval[1]);
                    string postData = outgoingQueryString.ToString();
                    System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
                    postBytes = ascii.GetBytes(postData.ToString());
                    request.ContentLength = postBytes.Length;
                    letStreamRequestStartResponseThread = true; // on laisse GetRequestStreamCallback faire l'appel à BeginGetResponse
                    request.BeginGetRequestStream(new System.AsyncCallback(GetRequestStreamCallback), request);
                }
            }
            if (!letStreamRequestStartResponseThread)
            {
                // HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                request.BeginGetResponse(new System.AsyncCallback(FinishWebRequest), null);
            }
        }
        catch (WebException webExcp)
        {
            error = webExcp.ToString();
        }
        
        while (!isDone)
            yield return null;
    }
    void GetRequestStreamCallback(System.IAsyncResult asynchronousResult)
    {
        HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
        
        // End the operation
        Stream postStream = request.EndGetRequestStream(asynchronousResult);
        
        postStream.Write(postBytes, 0, postBytes.Length);
        postStream.Flush();
        postStream.Close();
        
        // Start the asynchronous operation to get the response
        request.BeginGetResponse(new System.AsyncCallback(FinishWebRequest), request);
    }
    void FinishWebRequest(System.IAsyncResult result)
    {
        try {
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Debug.Log(response.ContentType);
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                text = reader.ReadToEnd();
            }
            else if (
                response.StatusCode == System.Net.HttpStatusCode.Forbidden ||
                response.StatusCode == System.Net.HttpStatusCode.BadGateway ||
                response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                response.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed ||
                response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                error = "Resource unavailable. Code=" + response.StatusCode;
                Debug.Log(error);
            }
            else
            {
                // FIXME
            }
        }
        catch (WebException webExcp)
        {
            error = webExcp.ToString();
        }
        
        isDone = true;
    }

    //http://stackoverflow.com/questions/3674692/mono-webclient-invalid-ssl-certificates
    private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
    {
        // all Certificates are accepted
        return true;
    }

}
