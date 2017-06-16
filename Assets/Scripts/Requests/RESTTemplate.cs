using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class RESTTemplate {

    // Synchronous POST
    public static IEnumerator SyncPOST(string url, string requestBody)
    {

        UnityWebRequest www = new UnityWebRequest(url);
        www.SetRequestHeader("Content-Type", "application/json");
        www.method = UnityWebRequest.kHttpVerbPOST;
        www.uploadHandler = new UploadHandlerRaw(Encoding.Default.GetBytes(requestBody));
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.Send();
        yield return www.downloadHandler.text;

    }

    // Asynchronous POST
    public static void AsyncPOST(string url, string requestBody)
    {

        UnityWebRequest www = new UnityWebRequest(url);
        www.SetRequestHeader("Content-Type", "application/json");
        www.method = UnityWebRequest.kHttpVerbPOST;
        www.uploadHandler = new UploadHandlerRaw(Encoding.Default.GetBytes(requestBody));
        www.Send();

    }

}
