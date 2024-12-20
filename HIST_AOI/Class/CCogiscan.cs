using HIST_AOI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HIST_AOI.Class
{
    class CCogiscan
    {
        private wsCogiscan.RPCServices rPCServices;
        private String strigSendC;
        private String xml = "";
        siixsem_aoi_dbEntities m_dbAOI;
        public CCogiscan()
        {
            rPCServices = new wsCogiscan.RPCServicesClient();
            m_dbAOI = new siixsem_aoi_dbEntities();
        }

        public string StrigSendC { get => strigSendC; set => strigSendC = value; }

        public CModelInfo query_item(String item)
        {
            String parameters = StrigSendC = "<Parameter name=\"itemId\">" + item + "</Parameter>";

            CModelInfo model = new CModelInfo();
            try
            {

                xml = rPCServices.executeCommand("queryItem", "<Parameters>" + parameters + "</Parameters>");

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlElement root = doc.DocumentElement;
                //XmlNodeList camxMessageItem = root.SelectNodes("Item");
                model.Item_type = root.Attributes["itemType"].Value;
                model.DjNumber = "true";
                XmlNodeList camxMessageInfo = root.SelectNodes("Product");
                foreach (XmlNode messageInfo in camxMessageInfo)
                {
                    model.BatchId = messageInfo.Attributes["batchId"].Value;
                    model.Name = messageInfo.Attributes["partNumber"].Value;
                    model.Revision = messageInfo.Attributes["revision"].Value;
                    model.Status = messageInfo.Attributes["status"].Value;
                    model.Operation = messageInfo.Attributes["operation"].Value;
                    model.DefDisposition = messageInfo.Attributes["defectDisposition"].Value;
                    model.Route = messageInfo.Attributes["releaseRouteName"].Value;
                    model.Part_number = messageInfo.Attributes["partNumber"].Value;
                    
                    //model.Item_type = messageInfo.Attributes["itemType"].Value;
                }
            }
            catch (Exception ex)
            {
                model.BatchId = ex.Message;
                model.DjNumber = "false";
            }

            return model;
        }
        public List<String> getContents(String item, ref String result)
        {
            String parameters = StrigSendC = "<Parameter name=\"containerId\">" + item + "</Parameter>";

            List<String> serials = new List<string>();

            try
            {

                xml = rPCServices.executeCommand("getContents", "<Parameters>" + parameters + "</Parameters>");

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlElement root = doc.DocumentElement;

                XmlNodeList camxContents = root.ChildNodes;
                foreach (XmlNode itemPallet in camxContents)
                {
                    serials.Add(itemPallet.Attributes["itemId"].Value);
                }
            }
            catch (Exception ex)
            {
               result = ex.Message;
            }

            return serials;
        }

        public String updateQuarantineRule(String name)
        {
            //res = "<Parameters>
            //<Parameter name=\"title\">Carrier" + name + "</Parameter>
            //<Parameter name=\"description\">test" + name + "</Parameter>
            //<Parameter name=\"itemTypeClass\">Product Carrier</Parameter>
            //<Parameter name=\"itemId\">" + name + "</Parameter>
            //<Parameter name=\"enabled\">true</Parameter>
            //</Parameters>";
            String parameters = StrigSendC = "<Parameter name=\"title\">Carrier" + name + "</Parameter>" +
                                             "<Parameter name=\"description\">test" + name + "</Parameter>" +
                                             "<Parameter name=\"itemTypeClass\">Product Carrier</Parameter>" +
                                             "<Parameter name=\"itemId\">" + name + "</Parameter>" +
                                             "<Parameter name=\"enabled\">true</Parameter>";

            try
            {
                xml = rPCServices.executeCommand("updateQuarantineRule", "<Parameters>" + parameters + "</Parameters>");
            }
            catch (Exception ex)
            {
                xml = ex.Message;
            }

            return xml;
        }

        public String unloadItem(String name)
        {

            String parameters = StrigSendC = "<Parameter name=\"contentId\">" + name + "</Parameter>";

            try
            {
                xml = rPCServices.executeCommand("unload", "<Parameters>" + parameters + "</Parameters>");
            }
            catch (Exception ex)
            {
                xml = ex.Message;
            }

            return xml;
        }
        public String get_panel_ID(String item)
        {
            String parameters = StrigSendC = "<Parameter name=\"barcode\">" + item + "</Parameter>";
            String panelID = "NOT_FOUND";

            CModelInfo model = new CModelInfo();
            try
            {

                xml = rPCServices.executeCommand("queryProductGroup", "<Parameters>" + parameters + "</Parameters>");

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlElement root = doc.DocumentElement;

                panelID = root.Attributes["barcode"].Value;
               // panelID = camxMessageInfo.Attributes["barcode"].Value;

            }
            catch (Exception ex)
            {
                panelID = "NOT_FOUND";
            }

            return panelID;
        }
        public List<String> get_panel(String item)
        {
            String parameters = StrigSendC = "<Parameter name=\"barcode\">" + item + "</Parameter>";
            String panelID = "NOT_FOUND";
            List<String> serials = new List<String>();
            
            CModelInfo model = new CModelInfo();
            try
            {
                xml = rPCServices.executeCommand("queryProductGroup", "<Parameters>" + parameters + "</Parameters>");

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlElement root = doc.DocumentElement;

                panelID = root.Attributes["barcode"].Value;
                XmlNodeList nodes = root.SelectNodes("Product");
                serials.Add(panelID);

                foreach (XmlNode node in nodes)
                {
                    serials.Add(node.InnerText);
                }

            }
            catch (Exception ex)
            {
                panelID = "NOT_FOUND";
            }

            return serials;
        }
        public List<String> get_panel(String item, bool side)
        {
            String cont = "";
            String parameters = StrigSendC = "<Parameter name=\"barcode\">" + item + "</Parameter>";
            String panelID = "NOT_FOUND";
            List<String> serials = new List<String>();

            if (side == true) cont = "C";
            else cont = "S";

            CModelInfo model = new CModelInfo();
            try
            {
                xml = rPCServices.executeCommand("queryProductGroup", "<Parameters>" + parameters + "</Parameters>");

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlElement root = doc.DocumentElement;

                XmlNodeList nodes = root.SelectNodes("Product");

                foreach (XmlNode node in nodes)
                {
                    if (node.InnerText.Contains(cont))
                        serials.Add(node.InnerText);
                }

            }
            catch (Exception ex)
            {
                panelID = "NOT_FOUND";
            }

            return serials;
        }
        public String startOperation(String serial, String toolId, String operationName)
        {
            xml = rPCServices.executeCommand("startOperation", "<Parameters>"
                + "<Parameter name=\"productId\">" + serial + "</Parameter>"
                + "<Parameter name=\"operationName\">" + operationName + "</Parameter>"
                + "<Parameter name=\"toolId\">" + toolId + "</Parameter>"
                + "</Parameters>");

            return xml;
        }
        public String endOperation(String serial, String tool, String operationName)
        {

            //"endOperation","<Parameters><Parameter name=\"productId\">" + serial[x] + "</Parameter><Parameter name=\"operationName\">" + osmt + "</Parameter></Parameters>"
            xml = rPCServices.executeCommand("endOperation", "<Parameters>"
                + "<Parameter name=\"productId\">" + serial + "</Parameter>"
                + "<Parameter name=\"operationName\">" + operationName + "</Parameter>"
                + "</Parameters>");
            return xml;
        }

        //executeCommand(setProductDefectDisposition,
        //<Parameters>
        //<Parameter name = "overwrite" > false </ Parameter >
        //< Extensions >
        //< ProductDisposition
        //productId= "SN11111"
        //disposition= "OK" >
        //</ ProductDisposition >
        //</ Extensions >
        //</ Parameters >

        public String setProcessStepStatus(String serial, String line, bool result)
        {
            string temp = "";
            if (result == true)
            {
                xml = rPCServices.executeCommand("setProcessStepStatus", "<Parameters>"
                    + "<Extensions><ProcessStepStatus itemInstanceId=\"" + serial + "\" processStepId=\"" + line + "\" status=\"" + "PASSED" + "\" /></Extensions>"
                    + "</Parameters>");

            }

            else
            {
                if (serial.Contains("AA000"))
                {
                    List<String> serialsPanel = get_panel(serial);
                    //foreach(String s in serialsPanel)
                    //{
                    //temp += "<ProductImageDisposition productId=\"" + s + "\" disposition=" + "\"NON-REWORKABLE\"" + "></ProductImageDisposition>";
                    xml = rPCServices.executeCommand("setProcessStepStatus", "<Parameters>"
                    + "<Extensions>" +
                    "<ProcessStepStatus productDisposition=\"N\" imageId=\"1\" itemInstanceId=\"" + serial + "\" processStepId=\"" + line + "\" status=\"" + "FAILED" + "\" >"
                    + "<Indictment  indictmentId = \"" + serial + "\" description = \"Test " + serial + " failed.\"/>"
                    + "</ProcessStepStatus>" +
                    "</Extensions>"
                    + "</Parameters>");
                    xml = rPCServices.executeCommand("setProcessStepStatus", "<Parameters>"
                    + "<Extensions>" +
                    "<ProcessStepStatus productDisposition=\"N\" imageId=\"2\" itemInstanceId=\"" + serial + "\" processStepId=\"" + line + "\" status=\"" + "FAILED" + "\" >"
                    + "<Indictment  indictmentId = \"" + serial + "\" description = \"Test " + serial + " failed.\"/>"
                    + "</ProcessStepStatus>" +
                    "</Extensions>"
                    + "</Parameters>");

                    //}
                    //xml = rPCServices.executeCommand("setProductDefectDisposition", 
                    //"<Parameters>"
                    //+ "<Parameter name = \"overwrite\" >true</Parameter > "
                    //+ "<Extensions>"
                    //+ "<ProductDisposition productId=\"" + serial + "\" disposition=" + "\"NON-REWORKABLE\"" + "></ProductDisposition>"
                    //+ "</Extensions>"
                    //+ "</Parameters>");

                }
                else
                    xml = rPCServices.executeCommand("setProcessStepStatus", "<Parameters>"
                        + "<Extensions><ProcessStepStatus proctDisposition=\"N\" itemInstanceId=\"" + serial + "\" processStepId=\"" + line + "\" status=\"" + "FAILED" + "\" >"
                        + "<Indictment  indictmentId = \"" + serial + "\" description = \"Test " + serial + " failed.\"/>"
                        + "</ProcessStepStatus></Extensions>"
                        + "</Parameters>");
            }
           
            return xml;
            
        }

        public String setDispostion(String serial, bool result)
        {
            

            List<String> serialsPanel = get_panel(serial);

            if (result == true)
            {
                xml = rPCServices.executeCommand("setProductDefectDisposition",
                "<Parameters>"
                + "<Parameter name = \"overwrite\" >true</Parameter > "
                + "<Extensions>"
                + "<ProductDisposition productId=\"" + serial + "\">"
                + "<ProductImageDisposition imageId=\"1\" disposition=" + "\"OK\"" + "></ProductImageDisposition>"
                + "</ProductDisposition>"
                + "</Extensions>"
                + "</Parameters>");
                xml = rPCServices.executeCommand("setProductDefectDisposition",
                "<Parameters>"
                + "<Parameter name = \"overwrite\" >true</Parameter > "
                + "<Extensions>"
                + "<ProductDisposition productId=\"" + serial + "\">"
                + "<ProductImageDisposition imageId=\"2\" disposition=" + "\"OK\"" + "></ProductImageDisposition>"
                + "</ProductDisposition>"
                + "</Extensions>"
                + "</Parameters>");
            }
            //else
            //{
            //    xml = rPCServices.executeCommand("setProductDefectDisposition",
            //    "<Parameters>"
            //    + "<Parameter name = \"overwrite\" >true</Parameter > "
            //    + "<Extensions>"
            //    + "<ProductDisposition productId=\"" + serial + "\">"
            //    + "<ProductImageDisposition imageId=\"1\" disposition=" + "\"NON-REWORKABLE\"" + "></ProductImageDisposition>"
            //    + "</ProductDisposition>"
            //    + "</Extensions>"
            //    + "</Parameters>");
            //    xml = rPCServices.executeCommand("setProductDefectDisposition",
            //    "<Parameters>"
            //    + "<Parameter name = \"overwrite\" >true</Parameter > "
            //    + "<Extensions>"
            //    + "<ProductDisposition productId=\"" + serial + "\">"
            //    + "<ProductImageDisposition imageId=\"2\" disposition=" + "\"NON-REWORKABLE\"" + "></ProductImageDisposition>"
            //    + "</ProductDisposition>"
            //    + "</Extensions>"
            //    + "</Parameters>");
            //}

            return xml;

        }


        public void setAOIInspection(String serial, String toolID, bool result, ref String message, String STATUS)
        {
            String resultPost = ""; // startOperation(serial, toolID, "AOI Inspection");

            if (STATUS.Contains("WAITING")|| STATUS.Contains("ESPERANDO"))
                resultPost = startOperation(serial, toolID, "AOI Inspection");


            if (resultPost.Contains("Success") || STATUS.Contains("ACTIVE"))
            {
                resultPost = setProcessStepStatus(serial, "AOI Inspection", result);
                if (resultPost.Contains("Success"))
                {
                    resultPost = endOperation(serial, toolID, "AOI Inspection");
                    if (resultPost.Contains("Success"))
                    {
                        message = "[ " + DateTime.Now.ToString() + " ] " + "Se agrego historial de AOI inspection a COGISCAN" + resultPost;
                        m_dbAOI.insertResult(serial, "SUCCESS", toolID, message);
                    }
                    else
                    {
                        message = "[ " + DateTime.Now.ToString() + " ] " + "No se pudo dar el historial AOI en COGISCAN error en endOperation: " + resultPost;
                        m_dbAOI.insertResult(serial, "FAILED", toolID, message);
                    }
                }
                else
                {
                    message = "[ " + DateTime.Now.ToString() + " ] " + "No se pudo dar el historial AOI en COGISCAN error en setProcessStepStatus: " + resultPost;
                    m_dbAOI.insertResult(serial, "FAILED", toolID, message);
                }
            }
            else
            {
                message = "[ " + DateTime.Now.ToString() + " ] " + "No se pudo dar el historial AOI en COGISCAN error en startOperation: " + resultPost;
                m_dbAOI.insertResult(serial, "FAILED", toolID, message);
            }
        }

        public void setXRAY(String serial, String toolID, bool result, ref String message, String STATUS)
        {
            String resultPost = ""; // startOperation(serial, toolID, "AOI Inspection");

            if (STATUS.Contains("WAITING"))
                resultPost = startOperation(serial, "", "X-RAY");


            if (resultPost.Contains("Success") || STATUS.Contains("ACTIVE"))
            {
                resultPost = setProcessStepStatus(serial, "X-RAY", result);
                if (resultPost.Contains("Success"))
                {
                    resultPost = endOperation(serial, "", "X-RAY");
                    if (resultPost.Contains("Success"))
                    {
                        message = "[ " + DateTime.Now.ToString() + " ] " + "Se agrego historial de X-RAY a COGISCAN" + resultPost;
                        //m_dbAOI.insertResult(serial, "SUCCESS", toolID, message);
                    }
                    else
                    {
                        message ="[ " + DateTime.Now.ToString() + " ] " + "No se pudo dar el historial X-RAY en COGISCAN error en endOperation: " + resultPost;
                        ///m_dbAOI.insertResult(serial, "FAILED", toolID, message);
                    }
                }
                else
                {
                    message = "[ " + DateTime.Now.ToString() + " ] " + "No se pudo dar el historial X-RAY en COGISCAN error en setProcessStepStatus: " + resultPost;
                    //m_dbAOI.insertResult(serial, "FAILED", toolID, message);
                }
            }
            else
            {
                message = "[ " + DateTime.Now.ToString() + " ] " + "No se pudo dar el historial X-RAY en COGISCAN error en startOperation: " + resultPost;
                //m_dbAOI.insertResult(serial, "FAILED", toolID, message);
            }
        }

        public void setConformalInspection(String serial, bool result, ref String message)
        {
            String resultPost = startOperation(serial, "", "Conformal Inspection");
            if (resultPost.Contains("Success"))
            {
                resultPost = setProcessStepStatus(serial, "Conformal Inspection", result);
                if (resultPost.Contains("Success"))
                {
                    resultPost = endOperation(serial, "", "Conformal Inspection");
                    if (resultPost.Contains("Success"))
                        message = "Se agrego historial de Conformal inspection a COGISCAN" + resultPost;
                    else
                        message = "No se pudo dar el historial Conformal en COGISCAN error en endOperation: " + resultPost;
                }
                else
                    message = "No se pudo dar el historial Conformal en COGISCAN error en setProcessStepStatus: " + resultPost;
            }
            else
                message = "No se pudo dar el historial Conformal en COGISCAN error en startOperation: " + resultPost;
        }
    }
}
