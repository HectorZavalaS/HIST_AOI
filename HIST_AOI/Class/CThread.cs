using HIST_AOI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
namespace HIST_AOI.Class
{
    class CThread
    {
        private BackgroundWorker _bw;
        private BackgroundWorker _bwMessages;
        private BackgroundWorker _bwAOIWaiting;
        private BackgroundWorker _bwAOIActive;
        private BackgroundWorker _bwAOIAllWaiting;
        private BackgroundWorker _bwAOIAllActive;
        private wsCAMX.CAMXMessageBrokerClient m_CAMXEvents;
        private String m_token_id;
        private String m_user;
        private string m_password;
        private RichTextBox m_console;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        COracle m_oracle;
        CCAMX m_camx;
        CModelInfo m_modelInfo;
        private CCogiscan m_cogiscan;
        private String m_name;
        private ListView m_listLines;
        private List<String> m_messages;
        String m_server;
        String m_SID;
        private String m_dj;
        int m_error = 0;
        String m_StringError = "";
        private String m_messageInfo;
        wsCAMX.acknowledgeEventsResponse m_res;
        CCogiscanCGSDW m_db2;
        CCogiscanCGS m_db2_serv2;
        siixsem_mits_projectEntities m_dbMit;
        List<String> serials;
        List<String> serialsPanel;
        String batchID;
        String pallet;
        String productPN;
        Color m_color;
        CMySQL m_mySql;
        public string Dj { get => m_dj; set => m_dj = value; }
        public string Name { get => m_name; set => m_name = value; }
        public string BatchID { get => batchID; set => batchID = value; }

        public CThread(ref RichTextBox console, String NameLine, ref ListView lines, String server, String SID)
        {

            m_CAMXEvents = new wsCAMX.CAMXMessageBrokerClient();
            m_cogiscan = new CCogiscan();
            m_messages = new List<string>();
            m_user = "AOI_LINES";//NameLine.Replace(" ", "_");
            m_password = "AOI_LINES";//NameLine.Replace(" ", "_"); ;
            m_console = console;
            Name = NameLine;
            m_camx = new CCAMX(m_user, m_password, Name);
            m_server = server;
            m_SID = SID;
            m_oracle = new COracle(server, SID);
            m_db2 = new CCogiscanCGSDW();
            m_db2_serv2 = new CCogiscanCGS();
            m_modelInfo = new CModelInfo();
            m_dbMit = new siixsem_mits_projectEntities();
            serials = new List<string>();
            serialsPanel = new List<string>();
            batchID = "";
            m_mySql = new CMySQL();
            m_listLines = lines;
            Dj = "";
            KnownColor randomColorName;
            do {

                Random randomGen = new Random();
                KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
                randomColorName = names[randomGen.Next(names.Length)];

            } while (randomColorName == KnownColor.Black);
            m_color = Color.FromKnownColor(randomColorName == KnownColor.Black ? KnownColor.White : randomColorName); 

            initThreads();
            initThreadsAOI();
        }

        delegate void addLineDelegate(string texto);
        private void addLine(string texto)
        {
            if (m_console.InvokeRequired)
            {
                addLineDelegate delegado = new addLineDelegate(addLine);
                object[] parametros = new object[] { texto };
                m_console.Invoke(delegado, parametros);
            }
            else
            {
                try
                {
                    if (m_console.TextLength > 100000)
                    {
                        m_console.Clear();
                    }
                    if (!String.IsNullOrEmpty(texto) && !String.IsNullOrWhiteSpace(texto))
                    {
                        m_console.SelectionColor = m_color;
                        m_console.AppendText(texto + "\n");
                    }
                    
                }
                catch (Exception ex) { }
            }
        }
        #region ThreadMethods
        private void initThreads()
        {
            try
            {
                _bw = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };
                _bw.DoWork += bw_MonitorSMT;
                _bw.ProgressChanged += bw_ProgressChanged;
                _bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[initThreads] Error...");
            }

        }

        private void initThreadsAOI()
        {
            try
            {
                _bwAOIWaiting = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };
                _bwAOIWaiting.DoWork += checkAOIWaiting;
                //_bwAOIWaiting.ProgressChanged += bw_ProgressChanged;
                _bwAOIWaiting.RunWorkerCompleted += bw_RunWorkerCompleted;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[initThreadsAOIWaiting] Error...");
            }
            try
            {
                _bwAOIActive = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };
                _bwAOIActive.DoWork += checkAOIActive;
                //_bwAOIActive.DoWork += endOperation_AOI;
                //_bwAOIWaiting.ProgressChanged += bw_ProgressChanged;
                _bwAOIActive.RunWorkerCompleted += bw_RunWorkerCompleted;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[initThreadsAOIActive] Error...");
            }
            ////////////////////////////////////////////////////////////////////////////
            ///
            ////////////////////////////////////////////////////////////////////////////

            try
            {
                _bwAOIAllWaiting = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };
                _bwAOIAllWaiting.DoWork += checkAllAOIWaiting;
                //_bwAOIWaiting.ProgressChanged += bw_ProgressChanged;
                _bwAOIAllWaiting.RunWorkerCompleted += bw_RunWorkerCompleted;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[initThreadsAOIAllWaiting] Error...");
            }
            try
            {
                _bwAOIAllActive = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };
                _bwAOIAllActive.DoWork += checkAllAOIActive;
                //_bwAOIAllActive.DoWork += endOperation_AOI;
                //_bwAOIWaiting.ProgressChanged += bw_ProgressChanged;
                _bwAOIAllActive.RunWorkerCompleted += bw_RunWorkerCompleted;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[initThreadsAOIActive] Error...");
            }

        }

        /*******************************************************************************************************************/
        /*******************************************************************************************************************/

        public void StartAOIWait()
        {
            try
            {
                this._bwAOIWaiting.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[Start] Error...");
            }
        }

        public void StopAOIWait()
        {
            try
            {
                if (_bwAOIWaiting.IsBusy) _bwAOIWaiting.CancelAsync();
                //m_oracle.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[Stop] Error...");
            }
        }

        /*******************************************************************************************************************/
        /*******************************************************************************************************************/

        public void StartAOIActive()
        {
            try
            {
                this._bwAOIActive.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[Start] Error...");
            }
        }

        public void StopAOIActive()
        {
            try
            {
                if (_bwAOIActive.IsBusy) _bwAOIActive.CancelAsync();
                //m_oracle.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[Stop] Error...");
            }
        }

        /*******************************************************************************************************************/
        /*******************************************************************************************************************/

        public void StartAllAOIWait()
        {
            try
            {
                this._bwAOIAllWaiting.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[Start] Error...");
            }
        }

        public void StopAllAOIWait()
        {
            try
            {
                if (_bwAOIAllWaiting.IsBusy) _bwAOIAllWaiting.CancelAsync();
                //m_oracle.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[Stop] Error...");
            }
        }
        /*******************************************************************************************************************/
        /*******************************************************************************************************************/

        public void StartALLAOIActive()
        {
            try
            {
                this._bwAOIAllActive.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[Start] Error...");
            }
        }

        public void StopAllAOIActive()
        {
            try
            {
                if (_bwAOIAllActive.IsBusy) _bwAOIAllActive.CancelAsync();
                //m_oracle.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[Stop] Error...");
            }
        }
        /*******************************************************************************************************************/
        /*******************************************************************************************************************/

        public void Start()
        {
            try
            {
                this._bw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[Start] Error...");
            }
        }

        public void Stop()
        {
            try
            {
                if (_bw.IsBusy) _bw.CancelAsync();
                m_oracle.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[Stop] Error...");
            }
        }

        void newAttnMessage(String Msg)
        {
            try
            {
                _bwMessages = new BackgroundWorker
                {
                    WorkerSupportsCancellation = true
                };
                _bwMessages.DoWork += attendMsg;
                _bwMessages.RunWorkerAsync(Msg);
            }
            catch(Exception ex)
            {
                logger.Error(ex, "[newAttnMessage] Error...");
            }
        }

        static void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                    Console.WriteLine("You canceled!");
                else if (e.Error != null)
                    Console.WriteLine("Worker exception: " + e.Error.ToString());
                else
                    Console.WriteLine("Complete: " + e.Result);      // from DoWork
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[bw_RunWorkerCompleted] Error...");
            }
        }

        static void bw_ProgressChanged(object sender,
                                        ProgressChangedEventArgs e)
        {
            Console.WriteLine("Reached " + e.ProgressPercentage + "%");
        }

        /*******************************************************************************************************************/
        /*******************************************************************************************************************/
        /******************************** HILO PRINCIPAL DE MONITEREO DE LINEAS DE SMT**************************************/
        /*******************************************************************************************************************/
        /*******************************************************************************************************************/

        async void bw_MonitorSMT(object sender, DoWorkEventArgs e)
        {

            XmlDocument doc = new XmlDocument();
            CModelInfo model = new CModelInfo();
            XmlNodeList camxEventList;
            XmlNode camxMessageInfo;
            XmlNode productInfo;
            XmlNode SERIALPANEL;
            XmlNode SERIALPALLET;
            String strXML = "";
            string line = "";
            String serial = "";
            int i = 0;
            int resultTest = 0;

            wsCAMX.pullEventsResponse result;
            m_camx.login();
            m_camx.suscribeIWComplete();
            m_db2.connect();
            m_db2.close();
            while (true)
            {
                if (_bw.CancellationPending) { e.Cancel = true; return; }
                try
                {
                    result = await m_camx.pullEvents();
                    strXML = result.Body.@return;
                    doc.LoadXml(strXML);
                    camxEventList = doc.SelectNodes("CAMXEventList/CAMXEvent");

                    foreach (XmlNode camxEvent in camxEventList)
                    {

                        /*--------------------------------------------------------------------------------------------*/
                        /*------------------------OBTIENE LINEA Y ATIENDE MENSAJE CAMX--------------------------------*/
                        /*--------------------------------------------------------------------------------------------*/

                        camxMessageInfo = camxEvent.SelectSingleNode("MessageInfo");
                        if (camxMessageInfo != null)
                        {
                            line = camxMessageInfo.Attributes["sender"].Value;
                            m_messageInfo = camxMessageInfo.Attributes["messageId"].Value;
                            newAttnMessage(m_messageInfo);
                        }

                        /*--------------------------------------------------------------------------------------------*/
                        /*-------------------------------OBTIENE NUMERO DE GRUPO--------------------------------------*/
                        /*--------------------------------------------------------------------------------------------*/

                        productInfo = camxEvent.SelectSingleNode("ItemWorkComplete/Extensions/RecipeInformation");
                        if(productInfo != null)
                        {
                            batchID = productInfo.Attributes["batchId"].Value;
                            productPN = productInfo.Attributes["productPN"].Value;

                        }

                        /*--------------------------------------------------------------------------------------------*/
                        /*--------------------------- OBTIENE SERIAL DE PANEL   --------------------------------------*/
                        /*--------------------------------------------------------------------------------------------*/

                        SERIALPANEL = camxEvent.SelectSingleNode("ItemWorkComplete/Extensions/ProductGroupInformation");

                        if (SERIALPANEL != null)
                        {
                            if (!productPN.Contains("H61P-LB") || !productPN.Contains("H61P-HB") || !productPN.Contains("NAL80B-CHL-BG-FPC-TRIAL") || !productPN.Contains("T20A-C")
                              ||!productPN.Contains("P758-A") || !productPN.Contains("P758-B") || !productPN.Contains("P758-CHL-B-J700S6-TRIAL") || !productPN.Contains("3W0A-C-TRIAL")
                              || !productPN.Contains("790B-C-TRIAL") || !productPN.Contains("P758-C") || !productPN.Contains("790B-D-TRIAL"))
                            {
                                serial = SERIALPANEL.Attributes["groupId"].Value;
                            }
                            else
                            {
                                SERIALPALLET = camxEvent.SelectSingleNode("ItemWorkComplete");
                                if (SERIALPALLET != null)
                                    serial = SERIALPALLET.Attributes["itemInstanceId"].Value;

                            }
                            if (!serials.Contains(serial))
                            {
                                addLine("[" + DateTime.Now.ToString() + "] " + "Checking serial: " + serial + " ..... " + Name);
                                if(serial.Contains("ITEM")||serial.Contains("H61")||serial.Contains("PA"))
                                    checkAOIPallet(serial, ref resultTest);
                                else
                                    checkAOI(serial);
                                serials.Add(serial);
                            }
                        }
                        else
                        {
                            SERIALPALLET = camxEvent.SelectSingleNode("ItemWorkComplete");
                            if (SERIALPALLET != null)
                            {
                                serial = SERIALPALLET.Attributes["itemInstanceId"].Value;
                                m_modelInfo = m_cogiscan.query_item(serial);
                                if (m_modelInfo.Item_type.Contains("PALLET-SMT"))
                                {
                                    addLine("[" + DateTime.Now.ToString() + "] " + "Serial pallet: " + serial + " ..... " + Name);
                                    checkAOIPallet(serial, ref resultTest);
                                }
                            }
                        }

                        /*--------------------------------------------------------------------------------------------*/
                        /*--------------------------------------------------------------------------------------------*/
                        /*--------------------------------------------------------------------------------------------*/

                        camxMessageInfo = null;
                        productInfo = null;
                        SERIALPANEL = null;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "[bw_MonitorSMT] Error...");
                    addLine("[" + DateTime.Now.ToLongDateString() + "]" + "ERROR IN SERIAL " + serial + " - LINE " + line + " - " + ex.Message);
                }
                i++;
            }
        }

        private void endOperationAOI(string serial)
        {
            string message = "";
            int resultTest = 0;
            bool res = false;
            String panelID = "";
            try
            {
                m_modelInfo = m_cogiscan.query_item(serial.ToUpper());
                serialsPanel.Clear();

                // m_modelInfo = m_cogiscan.query_item(serial);
                if ((m_modelInfo.Status.Contains("WAITING") || m_modelInfo.Status.Contains("ACTIVE")) && m_modelInfo.Operation.Contains("AOI"))
                {
 
                    //addLine("[ " + DateTime.Now.ToString() + " ] " + serial + " tiene resultado en Oracle: " + (resultTest == 1 ? "OK" : resultTest == 3 ? "FALSE CALL(OK)" : " ") + "...consultando a Cogiscan si tiene registro AOI");
                    addLine("[" + DateTime.Now.ToString() + "] " + serial + " Result End operation:" +m_cogiscan.endOperation(serial, Name, "AOI Inspection"));
                }
                else
                {
                    addLine("[" + DateTime.Now.ToString() + "] " + "La pieza: " + serial + " esta en STATUS: " + m_modelInfo.Status + " " + m_modelInfo.Operation + " no se le puede dar historial de AOI");
                }

            }
            catch (Exception ex)
            {

            }
        }

        private int checkAOIPallet(string pallet, ref int resultTest)
        {
            siixsem_aoi_dbEntities m_dbAOI = new siixsem_aoi_dbEntities();
            string message = "";
            resultTest = 0;
            String result = "";
            bool res = false;
            String panelID = "";
            Pallets_smt container = null;
            int result_tr = 0;
            try
            {
                if (pallet.Contains("ITEM"))
                    m_mySql.getPalletByItem(pallet, ref container,ref message);
                else
                    m_mySql.getPalletByName(pallet, ref container, ref message);
                if(container != null)
                {
                    if(queryAOI("ITEM" + container.Item, ref resultTest, ref panelID))
                    {
                            insertPallet_Result resP = m_dbAOI.insertPallet("ITEM" + container.Item, (resultTest == 1 ? "OK " : "FALSE CALL ")).First();
                            List<String> serials = m_cogiscan.getContents(pallet, ref message);

                            if (serials.Count == 0) return 2;  // el serial ya no esta en ese pallet
                            if (resultTest == 1 || resultTest == 3) /// OK y FALSE CALL
                            {
                                foreach (String serial in serials)
                                {
                                    result = m_cogiscan.setProcessStepStatus(serial, "AOI Inspection", true);
                                   //m_dbAOI.insertResult(serial, "SUCCESS", Name, result);
                                    m_dbAOI.insertSerial(resP.IDPALLET, serial);
                                    addLine("[" + DateTime.Now.ToString() + "] " + serial + " ProcessStepStatus " + (resultTest == 1 ? "OK " : "FALSE CALL ") + result);
                                }
                            }
                            else
                            {
                                foreach (String serial in serials)
                                {
                                    result = m_cogiscan.setProcessStepStatus(serial, "AOI Inspection", false);
                                    m_dbAOI.insertResult(serial, "FAIL", Name, result);
                                    m_dbAOI.insertSerial(resP.IDPALLET, serial);
                                    addLine("[" + DateTime.Now.ToString() + "] " + serial + " ProcessStepStatus " + "NG " + result);
                                }
                            }
                            result = m_cogiscan.endOperation(pallet, Name, "AOI Inspection");
                            addLine("[" + DateTime.Now.ToString() + "] " + pallet + " endOperation " + resultTest);
                            if (result.Contains("Success"))
                            {
                                m_cogiscan.unloadItem(pallet);
                                m_mySql.updateCyclePallet(container.Item.Replace("ITEM", ""), ref message);
                                if (container.Ciclos + 1 == container.Limite)
                                {
                                    m_cogiscan.updateQuarantineRule(container.Name);
                                    m_mySql.updateStatusPallet(container.Name, "I", ref message);
                                }
                                //m_dbAOI.insertResult(container.Name, "UPDATE_PALLET", Name, "Prev QTY: " + container.Ciclos.ToString() + " New QTY: " + (container.Ciclos + 1).ToString());
                                result_tr = 1;
                            }
                    }
                }

            }
            catch (Exception ex)
            {

            }
             return result_tr;
        }



        private void checkAOI(string serial)
        {
            string message = "";
            int resultTest = 0;
            bool res = false;
            String panelID = "";
            try
            {
                m_modelInfo = m_cogiscan.query_item(serial.ToUpper());
                serialsPanel.Clear();
                res = queryAOI(serial, ref resultTest, ref panelID);
                if ((m_modelInfo.Status.Contains("WAITING") || m_modelInfo.Status.Contains("ACTIVE")) && m_modelInfo.Operation.Contains("AOI"))
                {
                    
                    if (res)
                    {

                        if (resultTest == 1 || resultTest == 3) /// OK y FALSE CALL
                        {
                            //addLine("[" + DateTime.Now.ToString() + "] " + serial + " tiene resultado en Oracle: " + (resultTest == 1 ? "OK" : resultTest == 3 ? "FALSE CALL(OK)" : " ") + "...consultando a Cogiscan si tiene registro AOI");
                            //m_cogiscan.setAOIInspection(serial.ToUpper(), Name, true, ref message, m_modelInfo.Status);
                            //if (m_modelInfo.Part_number.Contains("N925L"))
                            //    if (m_modelInfo.Part_number.Contains("33611") || m_modelInfo.Part_number.Contains("33610"))
                            //    {
                            //        List<String> seriales = m_cogiscan.get_panel(serial, serial.Contains("C") == true ? true : false);
                            //        foreach(String s in seriales)
                            //            updateMit(s, resultTest == 1 ? 1 : 21, m_modelInfo.Status);
                            //    }
                            //    else
                            //        updateMit(serial, resultTest == 1 ? 1 : 21, m_modelInfo.Status);
                        }
                        else
                        {
                            if (resultTest == 2) // NG
                            {
                                addLine("[" + DateTime.Now.ToString() + "] " + "El resultado de la prueba del serial: " + serial + " es NG se dara historial de AOI en NG.");
                                m_cogiscan.setAOIInspection(serial.ToUpper(), Name, false, ref message, m_modelInfo.Status);
                                if (m_modelInfo.Part_number.Contains("N925L"))
                                    if (m_modelInfo.Part_number.Contains("33611") || m_modelInfo.Part_number.Contains("33610"))
                                    {
                                        List<String> seriales = m_cogiscan.get_panel(serial, serial.Contains("C") == true ? true : false);
                                        foreach (String s in seriales)
                                            updateMit(s, 99, m_modelInfo.Status);
                                    }
                                    else
                                        updateMit(serial, 99, m_modelInfo.Status);
                            }
                            else  // NOT JUDGED
                            {
                                addLine("[" + DateTime.Now.ToString() + "] " + "El serial: " + serial + " no fue juzgado no se dara historial de AOI.");
                            }

                        }
                    }

                    addLine(message);
                }
                else
                {
                    addLine("[" + DateTime.Now.ToString() + "] " + "La pieza: " + serial + " esta en STATUS: " + m_modelInfo.Status + " " + m_modelInfo.Operation + " no se le puede dar historial de AOI");
                    if (m_modelInfo.Part_number.Contains("N925L")) ///////// CHECK AOI & SPI FOR MITSUBISHI
                    {
                        //res = queryAOI(serial, ref resultTest, ref panelID);   
                        if (res)
                        {
                            if (resultTest == 1 || resultTest == 3) /// OK y FALSE CALL
                            {
                                if (m_modelInfo.Part_number.Contains("33611") || m_modelInfo.Part_number.Contains("33610"))
                                {
                                    List<String> seriales = m_cogiscan.get_panel(serial, serial.Contains("C") == true ? true : false);
                                    foreach (String s in seriales)
                                        updateMit(s, resultTest == 1 ? 1 : 21, m_modelInfo.Status);
                                }
                                else
                                    updateMit(serial, resultTest == 1 ? 1 : 21, m_modelInfo.Status);
                            }
                            else if (resultTest == 2) // NG
                            {
                                if (m_modelInfo.Part_number.Contains("33611") || m_modelInfo.Part_number.Contains("33610"))
                                {
                                    List<String> seriales = m_cogiscan.get_panel(serial, serial.Contains("C") == true ? true : false);
                                    foreach (String s in seriales)
                                        updateMit(s, 99, m_modelInfo.Status);
                                }
                                else
                                    updateMit(serial, 99, m_modelInfo.Status);
                            }
                        }
                    }
                }

            }
            catch(Exception ex)
            {

            }
        }

        private bool queryAOI(string serial, ref int resultTest, ref string panelID)
        {
            bool res = m_oracle.QuerySerial(serial, ref resultTest);
            if (!res)
            {
                addLine("[" + DateTime.Now.ToString() + "] " + "No se encontro el serial: " + serial.ToUpper() + " en Oracle, se buscara panelID.");
                panelID = m_cogiscan.get_panel_ID(serial);
                if (!panelID.Contains("NOT_FOUND"))
                {
                    res = m_oracle.QuerySerial(panelID.ToUpper(), ref resultTest);
                    if (!res)
                    {
                        serialsPanel = m_cogiscan.get_panel(serial.ToUpper());
                        if (serialsPanel.Count > 0)
                            res = m_oracle.QuerySerials(serialsPanel, ref resultTest);
                        else addLine("[" + DateTime.Now.ToString() + "] " + "No se encontro información del serial en ninguna base de datos, no se dio Historial de AOI.");
                    }
                }
                else
                {
                    addLine("[" + DateTime.Now.ToString() + "] " + "No se encontro PanelID para serial: " + serial + ", STATUS: " + m_modelInfo.Status + " " + m_modelInfo.Operation);
                    addLine("[" + DateTime.Now.ToString() + "] " + "Se buscaran seriales del panel del serial...");
                    serialsPanel = m_db2.getPanelSerials(serial.ToUpper());
                    if (serialsPanel.Count > 0)
                        res = m_oracle.QuerySerials(serialsPanel, ref resultTest);
                    else addLine("[" + DateTime.Now.ToString() + "] " + "No se encontro información del serial en ninguna base de datos, no se dio Historial de AOI.");
                }
            }

            return res;
        }

        private void checkAOIWaiting(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(500);
            int resultTest = 0;
            string message = "";
            if (!String.IsNullOrEmpty(batchID))
            {
                addLine("[" + DateTime.Now.ToString() + "] " + "Consultando si existen seriales en Waiting AOI " + Name + " Batch " + batchID);
                String partN = m_db2.getProductPNByBatch(BatchID);
                List<String> serials = null;
                if (partN.Contains("H61P-LB") || partN.Contains("H61P-HB") || partN.Contains("T20A-C") || partN.Contains("NAL80B-CHL-BG-FPC-TRIAL") 
                 || partN.Contains("P758-A") || partN.Contains("P758-B") || partN.Contains("P758-CHL-B-J700S6-TRIAL") || partN.Contains("3W0A-C-TRIAL")
                 || partN.Contains("790B-C-TRIAL") || partN.Contains("P758-C") || partN.Contains("790B-D-TRIAL"))
                {
                    serials = m_db2_serv2.getSerialsActiveAOI(batchID);
                    if (serials != null && serials.Count > 0)
                        foreach (String serial in serials)
                        {
                            string pallet = m_db2.getPallet(serial);
                            if(checkAOIPallet("ITEM" + pallet, ref resultTest)==2)
                            {
                                m_modelInfo = m_cogiscan.query_item(serial.ToUpper());
                                m_cogiscan.setAOIInspection(serial.ToUpper(), Name, true, ref message, m_modelInfo.Status);
                            }
                        }
                    else
                        addLine("[" + DateTime.Now.ToString() + "] " + "El Batch " + batchID + " no contiene seriales en Espera de AOI.");
                }
                else
                {
                    serials = m_db2_serv2.getSerialsWaitingAOI(batchID);
                    if (serials != null && serials.Count > 0 && serials.Count() < 500)
                        foreach (String serial in serials)
                            checkAOI(serial);
                    else
                        addLine("[" + DateTime.Now.ToString() + "] " + "El Batch " + batchID + " no contiene seriales en Espera de AOI.");
                }
            }
        }

        public void checkAllAOIWaiting(object sender, DoWorkEventArgs e)
        {
                addLine("[" + DateTime.Now.ToString() + "] " + "Consultando si existen seriales en Waiting AOI ");
                List<String> serials = m_db2_serv2.getAllSerialsWaitingAOI();

                if (serials != null && serials.Count > 0)
                {
                    foreach (String serial in serials)
                    {
                        checkAOI(serial);
                    }
                }
        }

        public void checkAllAOIActive(object sender, DoWorkEventArgs e)
        {

                addLine("[" + DateTime.Now.ToString() + "] " + "Consultando si existen seriales en Active AOI ");
                List<String> serials = m_db2_serv2.getAllSerialsActiveAOI();

                if (serials != null && serials.Count > 0)
                {
                    foreach (String serial in serials)
                    {
                        checkAOI(serial);
                    }
                }

        }
        private void checkAOIActive(object sender, DoWorkEventArgs e)
        {
           
            if (!String.IsNullOrEmpty(batchID))
            {
                addLine("[" + DateTime.Now.ToString() + "] " + "Consultando si existen seriales en Active AOI " + Name);
                List<String> serials = m_db2_serv2.getSerialsActiveAOI(batchID);

                if (serials != null && serials.Count > 0)
                {
                    foreach (String serial in serials)
                    {
                        checkAOI(serial);
                    }
                }
            }

        }

        private void endOperation_AOI(object sender, DoWorkEventArgs e)
        {
                addLine("[" + DateTime.Now.ToString() + "] " + "Realizando End operation Active AOI " + Name);
                List<String> serials = m_db2_serv2.getAllSerialsActiveAOI();

                if (serials != null && serials.Count > 0)
                {
                    foreach (String serial in serials)
                    {
                        endOperationAOI(serial);
                    }
                }

        }

        /*******************************************************************************************************************/
        /*******************************************************************************************************************/

        async void attendMsg(object sender, System.ComponentModel.DoWorkEventArgs eargs)
        {
            try
            {
                string RESULT;

                String msg = eargs.Argument.ToString();

                m_res = await m_camx.acknowledgeEvents(msg);
                RESULT = m_res.Body.@return;
            }
            catch (Exception ex)
            {
                //addLine("Exception: " + ex.Message);
            }
        }
        void updateMit(String serial, int result, String STATUS)
        {
            String message = "";
            //String Xray_res = "";
            try
            {
                addLine("[" + DateTime.Now.ToLongDateString() + "]" + "CHEKING MITSUBISHI SERIAL IN SERVER 28: " + serial);
                m_dbMit.updateMit(serial, result);

                //if (serial.Contains("C XA") || serial.Contains("C XB"))
                //{
                //    try
                //    {
                //        Xray_res = m_dbMit.getXrayResult(serial).First().XRAY_RES;
                //    }
                //    catch (Exception ex)
                //    {
                //        Xray_res = "99";

                //    }
                //    m_cogiscan.setXRAY(serial.ToUpper(), "", Xray_res == "1" ? true : false, ref message, STATUS);
                //}
                addLine(message);
            }
            catch (Exception ex)
            {
                addLine("[" + DateTime.Now.ToLongDateString() + "]" + "ERROR IN MITSUBISHI SERIAL " + serial + ex.Message);
            }
        }
        #endregion
    }
}
