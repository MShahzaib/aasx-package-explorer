/*
Copyright (c) 2018-2019 Festo AG & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AasxIntegrationBase;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;

namespace AasxPackageExplorer
{
    public partial class ShowEclassInfoFlyout : UserControl, IFlyoutControl
    {

        //
        // Members / events
        //

        public static string SERVER_URL = "https://18.157.197.66:8443";


        public event IFlyoutControlClosed ControlClosed;

        //
        // Init
        //

        public ShowEclassInfoFlyout()
        {
            InitializeComponent();
        }

        public async void GetInfo(String irdi) {
            LabelIRDI.Text = irdi;
            var irdi_url_path = Regex.Replace(irdi, @"\W", "_");
            try
            {
                using (var httpClientHandler = new HttpClientHandler())
                {
                    // do not validate certificates
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                    using (var client = new HttpClient(httpClientHandler))
                    {
                        client.DefaultRequestHeaders.Add("Accept", "application/ld+json");
                        HttpResponseMessage response = await client.GetAsync($"{SERVER_URL}/eclass/IRDI_{irdi_url_path}");
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        string label_key = @"http://www.w3.org/2000/01/rdf-schema#label";
                        dynamic obj = JsonConvert.DeserializeObject(responseBody);
                        string type = (string)obj[0]["@type"][0];
                        LabelRDFLabel.Text = (string)obj[0][label_key]["@value"];
                        LabelRDFType.Text = Regex.Split(type, @"#").Last();
                    }
                }
            }
            catch (Exception exp)
            {
                AdminShellNS.LogInternally.That.SilentlyIgnoredError(exp);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        //
        // Outer
        //

        public void ControlStart()
        {
        }

        public void ControlPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ControlClosed?.Invoke();
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            ControlClosed?.Invoke();
        }
    }
}
