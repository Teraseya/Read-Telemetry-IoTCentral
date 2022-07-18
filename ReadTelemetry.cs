		/// <summary>
        /// Reads the specified telemetry from
        /// thes specified device.
        /// </summary>
        /// <param name="deviceId">The devoce from wich to read the telemetry.</param>
        /// <param name="telemetry">What telemetry to read.</param>
        /// <param name="log">A <see cref="ILogger"/> for logging.</param>
        /// <returns>-256 if error, otherwise the telemetry value.</returns>
		/// <remarks>yourkey and yourapp you must change to the actual values for your IoT Central app.
        /// </remarks>
        private static async Task<float> ReadTelemetry(string deviceId, string telemetry, ILogger log)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", "SharedAccessSignature yourkey");
                    using (HttpRequestMessage httpRequestMessage = new HttpRequestMessage())
                    {
                        httpRequestMessage.Method = new HttpMethod("GET");
                        httpRequestMessage.RequestUri = new Uri(string.Format("https://yourapp.azureiotcentral.com/api/devices/{0}/telemetry/{1}?api-version=1.0", deviceId, telemetry), UriKind.RelativeOrAbsolute);
                        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                        {
                            string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
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
                }
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
                return -256;
            }
        }
    }
}