using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;


namespace EdipostApiExamples
{
    class Example {
        private const String API_KEY = "5b10146aa8326ab219048595945b8592bc271ab0";

        private String apiKey;


        public Example() {
            apiKey = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("api:" + API_KEY));

            // Get default sender name and address
            GetDefaultConsignor();

            // Create receiver address
            int consigneeId = CreateConsignee();

            // Find available products for receiver
            GetProducts( consigneeId );

            // Create consignment
            int consignmentId = CreateConsignment( consigneeId );

            // Print the consignment and open in default PDF viewer
            PrintConsignment( consignmentId );

            Console.WriteLine("Please press enter to continue...");
            Console.ReadLine();
        }


        private void GetDefaultConsignor() {
            HttpWebRequest req = WebRequest.Create("http://api.edipost.no/consignor/default") as HttpWebRequest;
            req.Headers.Add("Authorization", "Basic " + apiKey);

            XmlDocument xml = new XmlDocument();
            xml.Load(req.GetResponse().GetResponseStream());

            String name = xml.SelectSingleNode("//consignor/companyName").InnerText;
            String address = xml.SelectSingleNode("//consignor/postAddress/address").InnerText;
            String zipCode = xml.SelectSingleNode("//consignor/postAddress/zipCode").InnerText;
            String city = xml.SelectSingleNode("//consignor/postAddress/city").InnerText;

            Console.WriteLine("Sende: " + name + ", " + address + ", " + zipCode + " " + city);
            Console.WriteLine();
        }


        private int CreateConsignee() {
            HttpWebRequest req = WebRequest.Create("http://api.edipost.no/consignee") as HttpWebRequest;
            req.Headers.Add("Authorization", "Basic " + apiKey);
            req.Accept = "application/vnd.edipost.party+xml";
            req.ContentType = "application/vnd.edipost.party+xml";
            req.Method = "POST";
            req.KeepAlive = true;

            String data = @"
                <consignee>

                  <companyName>Normenn AS</companyName>
                  <customerNumber>5555</customerNumber>

                  <postAddress>
                    <address>Postboks 123</address>
                    <address2></address2>
                    <zipCode>2805</zipCode>
                    <city>Gjøvik</city>
                  </postAddress>

                  <streetAddress>
                    <address>Strandgaten 321</address>
                    <address2></address2>
                    <zipCode>2815</zipCode>
                    <city>Gjøvik</city>
                  </streetAddress>

                  <country>NO</country>

                  <contact>
                    <name>Ola Normann</name>
                    <email>ola@normann.no</email>
                    <telephone>611 12 345</telephone>
                    <cellphone>987 65 432</cellphone>
                    <telefax>611 12 346</telefax>
                  </contact>

                </consignee>
            ";


            byte[] buffer = Encoding.UTF8.GetBytes(data);
            req.ContentLength = buffer.Length;

            Stream postData = req.GetRequestStream();
            postData.Write(buffer, 0, buffer.Length);
            postData.Close();

            XmlDocument xml = new XmlDocument();
            xml.Load( req.GetResponse().GetResponseStream() );

            String id = xml.SelectSingleNode("//consignee/@id").InnerText;
            Console.WriteLine("Receiver id: " + id);
            Console.WriteLine();

            return Int32.Parse(id);
        }


        private void GetProducts( int id ) {
            HttpWebRequest req = WebRequest.Create("http://api.edipost.no/consignee/" + id + "/products") as HttpWebRequest;
            req.Headers.Add("Authorization", "Basic " + apiKey);
            req.Accept = "application/vnd.edipost.collection+xml";
            req.Method = "GET";

            XmlDocument xml = new XmlDocument();
            xml.Load(req.GetResponse().GetResponseStream());
            XmlNodeList entries = xml.SelectNodes("//collection/entry");

            Console.WriteLine("Available products:");

            foreach (XmlNode e in entries) {
                String productId = e.SelectSingleNode("@id").InnerText;
                String productName = e.SelectSingleNode( "@name" ).InnerText;
                String transporter = e.SelectSingleNode( "transporter/@name" ).InnerText;
                Console.WriteLine( " * " + transporter + " - " + productName + " (" + productId + ")" );
            }

            Console.WriteLine();
        }


        private int CreateConsignment( int consigneeId ) {
            HttpWebRequest req = WebRequest.Create("http://api.edipost.no/consignment") as HttpWebRequest;
            req.Headers.Add("Authorization", "Basic " + apiKey);
            req.Accept = "application/vnd.edipost.consignment+xml";
            req.ContentType = "application/vnd.edipost.consignment+xml";
            req.Method = "POST";
            req.KeepAlive = true;

            String data = @"
                <consignment>
                    <consignee id=""${consignmentId}"" />

                    <items>
                        <item>
                            <weight>1.0</weight>
                            <height>0.0</height>
                            <length>0.0</length>
                            <width>0.0</width>
                        </item>
                    </items>

                    <product id=""8"">
                    <services>
                        <service id=""55"">
                        <properties>
                            <property key=""COD_AMOUNT"" value=""1230.00""/>
                            <property key=""COD_REFERENCE"" value=""Dette er en ref""/>
                        </properties>
                        </service>
                    </services>
                    </product>

                    <contentReference>Innholdet er flott</contentReference>
                </consignment>

            ".Replace("${consignmentId}", consigneeId.ToString());


            byte[] buffer = Encoding.UTF8.GetBytes(data);
            req.ContentLength = buffer.Length;

            Stream postData = req.GetRequestStream();
            postData.Write(buffer, 0, buffer.Length);
            postData.Close();


            XmlDocument xml = new XmlDocument();
            xml.Load(req.GetResponse().GetResponseStream());

            String id = xml.SelectSingleNode("//consignment/@id").InnerText;
            String consignmentNumber = xml.SelectSingleNode("//consignment/shipmentNumber").InnerText;

            Console.WriteLine("Consignment id: " + id);
            Console.WriteLine("Consignment number: " + consignmentNumber);
            Console.WriteLine();

            return Int32.Parse(id);
        }


        private void PrintConsignment( int consignmentId ) {
            HttpWebRequest req = WebRequest.Create("http://api.edipost.no/consignment/" + consignmentId + "/print") as HttpWebRequest;
            req.Headers.Add("Authorization", "Basic " + apiKey);
            req.Accept = "application/pdf";
            req.Method = "GET";


            using (MemoryStream ms = new MemoryStream())
            {
                req.GetResponse().GetResponseStream().CopyTo(ms);

                String path = Path.GetTempFileName() + ".pdf";
                Console.WriteLine("Writing PDF label to " + path);
                Console.WriteLine();

                File.WriteAllBytes( path, ms.ToArray() );
                Process.Start(path);
            }
        }


        static void Main(string[] args) {
            new Example();
        }
    }
}

