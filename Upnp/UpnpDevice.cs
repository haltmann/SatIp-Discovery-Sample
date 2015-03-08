﻿/*  
    Copyright (C) <2007-2015>  <Kay Diefenthal>

    SatIp.DiscoverySample is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SatIp.DiscoverySample is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with SatIp.DiscoverySample.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using SatIp.DiscoverySample.Logging;

namespace SatIp.DiscoverySample.Upnp
{
    public class UpnpDevice
    {
        private string _baseHost = "";
        private string _deviceType = "";
        private string _friendlyName = "";
        private string _manufacturer = "";
        private string _manufacturerUrl = "";
        private string _modelDescription = "";
        private string _modelName = "";
        private string _modelNumber = "";
        private string _modelUrl = "";
        private string _serialNumber = "";
        private string _uDN = "";
        private string _presentationUrl = "";
        private string _deviceDescription;
        private UpnpIcon[] _iconList = new UpnpIcon[4];
        private string _frontends = "";
        private string _basePort;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="url">Device URL.</param>
        internal UpnpDevice(string url)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            Init(url);
        }

        #region method Init

        public string GetImage(int index)
        {
            var icon=(UpnpIcon)_iconList.GetValue(index);
            return  icon.Url;
        }

        private void Init(string url)
        {
            try
            {
                Logger.Info("the Description Url is {0}", url);
                var document = XDocument.Load(url);
                var xnm= new XmlNamespaceManager(new NameTable());
                XNamespace n1 = "urn:ses-com:satip";
                XNamespace n0 = "urn:schemas-upnp-org:device-1-0";
                xnm.AddNamespace("root", n0.NamespaceName);
                xnm.AddNamespace("satip",n1.NamespaceName);
                if (document.Root != null)
                {
                    var deviceElement = document.Root.Element(n0 + "device");
                    var addressline = Regex.Split(url, @"://+");
                    var address = addressline[1].Split(':');
                    BaseHost = address[0];
                    var port = address[1].Split('/');
                    BasePort = port[0];
                    _deviceDescription = document.Declaration + document.ToString();
                    Logger.Info("The Description has this Content {0}",_deviceDescription);
                    if (deviceElement != null)
                    {
                        var devicetypeElement = deviceElement.Element(n0 + "deviceType");
                        if (devicetypeElement != null)
                            _deviceType = devicetypeElement.Value;
                        var friendlynameElement = deviceElement.Element(n0 + "friendlyName");
                        if (friendlynameElement != null)
                            _friendlyName = friendlynameElement.Value;
                        var manufactureElement = deviceElement.Element(n0 + "manufacturer");
                        if (manufactureElement != null)
                            _manufacturer = manufactureElement.Value;
                        var manufactureurlElement = deviceElement.Element(n0 + "manufacturerURL");
                        if (manufactureurlElement != null)
                            _manufacturerUrl = manufactureurlElement.Value;
                        var modeldescriptionElement = deviceElement.Element(n0 + "modelDescription");
                        if (modeldescriptionElement != null)
                            _modelDescription = modeldescriptionElement.Value;
                        var modelnameElement = deviceElement.Element(n0 + "modelName");
                        if (modelnameElement != null)
                            _modelName = modelnameElement.Value;
                        var modelnumberElement = deviceElement.Element(n0 + "modelNumber");
                        if (modelnumberElement != null)
                            _modelNumber = modelnumberElement.Value;
                        var modelurlElement = deviceElement.Element(n0 + "modelURL");
                        if (modelurlElement != null)
                            _modelUrl = modelurlElement.Value;
                        var serialnumberElement = deviceElement.Element(n0 + "serialNumber");
                        if (serialnumberElement != null)
                            _serialNumber = serialnumberElement.Value;
                        var uniquedevicenameElement = deviceElement.Element(n0 + "UDN");
                        if (uniquedevicenameElement != null) _uDN = uniquedevicenameElement.Value;
                        var iconList = deviceElement.Element(n0 + "iconList");
                        if (iconList != null)
                        {
                            var icons = from query in iconList.Descendants(n0 + "icon")
                                select new UpnpIcon
                                {
                                    // Needed to change mimeType to mimetype. XML is case sensitive 
                                    MimeType = (string) query.Element(n0 + "mimetype"),
                                    Url = (string) query.Element(n0 + "url"),
                                    Height = (int) query.Element(n0 + "height"),
                                    Width = (int) query.Element(n0 + "width"),
                                    Depth = (int) query.Element(n0 + "depth"),
                                };

                            _iconList = icons.ToArray();
                        }
                        _presentationUrl = deviceElement.Element(n0 + "presentationURL").Value;
                        _frontends = deviceElement.Element(n1 + "X_SATIPCAP").Value;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error("It give a Problem with the Description {0}", exception);
            }
        }
        
        #endregion


        #region Proeprties implementation
        
        /// <summary>
        /// Gets device type.
        /// </summary>
        public string DeviceType
        {
            get{ return _deviceType; }
        }

        /// <summary>
        /// Gets device short name.
        /// </summary>
        public string FriendlyName
        {
            get{ return _friendlyName; }
        }

        /// <summary>
        /// Gets manufacturer's name.
        /// </summary>
        public string Manufacturer
        {
            get{ return _manufacturer; }
        }

        /// <summary>
        /// Gets web site for Manufacturer.
        /// </summary>
        public string ManufacturerUrl
        {
            get{ return _manufacturerUrl; }
        }

        /// <summary>
        /// Gets device long description.
        /// </summary>
        public string ModelDescription
        {
            get{ return _modelDescription; }
        }

        /// <summary>
        /// Gets model name.
        /// </summary>
        public string ModelName
        {
            get{ return _modelName; }
        }

        /// <summary>
        /// Gets model number.
        /// </summary>
        public string ModelNumber
        {
            get{ return _modelNumber; }
        }

        /// <summary>
        /// Gets web site for model.
        /// </summary>
        public string ModelUrl
        {
            get{ return _modelUrl; }
        }

        /// <summary>
        /// Gets serial number.
        /// </summary>
        public string SerialNumber
        {
            get{ return _serialNumber; }
        }

        /// <summary>
        /// Gets unique device name.
        /// </summary>
        public string UDN
        {
            get{ return _uDN; }
        }

        // iconList
        // serviceList
        // deviceList

        /// <summary>
        /// Gets device UI url.
        /// </summary>
        public string PresentationUrl
        {
            get{ return _presentationUrl; }
        }

        /// <summary>
        /// Gets UPnP device XML description.
        /// </summary>
        public string DeviceDescription
        {
            get{ return _deviceDescription; }
        }

        public string BaseHost
        {
            get { return _baseHost; }
            set { _baseHost = value; }
        }

        public string BasePort
        {
            get { return _basePort; }
            set { _basePort = value; }
        }

        public string Frontends
        {
            get { return _frontends; }
            set { _frontends = value; }
        }

        #endregion

    }
}
