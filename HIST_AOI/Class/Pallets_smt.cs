using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIST_AOI.Class
{
    class Pallets_smt
    {
        private int m_id;
        private String m_item;
        private String m_name;
        private int m_ciclos;
        private int m_limite;
        private String m_status;
        private String m_damaged;

        public int Id { get => m_id; set => m_id = value; }
        public string Item { get => m_item; set => m_item = value; }
        public string Name { get => m_name; set => m_name = value; }
        public int Ciclos { get => m_ciclos; set => m_ciclos = value; }
        public int Limite { get => m_limite; set => m_limite = value; }
        public string Status { get => m_status; set => m_status = value; }
        public string Damaged { get => m_damaged; set => m_damaged = value; }
    }
}
