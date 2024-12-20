using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.Data.DB2;

namespace HIST_AOI.Class
{
    class CCogiscanCGS
    {
        String m_ip;
        String m_user;
        String m_password;
        String m_alias;
        String m_strConection;
        DB2Connection m_CgsDB;
        DB2Command m_DB2Command;
        DB2DataReader m_Db2DataReader;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public CCogiscanCGS()
        {
            m_ip = "192.168.3.2:50000";
            m_user = "CGSAPP";
            m_password = "T0mcat4Fun";
            m_alias = "CGS";
            m_DB2Command = null;
            m_Db2DataReader = null;
            //m_CgsDB = new DB2Connection();

            m_strConection = "server=" + m_ip + ";database=" + m_alias + ";uid=" + m_user + ";pwd=" + m_password + ";";
        }
        public bool connect()
        {
            bool result = false;
            try
            {
                m_CgsDB = new DB2Connection(m_strConection);

                m_CgsDB.Open();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine(ex.Message);
            }

            return result;
        }
        public void close()
        {
            if (m_CgsDB != null) m_CgsDB.Close();
        }
        public List<String> getAllSerialsWaitingAOI()
        {
            List<String> serials = new List<string>();

            try
            {
                close();
                connect();
                m_DB2Command = m_CgsDB.CreateCommand();

                ////////////////////////////// SE OBTIENE LA FECHA DEL SERIAL Y SU BATCH

                m_DB2Command.CommandText = "SELECT I.ITEM_ID FROM CGS.ITEM I "
                                        + "JOIN CGSPCM.PRODUCT PROD ON(PROD.PRODUCT_KEY = I.ITEM_KEY) "
                                        + "JOIN CGS.PART_NUMBER PN ON(PN.PART_NUMBER_KEY = I.PART_NUMBER_KEY) "
                                        + "JOIN CGSPCM.ROUTE_STEP RS ON(RS.ROUTE_STEP_KEY = PROD.ROUTE_STEP_KEY) "
                                        + "JOIN CGSPCM.OPERATION OPER ON(OPER.OPERATION_KEY = RS.OPERATION_KEY) "
                                        + "JOIN CGSPCM.PRODUCT_BATCH BATCH ON(BATCH.BATCH_KEY = PROD.BATCH_KEY) "
                                        + "JOIN CGS.ITEM_TYPE IT ON(IT.ITEM_TYPE_KEY = I.ITEM_TYPE_KEY) "
                                        + "JOIN CGS.ITEM_TYPE_CLASS ITC ON(ITC.ITEM_CLASS_KEY = IT.ITEM_CLASS_KEY) "
                                        + "LEFT OUTER JOIN CGS.ITEM TOOL ON(TOOL.ITEM_KEY = PROD.TOOL_KEY) "
                                        + "WHERE ITC.SHORT_NAME <> 'Product Carrier'  AND "
                                        + "      OPERATION_NAME = 'AOI Inspection' AND STATUS = 'W' order by I.INIT_TMST DESC";

                m_Db2DataReader = m_DB2Command.ExecuteReader();
                if (m_Db2DataReader.HasRows)
                {
                    while (m_Db2DataReader.Read())
                    {
                        serials.Add(m_Db2DataReader.GetString(0).ToUpper());
                    }
                }
                m_Db2DataReader.Close();
                close();

            }
            catch (Exception ex)
            {
                logger.Error(ex, "[getAllSerialsWaitingAOI] Error ");
            }

            return serials;
        }
        public List<String> getAllDJsWaitingAOI()
        {
            List<String> serials = new List<string>();

            DateTime date = DateTime.Now;

            //if(date.Day > 15)
                date = date.AddDays(-3);
            //else
            //    date = date.AddDays((date.Day-1) * -1);

            try
            {
                close();
                connect();
                m_DB2Command = m_CgsDB.CreateCommand();

                ////////////////////////////// SE OBTIENE LA FECHA DEL SERIAL Y SU BATCH

                m_DB2Command.CommandText = "SELECT DISTINCT BATCH.BATCH_ID FROM CGS.ITEM I " +
                                            "JOIN CGSPCM.PRODUCT PROD ON(PROD.PRODUCT_KEY = I.ITEM_KEY) " +
                                            "JOIN CGS.PART_NUMBER PN ON(PN.PART_NUMBER_KEY = I.PART_NUMBER_KEY) " +
                                            "JOIN CGSPCM.ROUTE_STEP RS ON(RS.ROUTE_STEP_KEY = PROD.ROUTE_STEP_KEY) " +
                                            "JOIN CGSPCM.OPERATION OPER ON(OPER.OPERATION_KEY = RS.OPERATION_KEY) " +
                                            "JOIN CGSPCM.PRODUCT_BATCH BATCH ON(BATCH.BATCH_KEY = PROD.BATCH_KEY) " +
                                            "JOIN CGS.ITEM_TYPE IT ON(IT.ITEM_TYPE_KEY = I.ITEM_TYPE_KEY) " +
                                            "JOIN CGS.ITEM_TYPE_CLASS ITC ON(ITC.ITEM_CLASS_KEY = IT.ITEM_CLASS_KEY) " +
                                            "LEFT OUTER JOIN CGS.ITEM TOOL ON(TOOL.ITEM_KEY = PROD.TOOL_KEY) " +
                                            " WHERE ITC.SHORT_NAME <> 'Product Carrier'  AND " +
                                            " OPERATION_NAME = 'AOI Inspection' AND STATUS = 'W' and I.INIT_TMST > '" + date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() +"'" + //'2020-07-01' " +
                                            //" OPERATION_NAME = 'AOI Inspection' AND STATUS = 'A' and batch_id='228559'" + //and I.INIT_TMST > '" + date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() +"'" + //'2020-07-01' " +
                                            " AND BATCH.BATCH_ID NOT LIKE '%PRUEBA%'" +
                                            " order by BATCH.BATCH_ID DESC WITH UR";

                m_Db2DataReader = m_DB2Command.ExecuteReader();
                if (m_Db2DataReader.HasRows)
                {
                    while (m_Db2DataReader.Read())
                    {
                        serials.Add(m_Db2DataReader.GetString(0).ToUpper());
                    }
                }
                m_Db2DataReader.Close();
                close();

            }
            catch (Exception ex)
            {
                logger.Error(ex, "[getAllDJSWaitingAOI] Error ");
            }

            return serials;
        }

        public List<String> getDJsH61P()
        {
            List<String> serials = new List<string>();

            DateTime date = DateTime.Now;

            //if(date.Day > 15)
            date = date.AddDays(-2);
            //else
            //    date = date.AddDays((date.Day-1) * -1);

            try
            {
                close();
                connect();
                m_DB2Command = m_CgsDB.CreateCommand();

                ////////////////////////////// SE OBTIENE LA FECHA DEL SERIAL Y SU BATCH

                m_DB2Command.CommandText = "SELECT DISTINCT BATCH.BATCH_ID FROM CGS.ITEM I " +
                                            "JOIN CGSPCM.PRODUCT PROD ON(PROD.PRODUCT_KEY = I.ITEM_KEY) " +
                                            "JOIN CGS.PART_NUMBER PN ON(PN.PART_NUMBER_KEY = I.PART_NUMBER_KEY) " +
                                            "JOIN CGSPCM.ROUTE_STEP RS ON(RS.ROUTE_STEP_KEY = PROD.ROUTE_STEP_KEY) " +
                                            "JOIN CGSPCM.OPERATION OPER ON(OPER.OPERATION_KEY = RS.OPERATION_KEY) " +
                                            "JOIN CGSPCM.PRODUCT_BATCH BATCH ON(BATCH.BATCH_KEY = PROD.BATCH_KEY) " +
                                            "JOIN CGS.ITEM_TYPE IT ON(IT.ITEM_TYPE_KEY = I.ITEM_TYPE_KEY) " +
                                            "JOIN CGS.ITEM_TYPE_CLASS ITC ON(ITC.ITEM_CLASS_KEY = IT.ITEM_CLASS_KEY) " +
                                            "LEFT OUTER JOIN CGS.ITEM TOOL ON(TOOL.ITEM_KEY = PROD.TOOL_KEY) " +
                                            " WHERE ITC.SHORT_NAME <> 'Product Carrier'  AND " +
                                            " OPERATION_NAME = 'AOI Inspection' AND STATUS = 'A'" +
                                            " AND BATCH.BATCH_ID NOT LIKE '%PRUEBA%' AND (PART_NUMBER LIKE '%H61P-LB%' OR PART_NUMBER LIKE '%H61P-HB%' OR PART_NUMBER LIKE 'T20A-C' OR PART_NUMBER LIKE 'NAL80B-CHL-BG-FPC-TRIAL' OR PART_NUMBER LIKE '%P758-A%' OR PART_NUMBER LIKE '%P758-B%' or PART_NUMBER LIKE '%P758-CHL-B-J700S6-TRIAL%' or PART_NUMBER LIKE '%3W0A-C-TRIAL%' or PART_NUMBER LIKE '%790B-C-TRIAL%' " +
                                            " or PART_NUMBER LIKE '%P758-C%' or PART_NUMBER LIKE '%790B-D-TRIAL%')" +
                                            " AND PROD.LAST_STATUS_CHANGE_TMST > '" + date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString() +"'" + //'2020-07-01' "+
                                            " AND BATCH.BATCH_ID NOT LIKE '%PRUEBA%'" +
                                            " order by BATCH.BATCH_ID DESC WITH UR";

                m_Db2DataReader = m_DB2Command.ExecuteReader();
                if (m_Db2DataReader.HasRows)
                {
                    while (m_Db2DataReader.Read())
                    {
                        serials.Add(m_Db2DataReader.GetString(0).ToUpper());
                    }
                }
                m_Db2DataReader.Close();
                close();

            }
            catch (Exception ex)
            {
                logger.Error(ex, "[getAllDJSWaitingAOI] Error ");
            }

            return serials;
        }
        public List<String> getSerialsWaitingAOI(String Batch_id)
        {
            List<String> serials = new List<string>();

            try
            {
                close();
                connect();
                m_DB2Command = m_CgsDB.CreateCommand();

                ////////////////////////////// SE OBTIENE LA FECHA DEL SERIAL Y SU BATCH

                m_DB2Command.CommandText = "SELECT I.ITEM_ID FROM CGS.ITEM I "
                                        + "JOIN CGSPCM.PRODUCT PROD ON(PROD.PRODUCT_KEY = I.ITEM_KEY) "
                                        + "JOIN CGS.PART_NUMBER PN ON(PN.PART_NUMBER_KEY = I.PART_NUMBER_KEY) "
                                        + "JOIN CGSPCM.ROUTE_STEP RS ON(RS.ROUTE_STEP_KEY = PROD.ROUTE_STEP_KEY) "
                                        + "JOIN CGSPCM.OPERATION OPER ON(OPER.OPERATION_KEY = RS.OPERATION_KEY) "
                                        + "JOIN CGSPCM.PRODUCT_BATCH BATCH ON(BATCH.BATCH_KEY = PROD.BATCH_KEY) "
                                        + "JOIN CGS.ITEM_TYPE IT ON(IT.ITEM_TYPE_KEY = I.ITEM_TYPE_KEY) "
                                        + "JOIN CGS.ITEM_TYPE_CLASS ITC ON(ITC.ITEM_CLASS_KEY = IT.ITEM_CLASS_KEY) "
                                        + "LEFT OUTER JOIN CGS.ITEM TOOL ON(TOOL.ITEM_KEY = PROD.TOOL_KEY) "
                                        + "WHERE ITC.SHORT_NAME <> 'Product Carrier' and BATCH.BATCH_ID = '" + Batch_id + "' AND "
                                        + "      OPERATION_NAME = 'AOI Inspection' AND (STATUS = 'W') WITH UR";

                m_Db2DataReader = m_DB2Command.ExecuteReader();
                if (m_Db2DataReader.HasRows)
                {
                    while (m_Db2DataReader.Read())
                    {
                        serials.Add(m_Db2DataReader.GetString(0).ToUpper());
                    }
                }
                m_Db2DataReader.Close();
                close();

            }
            catch (Exception ex)
            {
                logger.Error(ex, "[getSerialsWaitingAOI] Error " + Batch_id);
            }

            return serials;
        }
        public List<String> getSerialsActiveAOI(String Batch_id)
        {
            List<String> serials = new List<string>();

            try
            {
                close();
                connect();
                m_DB2Command = m_CgsDB.CreateCommand();

                ////////////////////////////// SE OBTIENE LA FECHA DEL SERIAL Y SU BATCH

                m_DB2Command.CommandText = "SELECT I.ITEM_ID FROM CGS.ITEM I "
                                        + "JOIN CGSPCM.PRODUCT PROD ON(PROD.PRODUCT_KEY = I.ITEM_KEY) "
                                        + "JOIN CGS.PART_NUMBER PN ON(PN.PART_NUMBER_KEY = I.PART_NUMBER_KEY) "
                                        + "JOIN CGSPCM.ROUTE_STEP RS ON(RS.ROUTE_STEP_KEY = PROD.ROUTE_STEP_KEY) "
                                        + "JOIN CGSPCM.OPERATION OPER ON(OPER.OPERATION_KEY = RS.OPERATION_KEY) "
                                        + "JOIN CGSPCM.PRODUCT_BATCH BATCH ON(BATCH.BATCH_KEY = PROD.BATCH_KEY) "
                                        + "JOIN CGS.ITEM_TYPE IT ON(IT.ITEM_TYPE_KEY = I.ITEM_TYPE_KEY) "
                                        + "JOIN CGS.ITEM_TYPE_CLASS ITC ON(ITC.ITEM_CLASS_KEY = IT.ITEM_CLASS_KEY) "
                                        + "LEFT OUTER JOIN CGS.ITEM TOOL ON(TOOL.ITEM_KEY = PROD.TOOL_KEY) "
                                        + "WHERE ITC.SHORT_NAME <> 'Product Carrier' and BATCH.BATCH_ID = '" + Batch_id + "' AND "
                                        + "      OPERATION_NAME = 'AOI Inspection' AND STATUS = 'A'";

                m_Db2DataReader = m_DB2Command.ExecuteReader();
                if (m_Db2DataReader.HasRows)
                {
                    while (m_Db2DataReader.Read())
                    {
                        serials.Add(m_Db2DataReader.GetString(0).ToUpper());
                    }
                }
                m_Db2DataReader.Close();
                close();

            }
            catch (Exception ex)
            {
                logger.Error(ex, "[getSerialsActiveAOI] Error " + Batch_id);
            }

            return serials;

        }
        public List<String> getAllSerialsActiveAOI()
        {
            List<String> serials = new List<string>();

            try
            {
                close();
                connect();
                m_DB2Command = m_CgsDB.CreateCommand();

                ////////////////////////////// SE OBTIENE LA FECHA DEL SERIAL Y SU BATCH

                m_DB2Command.CommandText = "SELECT I.ITEM_ID FROM CGS.ITEM I "
                                        + "JOIN CGSPCM.PRODUCT PROD ON(PROD.PRODUCT_KEY = I.ITEM_KEY) "
                                        + "JOIN CGS.PART_NUMBER PN ON(PN.PART_NUMBER_KEY = I.PART_NUMBER_KEY) "
                                        + "JOIN CGSPCM.ROUTE_STEP RS ON(RS.ROUTE_STEP_KEY = PROD.ROUTE_STEP_KEY) "
                                        + "JOIN CGSPCM.OPERATION OPER ON(OPER.OPERATION_KEY = RS.OPERATION_KEY) "
                                        + "JOIN CGSPCM.PRODUCT_BATCH BATCH ON(BATCH.BATCH_KEY = PROD.BATCH_KEY) "
                                        + "JOIN CGS.ITEM_TYPE IT ON(IT.ITEM_TYPE_KEY = I.ITEM_TYPE_KEY) "
                                        + "JOIN CGS.ITEM_TYPE_CLASS ITC ON(ITC.ITEM_CLASS_KEY = IT.ITEM_CLASS_KEY) "
                                        + "LEFT OUTER JOIN CGS.ITEM TOOL ON(TOOL.ITEM_KEY = PROD.TOOL_KEY) "
                                        + "WHERE ITC.SHORT_NAME <> 'Product Carrier' AND "
                                        + "      OPERATION_NAME = 'AOI Inspection' AND STATUS = 'A' and order by I.INIT_TMST DESC";

                m_Db2DataReader = m_DB2Command.ExecuteReader();
                if (m_Db2DataReader.HasRows)
                {
                    while (m_Db2DataReader.Read())
                    {
                        serials.Add(m_Db2DataReader.GetString(0).ToUpper()) ;
                    }
                }
                m_Db2DataReader.Close();
                close();

            }
            catch (Exception ex)
            {
                logger.Error(ex, "[getAllSerialsActiveAOI] Error ");
            }

            return serials;

        }

    }
}
