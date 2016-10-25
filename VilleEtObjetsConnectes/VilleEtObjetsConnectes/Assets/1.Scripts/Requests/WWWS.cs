using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class WWWS /*: IEnumerable*/ {
    HttpWebRequest request;
    public string text;
    string url;
    public bool isDone {get; private set;}
    
    public WWWS(string url) {
        this.url = url;
        isDone = false;
    }

    //IEnumerator IEnumerable.GetEnumerator()
    public IEnumerator next()
    {
        isDone = false;
        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
        
        request = (HttpWebRequest) WebRequest.Create(url);
        // HttpWebResponse response = (HttpWebResponse) request.GetResponse();
        request.BeginGetResponse(new System.AsyncCallback(FinishWebRequest), null);
        
        while (!isDone)
            yield return null;
    }
    void FinishWebRequest(System.IAsyncResult result)
    {
        WebResponse response = request.EndGetResponse(result);
        
        Stream dataStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(dataStream);
        text = reader.ReadToEnd();
        
        isDone = true;
    }

    //http://stackoverflow.com/questions/3674692/mono-webclient-invalid-ssl-certificates
    private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
    {
        // all Certificates are accepted
        return true;
    }

}
