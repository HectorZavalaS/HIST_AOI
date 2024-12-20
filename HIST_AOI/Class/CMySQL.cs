using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIST_AOI.Class
{
    class CMySQL
    {
        private MySqlConnection m_conn;
        private DataTable m_data;
        private MySqlDataAdapter m_da;
        private MySqlCommand m_cb;
        private String m_cadC;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public CMySQL()
        {
            m_cadC = String.Format("server={0};user id={1}; password={2}; database=siixsem_product_carrier; pooling=false",
                            "192.168.3.13", "root", "S3m4dm1n2017!");
            m_cb = new MySqlCommand();
        }

        public bool connect()
        {
            bool result = false;
            try
            {
                m_conn = new MySqlConnection(m_cadC);
                m_conn.Open();
                result = true;
            }
            catch (MySqlException ex)
            {
                ///MessageBox.Show("Error connecting to the server: " + ex.Message);
                ///
                logger.Error(ex, "Error trying to connect MySQL Server 13.");
                result = false;
            }
            return result;
        }

        public bool executeSPWhitoutP(String nameProc)
        {
            bool result = false;
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                connect();
                m_cb.Connection = m_conn;

                m_cb.CommandText = nameProc;
                m_cb.CommandType = CommandType.StoredProcedure;

                m_cb.ExecuteNonQuery();

                //Console.WriteLine("Employee number: " + cmd.Parameters["@empno"].Value);
                //Console.WriteLine("Birthday: " + cmd.Parameters["@bday"].Value);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message);
            }
            m_conn.Close();
            Console.WriteLine("Done.");
            return result;
        }
        public bool getSerialResult(String serial, ref String resultTest)
        {
            bool result = false;
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                if (connect())
                {
                    m_cb.Connection = m_conn;

                    int dia = DateTime.Now.DayOfYear;

                    string query = "select STATUS from fct_result where serial = ?serial order by FCT_DATE desc limit 1";

                    MySqlCommand mycomand = new MySqlCommand(query, m_cb.Connection);
                    mycomand.Parameters.AddWithValue("?serial", serial);


                    MySqlDataReader myreader = mycomand.ExecuteReader();
                    //myreader.Read();

                    if (myreader.HasRows)
                    {
                        while (myreader.Read())
                        {
                            resultTest = myreader["STATUS"].ToString();
                        }
                        result = true;
                        if (String.IsNullOrEmpty(resultTest)) resultTest = "FAILED";
                    }
                    else
                    {
                        resultTest = "FAILED";
                        result = true;
                    }
                    m_conn.Close();
                }
                else
                {
                    resultTest = "No se pudo conectar a la base de datos MySQL Server 13. No se obtuvo resultado para el serial: " + serial;
                    result = false;
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                logger.Error(ex, "Error trying to connect to MySQL has occurred.");
                resultTest = "Error " + ex.Number + " has occurred: \n" + ex.Message;
                result = false;
            }
           
            Console.WriteLine("Done.");
            return result;
        }
        public bool getPalletByItem(String item, ref Pallets_smt pallet, ref String msgResult)
        {
            bool result = false;
            
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                if (connect())
                {

                    m_cb.Connection = m_conn;

                    int dia = DateTime.Now.DayOfYear;

                    string query = "select id, item, name, ciclos, limite, status, damaged from siixsem_pallets_smt where item = ?item ";

                    MySqlCommand mycomand = new MySqlCommand(query, m_cb.Connection);
                    mycomand.Parameters.AddWithValue("?item", item.Replace("ITEM",""));


                    MySqlDataReader myreader = mycomand.ExecuteReader();
                    //myreader.Read();

                    if (myreader.HasRows)
                    {
                        try
                        {
                            while (myreader.Read())
                            {
                                pallet = new Pallets_smt();
                                pallet.Id = Convert.ToInt32(myreader["id"]);
                                pallet.Item = myreader["item"].ToString();
                                pallet.Name = myreader["name"].ToString();
                                pallet.Ciclos = Convert.ToInt32(myreader["ciclos"]);
                                pallet.Limite = Convert.ToInt32(myreader["limite"]);
                                pallet.Status = myreader["status"].ToString();
                                pallet.Damaged = myreader["damaged"].ToString();
                            }
                            myreader.Close();
                            result = true;
                        }
                        catch(Exception ex)
                        {
                            myreader.Close();
                        }
                        //if (String.IsNullOrEmpty(msgResult)) msgResult = "FAILED";
                    }
                    else
                    {
                        msgResult = "No se encontro el pallet " + item  + ".";
                        result = true;
                    }
                    m_conn.Close();
                }
                else
                {
                    msgResult = "No se pudo conectar a la base de datos MySQL Server 13. No se obtuvo resultado para el pallet: " + item;
                    result = false;
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                logger.Error(ex, "Error trying to connect to MySQL has occurred.");
                msgResult = "Error " + ex.Number + " has occurred: \n" + ex.Message;
                result = false;
            }

            Console.WriteLine("Done.");
            return result;
        }
        public bool updateCyclePallet(String item, ref String msgResult)
        {
            bool result = false;
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                if (connect())
                {
                    m_cb.Connection = m_conn;

                    int dia = DateTime.Now.DayOfYear;

                    string query = "update siixsem_pallets_smt set ciclos = ciclos + 1 where item = ?item ";

                    MySqlCommand mycomand = new MySqlCommand(query, m_cb.Connection);
                    mycomand.Parameters.AddWithValue("?item", item);


                    MySqlDataReader myreader = mycomand.ExecuteReader();

                    m_conn.Close();
                }
                else
                {
                    msgResult = "No se pudo conectar a la base de datos MySQL Server 13. No se acumulo ciclo para el pallet: " + item;
                    result = false;
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                logger.Error(ex, "Error trying to connect to MySQL has occurred.");
                msgResult = "Error " + ex.Number + " has occurred: \n" + ex.Message;
                result = false;
            }

            Console.WriteLine("Done.");
            return result;
        }

        public bool updateStatusPallet(String item, String newStatus, ref String msgResult)
        {
            bool result = false;
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                if (connect())
                {
                    m_cb.Connection = m_conn;

                    int dia = DateTime.Now.DayOfYear;

                    string query = "update siixsem_pallets_smt set status = '" + newStatus + "' where item = ?item ";

                    MySqlCommand mycomand = new MySqlCommand(query, m_cb.Connection);
                    mycomand.Parameters.AddWithValue("?item", item);


                    MySqlDataReader myreader = mycomand.ExecuteReader();

                    m_conn.Close();
                }
                else
                {
                    msgResult = "No se pudo conectar a la base de datos MySQL Server 13. No se acumulo ciclo para el pallet: " + item;
                    result = false;
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                logger.Error(ex, "Error trying to connect to MySQL has occurred.");
                msgResult = "Error " + ex.Number + " has occurred: \n" + ex.Message;
                result = false;
            }

            Console.WriteLine("Done.");
            return result;
        }
        public bool getPalletByName(String item, ref Pallets_smt pallet, ref String msgResult)
        {
            bool result = false;
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                if (connect())
                {
                    m_cb.Connection = m_conn;

                    int dia = DateTime.Now.DayOfYear;

                    string query = "select id, item, name, ciclos, limite, status, damaged from siixsem_pallets_smt where name = ?name ";

                    MySqlCommand mycomand = new MySqlCommand(query, m_cb.Connection);
                    mycomand.Parameters.AddWithValue("?name", item);


                    MySqlDataReader myreader = mycomand.ExecuteReader();
                    //myreader.Read();

                    if (myreader.HasRows)
                    {
                        while (myreader.Read())
                        {
                            pallet.Id = Convert.ToInt32(myreader["id"]);
                            pallet.Item = myreader["item"].ToString();
                            pallet.Name = myreader["name"].ToString();
                            pallet.Ciclos = Convert.ToInt32(myreader["ciclos"]);
                            pallet.Limite = Convert.ToInt32(myreader["limite"]);
                            pallet.Status = myreader["status"].ToString();
                            pallet.Damaged = myreader["damaged"].ToString();
                        }
                        result = true;
                        //if (String.IsNullOrEmpty(msgResult)) msgResult = "FAILED";
                    }
                    else
                    {
                        msgResult = "No se encontro el pallet " + item + ".";
                        result = true;
                    }
                    m_conn.Close();
                }
                else
                {
                    msgResult = "No se pudo conectar a la base de datos MySQL Server 13. No se obtuvo resultado para el pallet: " + item;
                    result = false;
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                logger.Error(ex, "Error trying to connect to MySQL has occurred.");
                msgResult = "Error " + ex.Number + " has occurred: \n" + ex.Message;
                result = false;
            }

            Console.WriteLine("Done.");
            return result;
        }
        public bool insertLastSerial150(String contador, String julianDay, String batchQty, String djGroup)
        {
            bool result = false;
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                connect();
                m_cb.Connection = m_conn;

                int dia = DateTime.Now.DayOfYear;

                string query = "INSERT INTO siixsem_xml_laser.siixsem_150b_ldm_t (contador, julian_date,cantidad,dj) VALUES (?cont,?jd,?cant,?dj)";

                MySqlCommand mycomand = new MySqlCommand(query, m_cb.Connection);
                mycomand.Parameters.AddWithValue("?cont", contador);
                mycomand.Parameters.AddWithValue("?jd", julianDay);
                mycomand.Parameters.AddWithValue("?cant", batchQty);
                mycomand.Parameters.AddWithValue("?dj", djGroup);


                MySqlDataReader myreader = mycomand.ExecuteReader();

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message);
                result = false;
            }
            m_conn.Close();
            Console.WriteLine("Done.");
            return result;
        }
    }
}
