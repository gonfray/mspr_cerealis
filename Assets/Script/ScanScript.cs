using System;
using System.Web;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class ScanScript : MonoBehaviour
{
    public GameObject Form;
    public Text inputName;
    public Text inputMail;
    
    public void PressReturnButton ()
    {
        SceneManager.LoadScene(0);
    }

    public void PressShareButton ()
    {
        Form.SetActive(true);
    }

    public void PressSubmitButton ()
    {
        HttpRequestUnity(inputName.text, inputMail.text);
        inputName.text = "";
        inputMail.text = "";
        StartCoroutine(nameof(TakeScreenshotAndShare));
    }

    private async void HttpRequestUnity(string lastname, string email){
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://cerealis.with4.dolicloud.com/api/index.php/contacts"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("DOLAPIKEY", "J8TaA0q5asF94oc7o5hlYD6VQkM7KW5z"); 

                request.Content = new StringContent("{\"lastname\":\""+lastname+"\",\"email\":\""+email+"\"}");
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json"); 

                var response = await httpClient.SendAsync(request);
                Debug.Log(response);
            }
        }
    }

    public IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();
        Form.SetActive(false);

        Texture2D ss = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
        ss.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
        ss.Apply();

        string filePath = Path.Combine( Application.temporaryCachePath, "shared img.png" );
        File.WriteAllBytes( filePath, ss.EncodeToPNG() );

        // To avoid memory leaks
        Destroy( ss );

        new NativeShare().AddFile( filePath )
            .SetSubject( "" ).SetText( "" ).SetUrl( "" )
            .SetCallback( ( result, shareTarget ) => Debug.Log( "Share result: " + result + ", selected app: " + shareTarget ) )
            .Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }
}
