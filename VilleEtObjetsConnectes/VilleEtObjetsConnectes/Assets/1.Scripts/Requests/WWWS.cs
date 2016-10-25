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
    
    public WWWS(string url, string method="GET") {
        this.url = url;
        this.method = method;
        isDone = false;
    }

    //IEnumerator IEnumerable.GetEnumerator()
    public IEnumerator next()
    {
        isDone = false;
        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
        
        try {
            request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = method;
            request.AllowAutoRedirect = true;
            if (request.Method == "POST")
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            // HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            request.BeginGetResponse(new System.AsyncCallback(FinishWebRequest), null);
            error = null;
        }
        catch (WebException webExcp)
        {
            error = webExcp.ToString();
        }
        
        while (!isDone)
            yield return null;
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
