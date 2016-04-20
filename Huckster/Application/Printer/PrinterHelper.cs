using Domain.Order.Printer;
using Domain.Order.Queries.Models;
using Domain.Order;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Application.Printer
{
    public static class PrinterHelper
    {
        public static string OrderToPrintRequestXML(Order order)
        {
            var printRequest = new PrintRequestInfo();
            printRequest.ePOSPrint = new PrintRequestInfoEPOSPrint();
            printRequest.ePOSPrint.Parameter = new PrintRequestInfoEPOSPrintParameter();
            printRequest.ePOSPrint.Parameter.devid = "local_printer";
            printRequest.ePOSPrint.Parameter.timeout = 10000;
            printRequest.ePOSPrint.PrintData = new PrintRequestInfoEPOSPrintPrintData();

            printRequest.ePOSPrint.PrintData.eposprint = new eposprint();
            printRequest.ePOSPrint.PrintData.eposprint.Items = new List<object>();

            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintSound()
            {
                pattern = "pattern_a",
                repeat = 1
            });

            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { lang = "en" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { smooth = true, smoothSpecified = true });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { align = "center" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { font = "font_b" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { width = 2, height = 2, widthSpecified = true, heightSpecified = true });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { reverse = false, reverseSpecified = true, ul = false, ulSpecified = true, em = true, emSpecified = true, color = "color_1" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { Value = "Huckster Order\n" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintFeed() { unit = 12, unitSpecified = true });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { Value = "\n" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { align = "left" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { font = "font_a" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { width = 1, height = 1, widthSpecified = true, heightSpecified = true });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { reverse = false, reverseSpecified = true, ul = false, ulSpecified = true, em = false, emSpecified = true, color = "color_1" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { Value = $"Order\t{order.Id}\n" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { width = 1, height = 1, widthSpecified = true, heightSpecified = true });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { reverse = false, reverseSpecified = true, ul = false, ulSpecified = true, em = false, emSpecified = true, color = "color_1" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { Value = $"Requested Time\t{order.DeliveryTime:MMM dd yyyy HH:mm}\n" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { Value = "\n" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { width = 1, height = 1, widthSpecified = true, heightSpecified = true });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { reverse = false, reverseSpecified = true, ul = false, ulSpecified = true, em = false, emSpecified = true, color = "color_1" });
            foreach (var item in order.OrderItems)
            {
                printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { Value = $"{item.Name}\n" });
                printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { Value = $"\t${item.Price} x{item.Quantity}" });
                printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { x = 484, xSpecified = true });
                printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { Value = $"${item.Price * item.Quantity}\n\n" });
                printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { Value = "\n" });
            }

            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { reverse = false, reverseSpecified = true, ul = false, ulSpecified = true, em = true, emSpecified = true, color = "color_1" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { width = 2, height = 1, widthSpecified = true, heightSpecified = true });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { Value = "TOTAL" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { x = 264, xSpecified = true });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { Value = $"${order.OrderItems.Sum(_ => _.Price * _.Quantity)}\n\n" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { width = 1, height = 1, widthSpecified = true, heightSpecified = true });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { Value = "Special Intructions:\n\n" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { Value = order.Instructions });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { reverse = false, reverseSpecified = true, ul = false, ulSpecified = true, em = false, emSpecified = true, color = "color_1" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { width = 1, height = 1, widthSpecified = true, heightSpecified = true });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintFeed() { unit = 12, unitSpecified = true });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintText() { align = "center" });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintFeed() { unit = 3, unitSpecified = true });
            printRequest.ePOSPrint.PrintData.eposprint.Items.Add(new eposprintCut() { type = "feed" });

            var orderXML = "";
            XmlSerializer xmlSerializer = new XmlSerializer(printRequest.GetType());

            using (var textWriter = new StringWriterUtf8())
            {
                xmlSerializer.Serialize(textWriter, printRequest);
                orderXML = textWriter.ToString();
            }

            return orderXML;
        }
    }

    public class StringWriterUtf8 : StringWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }
}
