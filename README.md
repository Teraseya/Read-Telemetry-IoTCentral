# Citire telemetrie in Azure IoT Central
Unul din scenariile des intinite in Azure IoT Central este ca un device sa vrea sa acceseze telemetria unui alt device conectat la acceasi IoT Central.
Un exemplu de astfel de scenariu este un device se ocupa cu afisare datelor colectate de la celalte device, toate device fiind conectate la acceasi IoT Central.
![OSS](https://user-images.githubusercontent.com/14031360/179508291-d7398901-72ff-439e-ab04-b64b53f04f1b.jpg)

Acest scenariu din nefericere nu este posibil in acest moment in Azure IoT Central.
Dar totusi desi, "din cutie" nu e suppotat, acest scenariu se poate implemnta.

## Aceasta arhitectura si codul aferent este prezentat aici:

### Pasul 1: 
Device care doreste semnaleaza acest lucru, de exemplu seteaza valoarea un date telemetrice la o valoare prestabilita semnalizind ca doreste date. 
Se poate folosi si un device twin.In device twin se stocheaza id device de la care vrea citirea de date, care e device cu senzori.

### Pasul 2: 
O regula in IoT Central cind se indeplineste conditia ca se doresc date apeleaza o functie Azure.

### Pasul 3: 
Functia folosind Azure IoT Central REST API interogheaza device pentru date.

### Pasul 4: 
Functia excuta o comanda pe device care a cerut datele, care are ca paramaetru datele citite. Si in acest s-ar pute folosi un device twin.
![OSS2](https://user-images.githubusercontent.com/14031360/179508614-10ff7272-442f-40df-84e1-12195780c8c7.jpg)

Exemplu explicat (atentie lipseste autentificarea)
``` c#
using (HttpRequestMessage httpRequestMessage = new HttpRequestMessage())
 {
    httpRequestMessage.Method = new HttpMethod("GET");
    //interogare REST API 
    //yourapp este aplicatia IoTCentral
    //deviceId este id device de la care vreau date
    //telemetry este "numele" datei pe care o vreau
    httpRequestMessage.RequestUri = new Uri(string.Format("https://yourapplication.azureiotcentral.com/api/devices/{0}/telemetry/{1}?api-version=1.0", deviceId, telemetry), UriKind.RelativeOrAbsolute);
    HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
            //extrag din json valoarea, a parser ar fi prea "heavy" pentru acesta utilizare
            //Exemplu de raspuns {"value":234.4}
            string str = "\"value\":";
            int startIndex = result.IndexOf(str) + str.Length;
            int num = result.IndexOf("}", startIndex + 1);
            if (startIndex >= str.Length && num != -1)
            {                
                string telemetryValue = result.Substring(startIndex, num - startIndex);
                log.LogInformation("Telemetry interrogation was ok , " + telemetryValue + " was the value");
                return float.Parse(telemetryValue);
            }
            log.LogInformation("Telemetry interrogation failed");
         }
        return -256;
    }
``` 
