using HIST_AOI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIST_AOI.Class
{
    class CMonitor_AOI
    {
        List<CLine> m_lines;
        private siixsem_main_dbEntities m_db;
        private BackgroundWorker _bwAOIAllWaiting;
        private BackgroundWorker _bwAOIKoh;
        private BackgroundWorker _bwAOIDJWaiting;
        private BackgroundWorker _bwAOIAllActive;
        CMySQL m_mySql;
        CCogiscanCGSDW m_db2_serv11;
        CCogiscanCGS m_db2_serv2;
        siixsem_aoi_koh_youngEntities m_db28;// = new siixsem_aoi_koh_youngEntities();
        String m_batch_id;
        private CCogiscan m_cogiscan;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        RichTextBox m_console;
        ListView m_listLines;


        internal List<CLine> Lines { get => m_lines; set => m_lines = value; }
        public string Batch_id { get => m_batch_id; set => m_batch_id = value; }

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
                    if(!String.IsNullOrEmpty(texto)&&!String.IsNullOrWhiteSpace(texto))
                        m_console.AppendText(texto + "\n");
                }
                catch (Exception ex) {
                    //m_console.AppendText(ex.Message + "\n");
                }
            }
        }
        public CMonitor_AOI(ref RichTextBox console/*, ref ListView list*/)
        {
            m_console = console;
            m_lines = new List<CLine>();
            m_db = new siixsem_main_dbEntities();
            m_db2_serv2 = new CCogiscanCGS();
            m_db2_serv11 = new CCogiscanCGSDW();
            m_db28 = new siixsem_aoi_koh_youngEntities();
            m_cogiscan = new CCogiscan();
            m_mySql = new CMySQL();
            initThreadsAOI();
            getLinesAOI();

        }
        private void initThreadsAOI()
        {
            try
            {
                _bwAOIKoh = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };
                _bwAOIKoh.DoWork += checkAOIKoh;
                //_bwAOIWaiting.ProgressChanged += bw_ProgressChanged;
                _bwAOIKoh.RunWorkerCompleted += bw_RunWorkerCompleted;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[initThreadsAOIAllWaiting] Error...");
            }

            try
            {
                _bwAOIDJWaiting = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };
                _bwAOIDJWaiting.DoWork += checkDJAOIWaiting;
                //_bwAOIWaiting.ProgressChanged += bw_ProgressChanged;
                _bwAOIDJWaiting.RunWorkerCompleted += bw_RunWorkerCompleted;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[initThreadsAOIAllWaiting] Error...");
            }

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
        public void checkAOIWAIT()
        {
            if(!_bwAOIAllWaiting.IsBusy)
                _bwAOIAllWaiting.RunWorkerAsync();
        }
        public void checkAOIWAITKoh()
        {
            if (!_bwAOIKoh.IsBusy)
                _bwAOIKoh.RunWorkerAsync();
        }
        public void checkAOIDJWAIT()
        {
            if (!_bwAOIDJWaiting.IsBusy)
                _bwAOIDJWaiting.RunWorkerAsync();
        }
        public void checkAOIKoh(object sender, DoWorkEventArgs e)
        {
            try
            {
                CModelInfo m_modelInfo;
                String tool = "";
                bool resulTest = true;
                String message = "";
                //addLine("[" + DateTime.Now.ToString() + "] " + "Consultando si existen Seriales de linea 18 y 19.");
                List<getAOIPending_Result> serials = m_db28.getAOIPending().ToList();
                //List<getAOIPendingTest_Result> serials = m_db28.getAOIPendingTest().ToList();

                foreach (getAOIPending_Result serial in serials)
                //foreach (getAOIPendingTest_Result serial in serials)
                {
                    try
                    {
                        //m_modelInfo = m_cogiscan.query_item(serial.serial.Trim().ToUpper());
                        //addLine("Serial: " + serial.serial);
                        if (serial.linea == 222){
                            m_cogiscan.setDispostion(serial.serial.Trim().ToUpper(),serial.estatus.Contains("PASS") ? true : serial.estatus.Contains("GOOD") ? true : false);
                            m_db28.setAOIPending(serial.serial);
                        }
                        else
                        {
                            m_modelInfo = m_cogiscan.query_item(serial.serial.Trim().ToUpper());
                            tool = "AOI_LINEA" + serial.linea.ToString();
                            addLine("Serial: " + serial.serial + " " + m_modelInfo.Operation + " " + m_modelInfo.Status + " Tool ID: " + tool);
                            if (m_modelInfo.DjNumber.Contains("false")) { m_db28.setAOIPendingN(serial.serial); }
                            else
                            //if (m_modelInfo.Item_type.Contains("PALLET-SMT")&&tool.Contains("AOI_LINEA22"))
                            //{
                            //    List<String> serialsPallet = m_cogiscan.getContents(serial.serial.Trim().ToUpper(), ref message);
                                

                            //    if (serialsPallet.Count() > 0)
                            //    {
                            //        message = m_cogiscan.startOperation(serial.serial, tool, "AOI Inspection");
                            //        addLine("Start Operation " + tool + " " + serial.serial + message);
                            //        foreach (String serialPallet in serialsPallet)
                            //        {
                            //            m_modelInfo = m_cogiscan.query_item(serialPallet.ToUpper());
                            //            if ((m_modelInfo.Status.Contains("WAITING") || m_modelInfo.Status.Contains("ESPERANDO") || m_modelInfo.Status.Contains("ACTIVE")) && m_modelInfo.Operation.Contains("AOI"))
                            //            {
                            //                if (serial.estatus.Contains("PASS") || serial.estatus.Contains("GOOD"))
                            //                    resulTest = true;
                            //                else
                            //                    resulTest = false;

                            //                addLine("Se dara resultado " + serial.estatus + " a la pieza " + serialPallet);

                            //                m_cogiscan.setAOIInspection(serialPallet.ToUpper(), tool, resulTest, ref message, m_modelInfo.Status);
                            //                try
                            //                {
                            //                    m_db28.insertSerial(serial.id_aoi, serialPallet.ToUpper());
                            //                }
                            //                catch (Exception ex)
                            //                {
                            //                    addLine("Error al insertar serial " + serialPallet.ToUpper() + " del ITEM " + serial.serial + ex.Message);
                            //                }

                            //                addLine(message);
                            //            }
                            //            else
                            //            {
                            //                if (m_modelInfo.Operation.Contains("MIDDLE TEST") || m_modelInfo.Operation.Contains("CONFORMAL") || m_modelInfo.Operation.Contains("ICT") || m_modelInfo.Operation.Contains("FCT")
                            //                    || m_modelInfo.Operation.Contains("PACKING") || m_modelInfo.Operation.Contains("OQC") || m_modelInfo.Operation.Contains("ROUTER") || m_modelInfo.Operation.Contains("MANUAL"))
                            //                {
                            //                    m_db28.setAOIPending(serial.serial);
                            //                }

                            //            }
                            //        }
                            //        message = m_cogiscan.endOperation(serial.serial, tool, "AOI Inspection");
                            //        addLine("End Operation " + tool + " " + serial.serial + message);
                            //        if (message.Contains("Success"))
                            //            m_db28.setAOIPending(serial.serial);
                            //    }
                            //    else
                            //    {
                            //        m_db28.setAOIPending(serial.serial);
                            //    }
                            //}
                            //else
                                if (tool.Contains("AOI_LINEA22"))
                                {
                                    if ((m_modelInfo.Status.Contains("WAITING") || m_modelInfo.Status.Contains("ESPERANDO") || m_modelInfo.Status.Contains("ACTIVE")) && m_modelInfo.Operation.Contains("AOI"))
                                    {
                                        //tool = serial.linea == 18 ? "AOI_LINEA18" : serial.linea == 19 ? "AOI_LINEA19" : "AOI_LINEA20";
                                        tool = "AOI_LINEA" + serial.linea.ToString();
                                        if (tool.Contains("AOI_LINEA241")) tool = "AOI_LINEA24R";
                                        if (tool.Contains("AOI_LINEA242")) tool = "AOI_LINEA24F";
                                        if (tool.Contains("AOI_LINEA271")) tool = "AOI_LINEA27R";
                                        if (tool.Contains("AOI_LINEA272")) tool = "AOI_LINEA27F";
                                        if (tool.Contains("AOI_LINEA261")) tool = "AOI_LINEA26R";
                                        if (tool.Contains("AOI_LINEA262")) tool = "AOI_LINEA26F";



                                        if (serial.estatus.Contains("PASS") || serial.estatus.Contains("GOOD"))
                                            resulTest = true;
                                        else resulTest = false;

                                        //if (serial.serial.Contains("AA000") && serial.estatus.Contains("NG"))
                                        //{
                                        //    m_cogiscan.setDispostion(serial.serial.Trim());
                                        //}
                                        //else 
                                        m_cogiscan.setAOIInspection(serial.serial.Trim(), tool, resulTest, ref message, m_modelInfo.Status);

                                        addLine("\t" + message);

                                        if (message.Contains("Success"))
                                            m_db28.setAOIPending(serial.serial);

                                        //isPallet_Result isPallet = m_db.isPallet(m_modelInfo.Part_number).First();
                                        //if (isPallet.ISPALLET == 0)
                                        //{
                                        //    m_cogiscan.setAOIInspection(serial.serial.Trim(), tool, resulTest, ref message, m_modelInfo.Status);
                                        //    addLine(message);
                                        //    if (message.Contains("Success"))
                                        //        m_db28.setAOIPending(serial.serial);
                                        //}
                                        //else
                                        //{
                                        //    checkAOIPalletSQLServer(serial.serial, serial.estatus, tool);
                                        //}

                                    }
                                }

                            else
                            {
                                //if (m_modelInfo.Operation.ToUpper().Contains("MIDDLE TEST") || m_modelInfo.Operation.ToUpper().Contains("CONFORMAL") || m_modelInfo.Operation.ToUpper().Contains("ICT") || m_modelInfo.Operation.ToUpper().Contains("FCT")
                                //|| m_modelInfo.Operation.ToUpper().Contains("PACKING") || m_modelInfo.Operation.ToUpper().Contains("OQC") || m_modelInfo.Operation.ToUpper().ToUpper().Contains("ROUTER") || m_modelInfo.Operation.ToUpper().Contains("MANUAL")
                                //|| m_modelInfo.Operation.ToUpper().Contains("SMT") || m_modelInfo.Operation.ToUpper().Contains("REFLOW") || m_modelInfo.Operation.ToUpper().Contains("REWORK") || m_modelInfo.Operation.ToUpper().Contains("SPG"))
                                {
                                    m_db28.setAOIPending(serial.serial);
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                       // addLine(tool + " sin serial.");
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
        public void checkAllAOIWaiting(object sender, DoWorkEventArgs e)
        {
            try
            {
                addLine("[" + DateTime.Now.ToString() + "] " + "Consultando si existen DJS con seriales en Waiting AOI ");
                List<String> batchs = m_db2_serv2.getAllDJsWaitingAOI();
                List<String> batchsh61P = m_db2_serv2.getDJsH61P();

                if (batchsh61P != null && batchsh61P.Count > 0)
                {
                    foreach (String batch in batchsh61P)
                    {
                        try
                        {
                            String tool = m_db2_serv11.getLineByBatch(batch);
                            if (!tool.Contains("L19") && !tool.Contains("L18"))
                            {
                                getAOILine_Result LineAOI = m_db.getAOILine(tool).First();

                                addLine("[" + DateTime.Now.ToString() + "] " + "Consultando batch " + batch + " en linea " + LineAOI.DESCRIPTION_LINE);
                                CLine line = new CLine(ref this.m_console, LineAOI.DESCRIPTION_LINE, ref this.m_listLines, LineAOI.IP_AOI, LineAOI.SID);
                                line.Events.BatchID = batch;
                                line.Events.StartAOIWait();
                            }
                        }
                        catch (Exception ex)
                        {
                            addLine("[" + DateTime.Now.ToString() + "] " + "La dj: " + batch + " no tiene una Linea asociada.");
                        }
                    }
                }
                if (batchs != null && batchs.Count > 0)
                {
                    foreach (String batch in batchs)
                    {
                        try
                        {
                            getAOILine_Result LineAOI =  m_db.getAOILine(m_db2_serv11.getLineByBatch(batch)).First();
                            //LineAOI.DESCRIPTION_LINE = "AOI_LINEA2";
                            //LineAOI.IP_AOI = "192.168.3.44";
                            addLine("[" + DateTime.Now.ToString() + "] " + "Consultando batch " + batch + " en linea " + LineAOI.DESCRIPTION_LINE);
                            CLine line = new CLine(ref this.m_console, LineAOI.DESCRIPTION_LINE, ref this.m_listLines, LineAOI.IP_AOI, LineAOI.SID);
                            line.Events.BatchID = batch;
                            line.Events.StartAOIWait();
                        }
                        catch(Exception ex)
                        {
                            addLine("[" + DateTime.Now.ToString() + "] " + "La dj: " + batch + " no tiene una Linea asociada.");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                addLine("[" + DateTime.Now.ToString() + "] " + ex.Message);
            }
        }

        public void checkDJAOIWaiting(object sender, DoWorkEventArgs e)
        {
            try
            {
                addLine("[" + DateTime.Now.ToString() + "] " + "Consultando si existen DJS con seriales en Waiting AOI ");

                try
                {
                    getAOILine_Result LineAOI =  m_db.getAOILine(m_db2_serv11.getLineByBatch(Batch_id)).First();
                    //LineAOI.DESCRIPTION_LINE = "AOI_LINEA1";
                    //LineAOI.IP_AOI = "192.168.3.44";
                    LineAOI.SID = "prismdb";
                    addLine("[" + DateTime.Now.ToString() + "] " + "Consultando batch " + Batch_id + " en linea " + LineAOI.DESCRIPTION_LINE);
                    CLine line = new CLine(ref this.m_console, LineAOI.DESCRIPTION_LINE, ref this.m_listLines, LineAOI.IP_AOI, LineAOI.SID);
                    line.Events.BatchID = Batch_id;
                    line.Events.StartAOIWait();
                }
                catch (Exception ex)
                {
                    addLine("[" + DateTime.Now.ToString() + "] " + "La dj: " + Batch_id + " no tiene una Linea asociada.");
                }

            }
            catch (Exception ex)
            {
                addLine("[" + DateTime.Now.ToString() + "] " + ex.Message);
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
                    //checkAOI(serial);
                }
            }

        }
        private void checkAOIPalletSQLServer(string pallet, String resultTest,String tool)
        {
            siixsem_aoi_dbEntities m_dbAOI = new siixsem_aoi_dbEntities();
            string message = "";
            String result = "";
            Pallets_smt container = null;

            try
            {
                if (pallet.Contains("ITEM"))
                    m_mySql.getPalletByItem(pallet, ref container, ref message);
                else
                    m_mySql.getPalletByName(pallet, ref container, ref message);
                if (container != null)
                {
                    List<String> serials = m_cogiscan.getContents(pallet, ref message);
                    if (resultTest.Contains("PASS") || resultTest.Contains("GOOD")) /// OK y FALSE CALL
                    {
                        foreach (String serial in serials)
                        {
                            result = m_cogiscan.setProcessStepStatus(serial, "AOI Inspection", true);
                            m_dbAOI.insertResult(serial, "SUCCESS", tool, result);
                            addLine("[" + DateTime.Now.ToString() + "] " + serial + " ProcessStepStatus " + (resultTest.Contains("GOOD") ? "OK " : "FALSE CALL ") + result);
                        }
                    }
                    else
                    {
                        foreach (String serial in serials)
                        {
                            result = m_cogiscan.setProcessStepStatus(serial, "AOI Inspection", false);
                            m_dbAOI.insertResult(serial, "FAIL", tool, result);
                            addLine("[" + DateTime.Now.ToString() + "] " + serial + " ProcessStepStatus " + "NG " + result);
                        }
                    }
                    result = m_cogiscan.endOperation(pallet, tool, "AOI Inspection");
                    addLine("[" + DateTime.Now.ToString() + "] " + pallet + " endOperation " + resultTest);
                    if (result.Contains("Success"))
                    {
                        
                        m_mySql.updateCyclePallet(container.Item.Replace("ITEM", ""), ref message);
                        if (container.Ciclos + 1 == container.Limite)
                        {
                            m_cogiscan.updateQuarantineRule(container.Name);
                            m_mySql.updateStatusPallet(container.Name, "I", ref message);
                        }
                        m_dbAOI.insertResult(container.Name, "UPDATE_PALLET", tool, "Prev QTY: " + container.Ciclos.ToString() + " New QTY: " + (container.Ciclos + 1).ToString());
                    }
                    m_db28.setAOIPending(pallet);
                }

            }
            catch (Exception ex)
            {

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
        public void start()
        {

            foreach (CLine line in Lines)
            { 
                line.Events.Start();
            }
        }
        public void stop()
        {

            foreach (CLine line in Lines)
            {
                line.Events.Stop();
            }
        }

        public void checkWaiting()
        {
            
            foreach (CLine line in Lines)
            {
                line.Events.StartAOIWait();
            }
        }

        public void checkAllWaiting()
        {
            foreach (CLine line in Lines)
                Lines.First().Events.StartAllAOIWait();
        }
        public void checkActive()
        {

            foreach (CLine line in Lines)
            {
                line.Events.StartAOIActive();
            }
        }
        public void checkAllActive()
        {
            foreach (CLine line in Lines)
                Lines.First().Events.StartALLAOIActive();
        }
        private void getLinesAOI()
        {
            var Lines = m_db.getAOILines();
            CLine line;

            m_console.AppendText("Cargando lineas..." + "\n");

            foreach (getAOILines_Result nameline in Lines)
            {
                String txtLine = nameline.DESCRIPTION_LINE;
                m_console.AppendText(txtLine + " cargada...\n");
                line = new CLine(ref m_console, txtLine, ref m_listLines,nameline.IP_AOI,nameline.SID);
                m_lines.Add(line);

            }

            m_console.AppendText("Lineas cargadas..." + "\n");
        }
    }

}
