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
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AasxPackageExplorer
{
    public partial class ReserveAtEclassFlyout : UserControl, IFlyoutControl
    {
        //
        // Members / events
        //

        public static string URL_PREFIX = "https://18.157.197.66:8443/eclass";

        public string irdi_uri_path { get; set; }
        public string irdi_identifier { get; set; }
        public string type { get; set; }
        public string prefName { get; set; }
        public string label { get; set; }

        public event IFlyoutControlClosed ControlClosed;

        public string ResultIRDI = null;

        //
        // Init
        //

        public ReserveAtEclassFlyout()
        {
            InitializeComponent();
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
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Return)
            {
                ControlClosed?.Invoke();
            }
            if (e.Key == Key.Escape)
            {
                this.ResultIRDI = null;
                ControlClosed?.Invoke();
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.ResultIRDI = null;
            ControlClosed?.Invoke();
        }

        //
        // Mechanics
        //

        private void textChangedEventHandler(object sender, TextChangedEventArgs args)
        {
            if (TextBoxIRDIVersion != null && TextBoxIRDI != null && LabelIdentifier != null && LabelURI != null)
            {
                irdi_identifier = $"0173-{TextBoxIRDI.Text}#{TextBoxIRDIVersion.Text}";
                irdi_uri_path = "IRDI_" + Regex.Replace(irdi_identifier, @"\W", "_");
                LabelIdentifier.Text = irdi_identifier;
                LabelURI.Text = irdi_uri_path;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (sender == ButtonSelectCertFile)
            //{
            //    // choose filename
            //    var dlg = new Microsoft.Win32.OpenFileDialog();
            //    dlg.DefaultExt = "*.*";
            //    dlg.Filter = "PFX file (*.pfx)|*.pfx|Cert file (*.cer)|*.cer|All files (*.*)|*.*";

            //    // save
            //    if (true == dlg.ShowDialog())
            //    {
            //        ComboBoxCertFile.Text = dlg.FileName;
            //    }
            //}

            if (sender == ButtonSaveEclassEntry)
            {

                try
                {
                    type = TextBoxType.Text;
                    prefName = TextBoxPreferredName.Text;
                    label = TextBoxShortDescription.Text;

                    JArray jsonObj =
                        new JArray(
                            new JObject(
                                new JProperty("@id", URL_PREFIX + '/' + irdi_uri_path),
                                new JProperty("@type", new JArray($"{URL_PREFIX}/ECLASS_Model#{type}")),
                                new JProperty("http://www.w3.org/2000/01/rdf-schema#label",
                                    new JObject(new JObject(
                                        new JProperty("@value", label),
                                        new JProperty("@language", "en-us")
                                        )
                                    )
                                ), new JProperty($"{URL_PREFIX}/ECLASS_Model#preferred_name",
                                    new JObject(new JObject(
                                        new JProperty("@value", prefName),
                                        new JProperty("@language", "en-us")
                                        )
                                    )
                                )
                            )
                        );

                    using (var httpClientHandler = new HttpClientHandler())
                    {
                        // do not validate certificates
                        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                        using (var client = new HttpClient(httpClientHandler))
                        {
                            var httpContent = new StringContent(jsonObj.ToString(), Encoding.UTF8, "application/ld+json");
                            HttpResponseMessage response = await client.PutAsync("https://18.157.197.66:8443/eclass/" + irdi_uri_path, httpContent);
                            response.EnsureSuccessStatusCode();
                            this.ResultIRDI = irdi_identifier;
                            ControlClosed?.Invoke();
                        }
                    }
                }
                catch (Exception ex)
                {
                    AdminShellNS.LogInternally.That.SilentlyIgnoredError(ex);
                }
            }
        }
    }
}
